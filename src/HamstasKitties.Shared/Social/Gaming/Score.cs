using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HamstasKitties.Social.Gaming
{
    public class Score
    {
        public int Mode { get; set; }
        public ulong Rank { get; set; }
        public string PlayerName { get; set; }
        public double ScoreResult { get; set; }
        public string Country { get; set; }
        public string PhotoURL { get; set; }
        public bool IsCurrentUser { get; set; }
    }
}
