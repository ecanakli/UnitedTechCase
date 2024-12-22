using UnitedTechCase.Scripts.Interfaces;
using UnityEngine;
using UnityEngine.UI;

namespace UnitedTechCase.Scripts.SpecialPowers
{
    public class SpecialPowerButton : MonoBehaviour
    {
        [SerializeField]
        private Button button;

        public Button Button => button;

        [SerializeField]
        private Image buttonBackground;

        [SerializeField]
        private Color selectedColor = new(1f, 0.27f, 0.31f);

        [SerializeField]
        private Color defaultColor = Color.white;


        [SerializeField]
        private ScriptableObject powerData;

        public ISpecialPower Power => powerData as ISpecialPower;

        public void OnClicked()
        {
            SetInteractable(false);
            ChangeBackgroundColor(true);
        }

        public void SetInteractable(bool state)
        {
            button.interactable = state;
            ChangeBackgroundColor(false);
        }

        public void DisableButton()
        {
            button.interactable = false;
            ChangeBackgroundColor(false);
        }

        private void ChangeBackgroundColor(bool selected)
        {
            if (buttonBackground != null)
            {
                buttonBackground.color = selected ? selectedColor : defaultColor;
            }
        }
    }
}