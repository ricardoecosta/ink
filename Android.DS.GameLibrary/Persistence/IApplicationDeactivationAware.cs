using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Core;

namespace GameLibrary.Core
{
    /// <summary>
    /// Any entity which state should be saved automatically on app deactivation.
    /// </summary>
    public interface IApplicationDeactivationAware
    {
        void PersistCurrentState();
        void RegisterApplicationDeactivationAwareComponent(Director director);
    }
}
