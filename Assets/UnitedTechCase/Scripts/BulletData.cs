using UnityEngine;

namespace UnitedTechCase.Scripts
{
    [CreateAssetMenu(fileName = "New Bullet Data", menuName = "Game/BulletData")]
    public class BulletData : ScriptableObject
    {
        [Header("Bullet Settings")]
        [Range(1f, 100f)]
        [SerializeField]
        private float speed = 25f;

        [SerializeField]
        private Vector3 direction = Vector3.forward;

        [SerializeField]
        private int extraBullets;

        private float _defaultSpeed;
        private Vector3 _defaultDirection;
        private int _defaultExtraBullets;

        public float Speed
        {
            get => speed;
            private set => speed = value;
        }

        public Vector3 Direction
        {
            get => direction;
            private set => direction = value;
        }

        public int ExtraBullets
        {
            get => extraBullets;
            private set => extraBullets = value;
        }

        private void OnEnable()
        {
            CacheDefaultValues();
        }

        private void CacheDefaultValues()
        {
            _defaultSpeed = speed;
            _defaultDirection = direction;
            _defaultExtraBullets = extraBullets;
        }

        public void ResetToDefaultValues()
        {
            Speed = _defaultSpeed;
            Direction = _defaultDirection;
            ExtraBullets = _defaultExtraBullets;
        }

        public void ModifySpeed(float multiplier)
        {
            Speed *= multiplier;
        }

        public void ModifyBullets(int bulletCount)
        {
            ExtraBullets = bulletCount;
        }

        public void ModifyDirection(Vector3 newDirection)
        {
            Direction = newDirection.normalized;
        }
    }
}