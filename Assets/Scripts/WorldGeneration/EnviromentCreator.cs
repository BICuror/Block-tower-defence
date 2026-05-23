using UnityEngine.Events;
using UnityEngine;
using Zenject;
using Combat;

namespace WorldGeneration
{
    public sealed class EnviromentCreator : MonoBehaviour
    {
        [SerializeField] private Townhall _townhall;
        [Inject] private IslandDataContainer _islandDataContainer;
        [Inject] private DiContainer _diContainer;
        
        private IslandData _islandData => _islandDataContainer.Data;

        public UnityEvent<Vector3> CenterSet;

        private GameObject _eniviorment;

        public void CreateEnviroment(Vector3 center)
        {
            CenterSet.Invoke(center);
            _townhall.SetPosition(center);

            if (_eniviorment != null) Destroy(_eniviorment);

            _eniviorment = _diContainer.InstantiatePrefab(_islandData.EniviromentObject, new Vector3(center.x, 0f, center.z), Quaternion.identity, null);
        }
    }
}