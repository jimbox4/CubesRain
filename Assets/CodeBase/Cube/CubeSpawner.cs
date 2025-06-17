using System.Collections;
using CodeBase.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.Cube
{
    public class CubeSpawner : MonoBehaviour
    {
        [SerializeField] private Transform _spawnArea;
        [SerializeField] private Vector2 _spawnAreaRange;
        [SerializeField] private Cube _cubePrefab;

        private Pool<Cube> _pool;

        private int _cubeCount;
        private bool _isSpawning = true;

        private void Start()
        {
            int cubeCount = 10;
            _pool = new Pool<Cube>(cubeCount, _cubePrefab);

            StartCoroutine(Spawn());
        }

        private void CubeInitialize(Cube cube)
        {
            cube.SetEnable();
            cube.SetPosition(GetRandomPosition());
        }

        private IEnumerator Spawn()
        {
            WaitForSeconds wait = new WaitForSeconds(1f);

            while (_isSpawning)
            {
                if (_pool.TryTake(out var cube))
                {
                    CubeInitialize(cube);
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

        private void OnDisable()
        {
            _isSpawning = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(_spawnArea.position, new Vector3(_spawnAreaRange.x, 0, _spawnAreaRange.y));
        }
    }
}