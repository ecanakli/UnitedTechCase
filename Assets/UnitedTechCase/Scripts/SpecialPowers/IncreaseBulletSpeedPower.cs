using UnitedTechCase.Scripts.Interfaces;
using UnitedTechCase.Scripts.Managers;
using UnityEngine;

namespace UnitedTechCase.Scripts.SpecialPowers
{
    [CreateAssetMenu(fileName = "IncreaseBulletSpeedPower", menuName = "SpecialPowers/IncreaseBulletSpeed")]
    public class IncreaseBulletSpeedPower : ScriptableObject, ISpecialPower
    {
        [SerializeField]
        private float speedMultiplier = 1.5f;

        public void OnPowerAdded(GameManager gameManager, GameData gameData)
        {
            var bulletData = gameData.BulletData;
            bulletData.ModifySpeed(speedMultiplier);
        }
    }
}