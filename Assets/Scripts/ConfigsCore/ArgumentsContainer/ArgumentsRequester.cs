using System.Collections.Generic;
using System.Reflection;

namespace Configs
{
    public abstract class ArgumentsRequester
    {
        public List<ArgumentData> GetAllArgumentsData()
        {
            List<ArgumentData> argumentDatas = new();
            
            foreach (FieldInfo fieldInfo in GetType().GetAllFieldsFromThisAndDerivedTypes())
            {
                ArgumentAttribute argumentAttribute = fieldInfo.GetCustomAttribute(typeof(ArgumentAttribute)) as ArgumentAttribute;
                
                if (argumentAttribute == null) continue;

                argumentDatas.Add(new(fieldInfo.FieldType, argumentAttribute.FieldName));
            }

            return argumentDatas;
        }

        public void InjectArgumentEntries(IReadOnlyList<ArgumentDataEntry> argumentEntries)
        {
            IEnumerable<FieldInfo> fieldInfos = GetType().GetAllFieldsFromThisAndDerivedTypes();
            
            Dictionary<string, ArgumentDataEntry> argumentDataEntries = GetArgumentEntriesAsDictionary(argumentEntries);
            
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                ArgumentAttribute argumentAttribute = fieldInfo.GetCustomAttribute(typeof(ArgumentAttribute)) as ArgumentAttribute;
                
                if (argumentAttribute == null) continue;
                
                fieldInfo.SetValue(this, argumentDataEntries[argumentAttribute.FieldName].GetValue(fieldInfo.FieldType));
            }
        }

        private Dictionary<string, ArgumentDataEntry> GetArgumentEntriesAsDictionary(IReadOnlyList<ArgumentDataEntry> argumentEntries)
        {
            Dictionary<string, ArgumentDataEntry> dictionary = new();

            foreach (ArgumentDataEntry argumentDataEntry in argumentEntries)
            {
                dictionary.Add(argumentDataEntry.Name, argumentDataEntry);
            }
            
            return dictionary;
        }
    }
}