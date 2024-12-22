using UnitedTechCase.Scripts.Interfaces;
using UnitedTechCase.Scripts.Managers;
using UnityEngine;

namespace UnitedTechCase.Scripts.SpecialPowers
{
    [CreateAssetMenu(fileName = "IncreaseFireRatePower", menuName = "SpecialPowers/IncreaseFireRate")]
    public class IncreaseFireRatePower : ScriptableObject, ISpecialPower
    {
        [SerializeField]
        private float fireRateMultiplier = 0.5f;

        public void OnPowerAdded(GameManager gameManager, GameData gameData)
        {
            gameData.BaseFireRate *= fireRateMultiplier;
        }
    }
}