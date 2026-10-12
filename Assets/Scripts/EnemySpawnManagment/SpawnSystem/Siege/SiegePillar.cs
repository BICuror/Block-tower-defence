using UnityEngine;
using Cashing;
using Combat;

namespace Combat.EnemySpawnSystem.Siege
{
    public sealed class SiegePillar : MonoBehaviour
    {
        [Cached] private EntityHealth _entityHealth;
        [SerializeField] private Collider _collider;
        [SerializeField] private Transform _enabledViewParent;
        [SerializeField] private Transform _mainMeshParent;

        private void Awake()
        {
            _collider.enabled = false;
            _mainMeshParent.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        }

        public void EnablePillar()
        {
            _collider.enabled = true;
            _enabledViewParent.gameObject.SetActive(true);
            _entityHealth.RefilHP();
        }
    }
}