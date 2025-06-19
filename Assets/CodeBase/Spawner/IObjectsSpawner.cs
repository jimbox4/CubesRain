using System;

namespace CodeBase.Spawner
{
    public interface IObjectsSpawner
    {
        public event Action<int> ObjectsCountSpawned;
        public event Action ObjectSpawned;
        public event Action ObjectReleased;

        public string GetName();
    }
}
