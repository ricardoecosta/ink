using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.UI;
using HnK.Management;
using GameLibrary.Animation;
using HnK.Scenes;
using HnK.GameModes;
using HnK.Constants;
using Microsoft.Xna.Framework.Audio;
using GameLibrary.Utils;

namespace HnK.Mechanics
{
    /// <summary>
    /// Class responsible for detecting and updated combo events. Combo multiplier is updated each time a new matching group is removed.
    /// The combo stops as soon as the user starts to drag a block.
    /// 
    /// FIXME: Makes absolutely no sense that this class is coupled to the matching groups feature.
    /// </summary>
    public class ComboManager : IUpdateable
    {
        public ComboManager()
        {
            MatchingGroups = new Dictionary<ulong, MatchingGroup>();

            OnComboUpdated += (comboMultiplier) =>
            {
                Level level = GameDirector.Instance.CurrentScene as Level ?? GameDirector.Instance.UnderlyingScene as Level;
                if (comboMultiplier == GlobalConstants.ComboMultiplierWhenToGenerateRainbowHamsta)
                {
                    level.GenerateRainbowHamsta();
                }
                else if (comboMultiplier % 2 == 0)
                {
                    ComboSoundCounter = (byte)(++ComboSoundCounter % ComboSoundEffects.Length);
                    GameDirector.Instance.SoundManager.PlaySound(ComboSoundEffects[ComboSoundCounter]);
                }
            };
        }

        public void StopCurrentCombo()
        {
            if (ComboMultiplier > 1 && OnComboFinished != null)
            {
                OnComboFinished(ComboMultiplier);
            }

            ComboMultiplier = 0;
        }

        public void IncrementComboMultiplier(MatchingGroup matchingGroup)
        {
            ComboMultiplier++;

            if (ComboMultiplier == 2)
            {
                if (OnComboStarted != null)
                {
                    OnComboStarted(ComboMultiplier);
                }
            }
            else if (ComboMultiplier > 2)
            {
                if (OnComboUpdated != null)
                {
                    OnComboUpdated(ComboMultiplier);
                }
            }
        }

        public void RemoveMatchingGroup(MatchingGroup matchingGroup)
        {
            if (MatchingGroups.ContainsKey(matchingGroup.UniqueID))
            {
                MatchingGroups.Remove(matchingGroup.UniqueID);
                matchingGroup.ClearGroup(true);
            }
        }

        public bool IsMatchingGroupRegistered(MatchingGroup matchingGroup)
        {
            return MatchingGroups.ContainsKey(matchingGroup.UniqueID);
        }

        public void AddMatchingGroup(MatchingGroup matchingGroup)
        {
            if (matchingGroup != null && !MatchingGroups.ContainsKey(matchingGroup.UniqueID))
            {
                MatchingGroups.Add(matchingGroup.UniqueID, matchingGroup);
            }
        }

        public void Update(TimeSpan time)
        {
            List<MatchingGroup> matchingGroups = new List<MatchingGroup>(MatchingGroups.Values);
            foreach (var matchingGroup in matchingGroups)
            {
                if (matchingGroup != null)
                {
                    matchingGroup.Update(time);
                }
            }
        }

        #region Events

        public delegate void ComboHandler(int multiplier);
        public event ComboHandler OnComboStarted;
        public event ComboHandler OnComboUpdated;
        public event ComboHandler OnComboFinished;

        #endregion

        #region Properties

        private int ComboMultiplier { get; set; }
        private Dictionary<ulong, MatchingGroup> MatchingGroups { get; set; }

        #endregion

        private byte ComboSoundCounter = 1;
        private SoundEffect[] ComboSoundEffects = new SoundEffect[] 
            {
                GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Yeah),
                GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Wooow),
                GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Amazing),
                GameDirector.Instance.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Fantastic),
            };
    }
}
