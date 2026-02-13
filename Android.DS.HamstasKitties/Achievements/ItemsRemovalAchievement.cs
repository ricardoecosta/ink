using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Social.Achievements;
using GameLibrary.UI;
using HnK.Mechanics;
using HnK.Scenes;
using HnK.GameModes;
using System.Runtime.Serialization;
using HnK.Management;
using HnK.Persistence;
using HnK.Constants;
using GameLibrary.Core;

namespace HnK.Achievements
{
    [DataContract]
    public class ItemsRemovalAchievement : Achievement
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public ItemsRemovalAchievement()
            : this(null, null, null, 0, Block.BlockTypes.Block1)
        { }

        public ItemsRemovalAchievement(String id, String name, String description, int targetNumber, Block.BlockTypes blockType)
            : base(id, AchievementType.Normal, name, description, 1, 0)
        {
            BlockType = blockType;
            TargetValue = targetNumber;
        }

        public override void Update(TimeSpan time, Director director)
        {
            String key = BlockType.ToString() + PersistableSettingsConstants.TotalRemovedBlocksPerTypeKeySuffix;
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
