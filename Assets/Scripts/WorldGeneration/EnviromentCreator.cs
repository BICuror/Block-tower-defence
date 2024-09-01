using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace WorldGeneration
{
    public sealed class EnviromentCreator : MonoBehaviour
    {
        [Inject] private IslandDataContainer _islandDataContainer;
        private IslandData _islandData => _islandDataContainer.Data;

        public UnityEvent<Vector3> CenterSet;

        private GameObject _eniviorment;

        public void CreateEnviroment(Vector3 center)
        {
            CenterSet.Invoke(center);

            if (_eniviorment != null) Destroy(_eniviorment); 

            _eniviorment = Instantiate(_islandData.EniviromentObject, new Vector3(center.x, 0f, center.z), Quaternion.identity);
        }
    }
}
