using System.Collections.Generic;
using CodeBase.Pool;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Spawner
{
    public class Spawner<T> where T : PoolableObject<T>
    {
        public List<T> Spawn(int count, T prefab)
        {
            List<T> instances = new List<T>();
        
            for (int i = 0; i < count; i++)
            {
                var instance = Spawn(prefab);
                instances.Add(instance);
            }
        
            return instances;
        }

        public T Spawn(T prefab)
        {
            return Object.Instantiate(prefab, Vector3.zero, Quaternion.identity, null);
        }
    }
}
