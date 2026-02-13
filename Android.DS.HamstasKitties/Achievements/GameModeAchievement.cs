using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Social.Achievements;
using GameLibrary.UI;
using System.Runtime.Serialization;
using HnK.GameModes;
using HnK.Management;
using GameLibrary.Core;

namespace HnK.Achievements
{
    [DataContract]
    public class GameModeAchievement : Achievement
    {
        public GameModeAchievement(String id, String name, String description, GameDirector.GameModes mode, int targetScore)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            Mode = mode;
            TargetValue = targetScore;
        }

        public override void Update(TimeSpan time, Director director)
        {
            if (Mode == GameDirector.Instance.CurrentGameMode && director.CurrentSceneType == (int)GameDirector.ScenesTypes.LevelScreen)
            {
                SetCompleted();
            }
        }

        [DataMember]
        public int TargetValue { get; set; }

        [DataMember]
        public GameDirector.GameModes Mode { get; set; }
    }
}