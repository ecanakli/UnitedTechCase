using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnitedTechCase.Scripts.Interfaces;
using UnityEngine;
using Zenject;

namespace UnitedTechCase.Scripts.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Prefab Instances")]
        [SerializeField]
        private Character characterPrefab;

        [SerializeField]
        private Bullet bulletPrefab;

        [Header("Prefab Parents")]
        [SerializeField]
        private Transform characterTransformParent;

        [SerializeField]
        private Transform bulletTransformParent;

        private const int InitializeCharacterSize = 2;
        private const int InitializeBulletSize = 20;
        private readonly Vector3 _characterSpawnPosition = new(0f, 0f, 2.13f);
        private readonly Vector3 _characterCenterPosition = new(0f, 0f, -13.41f);

        private readonly List<Character> _activeCharacters = new();
        private readonly List<Bullet> _activeBullets = new();

        private ObjectPoolManager _objectPoolManager;
        private UIManager _uiManager;
        private SpecialPowerManager _specialPowerManager;
        private GameData _gameData;

        private CancellationTokenSource _fireCancellationTokenSource;

        [Inject]
        public void Construct(ObjectPoolManager objectPoolManager, UIManager uiManager,
            SpecialPowerManager specialPowerManager, GameData gameData, DiContainer container)
        {
            _objectPoolManager = objectPoolManager;
            _uiManager = uiManager;
            _specialPowerManager = specialPowerManager;
            _gameData = gameData;
        }

        private void Awake()
        {
            SubscribeEvents();
            CreatePoolObjects();
        }

        private void SubscribeEvents()
        {
            _uiManager.OnGameSequenceStart += OnStartGame;
            _uiManager.OnGameSequenceRestart += OnRestartGame;
            _uiManager.OnInGameUIAnimationsCompleted += StartCharacterFiring;
            _specialPowerManager.OnPowerAdded += HandlePowerAdded;
        }

        private void UnSubscribeEvents()
        {
            _uiManager.OnGameSequenceStart -= OnStartGame;
            _uiManager.OnGameSequenceRestart -= OnRestartGame;
            _uiManager.OnInGameUIAnimationsCompleted -= StartCharacterFiring;
            _specialPowerManager.OnPowerAdded -= HandlePowerAdded;

            DisposeFireToken();
        }

        private void CreatePoolObjects()
        {
            _objectPoolManager.CreatePool<Character>(characterPrefab.gameObject, InitializeCharacterSize,
                characterTransformParent);
            _objectPoolManager.CreatePool<Bullet>(bulletPrefab.gameObject, InitializeBulletSize, bulletTransformParent);
        }

        #region CharacterMove

        private void OnStartGame()
        {
            var newCharacter = SpawnCharacter(_characterSpawnPosition, Quaternion.Euler(0f, -180f, 0f));
            MoveCharacter(newCharacter, _characterCenterPosition);
        }

        private Character SpawnCharacter(Vector3 spawnPosition, Quaternion rotation)
        {
            var character = _objectPoolManager.Spawn<Character>(spawnPosition, rotation);
            _activeCharacters.Add(character);
            character.Initialize(_gameData);
            return character;
        }

        private async void MoveCharacter(Character character, Vector3 movePosition)
        {
            try
            {
                await character.Move(movePosition)
                    .AttachExternalCancellation(gameObject.GetCancellationTokenOnDestroy());
                _uiManager.AnimateInGameUI();
            }
            catch (OperationCanceledException)
            {
                Debug.Log("Character movement cancelled due to object destruction.");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Unexpected error during character movement: {ex}");
            }
        }

        public void SpawnAdditionalCharacter()
        {
            var spawnPosition = _characterCenterPosition + new Vector3(2.5f, 0f, -1.3f);
            SpawnCharacter(spawnPosition, Quaternion.Euler(0f, -180f, 0f));
        }

        #endregion

        #region CharacterFire

        private void StartCharacterFiring()
        {
            ResetActiveBullets();
            StopCharacterFiring();
            _fireCancellationTokenSource = new CancellationTokenSource();
            CharacterFireAsync(_fireCancellationTokenSource).Forget();
        }

        private async UniTaskVoid CharacterFireAsync(CancellationTokenSource cancellationTokenSource)
        {
            while (!cancellationTokenSource.IsCancellationRequested)
            {
                foreach (var character in _activeCharacters)
                {
                    character.Fire();
                }

                await UniTask.Delay(Mathf.RoundToInt(_gameData.BaseFireRate * 1000),
                    cancellationToken: cancellationTokenSource.Token);
            }
        }

        private void ResetActiveBullets()
        {
            foreach (var bullet in _activeBullets)
            {
                bullet.ReturnToPool();
            }

            _activeBullets.Clear();
        }

        private void StopCharacterFiring()
        {
            _fireCancellationTokenSource?.Cancel();
        }

        private void DisposeFireToken()
        {
            _fireCancellationTokenSource?.Cancel();
            _fireCancellationTokenSource?.Dispose();
        }

        #endregion

        private void OnRestartGame()
        {
            StopCharacterFiring();

            foreach (var character in _activeCharacters)
            {
                character.OnGameEnd(_characterSpawnPosition);
            }

            _activeCharacters.Clear();

            _gameData.ResetToDefault();
            _specialPowerManager.ResetPowers();
        }

        private void HandlePowerAdded(ISpecialPower power)
        {
            power.OnPowerAdded(this, _gameData);
            foreach (var character in _activeCharacters)
            {
                character.Initialize(_gameData);
            }
        }

        public void RegisterBullet(Bullet bullet)
        {
            _activeBullets.Add(bullet);
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }
    }
}