using System.Collections.Generic;
using UnityEngine;
using EventLinks;
using Coinfigs;
using Configs;
using System;
using Combat;

public static class ScriptableLogicFactory
{
    public static Domain CreateDomain(DomainConfig config, CombatEntity owner = null)
    {
        Domain domain = new Domain();

        ScriptableLogicConstructorArgs args = new ScriptableLogicConstructorArgs();
        args.Domain = domain;
        if (owner) args.Owner = owner;
        
        InitializeDomain(domain, config, args);
        
        return domain;
    }

    public static void InitializeDomain(Domain domain, DomainConfig config, ScriptableLogicConstructorArgs args)
    {
        Dictionary<Type, SubDomain> subDomains = new();
        
        foreach (SubDomainEntry subDomainEntry in config.SubDomainEntries)
        {
            subDomains.Add(subDomainEntry.GetContainedType(), CreateSubDomain(subDomainEntry, args));
        }
        
        domain.SetSubDomains(subDomains);
        
        List<Command> commands = new();
        
        foreach (CommandEntry commandEntry in config.CommandEntries)
        {
            Command command = GenerateCommand(commandEntry, args);
            
            commands.Add(command);
        }
        
        domain.SetCommands(commands);

        List<EventLink> eventLinks = new();
        
        foreach (EventLinkEntry eventEntry in config.EventEntries)
        {
            eventLinks.Add(CreateEventLink(eventEntry, args));
        }
        
        domain.SetEventLinks(eventLinks);
    }

    private static Command GenerateCommand(CommandEntry entry, ScriptableLogicConstructorArgs args)
    {
        Command command = Activator.CreateInstance(entry.GetContainedType()) as Command;

        command.InjectArgumentEntries(entry.Arguments);
        command.Initialize(args, CreateConditionalContainer(entry.ConditionalExecution, args));
        
        return command;
    }
    
    private static EventLink CreateEventLink(EventLinkEntry entry, ScriptableLogicConstructorArgs args)
    {
        EventLink eventLink = Activator.CreateInstance(entry.Config.GetContainedType()) as EventLink;
        
        List<Command> commands = new();
        
        foreach (CommandEntry commandEntry in entry.CommandEntries)
        {
            commands.Add(GenerateCommand(commandEntry, args));
        }

        object executorObject = GetExecutorObjectFromTypeConfig(entry.Config);
        
        eventLink.Initialize(args, commands, CreateConditionalContainer(entry.ConditionalExecution, args), executorObject);
             
        return eventLink;
    }
    
    private static SubDomain CreateSubDomain(SubDomainEntry entry, ScriptableLogicConstructorArgs args)
    {
        SubDomain subDomain = Activator.CreateInstance(entry.Config.GetContainedType()) as SubDomain;
        
        subDomain.Initialize(args);
        subDomain.InjectArgumentEntries(entry.Arguments);
             
        return subDomain;
    }
    
    public static object GetExecutorObjectFromTypeConfig(TypeConfig config)
    {
        if (config.TryGetGenericType(out List<Type> genericTypes))
        {
            return GetExecutorObject(true, genericTypes[0]);
        }
        
        return GetExecutorObject(false, null);
    }

    private static object GetExecutorObject(bool hasArgument, Type argumentType)
    {
        if (!hasArgument) return new TypedExecutor();

        if (argumentType == typeof(Vector3Int)) return new VectorTypedExecutor();
        if (argumentType == typeof(CombatEntity)) return new EntityTypedExecutor();
        if (argumentType == typeof(float)) return new FloatTypedExecutor();
        if (argumentType == typeof(int)) return new IntTypedExecutor();
        
        throw new Exception($"Can't create executor of type {argumentType}");
    }

    public static ConditionalContainer CreateConditionalContainer(ConditionalExecutionEntry entry, ScriptableLogicConstructorArgs constructorArgs)
    {
        List<Condition> conditions = new();

        foreach (ConditionConfigEntry conditionConfigEntry in entry.ConditionEntries)
        {
            conditions.Add(CreateCondition(conditionConfigEntry, constructorArgs));
        }

        return new ConditionalContainer(conditions, entry);
    }

    private static Condition CreateCondition(ConditionConfigEntry config, ScriptableLogicConstructorArgs constructorArgs)
    {
        Condition condition = Activator.CreateInstance(config.GetContainedType()) as Condition;
        
        condition.Initialize(constructorArgs);
        condition.InjectArgumentEntries(config.Arguments);
        
        return condition;
    }
    
    //ideally, it should not be instantiated outside of this class
    public struct ScriptableLogicConstructorArgs
    {
        public CombatEntity Owner;
        public Domain Domain;
    }
}