using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectMercury;
using HamstasKitties.UI;

namespace HamstasKitties.Core
{
    /// <summary>
    /// Class that will manage all application resources.
    /// </summary>
    public class ResourcesManager : IManager
    {
        public ResourcesManager(ContentManager contentManager)
        {
            ContentManager = contentManager;
            TexturesCache = new Dictionary<int, Texture>();
        }

        public ContentManager ContentManager { get; set; }
        private Dictionary<int, Texture> TexturesCache { get; set; }

        public bool Initialize()
        {
            if (ContentManager == null)
            {
                return false;
            }

            TexturesCache.Clear();

            return true;
        }

        public bool Finalize()
        {
            TexturesCache.Clear();
            ContentManager.Unload();

            return true;
        }

        public void UnloadAllAssets()
        {
            ContentManager.Unload();
        }
    }
}
