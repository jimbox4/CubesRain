using System;
using CodeBase.Pool;
using CodeBase.Spawner;
using UnityEngine;

namespace CodeBase.Bombs
{
    public class BombsSpawner : MonoBehaviour, IObjectsSpawner
    {
        [SerializeField] private Bomb _bombPrefab;
    
        public event Action<int> ObjectsCountSpawned;
        public event Action ObjectSpawned;
        public event Action ObjectReleased;


        private Pool<Bomb> _pool;

        private void Start()
        {
            int bombCount = 10;
            _pool = new Pool<Bomb>(bombCount, _bombPrefab);
            ObjectsCountSpawned?.Invoke(bombCount);

            foreach (var bomb in _pool.Objects)
            {
                bomb.Released += OnBombReleased;
            }
        }

        public string GetName()
        {
            return nameof(BombsSpawner);
        }
        
        public void Spawn(Vector3 position)
        {
            if (_pool.TryTake(out var bomb))
            {
                ObjectSpawned?.Invoke();
                BombInitialize(bomb, position);
            }
        }

        private void BombInitialize(Bomb bomb, Vector3 position)
        {
            bomb.SetPosition(position);
            bomb.SetEnable();
        }

        private void OnBombReleased(Bomb bomb)
        {
            ObjectReleased?.Invoke();
        }

        private void OnDisable()
        {
            foreach (var bomb in _pool.Objects)
            {
                bomb.Released -= OnBombReleased;
            }
        }
    }
}
