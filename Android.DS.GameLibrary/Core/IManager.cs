using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLibrary.Core
{
    /// <summary>
    /// Interface for all managers.
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// Initializes all data of the manager.
        /// </summary>
        /// <returns>True if manager is initialized with success. false if some error occurs.</returns>
        bool Initialize();

        /// <summary>
        /// Finalizes the manager.
        /// </summary>
        /// <returns>True if manager is finalized with success. false if some error occurs.</returns>
        bool Finalize();
    }
}
