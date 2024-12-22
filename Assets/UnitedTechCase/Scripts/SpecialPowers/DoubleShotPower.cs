using UnitedTechCase.Scripts.Interfaces;
using UnitedTechCase.Scripts.Managers;
using UnityEngine;

namespace UnitedTechCase.Scripts.SpecialPowers
{
    [CreateAssetMenu(fileName = "DoubleShotPower", menuName = "SpecialPowers/DoubleShot")]
    public class DoubleShotPower : ScriptableObject, ISpecialPower
    {
        public void OnPowerAdded(GameManager gameManager, GameData gameData)
        {
            gameData.DoubleShotEnabled = true;
        }
    }
}