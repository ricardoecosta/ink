using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using GameLibrary.Social.Achievements;
using HnK.GameModes;
using GameLibrary.UI;
using HnK.Management;
using GameLibrary.Core;

namespace HnK.Achievements
{
    [DataContract]
    public class LevelAchievement : Achievement
    {
        public LevelAchievement()
            : this(null, null, null, 0)
        { }

        public LevelAchievement(String id, String name, String description, int targetNumber)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            TargetValue = targetNumber;
        }

        public override void Update(TimeSpan time, Director director)
        {
            if (GameDirector.Instance.CurrentGameMode != GameDirector.GameModes.GoldRush && director.CurrentSceneType == (int)GameDirector.ScenesTypes.LevelScreen)
            {
                Level level = (Level)director.CurrentScene;
                if (level.CurrentLevelNumber >= TargetValue)
                {
                    SetCompleted();
                }
            }
        }

        [DataMember]
        public int TargetValue { get; set; }
    }
}
