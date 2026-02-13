using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using GameLibrary.Social.Achievements;
using HnK.GameModes;
using GameLibrary.UI;
using GameLibrary.Core;
using HnK.Management;

namespace HnK.Achievements
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

        public override void Update(TimeSpan time, Director director)
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
