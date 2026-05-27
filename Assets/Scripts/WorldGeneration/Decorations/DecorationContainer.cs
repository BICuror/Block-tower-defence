 using NaughtyAttributes;
using UnityEngine;

namespace WorldGeneration
{
    public class DecorationContainer : MonoBehaviour
    {
        [SerializeField] private int _maxDecorationRadius = 1;
        [SerializeField] private bool _useRaycastToDeactivateDecorations;
        [ShowIf("_useRaycastToDeactivateDecorations")] [SerializeField] private LayerSetting _decorationLayerSetting;
        private ListDictionary<Vector2Int, DecorationObject> _decorations = new();
        
        public void CreateNewContainer()
        {
            DestroyAllDecorations();

            _decorations = new();
        } 

        public void AddDecorations(int x, int z, DecorationObject decoration)
        {
            _decorations.Add(new Vector2Int(x, z), decoration);
        }

        public void ActivateAllDecorations()
        {
            _decorations.GetAllItems().ForEach(decorationObject => decorationObject.SetState(true));
        }

        public void SetActiveDecorations(int x, int z, bool state)
        {
            Vector2Int position = new Vector2Int(x, z);

            if (_useRaycastToDeactivateDecorations && !state)
            {
                TileMap.GetHitObjects(new Vector2Int(x, z), _decorationLayerSetting).ForEach(decorationGameObject =>
                {
                    DecorationObject decorationObject = decorationGameObject.GetComponent<DecorationObject>();
                                        
                    if (_decorations.Contains(position) && !_decorations.Get(position).Contains(decorationObject))
                    {
                        decorationObject.SetState(false);
                    }
                });
            }
            
            if (!_decorations.Contains(position)) return;
            
            _decorations.Get(position).ForEach(decorationObject => decorationObject.SetState(state));
        }
        
        public void ApplyMaterialToAllDecorations(Material materialToApply)
        {
            _decorations.GetAllItems().ForEach(decorationObject => decorationObject.SetMaterial(materialToApply));
        }
        
        private void DestroyAllDecorations()
        { 
            _decorations.GetAllItems().ForEach(decorationObject => Destroy(decorationObject.gameObject));
        }
    }
}