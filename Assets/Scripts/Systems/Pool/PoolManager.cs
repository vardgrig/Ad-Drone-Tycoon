using System;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Pool
{
    public class PoolManager : MonoBehaviour
    {

        private readonly Dictionary<Type, object> _pools = new();

        public void CreatePool<T>(T prefab, int size) where T : Component
        {
            if (_pools.ContainsKey(typeof(T))) return;
            
            var poolContainer = new GameObject($"{typeof(T).Name} Pool");
            poolContainer.transform.SetParent(this.transform);
            _pools[typeof(T)] = new GenericPool<T>(prefab, size, poolContainer.transform);
        }

        public T Get<T>() where T : Component
        {
            if (_pools.ContainsKey(typeof(T))) 
                return (_pools[typeof(T)] as GenericPool<T>)?.Get();
            
            Debug.LogError($"Pool for type {typeof(T).Name} does not exist.");
            return null;
        }

        public void Return<T>(T obj) where T : Component
        {
            if (!_pools.ContainsKey(typeof(T)))
            {
                Debug.LogError($"Cannot return object. Pool for type {typeof(T).Name} does not exist.");
                return;
            }
            (_pools[typeof(T)] as GenericPool<T>)?.Return(obj);
        }
    }
}