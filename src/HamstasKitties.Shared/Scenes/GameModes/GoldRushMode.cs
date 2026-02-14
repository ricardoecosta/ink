using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.GameModes;
using HamstasKitties.Management;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using HamstasKitties.Constants;
using HamstasKitties.Mechanics;

namespace HamstasKitties.Scenes.GameModes
{
    class GoldRushMode : CountdownMode
    {
        public GoldRushMode()
            : base(GlobalConstants.DroppinModeDurationInSeconds) { }

        public override void PlaySong()
        {
            Director.SoundManager.PlaySong(Director.CurrentResourcesManager.GetCachedSong((int)GameDirector.SongAssets.DroppinModeBGMusic), true);
        }

        public void StopCountdownTimer()
        {
            CountdownTimer.Stop();
        }

        public void ResetCountdownTimerAndPlaceNewGoldenHamsta()
        {
            Block randomNextLineBlock = null;
            while (randomNextLineBlock == null || randomNextLineBlock.Type == Block.BlockTypes.RainbowHamsta)
            {
                randomNextLineBlock = LevelBoardController.BlockEmitter.NextLineOfBlocks[Rand.Next(GlobalConstants.NumberOfBlockGridColumns)];
            }

            randomNextLineBlock.ConvertToGoldenBlock();
            ResetTimer(TotalLevelTimeInSeconds * 1000);

            // Update stats
            LevelBoardController.IncrementStats(Block.BlockTypes.GoldenBlock + PersistableSettingsConstants.TotalCreatedBlocksPerTypeKeySuffix, true);
        }

        protected override float NextLevelUpProgress()
        {
            Block block = LevelBoardController.GetGoldenBlockThatReachedBottom();
            if (block != null)
            {
                int points = CurrentLevelTime.Seconds * ScoreConstants.DroppinSpeedPoints;
                EnqueueLevelAnimation(new QueuedLevelAnimation(QueuedLevelAnimation.Types.Points, points));

                Score += points;
                StopCountdownTimer();
                block.OrderRemoval(Block.RemovalEffectEnum.RainbowExplosion);

                return 100;
            }

            return 0;
        }
    }
}
