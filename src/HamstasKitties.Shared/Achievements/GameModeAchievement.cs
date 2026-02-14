using System;
using HamstasKitties.Social.Achievements;
using HamstasKitties.Core;
using HamstasKitties.Management;
using System.Runtime.Serialization;

namespace HamstasKitties.Achievements
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

        public override void Update(TimeSpan time, Core.Director director)
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
