using UnitedTechCase.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UnitedTechCase.Scripts
{
    public class SpecialPowerButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        public Button Button => button;

        [SerializeField]
        private ScriptableObject powerData;

        public ISpecialPower Power => powerData as ISpecialPower;
    }
}