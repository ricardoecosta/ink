using System;
using HamstasKitties.Social.Achievements;
using HamstasKitties.Core;
using HamstasKitties.Management;
using HamstasKitties.GameModes;
using System.Runtime.Serialization;

namespace HamstasKitties.Achievements
{
    [DataContract]
    public class ComboAchievement : Achievement
    {
        public ComboAchievement(String id, String name, String description, int targetValue)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            TargetValue = targetValue;
        }

        public override void Update(TimeSpan time, Core.Director director)
        {
            if (director.CurrentSceneType == (int)GameDirector.ScenesTypes.LevelScreen)
            {
                Level level = (Level)director.CurrentScene;
                if (level.LevelBoardController.MaxCombo >= TargetValue)
                {
                    SetCompleted();
                }
            }
        }

        [DataMember]
        public int TargetValue { get; set; }
    }
}
