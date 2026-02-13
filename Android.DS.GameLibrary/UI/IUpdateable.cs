using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.UI
{
    /// <summary>
    /// Interface for objects that can be updated by Game update call.
    /// </summary>
    public interface IUpdateable
    {
        /// <summary>
        /// Updates the object state.
        /// </summary>
        /// <param name="time"></param>
        void Update(TimeSpan elapsedTime);
    }
}
