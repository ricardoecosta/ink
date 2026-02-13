using GameLibrary.UI;
using Microsoft.Xna.Framework;
using HnK.Management;
using GameLibrary.Core;
using HnK.Scenes.Menus;

namespace HnK.Layers
{
    /// <summary>
    /// Layer that will contains tutorial pages.
    /// </summary>
    public class TutorialContentLayer : PageableLayer
    {
        public TutorialContentLayer(Scene scene, int zOrder) :
            base (scene, zOrder, scene.Width) { }

		protected override void OnPageMoved (Vector2 pos)
		{
			// do nothing
		}

        public override void Initialize()
        {
            base.Initialize();
            
            GameDirector director = GameDirector.Instance;
            ResourcesManager resources = director.CurrentResourcesManager;

            TutorialPagesTextures = new GameLibrary.UI.Texture[]
            {
                resources.GetCachedTexture((int)GameDirector.TextureAssets.TutorialPage01),
                resources.GetCachedTexture((int)GameDirector.TextureAssets.TutorialPage02),
                resources.GetCachedTexture((int)GameDirector.TextureAssets.TutorialPage03),
                resources.GetCachedTexture((int)GameDirector.TextureAssets.TutorialPage04)
            };

            for (int i = 0; i < TutorialPagesTextures.Length; i++)
            {
                LayerObject page = new LayerObject(this, TutorialPagesTextures[i], new Vector2(i * ParentScene.Width, 0), Vector2.Zero);
                
                page.OnTap += (sender, point) =>
                {
                    if (SlidePageTweener == null || !SlidePageTweener.IsRunning)
                    {
                        if (!IsLastPageSelected())
                        {
                            ChangePage(++CurrentPage);
                        }
                        else
                        {
                            ((TutorialMenu)ParentScene).Close();
                        }
                    }
                };

                AddPage(page);
            }
        }

        private GameLibrary.UI.Texture[] TutorialPagesTextures { get; set; }
    }
}
