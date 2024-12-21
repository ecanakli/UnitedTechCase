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

        public void ResetToDefaultValues()
        {
            Speed = 25f;
            ExtraBullets = 0;
            Direction = Vector3.back;
        }
    }
}