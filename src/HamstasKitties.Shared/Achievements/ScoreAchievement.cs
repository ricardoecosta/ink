using System;
using HamstasKitties.Social.Achievements;
using HamstasKitties.Core;
using HamstasKitties.Management;
using HamstasKitties.GameModes;
using System.Runtime.Serialization;

namespace HamstasKitties.Achievements
{
    [DataContract]
    public class ScoreAchievement : Achievement
    {
        public ScoreAchievement(String id, String name, String description, int targetScore)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            TargetValue = targetScore;
        }

        public override void Update(TimeSpan time, Core.Director director)
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
