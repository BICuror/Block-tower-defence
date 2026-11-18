using System.Collections.Generic;
using System.Reflection;
using System;

namespace Configs
{
    public abstract class ArgumentsRequester
    {
        public List<ArgumentData> GetAllArgumentsData()
        {
            List<ArgumentData> argumentDatas = new();
            
            GetArgumentDataFromType(GetType());

            return argumentDatas;

            void GetArgumentDataFromType(Type type, string prefix = "")
            {
                foreach (FieldInfo fieldInfo in type.GetAllFieldsFromThisAndDerivedTypes())
                {
                    ArgumentAttribute argumentAttribute = fieldInfo.GetCustomAttribute(typeof(ArgumentAttribute)) as ArgumentAttribute;

                    if (argumentAttribute != null)
                    {
                        argumentDatas.Add(new(fieldInfo.FieldType, prefix + argumentAttribute.FieldName));
                        continue;
                    }
                    
                    ArgumentItemAttribute argumentItemAttribute = fieldInfo.GetCustomAttribute(typeof(ArgumentItemAttribute)) as ArgumentItemAttribute;

                    if (argumentItemAttribute != null)
                    {
                        GetArgumentDataFromType(fieldInfo.FieldType, argumentItemAttribute.FieldName);
                    }
                }
            }
        }

        public void InjectArgumentEntries(IReadOnlyList<ArgumentDataEntry> argumentEntries)
        {
            Dictionary<string, ArgumentDataEntry> argumentDataEntries = GetArgumentEntriesAsDictionary(argumentEntries);
            
            InjectArgumentDataEntries(this);

            return;

            void InjectArgumentDataEntries(object injectableObject, string prefix = "")
            {
                foreach (FieldInfo fieldInfo in injectableObject.GetType().GetAllFieldsFromThisAndDerivedTypes())
                {
                    ArgumentAttribute argumentAttribute = fieldInfo.GetCustomAttribute(typeof(ArgumentAttribute)) as ArgumentAttribute;
                    
                    if (argumentAttribute != null)
                    {
                        fieldInfo.SetValue(injectableObject, argumentDataEntries[prefix + argumentAttribute.FieldName].GetValue(fieldInfo.FieldType));
                        continue;
                    }
                    
                    ArgumentItemAttribute argumentItemAttribute = fieldInfo.GetCustomAttribute(typeof(ArgumentItemAttribute)) as ArgumentItemAttribute;

                    if (argumentItemAttribute != null)
                    {
                        InjectArgumentDataEntries(fieldInfo.GetRawConstantValue(), argumentItemAttribute.FieldName);
                    }
                }
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