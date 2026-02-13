using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Social.Achievements;
using GameLibrary.UI;
using HnK.Scenes;
using HnK.GameModes;
using System.Runtime.Serialization;
using GameLibrary.Core;
using HnK.Management;

namespace HnK.Achievements
{
    [DataContract]
    public class ComboAchievement : Achievement
    {
        public ComboAchievement(String id, String name, String description, int targetValue)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            TargetValue = targetValue;
        }

        public override void Update(TimeSpan time, Director director)
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
