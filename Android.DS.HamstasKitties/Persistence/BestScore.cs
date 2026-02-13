using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HnK.Persistence
{
    [DataContract()]
    public class BestScore
    {
        public BestScore() 
        {
            Score = 0;
            AlreadySubmitted = false;
            ToSubmit = false;
        }
        [DataMemberAttribute()]
        public double Score { get; set; }
        [DataMemberAttribute()]
        public bool AlreadySubmitted { get; set; }
        [DataMemberAttribute()]
        public bool ToSubmit { get; set; }
    }
}
