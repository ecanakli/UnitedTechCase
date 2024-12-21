using UnitedTechCase.Scripts.Interfaces;
using UnitedTechCase.Scripts.Managers;
using UnityEngine;

namespace UnitedTechCase.Scripts
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

    [CreateAssetMenu(fileName = "DoubleShotPower", menuName = "SpecialPowers/DoubleShot")]
    public class DoubleShotPower : ScriptableObject, ISpecialPower
    {
        public void OnPowerAdded(GameManager gameManager, GameData gameData)
        {
            gameData.DoubleShotEnabled = true;
        }
    }

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

    [CreateAssetMenu(fileName = "DuplicateCharacterPower", menuName = "SpecialPowers/DuplicateCharacter")]
    public class DuplicateCharacterPower : ScriptableObject, ISpecialPower
    {
        public void OnPowerAdded(GameManager gameManager, GameData gameData)
        {
            if (gameData.CharacterDuplicated)
            {
                return;
            }

            gameData.CharacterDuplicated = true;
            gameManager.SpawnAdditionalCharacter();
        }
    }
}