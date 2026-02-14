using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HamstasKitties.Constants
{
    public sealed class AnalyticsConstants
    {
        public const string FlurryAnalyticsAPIKey = "SHQJNXTQGBNRB4QZ35QP";

        public enum Events
        {
            StartedPlaying,
            FinishedPlaying,
            AchievementUnlocked,
            ScoreSubmitted
        }

        public enum EventParameters
        {
            SessionUUID,
            GameMode,
            Score,
            Level,
            PlayingTimeMinutesRounded,
            PlayingTimeMilliseconds,

            Id,
            Type,
            Name,
            Description
        }
    }
}
