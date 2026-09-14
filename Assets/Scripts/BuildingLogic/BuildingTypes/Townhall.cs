using Cysharp.Threading.Tasks;
using UnityEngine;
using Cashing;

namespace Combat
{
    public sealed class Townhall : MonoBehaviour
    {
        [SerializeField] private GameEndScreen _defeatScreen;
        [Cached] private BuildingHealth _buildingHealth;

        private void Start()
        {
            _buildingHealth.Died += OnDeath;
        }
        
        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition + Vector3.up;
        }

        private void OnDeath() => _defeatScreen.Enable().Forget();
    }
}