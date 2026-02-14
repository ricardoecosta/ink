using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.Core;
using HamstasKitties.Animation;
using HamstasKitties.UI;
using HamstasKitties.Scenes;
using HamstasKitties.Management;
using HamstasKitties.GameModes;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using HamstasKitties.Constants;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Timer = HamstasKitties.Animation.Timer;

namespace HamstasKitties.Mechanics
{
    /// <summary>
    /// Group of blocks that repesent a match.
    /// </summary>
    public class MatchingGroup : HamstasKitties.UI.IUpdateable
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public MatchingGroup(ComboManager comboChecker)
        {
            MatchedBlocks = new List<Block>(3);
            MatchedBlocksDictionary = new Dictionary<ulong, Block>(3);
            UniqueID = ++UniqueIDCounter;
            ComboChecker = comboChecker;
            TypeOfMatch = null;

            MatchingModeTimer = new Timer(MatchingModeDuration);

            MatchingModeTimer.OnFinished += (sender, args) =>
            {
                if (MatchedBlocks.Count >= GlobalConstants.MinBlocksToMatch)
                {
                    CheckIfBlockUpdateRequestIsNeeded();

                    List<Block> blocksToRemove = new List<Block>();
                    foreach (var block in MatchedBlocks)
                    {
                        block.RemoveOrUpgradeToSpecialType();
                    }

                    ComboChecker.IncrementComboMultiplier(this);
                    ClearGroup(true);
                }
            };
        }

        public void Update(TimeSpan time)
        {
            MatchingModeTimer.Update(time);
        }

