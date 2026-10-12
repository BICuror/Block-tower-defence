using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Linq;
using EventLinks;
using Coinfigs;
using Configs;
using System;

[Serializable] public sealed class DomainConfig
{
    [SerializeField] private List<CommandEntry> _commands;
    [SerializeField] private List<EventLinkEntry> _events;
    [SerializeField] private List<SubDomainEntry> _subDomains;
    
    public IReadOnlyList<CommandEntry> CommandEntries => _commands;
    public IReadOnlyList<EventLinkEntry> EventEntries => _events;
    public IReadOnlyList<SubDomainEntry> SubDomainEntries => _subDomains;
    
    public IEnumerable<INonScriptableObjectUpdatable> GetAllUpdatables()
    {
        List<INonScriptableObjectUpdatable> updatables = new();

        foreach (CommandEntry commandEntry in _commands)
        {
            updatables.AddRange(commandEntry.GetAllUpdatables());
        }

        foreach (EventLinkEntry eventLinkEntry in _events)
        {
            updatables.AddRange(eventLinkEntry.GetAllUpdatables());
        }
        
        foreach (SubDomainEntry subDomainEntry in _subDomains)
        {
            updatables.AddRange(subDomainEntry.GetAllUpdatables());
        }
        
        return updatables;
    }
}

public sealed class Domain
{
    private List<Command> _commands;
    private List<EventLink> _events;
    private Dictionary<Type, SubDomain> _subDomains;

    public void SetCommands(List<Command> commands)
    {
        _commands = commands;
    }
    
    public void SetEventLinks(List<EventLink> eventLinks)
    {
        _events = eventLinks;
    }

    public void SetSubDomains(Dictionary<Type, SubDomain> subDomains)
    {
        _subDomains = subDomains;
    }

    public IEnumerable<object> GetAllObjectsForInjection()
    {
        foreach (Command command in _commands)
        {
            yield return command;

            yield return command.ConditionalContainer.GetAllInjectableObjects();
        }
        
        foreach (EventLink eventLink in _events)
        {
            yield return eventLink;

            yield return eventLink.GetAllInjectableObjects();
        }
        
        foreach (SubDomain subDomain in _subDomains.Values)
        {
            yield return subDomain;
        }
    }
    
    public T GetSubDomain<T>() where T : SubDomain
    {
        return _subDomains[typeof(T)] as T;
    }

    public async UniTask Enable()
    {
        foreach (SubDomain subDomainsValue in _subDomains.Values)
        {
            await subDomainsValue.Enable();
        }
        
        foreach (EventLink eventLink in _events)
        {
            eventLink.Enable();
        }

        foreach (Command command in _commands)
        {
            await command.Enable();
        }
    }
    
    public async UniTask Disable()
    {
        foreach (SubDomain subDomainsValue in _subDomains.Values)
        {
            await subDomainsValue.Disable();
        }
        
        foreach (EventLink eventLink in _events)
        {
            eventLink.Disable();
        }

        foreach (Command command in _commands)
        {
            await command.Disable();
        }
    }
}