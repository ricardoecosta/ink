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
    public class ItemsCreatedAchievement : Achievement
    {
        public ItemsCreatedAchievement(String id, String name, String description, int targetNumber, Block.BlockTypes blockType)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            BlockType = blockType;
            TargetValue = targetNumber;
        }

        public override void Update(TimeSpan time, Core.Director director)
        {
            String key = BlockType.ToString() + PersistableSettingsConstants.TotalCreatedBlocksPerTypeKeySuffix;
            GameStateManager stateManager = GameDirector.Instance.StateManager;

            if (stateManager.State.GlobalStats.ContainsKey(key) && stateManager.State.GlobalStats[key] >= TargetValue)
            {
                SetCompleted();
            }
        }

        [DataMember]
        public Block.BlockTypes BlockType { get; set; }

        [DataMember]
        public int TargetValue { get; set; }
    }
}
