using System;
using System.Collections.Generic;
using UnitedTechCase.Scripts.Interfaces;
using UnityEngine;

namespace UnitedTechCase.Scripts.Managers
{
    public class SpecialPowerManager : MonoBehaviour
    {
        private const int MaxPowers = 3;
        private readonly List<ISpecialPower> _activePowers = new();

        public event Action<ISpecialPower> OnPowerAdded;
        public event Action<ISpecialPower> OnPowerRemoved;

        public void SelectPower(ISpecialPower power)
        {
            if (_activePowers.Count >= MaxPowers)
            {
                Debug.LogWarning("Maximum special powers selected!");
                return;
            }

            _activePowers.Add(power);
            OnPowerAdded?.Invoke(power);
        }

        public void ResetPowers()
        {
            foreach (var power in _activePowers)
            {
                OnPowerRemoved?.Invoke(power);
            }

            _activePowers.Clear();
        }

        public IEnumerable<ISpecialPower> GetActivePowers()
        {
            return _activePowers;
        }
    }
}