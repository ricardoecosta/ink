using System;
using HamstasKitties.Core;

namespace HamstasKitties.Persistence
{
    /// <summary>
    /// Any entity which state should be loaded automatically on app activation (restoration from tombstoning).
    /// </summary>
    public interface IApplicationActivationAware
    {
        void LoadPersistedState();
        void RegisterApplicationActivationAwareComponent(Director director);
    }
}
