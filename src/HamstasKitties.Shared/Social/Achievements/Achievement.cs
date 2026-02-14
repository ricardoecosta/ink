#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.Core;
using System.Runtime.Serialization;

namespace HamstasKitties.Social.Achievements
{
    /// <summary>
    /// Represents an generic Achievement.
    /// </summary>
    [DataContract]
    public abstract class Achievement
    {
        [DataContract]
        public enum AchievementType
        {
            [EnumMember(Value = "Normal")]
            Normal, //executed only on after level finished.

            [EnumMember(Value = "Runtime")]
            Runtime // executed on every game loop.
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="id">Id of the Achievement.</param>
        /// <param name="name">Name of the Achievement.</param>
        /// <param name="description">Description of the Achievement.</param>
        public Achievement(String id, AchievementType type, String name, String description, int reward, int importance)
        {
            Data = new AchievementData();
            Data.Id = id;
            Type = type;
            Data.Name = name;
            Data.Description = description;
            Data.Reward = reward;
            Data.Importance = importance;
            Data.Completed = false;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="data">Id of the Achievement.</param>
        /// <param name="type">Type of the Achievement.</param>
        public Achievement(AchievementData data, AchievementType type)
        {
            Data = data;
            Type = type;
        }

        /// <summary>
        /// Updates the achievement status.
        /// </summary>
        /// <param name="time">Game Time. Should be used only on Runtime achievements. For other types of achievements can be null</param>
        /// <param name="director">Current Director.</param>
        public abstract void Update(TimeSpan time, Director director);

        /// <summary>
        /// Marks the achievement as completed.
        /// </summary>
        public void SetCompleted()
        {
            Data.Completed = true;
            if (OnAchievementCompleted != null)
            {
                OnAchievementCompleted(this);
            }
        }

        public override bool Equals(object obj)
        {
            Achievement achievement = obj as Achievement;

            if (achievement != null)
            {
                return achievement.Data.Equals(Data);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Data != null ? Data.GetHashCode() : 0;
        }

        public delegate void OnAchievementCompletedHandler(Achievement achievement);
        public event OnAchievementCompletedHandler OnAchievementCompleted;

        [DataMember]
        public AchievementType Type { get; set; }

        [DataMember]
        public AchievementData Data { get; set; }
    }
}
