using System;
using System.Collections.Generic;
using HamstasKitties.Core.Interfaces;
using HamstasKitties.Persistence;

namespace HamstasKitties.Core
{
    public class SettingsManager : ISettingsManager, IApplicationDeactivationAware, IApplicationActivationAware
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SettingsManager(Director director)
        {
            Director = director;
            Data = new Dictionary<string, object>();
            KnownTypes = new List<Type>();
        }

        /// <summary>
        /// Saves given setting with given value.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="value"></param>
        public void SaveSetting(string id, object value)
        {
            if (id != null && value != null)
            {
                if (!Data.ContainsKey(id))
                {
                    Data.Add(id, value);
                }

                Data[id] = value;
            }
        }

        /// <summary>
        /// Loads saved setting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns> returns the loaded object or null if setting not exists.</returns>
        public T LoadSetting<T>(string id)
        {
            T obj = default(T);
            if (Data.ContainsKey(id))
            {
                obj = (T)Data[id];
            }

            return obj;
        }

        /// <summary>
        /// Test if given id exists on settings.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns> returns the loaded object or null if setting not exists.</returns>
        public bool ContainsSetting(string id)
        {
            return Data.ContainsKey(id);
        }

        /// <summary>
        /// Remove the data by given key.
        /// </summary>
        /// <param name="setting"></param>
        public void RemoveSetting(string id)
        {
            Data.Remove(id);
        }

        #region IManager Implementation

        public bool Initialize()
        {
            return true;
        }

        public bool Finalize()
        {
            return true;
        }

        #endregion

        public void LoadPersistedState()
        {
            // TODO: Implement persistence loading
        }

        public void RegisterApplicationActivationAwareComponent(Director director)
        {
            director.RegisterApplicationActivationAwareComponent(this);
        }

        public void PersistCurrentState()
        {
            // TODO: Implement persistence saving
        }

        public void RegisterApplicationDeactivationAwareComponent(Director director)
        {
            director.RegisterApplicationDeactivationAwareComponent(this);
        }

        #region Properties

        private Director Director { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public List<Type> KnownTypes { get; set; }

        #endregion
    }
}
