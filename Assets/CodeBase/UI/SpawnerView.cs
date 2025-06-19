using CodeBase.Spawner;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
    public abstract class SpawnerView<T> : MonoBehaviour where T : IObjectsSpawner
    {
        [SerializeField] private T _spawner;
        [SerializeField] private TextMeshProUGUI _label;
        [SerializeField] private TextMeshProUGUI _poolCountTextMesh;
        [SerializeField] private TextMeshProUGUI _spawnCountTextMesh;
        [SerializeField] private TextMeshProUGUI _sceneCountTextMesh;

        private int _spawnCount;
        private int _sceneCount;

        private void Start()
        {
            _label.text = _spawner.GetName();
            UpdateSceneCount();
            UpdateSpawnCount();
        }
        private void OnEnable()
        {
            _spawner.ObjectSpawned += OnObjectSpawned;
            _spawner.ObjectReleased += OnObjectReleased;
            _spawner.ObjectsCountSpawned += SetPoolCount;
        }

        private void OnDisable()
        {
            _spawner.ObjectSpawned -= OnObjectSpawned;
            _spawner.ObjectReleased -= OnObjectReleased;
            _spawner.ObjectsCountSpawned -= SetPoolCount;
        }

        private void OnObjectReleased()
        {
            _sceneCount--;
            string text = $"Scene count: {_sceneCount}";
            
            UpdateSceneCount();
        }

        private void OnObjectSpawned()
        {
            _sceneCount++;
            _spawnCount++;
                
            UpdateSceneCount();
            UpdateSpawnCount();
        }
        
        private void SetPoolCount(int count)
        {
            _poolCountTextMesh.text = $"Pool count: {count}";
        }

        private void UpdateSceneCount()
        {
            _sceneCountTextMesh.text = $"Scene count: {_sceneCount}";
        }
        
        private void UpdateSpawnCount()
        {
            _spawnCountTextMesh.text = $"Spawn count: {_spawnCount}";
        }
    }
}
