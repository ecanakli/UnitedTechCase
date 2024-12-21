using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnitedTechCase.Scripts.Interfaces;
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

        private RectTransform _startButtonRectTransform;
        private RectTransform _endButtonRectTransform;

        private float _startButtonOffScreenPositionLeft;
        private float _startButtonOffScreenPositionRight;
        private float _endButtonInitialXPosition;

        private CancellationTokenSource _animationCancellationToken;

        public event Action OnStartButtonMovedOffScreen;
        public event Action OnGameRestart;
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

        private void DisableAllSpecialPowerButtons()
        {
            foreach (var specialPowerButton in specialPowerButtons)
            {
                specialPowerButton.SetInteractable(false);
            }
        }

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

        private void InitializeUIState()
        {
            _startButtonRectTransform.anchoredPosition = new Vector2(0f, _startButtonRectTransform.anchoredPosition.y);
            _endButtonRectTransform.anchoredPosition =
                new Vector2(_endButtonInitialXPosition, _endButtonRectTransform.anchoredPosition.y);
            specialPowerButtonsParent.anchoredPosition = new Vector2(0, -specialPowerButtonsParent.rect.height);
        }

        private async void MoveStartButtonOffScreen()
        {
            _animationCancellationToken?.Cancel();
            _animationCancellationToken = new CancellationTokenSource();

            startButton.enabled = false;

            await PlayAnimation(
                _startButtonRectTransform,
                new Vector2(_startButtonOffScreenPositionLeft, _startButtonRectTransform.anchoredPosition.y),
                0.5f,
                Ease.InOutQuad);

            startButton.gameObject.SetActive(false);
            OnStartButtonMovedOffScreen?.Invoke();
        }

        public async void AnimateInGameUI()
        {
            _animationCancellationToken?.Cancel();
            _animationCancellationToken = new CancellationTokenSource();

            await PlayAnimation(
                specialPowerButtonsParent,
                new Vector2(0f, 0f),
                0.5f,
                Ease.OutBounce);

            endButton.enabled = true;
            await PlayAnimation(
                _endButtonRectTransform,
                new Vector2(-35f, _endButtonRectTransform.anchoredPosition.y),
                0.5f,
                Ease.InOutQuad);

            OnInGameUIAnimationsCompleted?.Invoke();
        }

        private async void ResetUIPositions()
        {
            _animationCancellationToken?.Cancel();
            _animationCancellationToken = new CancellationTokenSource();
            OnGameRestart?.Invoke();
            endButton.enabled = false;
            DisableAllSpecialPowerButtons();

            await PlayAnimation(
                _endButtonRectTransform,
                new Vector2(_endButtonInitialXPosition, _endButtonRectTransform.anchoredPosition.y),
                0.5f,
                Ease.InOutQuad);

            await PlayAnimation(
                specialPowerButtonsParent,
                new Vector2(0, -specialPowerButtonsParent.rect.height),
                0.5f,
                Ease.OutBounce);

            startButton.gameObject.SetActive(true);
            _startButtonRectTransform.anchoredPosition = new Vector2(_startButtonOffScreenPositionRight,
                _startButtonRectTransform.anchoredPosition.y);
            await PlayAnimation(
                _startButtonRectTransform,
                new Vector2(0f, _startButtonRectTransform.anchoredPosition.y),
                0.5f,
                Ease.InOutQuad);

            startButton.enabled = true;
            ResetUIButtons();
        }

        private async UniTask PlayAnimation(RectTransform target, Vector2 targetPosition, float duration, Ease easeType)
        {
            await target
                .DOAnchorPos(targetPosition, duration)
                .SetEase(easeType)
                .AsyncWaitForCompletion().AsUniTask()
                .AttachExternalCancellation(_animationCancellationToken.Token);
        }

        private void ResetUIButtons()
        {
            SetAllButtonsInteractable(true);
        }

        private void OnDestroy()
        {
            _animationCancellationToken?.Cancel();
            RemoveListeners();
        }
    }
}