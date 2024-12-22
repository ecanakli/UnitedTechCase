using UnitedTechCase.Scripts.Interfaces;
using UnitedTechCase.Scripts.Managers;
using UnityEngine;

namespace UnitedTechCase.Scripts.SpecialPowers
{
    [CreateAssetMenu(fileName = "ExtraBulletsPower", menuName = "SpecialPowers/ExtraBullets")]
    public class ExtraBulletsPower : ScriptableObject, ISpecialPower
    {
        [SerializeField]
        private int extraBulletCount = 2;

        public void OnPowerAdded(GameManager gameManager, GameData gameData)
        {
            gameData.BulletData.ModifyBullets(extraBulletCount);
        }
    }
}