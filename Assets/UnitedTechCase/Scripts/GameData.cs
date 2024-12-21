using UnityEngine;

namespace UnitedTechCase.Scripts
{
    [CreateAssetMenu(fileName = "New Game Data", menuName = "Game/GameData")]
    public class GameData : ScriptableObject
    {
        [Header("Character Stats")]
        [SerializeField]
        private float baseFireRate = 2f;

        [SerializeField]
        private bool doubleShotEnabled;

        [SerializeField]
        private bool characterDuplicated;

        [Header("Bullet Data")]
        [SerializeField]
        private BulletData bulletData;

        private BulletData _runtimeBulletData;

        public BulletData BulletData => _runtimeBulletData;

        public void Initialize()
        {
            _runtimeBulletData = Instantiate(bulletData);
        }

        public float BaseFireRate
        {
            get => baseFireRate;
            set => baseFireRate = value;
        }

        public bool DoubleShotEnabled
        {
            get => doubleShotEnabled;
            set => doubleShotEnabled = value;
        }

        public bool CharacterDuplicated
        {
            get => characterDuplicated;
            set => characterDuplicated = value;
        }

        public void ResetToDefault()
        {
            BulletData.ResetToDefaultValues();
            baseFireRate = 2f;
            doubleShotEnabled = false;
            characterDuplicated = false;
        }
    }
}