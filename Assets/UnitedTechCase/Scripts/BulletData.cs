using UnityEngine;

namespace UnitedTechCase.Scripts
{
    [CreateAssetMenu(fileName = "New Bullet Data", menuName = "Game/BulletData")]
    public class BulletData : ScriptableObject
    {
        [Header("Bullet Settings")]
        [Range(1f, 100f)]
        public float Speed = 25f;

        public Vector3 Direction = Vector3.back;

        public void ModifySpeed(float multiplier)
        {
            Speed *= multiplier;
        }

        public void ModifyDirection(Vector3 newDirection)
        {
            Direction = newDirection.normalized;
        }

        public void ResetToDefaultValues()
        {
            Speed = 25f;
            Direction = Vector3.back;
        }
    }
}