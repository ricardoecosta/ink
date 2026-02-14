using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using HamstasKitties.Mechanics;
using HamstasKitties.Management;
using HamstasKitties.Core.Interfaces;

namespace HamstasKitties.Persistence
{
    [DataContract()]
    public class GameModeState
    {
        public GameModeState()
        {
            CurrentStateSettings = new Dictionary<String, object>();
            StatsSettings = new Dictionary<String, double>();
        }

        /// <summary>
        /// Clears current state.
        /// </summary>
        public void ClearCurrentState()
        {
            CurrentStateSettings.Clear();
        }

        /// <summary>
        /// Tells if object is empty.
        /// </summary>
        /// <returns></returns>
        public bool HasCurrentState()
        {
            return (CurrentStateSettings != null && CurrentStateSettings.Count > 0);
        }

        public T Get<T>(String key)
        {
            T obj = default(T);
            if (key != null && CurrentStateSettings.ContainsKey(key))
            {
                obj = (T)CurrentStateSettings[key];
            }

            return obj;
        }

        public double GetStatistic<T>(String key)
        {
            if (key != null && StatsSettings.ContainsKey(key))
            {
                return StatsSettings[key];
            }
            return 0;
        }

        public void Add(String key, object value)
        {
            if (key != null && value != null)
            {
                if (CurrentStateSettings.ContainsKey(key))
                {
                    CurrentStateSettings.Remove(key);
                }
                CurrentStateSettings.Add(key, value);
            }
        }

        public void AddStatistic(String key, double value)
        {
            if (key != null)
            {
                if (StatsSettings.ContainsKey(key))
                {
                    StatsSettings.Remove(key);
                }
                StatsSettings.Add(key, value);
            }
        }

        [DataMemberAttribute()]
        public Dictionary<String, object> CurrentStateSettings { get; set; }
        [DataMemberAttribute()]
        public Dictionary<String, double> StatsSettings { get; set; }
    }
}
