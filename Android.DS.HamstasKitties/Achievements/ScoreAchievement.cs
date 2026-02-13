using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Social.Achievements;
using HnK.Scenes;
using HnK.GameModes;
using System.Runtime.Serialization;
using GameLibrary.Core;
using HnK.Management;

namespace HnK.Achievements
{
    [DataContract]
    public class ScoreAchievement : Achievement
    {
        public ScoreAchievement(String id, String name, String description, int targetScore)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            TargetValue = targetScore;
        }

        public override void Update(TimeSpan time, Director director)
        {
            if (director.CurrentScene != null && director.CurrentSceneType == (int)GameDirector.ScenesTypes.LevelScreen)
            {
                if (((Level)director.CurrentScene).Score >= TargetValue)
                {
                    SetCompleted();
                }
            }
        }

        [DataMember]
        public int TargetValue { get; set; }
    }
}
