using System;
using System.Collections;
using CodeBase.Bombs;
using CodeBase.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.Cube
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(MeshRenderer))]
    public class Cube : PoolableObject<Cube>
    {
        private const float MaxReleaseDelay = 5f;
        private const float MinReleaseDelay = 2f;
        
        [SerializeField] private LayerMask _boxLayerMask;
        [SerializeField] private Material _material;
        public override event Action<Cube> Released;

        private MeshRenderer _meshRenderer;
        private Rigidbody _rigidbody;
        private BoxCollider _collider;
        private BombsSpawner _bombsSpawner;
        
        private float _releaseDelay;
        private bool _isTouched = false;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Initialize(BombsSpawner  spawner)
        {
            _bombsSpawner =  spawner;
        }
        public override void Release()
        {
            _bombsSpawner.Spawn(transform.position);
            Released?.Invoke(this);
        }

        public override void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public override void SetEnable()
        {
            _meshRenderer.material = new Material(_material);
            _isTouched = false;
            _meshRenderer.enabled = true;
            _collider.enabled = true;
            _rigidbody.constraints = RigidbodyConstraints.None;
            transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        public override void SetDisable()
        {
            _isTouched = true;
            _collider.enabled = false;
            _meshRenderer.enabled = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        private void OnCollisionEnter(Collision other)
        {
            if (1 << other.gameObject.layer != _boxLayerMask || _isTouched)
            {
                return;
            }
            
            SetRandomColor();
            _isTouched = true;
            StartCoroutine(StartReleaseDelay());
        }

        private void SetRandomColor()
        {
            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);
            
            _meshRenderer.material.color = new Color(r, g, b);
        }
        
        private IEnumerator StartReleaseDelay()
        {
            _releaseDelay = Random.Range(MinReleaseDelay, MaxReleaseDelay);
            WaitForSeconds wait = new WaitForSeconds(_releaseDelay);
            
            yield return wait;

            Release();
        }
    }
}