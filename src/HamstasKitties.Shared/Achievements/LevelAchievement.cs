using System;
using HamstasKitties.Social.Achievements;
using HamstasKitties.Core;
using HamstasKitties.Management;
using HamstasKitties.GameModes;
using System.Runtime.Serialization;

namespace HamstasKitties.Achievements
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

        public override void Update(TimeSpan time, Core.Director director)
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
