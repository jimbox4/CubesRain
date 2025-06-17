using System;
using System.Collections;
using System.Collections.Generic;
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
        
        [SerializeField] private Material _materialPrefab;
        [SerializeField] private LayerMask _boxLayerMask;

        public override event Action<Cube> Released;

        private MeshRenderer _meshRenderer;
        private Rigidbody _rigidbody;
        private BoxCollider _collider;

        private float _releaseDelay;
        private bool _isTouched = false;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _meshRenderer = GetComponent<MeshRenderer>();
            _meshRenderer.material = new Material(_materialPrefab);
            _rigidbody = GetComponent<Rigidbody>();
        }

        public override void Release()
        {
            Released?.Invoke(this);
        }

        public override void SetPosition(Vector3 position)
        {
            transform.position = position;
        }

        public override void SetEnable()
        {
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

            _isTouched = true;
            StartCoroutine(StartReleaseDelay());
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