using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat
{
    public sealed class Townhall : MonoBehaviour
    {
        [SerializeField] private List<BuildingEntity> _registeredBuildings;
        [SerializeField] private DraggableObject[] _draggablesToCreateOnStart;
        [Inject] private DraggableCreator _draggableCreator;
        [Inject] private ItemFactory _itemFactory;
        [Inject] private GlobalBuildingContainer _globalBuildingContainer;
        
        private async void Start()
        {
            _registeredBuildings.ForEach(building => _globalBuildingContainer.Add(building));
            
            await UniTask.WaitForSeconds(1f);
            
            for (int i = 0; i < _draggablesToCreateOnStart.Length; i++)
            {
                _draggableCreator.CreateDraggableOnRandomPosition(_draggablesToCreateOnStart[i], transform.position, 4);
            }
            
            _itemFactory.CreateItem(3, transform.position);
            
        }
        
        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition + Vector3.up;
        }
    }
}