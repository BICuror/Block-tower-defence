using System;

namespace Configs
{
    public struct ArgumentData
    {
        public ArgumentType Type;
        public string FieldName;

        public ArgumentData(Type type, string fieldName)
        {
            Type = ArgumentTypeLookupTable.Table[type];
            
            FieldName = fieldName;
        }
    }
}