        /// <summary>
        /// Adds a new block to the group.
        /// </summary>
        /// <param name="block"></param>
        public void AddBlock(Block block)
        {
            if (block != null && !MatchedBlocksDictionary.ContainsKey(block.UniqueID))
            {
                if (TypeOfMatch == null)
                {
                    TypeOfMatch = block.Type;
                }

                SetupBlock(block);

                if (MatchedBlocks.Count >= GlobalConstants.MinBlocksToMatch)
                {
                    if (!ComboChecker.IsMatchingGroupRegistered(this))
                    {
                        ComboChecker.AddMatchingGroup(this);
                    }

                    if (!MatchingModeTimer.IsRunning)
                    {
                        MatchingModeTimer.Start();

                        // Matching mode sound effect
                        MatchingModeSoundCounter = (byte)(++MatchingModeSoundCounter % MatchingModeSounds.Length);
                        GameDirector.Instance.SoundManager.PlaySound(MatchingModeSounds[MatchingModeSoundCounter]);
                    }
                    else
                    {
                        MatchingModeTimer.Restart();

                        // Matching mode sound effect
                        MatchingModeSoundCounter = (byte)(++MatchingModeSoundCounter % MatchingModeSounds.Length);
                        GameDirector.Instance.SoundManager.PlaySound(MatchingModeSounds[MatchingModeSoundCounter]);
                    }

                    foreach (Block currentBlock in MatchedBlocks)
                    {
                        if (currentBlock.State != Block.States.MatchingMode)
                        {
                            currentBlock.StartMatchingMode();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Merges the given group with this group.
        /// </summary>
        /// <param name="groupToMerge"></param>
        public void MergeGroups(MatchingGroup groupToMerge)
        {
            if (groupToMerge != null)
            {
                while (groupToMerge.MatchedBlocks.Count > 0)
                {
                    AddBlock(groupToMerge.MatchedBlocks[0]);
                }
            }
        }

        /// <summary>
        /// Verifies if already exists the block.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool ContainsBlock(Block block)
        {
            if (block == null)
            {
                return false;
            }

            return MatchedBlocksDictionary.ContainsKey(block.UniqueID);
        }

        /// <summary>
        /// Removes a block of the group.
        /// </summary>
        /// <param name="block"></param>
        public void RemoveBlock(Block block)
        {
            if (block != null && MatchedBlocksDictionary.ContainsKey(block.UniqueID))
            {
                UnSetupBlock(block);
                MatchedBlocksDictionary.Remove(block.UniqueID);
                MatchedBlocks.Remove(block);
            }
        }

        /// <summary>
        /// Clears all group.
        /// </summary>
        /// <param name="groups"></param>
        /// <param name="removeSubscribers"></param>
        public void ClearGroup(bool removeSubscribers)
        {
            MatchingModeTimer.Stop();
            ClearsData();

            if (removeSubscribers)
            {
                OnMatchGroupRemoval = null;
            }
        }

        /// <summary>
        /// Setups the block for group.
        /// </summary>
        /// <param name="block"></param>
        private void SetupBlock(Block block)
        {
            // Reparent Block of Group
            if (block.MatchingGroup != null)
            {
                MatchingGroup group = block.MatchingGroup;
                group.RemoveBlock(block);
                if (group.MatchedBlocksDictionary.Count == 0)
                {
                    group.ClearGroup(true);
                }
            }

            block.MatchingGroup = this;
            MatchedBlocksDictionary.Add(block.UniqueID, block);
            MatchedBlocks.Add(block);
        }

        /// <summary>
        /// Auxiliar methods for event handlers behavior.
        /// </summary>
        private void RemoveItem(Block block)
        {
            RemoveBlock(block);
            RemovedItems++;
            UpdateMatchGroupState();
        }

        /// <summary>
        /// Updates the state of match group.
        /// </summary>
        private void UpdateMatchGroupState()
        {
            if (MatchedBlocksDictionary.Count == 0)
            {
                if (OnMatchGroupRemoval != null)
                {
                    OnMatchGroupRemoval(this);
                }
                ClearGroup(true);
            }
        }

        /// <summary>
        /// Clears all data of Match and invalidate the Match Group.
        /// </summary>
        private void ClearsData()
        {
            foreach (Block block in MatchedBlocks)
            {
                UnSetupBlock(block);
            }
            MatchedBlocksDictionary.Clear();
            MatchedBlocks.Clear();
        }

        /// <summary>
        /// Clears all events and group data from block.
        /// </summary>
        /// <param name="block"></param>
        public void UnSetupBlock(Block block)
        {
            block.MatchingGroup = null;
        }

        /// <summary>
        /// Checks if block needs or not update.
        /// </summary>
        /// <param name="matchingBlocks"></param>
        public void CheckIfBlockUpdateRequestIsNeeded()
        {
            MatchedBlocks = MatchedBlocks.OrderBy(a => Rand.Next(MatchedBlocks.Count)).ToList();

            Block randomNonPowerUpBlockFound = null;
            for (int i = 0; i < MatchedBlocks.Count; i++)
            {
                if (randomNonPowerUpBlockFound == null && MatchedBlocks[i].CurrentSpecialType == Block.SpecialTypes.None)
                {
                    randomNonPowerUpBlockFound = MatchedBlocks[i];
                }

                MatchedBlocks[i].NextSpecialType = Block.SpecialTypes.None;
            }

            if (randomNonPowerUpBlockFound != null)
            {
                if (MatchedBlocks.Count == GlobalConstants.MinBlocksToBombUpgrade)
                {
                    randomNonPowerUpBlockFound.RequestBombUpgrade();
                }
                else if (MatchedBlocks.Count == GlobalConstants.MinBlocksToGokuUpgrade)
                {
                    randomNonPowerUpBlockFound.RequestGokuUpgrade();
                }
                else if (MatchedBlocks.Count >= GlobalConstants.MinBlocksToMagicBombUpgrade)
                {
                    randomNonPowerUpBlockFound.RequestMagicBombUpgrade();
                }
            }
            else
            {
                Block newPowerBlock = MatchedBlocks[Rand.Next(MatchedBlocks.Count)];

                if (MatchedBlocks.Count == GlobalConstants.MinBlocksToBombUpgrade)
                {
                    newPowerBlock.NewBlockPowerUpToCreateAfterRemoval = Block.SpecialTypes.Bomb;
                    addOriginBlock(newPowerBlock, MatchedBlocks);
                }
                else if (MatchedBlocks.Count == GlobalConstants.MinBlocksToGokuUpgrade)
                {
                    newPowerBlock.NewBlockPowerUpToCreateAfterRemoval = Block.SpecialTypes.Goku;
                    addOriginBlock(newPowerBlock, MatchedBlocks);
                }
                else if (MatchedBlocks.Count >= GlobalConstants.MinBlocksToMagicBombUpgrade)
                {
                    newPowerBlock.NewBlockPowerUpToCreateAfterRemoval = Block.SpecialTypes.MagicBomb;
                    addOriginBlock(newPowerBlock, MatchedBlocks);
                }
            }
        }

        private void addOriginBlock(Block newPowerBlock, List<Block> matchedBlocks)
        {
            foreach (var block in matchedBlocks)
            {
                newPowerBlock.OriginBlocks.Add(block.UniqueID, block);
            }
        }

        /// <summary>
        ///  Finds any special blocks on the group
        /// </summary>
        /// <returns></returns>
        public Block FindAnyMatchedSpecialTypeBlock()
        {
            foreach (var block in MatchedBlocks)
            {
                if (block.NextSpecialType != Block.SpecialTypes.None && block.NextSpecialType == block.CurrentSpecialType)
                {
                    return block;
                }
            }

            return null;
        }

        #region Events

        public delegate void MatchGroupRemovalHandler(MatchingGroup group);
        public event MatchGroupRemovalHandler OnMatchGroupRemoval;

        #endregion

        #region Properties

        public ulong UniqueID { get; set; }
        private static ulong UniqueIDCounter;
        public Block.BlockTypes? TypeOfMatch { get; set; }
        public List<Block> MatchedBlocks { get; private set; }
        private Dictionary<ulong, Block> MatchedBlocksDictionary { get; set; }
        private int RemovedItems { get; set; }
        private Timer MatchingModeTimer { get; set; }
        private ComboManager ComboChecker { get; set; }
        public bool IsMatching { get { return MatchingModeTimer.IsRunning; } }

        #endregion

        private const float MatchingModeDuration = 1f;

        private static byte MatchingModeSoundCounter = 1;
        private readonly SoundEffect[] MatchingModeSounds =
        {
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaMatchingSound1),
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaMatchingSound2),
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaMatchingSound3),
            GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.HamstaMatchingSound4)
        };
    }
}
