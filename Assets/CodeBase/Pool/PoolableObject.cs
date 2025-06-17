using System;
using UnityEngine;

namespace CodeBase.Pool
{
    public abstract class PoolableObject<T> : MonoBehaviour where T : PoolableObject<T>
    {
        public abstract event Action<T> Released;
        public abstract void Release();

        public abstract void SetPosition(Vector3 position);
        public abstract void SetEnable();
        public abstract void SetDisable();
    }
}
