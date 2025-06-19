using System;
using System.Collections;
using CodeBase.Bombs;
using CodeBase.Pool;
using CodeBase.Spawner;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.Cube
{
    public class CubesSpawner : MonoBehaviour, IObjectsSpawner
    {
        [SerializeField] private Transform _spawnArea;
        [SerializeField] private Vector2 _spawnAreaRange;
        [SerializeField] private Cube _cubePrefab;
        [SerializeField] private BombsSpawner _bombsSpawner;
        
        public event Action<int> ObjectsCountSpawned;
        public event Action ObjectSpawned;
        public event Action ObjectReleased;
        
        private Pool<Cube> _pool;

        private bool _isSpawning = true;

        private void Start()
        {
            int cubeCount = 10;
            _pool = new Pool<Cube>();
            _pool.Spawned += CubeInitialize;
            _pool.Initialize(cubeCount, _cubePrefab);
            
            ObjectsCountSpawned?.Invoke(cubeCount);

            foreach (var cube in _pool.Objects)
            {
                cube.Released += OnCubeReleased;
            }
            
            StartCoroutine(Spawn());
        }

        public string GetName()
        {
            return nameof(CubesSpawner);
        }
        
        private void CubeInitialize(Cube cube)
        {
            cube.Initialize(_bombsSpawner);
        }
        private void CubeGetInitialize(Cube cube)
        {
            cube.SetEnable();
            cube.SetPosition(GetRandomPosition());
        }

        private IEnumerator Spawn()
        {
            WaitForSeconds wait = new WaitForSeconds(0.5f);

            while (_isSpawning)
            {
                if (_pool.TryTake(out var cube))
                {
                    ObjectSpawned?.Invoke();
                    CubeGetInitialize(cube);
                }

                yield return wait;
            }
        }

        private Vector3 GetRandomPosition()
        {
            float x = Random.Range(_spawnArea.position.x - _spawnAreaRange.x / 2,
                _spawnArea.position.x + _spawnAreaRange.x / 2);
            float y = _spawnArea.position.y;
            float z = Random.Range(_spawnArea.position.z - _spawnAreaRange.y / 2,
                _spawnArea.position.z + _spawnAreaRange.y / 2);

            return new Vector3(x, y, z);
        }

        private void OnCubeReleased(Cube cube)
        {
            ObjectReleased?.Invoke();
        }
        
        private void OnDisable()
        {
            _isSpawning = false;
            _pool.Spawned -= CubeInitialize;
            
            foreach (var cube in _pool.Objects)
            {
                cube.Released -= OnCubeReleased;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_spawnArea.position, new Vector3(_spawnAreaRange.x, 0, _spawnAreaRange.y));
        }
    }
}