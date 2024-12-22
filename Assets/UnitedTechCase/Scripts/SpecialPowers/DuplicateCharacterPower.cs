using UnitedTechCase.Scripts.Interfaces;
using UnitedTechCase.Scripts.Managers;
using UnityEngine;

namespace UnitedTechCase.Scripts.SpecialPowers
{
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