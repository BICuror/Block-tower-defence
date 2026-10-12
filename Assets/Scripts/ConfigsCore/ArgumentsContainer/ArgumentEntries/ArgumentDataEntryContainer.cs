using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System;

namespace Configs
{
    [Serializable] public abstract class ArgumentDataEntryContainer : INonScriptableObjectUpdatable
    {
        [HideInInspector] public string Name = "Non defined";
        
        [SerializeField, AllowNesting, EnableIf("DisplayArguments"), NonReorderable]
        private List<ArgumentDataEntry> _arguments;

        public IReadOnlyList<ArgumentDataEntry> Arguments => _arguments;

        protected abstract bool DisplayArguments { get; }
        protected abstract Type GetCurrentTargetType();

        public virtual List<INonScriptableObjectUpdatable> GetAllUpdatables()
        {
            List<INonScriptableObjectUpdatable> updatables = new() { this };

            return updatables;
        }

        public virtual void Update() => UpdateArguments();

        private void UpdateArguments()
        {
            if (!DisplayArguments)
            {
                _arguments.Clear();
                return;
            }

            Type targetType = GetCurrentTargetType();

            ArgumentsRequester targetInstance = Activator.CreateInstance(targetType) as ArgumentsRequester;

            if (targetInstance == null) Debug.LogError($"Target type {GetCurrentTargetType()} is not a valid or does not exist");

            List<ArgumentData> requiredArguments = targetInstance.GetAllArgumentsData();

            List<ArgumentDataEntry> distinctEntries = GetAllDistinctEntries(requiredArguments);
            List<ArgumentData> distinctDatas = GetAllDistinctDatas(requiredArguments);

            bool allArgumentsAreValid = requiredArguments.Count == _arguments.Count && distinctEntries.Count == 0 &&
                                        distinctDatas.Count == 0;

            if (allArgumentsAreValid) return;

            RemoveEntries(distinctEntries);

            AddRequiredEntries(distinctDatas);
        }

        private List<ArgumentDataEntry> GetAllDistinctEntries(List<ArgumentData> allData)
        {
            List<ArgumentDataEntry> distinctEntries = new();

            foreach (ArgumentDataEntry entry in _arguments)
            {
                if (!allData.Exists(data => DeclaresSameArgument(entry, data)))
                {
                    distinctEntries.Add(entry);
                }
            }

            return distinctEntries;
        }

        private List<ArgumentData> GetAllDistinctDatas(List<ArgumentData> allData)
        {
            List<ArgumentData> distinctData = new();

            foreach (ArgumentData data in allData)
            {
                if (!_arguments.Exists(entry => DeclaresSameArgument(entry, data)))
                {
                    distinctData.Add(data);
                }
            }

            return distinctData;
        }

        private void RemoveEntries(List<ArgumentDataEntry> exessiveEntires)
        {
            foreach (ArgumentDataEntry argumentDataEntry in exessiveEntires)
            {
                _arguments.Remove(argumentDataEntry);
            }
        }

        private void AddRequiredEntries(List<ArgumentData> exsessiveDatas)
        {
            foreach (ArgumentData exsessiveData in exsessiveDatas)
            {
                ArgumentDataEntry entry = new ArgumentDataEntry();
                entry.ArgumentType = exsessiveData.Type;
                entry.Name = exsessiveData.FieldName;

                _arguments.Add(entry);
            }
        }

        private bool DeclaresSameArgument(ArgumentDataEntry entry, ArgumentData data) => entry.ArgumentType == data.Type && entry.Name == data.FieldName;
    }
}