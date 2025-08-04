using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Systems.Pool
{
    public class GenericPool<T> where T : Component
    {
        private T _prefab;
        private Queue<T> _pool = new();
        private Transform _parent;

        public GenericPool(T prefab, int initialSize, Transform parent)
        {
            this._prefab = prefab;
            this._parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                var instance = Object.Instantiate(prefab, parent);
                instance.gameObject.SetActive(false);
                _pool.Enqueue(instance);
            }
        }

        public T Get()
        {
            if (_pool.Count == 0)
            {
                // Auto-grow the pool if it's empty
                var instance = Object.Instantiate(_prefab, _parent);
                return instance;
            }

            var obj = _pool.Dequeue();
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}