using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using IPoolable = UnitedTechCase.Scripts.Interfaces.IPoolable;

namespace UnitedTechCase.Scripts.Managers
{
    public class ObjectPoolManager : MonoBehaviour
    {
        private class Pool
        {
            public readonly Queue<IPoolable> Objects = new();
            public readonly GameObject Prefab;
            public readonly Transform Parent;

            public Pool(GameObject prefab, Transform parent)
            {
                Prefab = prefab;
                Parent = parent;
            }
        }

        private readonly Dictionary<Type, Pool> _pools = new();
        private DiContainer _container;

        [Inject]
        public void Construct(DiContainer container)
        {
            _container = container;
        }

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
