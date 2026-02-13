using System;
using System.Collections.Generic;
using System.IO;
using GameLibrary.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using ProjectMercury;

namespace GameLibrary.Core
{
	/// <summary>
	/// Class that will manage all application resources.
	/// </summary>
	public class ResourcesManager : IManager
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="contentManager">Plataform content manager.</param>
		public ResourcesManager(ContentManager contentManager)
		{
			ContentManager = contentManager;
			FontsCache = new Dictionary<int, SpriteFont>();
			TexturesCache = new Dictionary<int, GameLibrary.UI.Texture>();
			SpriteSheetsTexturesCache = new Dictionary<int, GameLibrary.UI.Texture>();
			SoundsCache = new Dictionary<int, SoundEffect>();
			SoundsLoopsCache = new Dictionary<int, SoundEffectInstance>();
			SongsCache = new Dictionary<int, Song>();
			ParticleEffectsCache = new Dictionary<int, ParticleEffect>();
			
		}
		
		/// <summary>
		/// Loads resource with given type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="resourceName"></param>
		/// <returns></returns>
		private T LoadResource<T>(string resourceName)
		{
			return ContentManager.Load<T>(resourceName);
		}
		
		public void CacheTextureFromSpriteSheet(GameLibrary.UI.Texture texture)
		{
			if (!TexturesCache.ContainsKey(texture.Id))
			{
				TexturesCache.Add(texture.Id, texture);
			}
		}
		
