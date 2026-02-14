using System;
using HamstasKitties.Social.Achievements;
using HamstasKitties.Core;
using HamstasKitties.Management;
using HamstasKitties.GameModes;
using System.Runtime.Serialization;

namespace HamstasKitties.Achievements
{
    /// <summary>
    /// For achievement based on unlocked items.
    /// </summary>
    [DataContract]
    public class ScreenClearedAchievement : Achievement
    {
        public ScreenClearedAchievement()
            : this(null, null, null, 0)
        { }

        public ScreenClearedAchievement(String id, String name, String description, int targetNumber)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            TargetValue = targetNumber;
        }

        public override void Update(TimeSpan time, Core.Director director)
        {
            if (director.CurrentSceneType == (int)GameDirector.ScenesTypes.LevelScreen)
            {
                Level level = (Level)director.CurrentScene;
                if (level.LevelBoardController.BoardClearedCounter >= TargetValue)
                {
                    SetCompleted();
                }
            }
        }

        [DataMember]
        public int TargetValue { get; set; }
    }
}
