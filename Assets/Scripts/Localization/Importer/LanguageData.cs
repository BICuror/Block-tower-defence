using System.Collections.Generic;

namespace CuroLocalization
{
    public sealed record LanguageData
    {
        public LanguageData(string name, int column)
        {
            Name = name;  
            Column = column;
        }
        
        public readonly string Name;
        public readonly int Column;

        public readonly Dictionary<string, string> Content = new();
    }
}