		public GameLibrary.UI.Texture LoadTextureFromDisk(int? textureId, string resourceRelativePath, bool cache)
		{
			Console.WriteLine("TextureId: " + textureId + " Path: " + resourceRelativePath);
			Texture2D textureFromDisk = ContentManager.Load<Texture2D>(resourceRelativePath);
			GameLibrary.UI.Texture texture = null;
			
			if (textureFromDisk != null)
			{
				texture = new GameLibrary.UI.Texture(textureId.Value, textureFromDisk, textureFromDisk.Bounds);
				
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
		
		public GameLibrary.UI.Texture GetCachedTexture(int textureId)
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

			GameLibrary.UI.Texture spriteSheet = null;
			if (!isWarmup)
			{
				spriteSheet = LoadTextureFromDisk(spriteSheetId, spriteSheetAssetWithRelativePath, false);
			}
			
			int resourceLoadedCounter = 0;
			foreach (string spriteSheetTextureName in spriteSheetSourceRectangles.Keys)
			{
				if (!isWarmup)
				{
					GameLibrary.UI.Texture newTextureFromSpriteSheet = new GameLibrary.UI.Texture((int)Enum.Parse(textureAssetsEnumType, spriteSheetTextureName, true), spriteSheetId, spriteSheet.SourceTexture, spriteSheetSourceRectangles[spriteSheetTextureName]);
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
				// While we're not done reading...
				while (!reader.EndOfStream)
				{
					// Get a line
					string line = reader.ReadLine();
					
					// Split at the equals sign
					string[] sides = line.Split('=');
					
					// Trim the right side and split based on spaces
					string[] rectParts = sides[1].Trim().Split(' ');
					
					// Create a rectangle from those parts
					Rectangle r = new Rectangle(
						int.Parse(rectParts[0]),
						int.Parse(rectParts[1]),
						int.Parse(rectParts[2]),
						int.Parse(rectParts[3]));
					
					// Add the name and rectangle to the dictionary
					spriteSheetSourceRectangles.Add(sides[0].Trim(), r);
				}
			}
			
			return spriteSheetSourceRectangles;
		}
		
		/// <summary>
		/// Loads sound effect from contents manager.
		/// </summary>
		/// <param name="soundId">Id to identify sound effect on cache if you set it on cache.</param>
		/// <param name="resourceName"> Name of the resource.</param>
		/// <param name="cache">Caches or not the sound effect.</param>
		/// <param name="isLooped">Sound runs in loop or not.</param>
		/// <returns>SoundEffect instance or null.</returns>
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
		
		/// <summary>
		/// Loads song from content manager.
		/// </summary>
		/// <param name="songId">Id to identify song on cache if you set it on cache.</param>
		/// <param name="resourceName"> Name of the resource.</param>
		/// <param name="cache">Caches or not the sound effect.</param>
		/// <returns>Song instance or null.</returns>
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
		
		/// <summary>
		/// Load sound effect and set it on cache.
		/// </summary>
		/// <param name="soundId">Sound effect identifier on cache.</param>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="isLooped">Sound runs in loop or not.</param>
		public void CacheSoundEffect(int soundId, string resourceName, bool isLooped)
		{
			LoadSoundEffect(soundId, resourceName, true, isLooped);
		}
		
		/// <summary>
		/// Gets sound effect from cache. To use this method you must call first the CacheSoundEffect function.
		/// </summary>
		/// <param name="textureId">Sound effect identifier on cache.</param>
		/// <returns>The SoundEffect instance or null if sound effect was not loaded.</returns>
		public SoundEffect GetCachedSoundEffect(int soundId)
		{
			if (SoundsCache.ContainsKey(soundId))
			{
				return SoundsCache[soundId];
			}
			return null;
		}
		
		/// <summary>
		/// Gets sound effect looped from cache. To use this method you must call first the CacheSoundEffect function.
		/// </summary>
		/// <param name="textureId">Sound effect identifier on cache.</param>
		/// <returns>The SoundEffect instance or null if sound effect was not loaded.</returns>
		public SoundEffectInstance GetCachedSoundLoop(int soundId)
		{
			if (SoundsLoopsCache.ContainsKey(soundId))
			{
				return SoundsLoopsCache[soundId];
			}
			return null;
		}
		
		/// <summary>
		/// Gets song from cache.
		/// </summary>
		/// <param name="textureId">Sound effect identifier on cache.</param>
		/// <returns>The Song instance or null if Song was not loaded.</returns>
		public Song GetCachedSong(int songId)
		{
			if (SongsCache.ContainsKey(songId))
			{
				return SongsCache[songId];
			}
			return null;
		}
		
		/// <summary>
		/// Loads font from contents manager.
		/// </summary>
		/// <param name="textureId">Id to identify font on cache if you set it on cache.</param>
		/// <param name="resourceName"> Name of the resource.</param>
		/// <param name="cache">Caches or not the texture.</param>
		/// <returns>SpriteFont instance or null.</returns>
		public SpriteFont LoadFont(int fontId, String resourceName, bool cache)
		{
			SpriteFont font = ContentManager.Load<SpriteFont>(resourceName);
			if (font != null && !FontsCache.ContainsKey(fontId) && cache)
			{
				FontsCache.Add(fontId, font);
			}
			return font;
		}
		
		/// <summary>
		/// Loads font and set it on cache.
		/// </summary>
		/// <param name="fontId">Font identifier on cache.</param>
		/// <param name="resourceName">Resource name.</param>
		public void CacheFont(int fontId, String resourceName)
		{
			LoadFont(fontId, resourceName, true);
		}
		
		public ParticleEffect LoadParticleEffect(int particleEffectId, String resourceName, bool cache)
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
		
		public void CacheParticleEffect(int particleEffectId, String resourceName)
		{
			LoadParticleEffect(particleEffectId, resourceName, true);
		}
		
		/// <summary>
		/// Gets texture from cache. To use this method you must call first the CacheTexture function.
		/// </summary>
		/// <param name="fontId">Font identifier on cache.</param>
		/// <returns>The font instance or null if font was not loaded.</returns>
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
		
		private Dictionary<int, GameLibrary.UI.Texture> TexturesCache { get; set; }
		private Dictionary<int, GameLibrary.UI.Texture> SpriteSheetsTexturesCache { get; set; }
		private Dictionary<int, SpriteFont> FontsCache { get; set; }
		private Dictionary<int, SoundEffect> SoundsCache { get; set; }
		private Dictionary<int, SoundEffectInstance> SoundsLoopsCache { get; set; }
		private Dictionary<int, Song> SongsCache { get; set; }
		private Dictionary<int, ParticleEffect> ParticleEffectsCache { get; set; }
		
		public ContentManager ContentManager { get; set; }
		
		#endregion
	}
}
