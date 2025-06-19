using System;
using System.Collections;
using CodeBase.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.Bombs
{
    [RequireComponent(typeof(Rigidbody))]
    public class Bomb : PoolableObject<Bomb>
    {
        private const float MaxExplosionDelay = 5f;
        private const float MinExplosionDelay = 2f;

        [Header("Explosion")] [SerializeField] private float _explosionRadius;
        [SerializeField] private LayerMask _objectsLayer;

        [Header("Components")] [SerializeField]
        private MeshRenderer _bombMeshRenderer;

        [SerializeField] private MeshRenderer _wickMeshRenderer;
        [SerializeField] private Material _material;

        [SerializeField] private Collider _collider;
        [SerializeField] private ParticleSystem _wickParticleSystem;
        [SerializeField] private ParticleSystem _explosionParticleSystem;

        public override event Action<Bomb> Released;

        private Rigidbody _rigidbody;

        private readonly float _releaseDelay = 1f;
        private readonly float _explosionForce = 5000f;

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _bombMeshRenderer.material = new Material(_material);
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
            SetAlpha(_bombMeshRenderer);
            SetAlpha(_wickMeshRenderer);

            _wickParticleSystem.Play();
            _wickMeshRenderer.enabled = true;
            _bombMeshRenderer.enabled = true;
            _collider.enabled = true;
            _rigidbody.constraints = RigidbodyConstraints.None;
            transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

            StartCoroutine(StartExplosionDelay());
        }

        public override void SetDisable()
        {
            _wickParticleSystem.Stop();
            _collider.enabled = false;
            _wickMeshRenderer.enabled = false;
            _bombMeshRenderer.enabled = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        private void Explode()
        {
            var colliders = Physics.OverlapSphere(transform.position, _explosionRadius, _objectsLayer);

            foreach (Collider collider in colliders)
            {
                Rigidbody component = collider.GetComponent<Rigidbody>();
                component.AddExplosionForce(_explosionForce, transform.position, _explosionRadius);
            }
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator StartExplosionDelay()
        {
            float explosionDelay = Random.Range(MinExplosionDelay, MaxExplosionDelay);
            float explosionTime = Time.time + explosionDelay;
            float startTime = Time.time;
            float currentAlpha; 

            while (Time.time < explosionTime)
            {
                currentAlpha = 1f / (explosionTime - startTime) * (explosionTime - Time.time);
                SetAlpha(_bombMeshRenderer, currentAlpha);
                SetAlpha(_wickMeshRenderer, currentAlpha);
                yield return null;
            }

            _wickParticleSystem.Stop();
            Explode();
            _explosionParticleSystem.Play();

            StartCoroutine(StartReleaseDelay());
        }

        private IEnumerator StartReleaseDelay()
        {
            WaitForSeconds wait = new WaitForSeconds(_releaseDelay);

            SetDisable();

            yield return wait;

            Release();
        }

        private void SetAlpha(MeshRenderer renderer, float value, float minValue, float maxValue)
        {
            float alpha = 1f / maxValue * (maxValue-value);
            Color color = renderer.material.color;
            renderer.material.color = new Color(color.r, color.g, color.b, alpha);
            //renderer.material.color = new Color(color.r, color.g, color.b, alpha);
        }

        private void SetAlpha(MeshRenderer renderer, float alpha = 1f)
        {
            Color color = renderer.material.color;
            renderer.material.color = new Color(color.r, color.g, color.b, alpha);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
    }
}