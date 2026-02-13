using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameLibrary.Core;

namespace GameLibrary.Core
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
