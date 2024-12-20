using UnitedTechCase.Scripts.Managers;
using UnityEngine;
using Zenject;
using IPoolable = UnitedTechCase.Scripts.Interfaces.IPoolable;

namespace UnitedTechCase.Scripts
{
    public abstract class PooledObject : MonoBehaviour, IPoolable
    {
        [Inject]
        private ObjectPoolManager _objectPoolManager;

        protected ObjectPoolManager ObjectPoolManager => _objectPoolManager;

        [Inject]
        public void Construct(ObjectPoolManager objectPoolManager)
        {
            _objectPoolManager = objectPoolManager;
        }

        public virtual void OnSpawned()
        {
            gameObject.SetActive(true);
        }

        public virtual void OnDeSpawned()
        {
            gameObject.SetActive(false);
        }

        public void ReturnToPool()
        {
            _objectPoolManager?.ReturnToPool(this);
        }
    }
}