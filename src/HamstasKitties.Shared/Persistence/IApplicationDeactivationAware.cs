using System;
using HamstasKitties.Core;

namespace HamstasKitties.Persistence
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
