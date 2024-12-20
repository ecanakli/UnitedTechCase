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
        private float baseBulletSpeed = 25f;

        [SerializeField]
        private bool doubleShotEnabled;

        [SerializeField]
        private int extraBullets;

        [SerializeField]
        private bool characterDuplicated;

        [Header("Bullet Data")]
        [SerializeField]
        private BulletData bulletData;

        public float BaseFireRate
        {
            get => baseFireRate;
            set => baseFireRate = value;
        }

        public float BaseBulletSpeed => baseBulletSpeed;

        public bool DoubleShotEnabled
        {
            get => doubleShotEnabled;
            set => doubleShotEnabled = value;
        }

        public int ExtraBullets
        {
            get => extraBullets;
            set => extraBullets = value;
        }

        public bool CharacterDuplicated
        {
            get => characterDuplicated;
            set => characterDuplicated = value;
        }

        public BulletData BulletData => bulletData;

        public void ResetToDefault()
        {
            bulletData.ResetToDefaultValues();
            baseFireRate = 2f;
            baseBulletSpeed = 25f;
            doubleShotEnabled = false;
            extraBullets = 0;
            characterDuplicated = false;
        }
    }
}