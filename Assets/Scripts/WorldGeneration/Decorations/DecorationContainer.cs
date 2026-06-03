using UnityEngine;

namespace WorldGeneration
{
    public class DecorationContainer : MonoBehaviour
    {
        [SerializeField] private int _maxDecorationRadius = 1;
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

        public void UpdateDecorationsState()
        {
            _decorations.GetAllKeys().ForEach(position =>
            {
                int highestDecorationRadius = GetHightestPossibleDecorationRadius(position, false);
                
                _decorations.Get(position).ForEach(decoration => decoration.SetState(decoration.TileRadius < highestDecorationRadius));
            });
        }

        public int GetHightestPossibleDecorationRadius(Vector2Int position, bool includeOtherDecorations = true)
        {
            int minimalValue = int.MaxValue;
            
            for (int x = -_maxDecorationRadius; x <= _maxDecorationRadius; x++)
            {
                for (int z = -_maxDecorationRadius; z <= _maxDecorationRadius; z++)
                {
                    Vector2Int checkPosition = new Vector2Int(position.x + x, position.y + z);

                    int currentRadius = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
                    
                    if (IsOccupied(checkPosition)) minimalValue = Mathf.Min(currentRadius, minimalValue);
                }
            }

            return minimalValue;

            bool IsOccupied(Vector2Int checkPosition) =>
                TileMap.HasTile(checkPosition, LayerSettingType.DecorationExclusionLayer) ||
                (includeOtherDecorations && TileMap.HasTile(checkPosition, LayerSettingType.BlockingDecoration));
        }

        private void DisableAllDecorationsAround(Vector2Int position)
        {
            for (int x = -_maxDecorationRadius; x <= _maxDecorationRadius; x++) 
            { 
                for (int z = -_maxDecorationRadius; z <= _maxDecorationRadius; z++) 
                { 
                    Vector2Int checkPosition = new Vector2Int(position.x + x, position.y + z);
                    
                    if (!_decorations.Contains(checkPosition)) continue;
                    
                    int currentRadius = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
                    
                    _decorations.Get(checkPosition).ForEach(decorationObject =>
                    {
                        if (decorationObject.TileRadius >= currentRadius)
                        {
                            decorationObject.SetState(false);
                        }
                    });
                }
            }
        }

        public void SetActiveDecorations(int x, int z, bool state)
        {
            Vector2Int position = new Vector2Int(x, z);
            
            if (!state) DisableAllDecorationsAround(position);
            
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