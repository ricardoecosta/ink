using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Runtime.Serialization;

namespace GameLibrary.Entity
{
    /// <summary>
    /// Class that represents an Player of the Game.
    /// </summary>
    [DataContract()]
    public class Player
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="name">Name of the player.</param>
        public Player(String name)
        {
            this.Name = name;
            this.Score = 0;
            this.Email = String.Empty;
            this.CurrentUnlockedLevel = 1;
        }

        #region Properties

        [DataMemberAttribute]
        public String Name { get; set; }
        [DataMemberAttribute]
        public String Email { get; set; }
        [DataMemberAttribute]
        public long Score { get; set; }
        [DataMemberAttribute]
        public long CurrentUnlockedLevel { get; set; }

        #endregion
    }
}
