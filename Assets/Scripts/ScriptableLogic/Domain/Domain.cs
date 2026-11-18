using System.Collections.Generic;
using ScriptableLogic.SubDomains;
using ScriptableLogic.EventLinks;
using ScriptableLogic.Conditions;
using ScriptableLogic.Commands;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;
using Configs;
using System;

[Serializable] public sealed class DomainConfig : INonScriptableObjectUpdatable
{
    [SerializeField] private List<CommandEntry> _commands;
    [SerializeField] private List<EventLinkEntry> _events;
    [SerializeField] private List<SubDomainEntry> _subDomains;

    [SerializeField, ReadOnly] private List<string> _arguments;
    
    public IReadOnlyList<CommandEntry> CommandEntries => _commands;
    public IReadOnlyList<EventLinkEntry> EventEntries => _events;
    public IReadOnlyList<SubDomainEntry> SubDomainEntries => _subDomains;
    
    public IEnumerable<INonScriptableObjectUpdatable> GetAllUpdatables()
    {
        List<INonScriptableObjectUpdatable> updatables = new() {this};
        
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

    public void Update()
    {
        _arguments.Clear();

        Dictionary<string, ArgumentDataEntry> entries = GetAllArgumentDataEntries();
        
        _arguments = entries.Keys.ToList();
    }

    public Dictionary<string, ArgumentDataEntry> GetAllArgumentDataEntries()
    {
        Dictionary<string, ArgumentDataEntry> entries = new();

        foreach (CommandEntry commandEntry in _commands)
        {
            GetAllArgumentsFromCommand(commandEntry);
        }
        
        foreach (EventLinkEntry eventLinkEntry in _events)
        {
            GetAllArgumentsFromEvent(eventLinkEntry);
        }
        
        foreach (SubDomainEntry subDomainEntry in _subDomains)
        {
            GetAllArgumentsFromSubDomains(subDomainEntry);
        }
        
        return entries;
        
        void GetAllArgumentsFromSubDomains(SubDomainEntry entry)
        {
            AddArgumentsRange(entry.Arguments, entry.GetArgumentPrefix());
        }

        void GetAllArgumentsFromEvent(EventLinkEntry entry)
        {
            string prefix = entry.Config.GetContainedType().Name;
            
            AddArgumentsRange(entry.Arguments, prefix);
            GetAllArgumentsFromConditionalExecution(entry.ConditionalExecution, prefix);
            
            foreach (CommandEntry commandEntry in entry.CommandEntries)
            {
                GetAllArgumentsFromCommand(commandEntry, prefix);
            }
        }

        void GetAllArgumentsFromCommand(CommandEntry entry, string prefix = "")
        {
            AddArgumentsRange(entry.Arguments, entry.GetArgumentPrefix());

            GetAllArgumentsFromConditionalExecution(entry.ConditionalExecution, $"{prefix}_{entry.GetArgumentPrefix()}");
        }

        void GetAllArgumentsFromConditionalExecution(ConditionalExecutionEntry entry, string prefix = "")
        {
            foreach (ConditionConfigEntry conditionConfigEntry in entry.ConditionEntries)
            {
                AddArgumentsRange(conditionConfigEntry.Arguments, prefix);
            }
        }

        void AddArgumentsRange(IEnumerable<ArgumentDataEntry> argumentDataEntries, string prefix = "")
        {
            foreach (ArgumentDataEntry argumentDataEntry in argumentDataEntries)
            {
                string key = $"{prefix}_{argumentDataEntry.Name}";
                
                if (entries.ContainsKey(key)) Debug.LogWarning($"Didn't add {key}, because it already exists.");
                else entries.Add(key, argumentDataEntry);
            }
        }
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