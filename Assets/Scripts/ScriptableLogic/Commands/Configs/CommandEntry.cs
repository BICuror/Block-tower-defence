using System.Linq;
using Configs;
using System;

namespace ScriptableLogic.Commands
{
    [Serializable] public sealed class CommandEntry : ConfigEntryWithConditionalExecution<CommandConfig>
    {
        public override string GetArgumentPrefix()
        {
            string prefix = base.GetArgumentPrefix();
            
            ArgumentDataEntry statArgumentDataEntry = Arguments.FirstOrDefault(entry => entry.ArgumentType == Configs.ArgumentType.StatConfig);

            if (statArgumentDataEntry.ArgumentType == Configs.ArgumentType.StatConfig)
            {
                prefix = $"{prefix}_{statArgumentDataEntry.StatConfigValue.GetStatType().Name}";
            }
            
            return prefix;
        }
    }
}