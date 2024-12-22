using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnitedTechCase.Scripts.Interfaces;
using UnitedTechCase.Scripts.SpecialPowers;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UnitedTechCase.Scripts.Managers
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Buttons")]
        [SerializeField]
        private Button startButton;

        [SerializeField]
        private Button endButton;

        [SerializeField]
        private List<SpecialPowerButton> specialPowerButtons;

        [SerializeField]
        private RectTransform specialPowerButtonsParent;

        [SerializeField]
        private float buttonAnimationDuration = 0.5f;

        private RectTransform _startButtonRectTransform;
        private RectTransform _endButtonRectTransform;

        private float _startButtonOffScreenPositionLeft;
        private float _startButtonOffScreenPositionRight;
        private float _endButtonInitialXPosition;
        private const float EndButtonInGameXPosition = -35f;

        private CancellationTokenSource _animationCancellationToken;

        public event Action OnGameSequenceStart;
        public event Action OnGameSequenceRestart;
        public event Action OnInGameUIAnimationsCompleted;

        [Inject]
        private SpecialPowerManager _specialPowerManager;

        private void Awake()
        {
            CacheUIElements();
            InitializeUIState();
            AddListeners();
        }

        private void AddListeners()
        {
            startButton.onClick.AddListener(MoveStartButtonOffScreen);
            endButton.onClick.AddListener(ResetUIPositions);
            foreach (var specialPowerButton in specialPowerButtons)
            {
                specialPowerButton.Button.onClick.AddListener(() =>
                    OnSpecialPowerButtonClicked(specialPowerButton.Power));
            }

            _specialPowerManager.OnMaxPowersReached += DisableAllSpecialPowerButtons;
        }

        private void RemoveListeners()
        {
            startButton.onClick.RemoveListener(MoveStartButtonOffScreen);
            endButton.onClick.RemoveListener(ResetUIPositions);

            foreach (var specialPowerButton in specialPowerButtons)
            {
                specialPowerButton.Button.onClick.RemoveAllListeners();
            }

            _specialPowerManager.OnMaxPowersReached -= DisableAllSpecialPowerButtons;
        }

        // Caches RectTransform components for optimized access
        private void CacheUIElements()
        {
            _startButtonRectTransform = startButton.GetComponent<RectTransform>();
            _endButtonRectTransform = endButton.GetComponent<RectTransform>();

            var screenWidth = Screen.width;
            var startButtonWidth = _startButtonRectTransform.rect.width;
            var offScreenPosition = screenWidth / 2f + startButtonWidth / 2;
            _startButtonOffScreenPositionLeft = -offScreenPosition;
            _startButtonOffScreenPositionRight = offScreenPosition;
            _endButtonInitialXPosition = _endButtonRectTransform.anchoredPosition.x;
        }

        // Sets the initial UI positions at the start of the game
        private void InitializeUIState()
        {
            _startButtonRectTransform.anchoredPosition = new Vector2(0f, _startButtonRectTransform.anchoredPosition.y);
            _endButtonRectTransform.anchoredPosition =
                new Vector2(_endButtonInitialXPosition, _endButtonRectTransform.anchoredPosition.y);
            specialPowerButtonsParent.anchoredPosition = new Vector2(0, -specialPowerButtonsParent.rect.height);
        }

        #region SpecialPowerButtons

        // Handles a special power button click and notifies the SpecialPowerManager
        private void OnSpecialPowerButtonClicked(ISpecialPower power)
        {
            _specialPowerManager.SelectPower(power);

            foreach (var specialPowerButton in specialPowerButtons)
            {
                if (specialPowerButton.Power == power)
                {
                    specialPowerButton.OnClicked();
                }
            }
        }

        // Enables or disables all special power buttons
        private void SetAllButtonsInteractable(bool state)
        {
            foreach (var specialPowerButton in specialPowerButtons)
            {
                if (specialPowerButton.Button.interactable != state)
                {
                    specialPowerButton.SetInteractable(true);
                }
            }
        }

        // Disables all special power buttons (used when the max powers are selected)
        private void DisableAllSpecialPowerButtons()
        {
            foreach (var specialPowerButton in specialPowerButtons)
            {
                if (specialPowerButton.Button.interactable)
                {
                    specialPowerButton.DisableButton();
                }
            }
        }

        #endregion


        #region InGameUIAnimations

        // Start Game Sequence
        private async void MoveStartButtonOffScreen()
        {
            _animationCancellationToken?.Cancel();
            _animationCancellationToken = new CancellationTokenSource();

            try
            {
                startButton.enabled = false;

                await PlayAnimation(
                    _startButtonRectTransform,
                    new Vector2(_startButtonOffScreenPositionLeft, _startButtonRectTransform.anchoredPosition.y),
                    buttonAnimationDuration,
                    Ease.InOutQuad).AttachExternalCancellation(_animationCancellationToken.Token);

                startButton.gameObject.SetActive(false);
                OnGameSequenceStart?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("MoveStartButtonOffScreen operation cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error in MoveStartButtonOffScreen: {ex}");
            }
        }


        // Starts UI animations after the character reaches its position
        public async void AnimateInGameUI()
        {
            _animationCancellationToken?.Cancel();
            _animationCancellationToken = new CancellationTokenSource();

            try
            {
                await PlayAnimation(
                    specialPowerButtonsParent,
                    new Vector2(0f, 0f),
                    buttonAnimationDuration,
                    Ease.OutExpo).AttachExternalCancellation(_animationCancellationToken.Token);

                endButton.enabled = true;

                await PlayAnimation(
                    _endButtonRectTransform,
                    new Vector2(EndButtonInGameXPosition, _endButtonRectTransform.anchoredPosition.y),
                    buttonAnimationDuration,
                    Ease.InOutQuad).AttachExternalCancellation(_animationCancellationToken.Token);

                OnInGameUIAnimationsCompleted?.Invoke();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("AnimateInGameUI cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error in AnimateInGameUI: {ex}");
            }
        }

        // Restart Game Sequence
        private async void ResetUIPositions()
        {
            _animationCancellationToken?.Cancel();
            _animationCancellationToken = new CancellationTokenSource();

            try
            {
                OnGameSequenceRestart?.Invoke();
                endButton.enabled = false;
                DisableAllSpecialPowerButtons();

                await PlayAnimation(
                    _endButtonRectTransform,
                    new Vector2(_endButtonInitialXPosition, _endButtonRectTransform.anchoredPosition.y),
                    buttonAnimationDuration,
                    Ease.InOutQuad).AttachExternalCancellation(_animationCancellationToken.Token);

                await PlayAnimation(
                    specialPowerButtonsParent,
                    new Vector2(0, -specialPowerButtonsParent.rect.height),
                    buttonAnimationDuration,
                    Ease.OutExpo).AttachExternalCancellation(_animationCancellationToken.Token);

                startButton.gameObject.SetActive(true);
                _startButtonRectTransform.anchoredPosition = new Vector2(
                    _startButtonOffScreenPositionRight,
                    _startButtonRectTransform.anchoredPosition.y);

                await PlayAnimation(
                    _startButtonRectTransform,
                    new Vector2(0f, _startButtonRectTransform.anchoredPosition.y),
                    buttonAnimationDuration,
                    Ease.InOutQuad).AttachExternalCancellation(_animationCancellationToken.Token);

                startButton.enabled = true;
                ResetUIButtons();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Reset UI operation cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error during ResetUIPositions: {ex}");
            }
        }


        private async UniTask PlayAnimation(RectTransform target, Vector2 targetPosition, float duration, Ease easeType)
        {
            try
            {
                await target
                    .DOAnchorPos(targetPosition, duration)
                    .SetEase(easeType)
                    .AsyncWaitForCompletion().AsUniTask()
                    .AttachExternalCancellation(_animationCancellationToken.Token);
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Animation cancelled.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error during animation: {ex}");
            }
        }

        #endregion

        private void ResetUIButtons()
        {
            SetAllButtonsInteractable(true);
        }

        private void OnDestroy()
        {
            DOTween.KillAll();
            DOTween.Clear(true);
            _animationCancellationToken?.Cancel();
            _animationCancellationToken?.Dispose();
            RemoveListeners();
        }
    }
}