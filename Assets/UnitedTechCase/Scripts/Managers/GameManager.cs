using System;
using System.Collections.Generic;
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
        private readonly Vector3 _characterSpawnPosition = new(0f, 0f, 2.13f);
        private readonly Vector3 _characterCenterPosition = new(0f, 0f, -13.41f);

        private readonly List<Character> _characters = new();
        private readonly List<Bullet> _bullets = new();

        public event Action OnGameSequenceStarted;
        public event Action OnGameSequenceRestarted;

        private ObjectPoolManager _objectPoolManager;
        private UIManager _uiManager;
        private SpecialPowerManager _specialPowerManager;
        private GameData _gameData;

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
            _uiManager.OnStartButtonMovedOffScreen += OnOnStartGame;
            _uiManager.OnInGameUIAnimationsCompleted += StartCharacterFiring;
            _uiManager.OnGameRestart += OnRestart;
            _specialPowerManager.OnPowerAdded += HandlePowerAdded;
        }

        private void UnSubscribeEvents()
        {
            _uiManager.OnStartButtonMovedOffScreen -= OnOnStartGame;
            _uiManager.OnInGameUIAnimationsCompleted -= StartCharacterFiring;
            _uiManager.OnGameRestart -= OnRestart;
            _specialPowerManager.OnPowerAdded -= HandlePowerAdded;
        }

        private void CreatePoolObjects()
        {
            _objectPoolManager.CreatePool<Character>(characterPrefab.gameObject, InitializeCharacterSize,
                characterTransformParent);
            _objectPoolManager.CreatePool<Bullet>(bulletPrefab.gameObject, 50, bulletTransformParent);
        }

        private void OnOnStartGame()
        {
            SpawnCharacter(_characterSpawnPosition, Quaternion.Euler(0f, 180f, 0f));
        }

        private void SpawnCharacter(Vector3 position, Quaternion rotation)
        {
            var character = _objectPoolManager.Spawn<Character>(position, rotation);
            if (_characters.Count != InitializeCharacterSize)
            {
                _characters.Add(character);
            }

            character.Initialize(_gameData);
            MoveCharacter(character, _characterCenterPosition);
        }

        public void SpawnAdditionalCharacter()
        {
            var newPos = _characterCenterPosition + new Vector3(5f, 0f, 0f);
            SpawnCharacter(newPos, Quaternion.Euler(0f, 180f, 0f));
        }

        private async void MoveCharacter(Character character, Vector3 movePosition)
        {
            await character.Move(movePosition);
            _uiManager.AnimateInGameUI();
        }

        private void StartCharacterFiring()
        {
            foreach (var character in _characters)
            {
                character.StartFiring();
            }

            OnGameSequenceStarted?.Invoke();
        }

        private void OnRestart()
        {
            foreach (var character in _characters)
            {
                character.OnGameEnd(_characterSpawnPosition);
            }

            _characters.Clear();
            _gameData.ResetToDefault();
            _specialPowerManager.ResetPowers();
            OnGameSequenceRestarted?.Invoke();
        }

        private void HandlePowerAdded(ISpecialPower power)
        {
            power.OnPowerAdded(this, _gameData);
            foreach (var character in _characters)
            {
                character.Initialize(_gameData);
            }
        }

        private void OnDestroy()
        {
            UnSubscribeEvents();
        }
    }
}