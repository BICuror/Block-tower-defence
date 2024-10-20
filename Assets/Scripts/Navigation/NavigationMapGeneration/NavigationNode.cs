using System.Collections.Generic;
using UnityEngine;
using System.Collections;

namespace Navigation
{
    public sealed class NavigationNode
    {
        public NavigationNode(Vector3 position)
        {   
            _position = position;
        }

        private Vector3 _position;
        private List<NavigationNode> _defaultNodes = new();
        private List<INavigationCondition> _presentConditions = new();
        private Dictionary<INavigationCondition, List<NavigationNode>> _optionalNodes = new();
        
        public Vector3 Position => _position;

        public NavigationNode GetNextNode() 
        {
            for (int i = 0; i < _presentConditions.Count; i++)
            {
                INavigationCondition currentCondition = _presentConditions[i];

                if (currentCondition.GetValue() == true)
                {
                    List<NavigationNode> optionalNodes = _optionalNodes[currentCondition];

                    return optionalNodes[Random.Range(0, optionalNodes.Count)];
                }
            }
            
            return GetRandomDefaultNode();
        }

        public void AddDefaultNode(NavigationNode node) => _defaultNodes.Add(node);
        public void AddOptionalNode(INavigationCondition condition, NavigationNode node)
        {
            if (_presentConditions.Contains(condition) == false)
            {
                _presentConditions.Add(condition);
                _optionalNodes.Add(condition, new());
            }

            _optionalNodes[condition].Add(node);
        }

        public NavigationNode GetRandomDefaultNode() => _defaultNodes[Random.Range(0, _defaultNodes.Count)];

        public bool ContainsDefaultNode(NavigationNode node) => _defaultNodes.Contains(node);
        public bool ContainsOptionalNode(INavigationCondition condition, NavigationNode node)
        {
            return _presentConditions.Contains(condition) && _optionalNodes[condition].Contains(node);
        }
    }
}
    