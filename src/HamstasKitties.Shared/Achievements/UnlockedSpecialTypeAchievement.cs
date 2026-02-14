using System;
using HamstasKitties.Social.Achievements;
using HamstasKitties.Core;
using HamstasKitties.Management;
using HamstasKitties.Mechanics;
using HamstasKitties.Constants;
using System.Runtime.Serialization;

namespace HamstasKitties.Achievements
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

        public override void Update(TimeSpan time, Core.Director director)
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
