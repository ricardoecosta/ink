using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using HamstasKitties.Core.Interfaces;
using HamstasKitties.Particles;
using Texture = HamstasKitties.UI.Texture;

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
            FontsCache = new Dictionary<int, SpriteFont>();
            TexturesCache = new Dictionary<int, Texture>();
            SpriteSheetsTexturesCache = new Dictionary<int, Texture>();
            SoundsCache = new Dictionary<int, SoundEffect>();
            SoundsLoopsCache = new Dictionary<int, SoundEffectInstance>();
            SongsCache = new Dictionary<int, Song>();
            ParticleEffectsCache = new Dictionary<int, ParticleEffect>();
        }

        private T LoadResource<T>(string resourceName)
        {
            return ContentManager.Load<T>(resourceName);
        }

        public void CacheTextureFromSpriteSheet(Texture texture)
        {
            if (!TexturesCache.ContainsKey(texture.Id))
            {
                TexturesCache.Add(texture.Id, texture);
            }
        }

        public Texture LoadTextureFromDisk(int? textureId, string resourceRelativePath, bool cache)
        {
            Texture2D textureFromDisk = ContentManager.Load<Texture2D>(resourceRelativePath);
            Texture texture = null;

            if (textureFromDisk != null)
            {
                texture = new Texture(textureId.Value, textureFromDisk, textureFromDisk.Bounds);

                if (cache && textureId.HasValue && !TexturesCache.ContainsKey(textureId.Value))
                {
                    TexturesCache.Add(textureId.Value, texture);
                }
            }

            return texture;
        }

        public void CacheTexture(int textureId, string resourceName)
        {
            LoadTextureFromDisk(textureId, resourceName, true);
        }

        public Texture GetCachedTexture(int textureId)
        {
            if (TexturesCache.ContainsKey(textureId))
            {
                return TexturesCache[textureId];
            }
            return null;
        }

        public int CacheAllTexturesFromSpriteSheet(string spriteSheetAssetWithRelativePath, int spriteSheetId, Type textureAssetsEnumType, bool isWarmup)
        {
            Dictionary<string, Rectangle> spriteSheetSourceRectangles = BuildSpriteSheetMapDictionary(spriteSheetAssetWithRelativePath + ".txt");

            Texture spriteSheet = null;
            if (!isWarmup)
            {
                spriteSheet = LoadTextureFromDisk(spriteSheetId, spriteSheetAssetWithRelativePath, false);
            }

            int resourceLoadedCounter = 0;
            foreach (string spriteSheetTextureName in spriteSheetSourceRectangles.Keys)
            {
                if (!isWarmup)
                {
                    Texture newTextureFromSpriteSheet = new Texture((int)Enum.Parse(textureAssetsEnumType, spriteSheetTextureName, true), spriteSheetId, spriteSheet.SourceTexture, spriteSheetSourceRectangles[spriteSheetTextureName]);
                    CacheTextureFromSpriteSheet(newTextureFromSpriteSheet);
                }
                resourceLoadedCounter++;
            }

            return resourceLoadedCounter;
        }

        private Dictionary<string, Rectangle> BuildSpriteSheetMapDictionary(string spriteSheetMapRelativePath)
        {
            Dictionary<string, Rectangle> spriteSheetSourceRectangles = new Dictionary<string, Rectangle>();

            using (StreamReader reader = new StreamReader(TitleContainer.OpenStream(Path.Combine(ContentManager.RootDirectory, spriteSheetMapRelativePath))))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] sides = line.Split('=');
                    string[] rectParts = sides[1].Trim().Split(' ');
                    Rectangle r = new Rectangle(
                        int.Parse(rectParts[0]),
                        int.Parse(rectParts[1]),
                        int.Parse(rectParts[2]),
                        int.Parse(rectParts[3]));
                    spriteSheetSourceRectangles.Add(sides[0].Trim(), r);
                }
            }

            return spriteSheetSourceRectangles;
        }

        public SoundEffect LoadSoundEffect(int soundId, string resourceName, bool cache, bool isLooped)
        {
            SoundEffect sound = ContentManager.Load<SoundEffect>(resourceName);
            if (sound != null && !SoundsCache.ContainsKey(soundId) && cache)
            {
                if (isLooped)
                {
                    SoundEffectInstance soundInstance = sound.CreateInstance();
                    soundInstance.IsLooped = true;
                    SoundsLoopsCache.Add(soundId, soundInstance);
                }
                else
                {
                    SoundsCache.Add(soundId, sound);
                }
            }
            return sound;
        }

        public Song LoadSong(int songId, string resourceName, bool cache)
        {
            Song song = ContentManager.Load<Song>(resourceName);
            if (song != null && !SongsCache.ContainsKey(songId) && cache)
            {
                SongsCache.Add(songId, song);
            }
            return song;
        }

        public void CacheSong(int songId, string resourceName)
        {
            LoadSong(songId, resourceName, true);
        }

        public void CacheSoundEffect(int soundId, string resourceName, bool isLooped)
        {
            LoadSoundEffect(soundId, resourceName, true, isLooped);
        }

        public SoundEffect GetCachedSoundEffect(int soundId)
        {
            if (SoundsCache.ContainsKey(soundId))
            {
                return SoundsCache[soundId];
            }
            return null;
        }

        public SoundEffectInstance GetCachedSoundLoop(int soundId)
        {
            if (SoundsLoopsCache.ContainsKey(soundId))
            {
                return SoundsLoopsCache[soundId];
            }
            return null;
        }

        public Song GetCachedSong(int songId)
        {
            if (SongsCache.ContainsKey(songId))
            {
                return SongsCache[songId];
            }
            return null;
        }

        public SpriteFont LoadFont(int fontId, string resourceName, bool cache)
        {
            SpriteFont font = ContentManager.Load<SpriteFont>(resourceName);
            if (font != null && !FontsCache.ContainsKey(fontId) && cache)
            {
                FontsCache.Add(fontId, font);
            }
            return font;
        }

        public void CacheFont(int fontId, string resourceName)
        {
            LoadFont(fontId, resourceName, true);
        }

        public ParticleEffect LoadParticleEffect(int particleEffectId, string resourceName, bool cache)
        {
            ParticleEffect particleEffect = ContentManager.Load<ParticleEffect>(resourceName);
            if (particleEffect != null && !ParticleEffectsCache.ContainsKey(particleEffectId) && cache)
            {
                ParticleEffectsCache.Add(particleEffectId, particleEffect);
            }
            particleEffect.Initialise();
            particleEffect.LoadContent(ContentManager);
            return particleEffect;
        }

        public void CacheParticleEffect(int particleEffectId, string resourceName)
        {
            LoadParticleEffect(particleEffectId, resourceName, true);
        }

        public SpriteFont GetCachedFont(int fontId)
        {
            if (FontsCache.ContainsKey(fontId))
            {
                return FontsCache[fontId];
            }
            return null;
        }

        public ParticleEffect GetCachedParticleEffect(int particleEffectId, bool createDeepCopy)
        {
            if (ParticleEffectsCache.ContainsKey(particleEffectId))
            {
                return createDeepCopy ? ParticleEffectsCache[particleEffectId].DeepCopy() : ParticleEffectsCache[particleEffectId];
            }
            return null;
        }

        public void UnloadAllAssets()
        {
            ContentManager.Unload();
        }

        #region IManager implementation

        public bool Initialize()
        {
            if (ContentManager == null)
            {
                return false;
            }
            FontsCache.Clear();
            TexturesCache.Clear();
            return true;
        }

        public bool Finalize()
        {
            FontsCache.Clear();
            TexturesCache.Clear();
            SoundsLoopsCache.Clear();
            SoundsCache.Clear();
            ContentManager.Unload();
            return true;
        }

        #endregion

        #region Properties

        private Dictionary<int, Texture> TexturesCache { get; set; }
        private Dictionary<int, Texture> SpriteSheetsTexturesCache { get; set; }
        private Dictionary<int, SpriteFont> FontsCache { get; set; }
        private Dictionary<int, SoundEffect> SoundsCache { get; set; }
        private Dictionary<int, SoundEffectInstance> SoundsLoopsCache { get; set; }
        private Dictionary<int, Song> SongsCache { get; set; }
        private Dictionary<int, ParticleEffect> ParticleEffectsCache { get; set; }

        public ContentManager ContentManager { get; set; }

        #endregion
    }
}
