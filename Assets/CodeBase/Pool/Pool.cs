using System;
using System.Collections.Generic;
using CodeBase.Spawner;

namespace CodeBase.Pool
{
    public class Pool<T> : IDisposable where T : PoolableObject<T>
    {
        public event Action<T> Spawned;
        
        public IReadOnlyList<T> Objects => _objects;
        
        private readonly Spawner<T> _spawner = new Spawner<T>();
        private readonly Queue<T> _instances = new Queue<T>();
        private readonly List<T> _objects = new List<T>();
        
        private bool _isInitialized = false;
        public Pool(int objectsCount, T prefab)
        {
            var instances = _spawner.Spawn(objectsCount, prefab);

            foreach (var instance in instances)
            {
                Spawned?.Invoke(instance);
                instance.SetDisable();
                instance.Released += Return;
                _objects.Add(instance);
                _instances.Enqueue(instance);
            }
            
            _isInitialized = true;
        }

        public Pool()
        {
            
        }

        public void Initialize(int objectsCount, T prefab)
        {
            if (_isInitialized)
            {
                return;
            }
            
            var instances = _spawner.Spawn(objectsCount, prefab);

            foreach (var instance in instances)
            {
                Spawned?.Invoke(instance);
                instance.SetDisable();
                instance.Released += Return;
                _objects.Add(instance);
                _instances.Enqueue(instance);
            }
            
            _isInitialized = true;
        }

        public bool TryTake(out T item)
        {
            item = null;

            if (_instances.Count == 0)
            {
                return false;
            }

            item = _instances.Dequeue();

            return true;
        }

        private void Return(T item)
        {
            item.SetDisable();
            _instances.Enqueue(item);
        }

        public void Dispose()
        {
            foreach (var instance in _objects)
            {
                instance.Released -= Return;
            }
        }
    }
}