using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using GameLibrary.Social.Achievements;
using HnK.Mechanics;
using GameLibrary.UI;
using HnK.GameModes;
using HnK.Management;
using HnK.Persistence;
using HnK.Constants;
using GameLibrary.Core;

namespace HnK.Achievements
{
    [DataContract]
    public class UnlockedSpecialTypeAchievement : Achievement
    {
        public UnlockedSpecialTypeAchievement()
            : this(null, null, null, 0, Block.SpecialTypes.None)
        { }

        public UnlockedSpecialTypeAchievement(String id, String name, String description, int targetNumber, Block.SpecialTypes blockType)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            SpecialType = blockType;
            TargetValue = targetNumber;
        }

        public override void Update(TimeSpan time, Director director)
        {
            String Key = SpecialType.ToString() + PersistableSettingsConstants.TotalUpgradedBlocksPerTypeKeySuffix;
            GameStateManager StateManager = GameDirector.Instance.StateManager;

            if (StateManager.State.GlobalStats.ContainsKey(Key) && StateManager.State.GlobalStats[Key] >= TargetValue)
            {
                SetCompleted();
            }
        }

        [DataMember]
        public Block.SpecialTypes SpecialType { get; set; }

        [DataMember]
        public int TargetValue { get; set; }
    }
}
