using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameLibrary.Core;
using GameLibrary.Core.Serializers;

namespace GameLibrary.Core
{
    public class SettingsManager : IManager, IApplicationDeactivationAware, IApplicationActivationAware
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public SettingsManager(Director director)
        {
            Director = director;
            Filename = DefaultFileName;

            Data = new Dictionary<string, object>();
            KnownTypes = new List<Type>();
        }

        /// <summary>
        /// Saves the given setting with given value.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="value"></param>
        public void SaveSetting(String id, object value)
        {
            if (id != null && value != null)
            {
                if (!Data.ContainsKey(id))
                {
                    Data.Add(id, value);
                }

                Data[id] = value;
                
                if (Serializer != null)
                {
                    Serializer.AddKnownType(value.GetType());
                }
            }
        }

        /// <summary>
        /// Loads saved setting 
        /// </summary>
        /// <param name="setting"></param>
        /// <returns> returns the loaded object or null is setting not exists.</returns>
        public T LoadSetting<T>(String id)
        {
            T obj = default(T);
            if (Data.ContainsKey(id))
            {
                obj = (T)Data[id];
            }

            return obj;
        }

        /// <summary>
        /// Test if the given id exists on settings. 
        /// </summary>
        /// <param name="setting"></param>
        /// <returns> returns the loaded object or null is setting not exists.</returns>
        public bool ContainsSetting(String id)
        {
            return Data.ContainsKey(id);
        }

        /// <summary>
        /// Remove the data by given key.
        /// </summary>
        /// <param name="setting"></param>
        public void RemoveSetting(String id)
        {
            Data.Remove(id);
        }

        /// <summary>
        /// Adds new Known Type for serialization.
        /// </summary>
        /// <param name="type"></param>
        public void AddKnownType(Type type)
        {
            if (type != null && !KnownTypes.Contains(type))
            {
                KnownTypes.Add(type);
                if (Serializer != null)
                {
                    Serializer.AddKnownType(type);
                }
            }
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
            LoadAndDeserialise();
        }

        public void RegisterApplicationActivationAwareComponent(Director director)
        {
            director.RegisterApplicationActivationAwareComponent(this);
        }

        public void PersistCurrentState()
        {
            if (Serializer != null)
            {
                Serializer.Save(Data);
            }
        }

        public void RegisterApplicationDeactivationAwareComponent(Director director)
        {
            director.RegisterApplicationDeactivationAwareComponent(this);
        }

        /// <summary>
        /// Create the method to save the data.
        /// </summary>
        private void LoadAndDeserialise()
        {
            Serializer = new DataContractSerializer<Dictionary<String, Object>>(Filename);

            foreach (Type type in KnownTypes)
            {
                Serializer.AddKnownType(type);
            }

            Data = Serializer.Load();

            if (Data == null)
            {
                Data = new Dictionary<string, object>();
            }
            else // fill the known types with a loaded data.
            {
                foreach (Object obj in Data.Values)
                {
                    AddKnownType(obj.GetType());
                }
            }
        }

        #region Properties

        private AbstractSerializer<Dictionary<String, Object>> Serializer { get; set; }
        private Director Director { get; set; }
        private Dictionary<String, Object> Data { get; set; }
        private String Filename { get; set; }
        private List<Type> KnownTypes { get; set; }

        private static readonly String DefaultFileName = "DS-Settings.dat";
        
        #endregion
    }
}
