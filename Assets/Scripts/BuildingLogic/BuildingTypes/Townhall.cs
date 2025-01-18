using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Combat
{
    public sealed class Townhall : MonoBehaviour
    {
        [SerializeField] private DraggableObject[] _draggablesToCreateOnStart;
        [Inject] private DraggableCreator _draggableCreator;
    
        private void Awake()
        {
            GetComponent<IDraggable>().Place();
        }
    
        private async void Start()
        {
            await UniTask.WaitForSeconds(1f);
            
            for (int i = 0; i < _draggablesToCreateOnStart.Length; i++)
            {
                _draggableCreator.CreateDraggableOnRandomPosition(_draggablesToCreateOnStart[i], transform.position, 4);
            }
        }
        
        public void SetPosition(Vector3 newPosition)
        {
            transform.position = newPosition + Vector3.up;
            FindObjectOfType<CameraRotationController>().SetTarget(transform);
        }
    }
}