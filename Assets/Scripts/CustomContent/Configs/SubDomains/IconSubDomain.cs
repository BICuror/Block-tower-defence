using Cysharp.Threading.Tasks;
using Combat.IngameUI;
using UnityEngine;
using Configs;

public sealed class IconSubDomain : SubDomain
{
    [Argument("IconSprite")] private Sprite _iconSprite;
    [Argument("HasValue")] private bool _hasValue;
    
    private EntityCanvasIcon _icon;
    
    public EntityCanvasIcon Icon => _icon;

    public override UniTask Enable()
    {
        _icon = Args.Owner.ComponentsContainer.Get<EntityCanvas>().AddIcon(_iconSprite, _hasValue);
        
        return UniTask.CompletedTask;
    }
    
    public override UniTask Disable()
    { 
        Args.Owner.ComponentsContainer.Get<EntityCanvas>().RemoveIcon(_icon);
        
        return UniTask.CompletedTask;
    }
}

public sealed class SetIconValueCommand : SubDomainCommand<IconSubDomain>, ICommand<int>
{
    public UniTask Execute(int args)
    {
        SubDomain.Icon.SetValue(args);
        
        return UniTask.CompletedTask;
    }
}