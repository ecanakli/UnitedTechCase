using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using IPoolable = UnitedTechCase.Scripts.Interfaces.IPoolable;

namespace UnitedTechCase.Scripts.Managers
{
    public class ObjectPoolManager : MonoBehaviour
    {
        // Represents a single pool of objects for a specific prefab
        private class Pool
        {
            public readonly Queue<IPoolable> Objects = new(); // Queue to hold pooled objects
            public readonly GameObject Prefab; // The prefab used to create new objects
            public readonly Transform Parent; // Parent transform to organize objects in the hierarchy

            public Pool(GameObject prefab, Transform parent)
            {
                Prefab = prefab;
                Parent = parent;
            }
        }

        private readonly Dictionary<Type, Pool> _pools = new(); // To manage pools by type
        private DiContainer _container;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Creates a pool for a specific type of IPoolable objects.
        /// </summary>
        /// <typeparam name="T">The type of objects to pool.</typeparam>
        /// <param name="prefab">The prefab used to create new instances.</param>
        /// <param name="initialSize">Initial number of objects to create.</param>
        /// <param name="parent">Optional parent transform for organizing objects in the hierarchy.</param>
        public void CreatePool<T>(GameObject prefab, int initialSize, Transform parent = null)
            where T : Component, IPoolable
        {
            if (_pools.ContainsKey(typeof(T)))
            {
                Debug.LogWarning($"Pool for {typeof(T).Name} already exists.");
                return;
            }

            var pool = new Pool(prefab, parent);

            for (var i = 0; i < initialSize; i++)
            {
                var instance = InstantiateObject(prefab, parent);
                instance.OnDeSpawned();
                pool.Objects.Enqueue(instance);
            }

            _pools[typeof(T)] = pool;
        }

        /// <summary>
        /// Spawns an object from the pool or creates a new one if the pool is empty.
        /// </summary>
        public T Spawn<T>(Vector3 position, Quaternion rotation) where T : Component, IPoolable
        {
            if (!_pools.ContainsKey(typeof(T)))
            {
                throw new InvalidOperationException($"No pool found for type {typeof(T).Name}");
            }

            var pool = _pools[typeof(T)];
            var instance = pool.Objects.Count > 0
                ? pool.Objects.Dequeue()
                : InstantiateObject(pool.Prefab, pool.Parent);

            // Ensure the object is of the expected type
            var component = instance as T;
            if (component == null)
            {
                throw new InvalidOperationException($"Spawned object is not of type {typeof(T).Name}");
            }

            var componentTransform = component.transform;
            componentTransform.position = position;
            componentTransform.rotation = rotation;

            instance.OnSpawned();
            return component;
        }

        public void ReturnToPool(IPoolable instance)
        {
            var type = instance.GetType();
            if (!_pools.TryGetValue(type, out var pool))
            {
                throw new InvalidOperationException($"No pool found for type {type.Name}");
            }

            instance.OnDeSpawned();
            pool.Objects.Enqueue(instance);
        }

        /// <summary>
        /// Instantiates a new object using Zenject's DiContainer.
        /// </summary>
        private IPoolable InstantiateObject(GameObject prefab, Transform parent)
        {
            var poolable = _container.InstantiatePrefabForComponent<IPoolable>(prefab, parent);
            if (poolable == null)
            {
                throw new InvalidOperationException($"Prefab {prefab.name} does not implement IPoolable");
            }

            return poolable;
        }
    }
}
