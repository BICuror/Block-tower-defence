using System;

namespace CuroSettings
{
    public abstract class Setting
    {
        protected readonly ISettingsSaveLoader SaveLoader;
        protected readonly string Key;
        
        public Action ValueChanged;
        public Action ValueLoaded;
    
        protected Setting(ISettingsSaveLoader saveLoader, string key)
        {
            SaveLoader = saveLoader;
            Key = key;
        }
        
        public abstract void Load();
        public abstract void Save();
    }
}