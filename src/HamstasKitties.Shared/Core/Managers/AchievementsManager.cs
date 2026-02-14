#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using HamstasKitties.Core.Interfaces;
using HamstasKitties.Social.Achievements;

namespace HamstasKitties.Core
{
    /// <summary>
    /// Manages all achievements of the game.
    /// </summary>
    public class AchievementsManager : IAchievementService
    {
        private const string AchievementsSettingsKey = "DS-Achievements";

        public AchievementsManager(Director director)
        {
            Director = director;
            AlreadyProcessed = false;
            AlreadySubscribed = false;
        }

        /// <summary>
        /// Adds an achievement to the manager.
        /// </summary>
        /// <param name="achievement">The achievement to add.</param>
        public void AddAchievement(Achievement achievement)
        {
            if (achievement != null)
            {
                if (!Achievements.ContainsKey(achievement.Type))
                {
                    Achievements.Add(achievement.Type, new List<Achievement>());
                }

                achievement.OnAchievementCompleted += new Achievement.OnAchievementCompletedHandler(AchievementCompletedHandler);
                Achievements[achievement.Type].Add(achievement);
            }
        }

        /// <summary>
        /// Gets the achievement by Id
        /// </summary>
        /// <param name="id">ID to verify.</param>
        /// <returns>Achievement object or null if achievement not exists.</returns>
        public Achievement GetAchievementByIdLegacy(String id)
        {
            if (id != null && id.Length > 0)
            {
                foreach (Achievement.AchievementType type in Achievements.Keys)
                {
                    foreach (Achievement achievement in Achievements[type])
                    {
                        if (achievement.Data.Id == id)
                        {
                            return achievement;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Processes all incompleted achievements by given type.
        /// </summary>
        /// <param name="type">Achievements type to be processed.</param>
        /// <param name="time">Game time. For some achievements can be necessary.</param>
        public void ProcessAchievementsByType(Achievement.AchievementType type, TimeSpan time)
        {
            if (Achievements.ContainsKey(type))
            {
                Achievements[type].ForEach((achievement) =>
                {
                    if (!achievement.Data.Completed)
                    {
                        achievement.Update(time, Director);
                    }
                });
            }
        }

        /// <summary>
        /// Gets the achievements by given type.
        /// </summary>
        /// <param name="type">Type of achievements.</param>
        /// <returns></returns>
        public List<Achievement> GetAchievementsList(Achievement.AchievementType type)
        {
            if (Achievements.ContainsKey(type))
            {
                return Achievements[type];
            }

            return null;
        }

        /// <summary>
        /// Subscribe all achievements completed event.
        /// </summary>
        public void SubscribeAllAchievements()
        {
            if (!AlreadySubscribed)
            {
                foreach (Achievement.AchievementType type in Achievements.Keys)
                {
                    foreach (Achievement achievement in Achievements[type])
                    {
                        achievement.OnAchievementCompleted += new Achievement.OnAchievementCompletedHandler(AchievementCompletedHandler);
                    }
                }
            }
            AlreadySubscribed = true;
        }

        /// <summary>
        /// Handles the on achievement event.
        /// </summary>
        /// <param name="achievement"></param>
        private void AchievementCompletedHandler(Achievement achievement)
        {
            if (OnAchievementCompleted != null)
            {
                OnAchievementCompleted(achievement);
            }
        }

        public delegate void OnAchievementCompletedHandler(Achievement achievement);
        public event OnAchievementCompletedHandler OnAchievementCompleted;

        /// <summary>
        /// Gets or sets the achievements dictionary.
        /// </summary>
        public Dictionary<Achievement.AchievementType, List<Achievement>> Achievements
        {
            get
            {
                if (!Director.SettingsManager.ContainsSetting(AchievementsSettingsKey))
                {
                    Director.SettingsManager.SaveSetting(AchievementsSettingsKey, new Dictionary<Achievement.AchievementType, List<Achievement>>());
                }
                return Director.SettingsManager.LoadSetting<Dictionary<Achievement.AchievementType, List<Achievement>>>(AchievementsSettingsKey);
            }
            set
            {
                Director.SettingsManager.SaveSetting(AchievementsSettingsKey, value);
            }
        }

        #region IAchievementService Implementation

        public bool Initialize()
        {
            return true;
        }

        public bool Finalize()
        {
            OnAchievementCompleted = null;
            return true;
        }

        public void UnlockAchievement(string achievementId)
        {
            // TODO: Implement achievement unlocking
            OnAchievementUnlocked?.Invoke(this, new AchievementUnlockedEventArgs
            {
                Achievement = new Interfaces.AchievementData { Id = achievementId }
            });
        }

        public IEnumerable<Interfaces.AchievementData> GetAchievements()
        {
            // TODO: Implement achievements retrieval
            return Enumerable.Empty<Interfaces.AchievementData>();
        }

        public Interfaces.AchievementData GetAchievementById(string id)
        {
            // TODO: Implement achievement lookup
            return null;
        }

        public event EventHandler<AchievementUnlockedEventArgs> OnAchievementUnlocked;

        #endregion

        private bool AlreadySubscribed { get; set; }
        private bool AlreadyProcessed { get; set; }
        private Director Director { get; set; }
    }
}
