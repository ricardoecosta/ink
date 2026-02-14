#nullable disable
#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace HamstasKitties.Social.Achievements
{
    [DataContract]
    public class AchievementData
    {
        public override bool Equals(object obj)
        {
            if (obj is AchievementData)
            {
                AchievementData data = ((AchievementData)obj);
                return this.Id == data.Id && this.Completed == data.Completed && this.Description == data.Description &&
                       this.Importance == data.Importance && this.Reward == data.Reward &&
                       this.Name == data.Name;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id != null ? Id.GetHashCode() : 0;
        }

        [DataMember]
        public String Id { get; set; }
        [DataMember]
        public bool Completed { get; set; }
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public String Description { get; set; }
        [DataMember]
        public int Reward { get; set; }
        [DataMember]
        public int Importance { get; set; }
    }
}
