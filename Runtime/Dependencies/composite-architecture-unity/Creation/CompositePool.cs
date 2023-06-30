using System;
using System.Collections.Generic;
using UnityEngine;

namespace CompositeArchitecture
{
    [Serializable]
    public class CompositePool<T> : CompositeElement
        where T : MonoBehaviour, ICompositeElement
    {
        [SerializeField] public Transform ParentTransform;
        [SerializeField] public T Prefab;
        [SerializeField, Range(1, 1000)] public int DefaultSize = 1;
        [SerializeField] public PoolExtensionStrategy PoolExtensionStrategy;

        [SerializeField] public List<T> SpawnedItems;
        [SerializeField] public List<T> PooledItems;

        public int CurrentSize;

        private InstantiationHandler _instantiationHandler;
        
        protected override void OnInject()
        {
            Resolve(out _instantiationHandler);
        }

        protected override void OnInitialize()
        {
            Debug.Assert(Prefab != null, $"Can't instantiate items for pool {typeof(T)}! The Prefab is null!");

            CurrentSize = DefaultSize;
            for (var i = 0; i < CurrentSize; i++)
            {
                var item = Instantiate();
                PooledItems.Add(item);
            }
        }

        private T Instantiate()
        {
            var item = _instantiationHandler.Instantiate(Prefab, ParentTransform);
            item.gameObject.SetActive(false);
            return item;
        }

        public T Spawn(Vector3? position = default, Quaternion? rotation = default, Transform parent = default, bool activate = true, Action<T> beforeInstall = default)
        {
            if (PooledItems.Count == 0)
            {
                Extend();
            }

            var item = PooledItems[^1];
            SpawnedItems.Add(item);
            PooledItems.RemoveAt(PooledItems.Count - 1);

            if (position.HasValue || rotation.HasValue || parent != null)
            {
                var t = item.transform;
                if (parent != null) t.SetParent(parent);
                if (position.HasValue) t.position = position.Value;
                if (rotation.HasValue) t.rotation = rotation.Value;
            }
            
            beforeInstall?.Invoke(item);
            InstallChild(item, lifecycleFlags: LifecycleFlags.WithoutActivate, fromSubContainer: true);
            if (activate)
            {
                item.Activate();
            }

            return item;
        }

        public void Despawn(T item)
        {
            if (SpawnedItems.Remove(item))
            {
                PooledItems.Add(item);
            }
            else
            {
                Debug.LogError($"Can't despawn {item}. It's not part of the spawned items!");
            }

            if (item.State.IsActive)
            {
                item.Deactivate();
            }

            if (item.State.IsDeinitialized == false)
            {
                item.Deinitialize();
            }
            
            item.transform.SetParent(ParentTransform);
        }
        
        private void Extend()
        {
            Debug.LogWarning($"Pool {ParentTransform.name} for {typeof(T)} is making an extension. Previous size = {CurrentSize}.");

            if (PoolExtensionStrategy == PoolExtensionStrategy.Singly)
            {
                var item = Instantiate();
                PooledItems.Add(item);
                CurrentSize += 1;
            }
            else
            {
                for (var i = 0; i < CurrentSize; i++)
                {
                    var item = Instantiate();
                    PooledItems.Add(item);
                }

                CurrentSize += CurrentSize;
            }
        }
    }
}