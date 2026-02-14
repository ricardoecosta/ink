using System;
using HamstasKitties.UI;
using HamstasKitties.Core;
using HamstasKitties.Management;
using HamstasKitties.Animation;
using Microsoft.Xna.Framework;
using HamstasKitties.Logic;
using System.Collections.Generic;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;
using HamstasKitties.Constants;
using System.IO;

namespace HamstasKitties.Scenes
{
    public class IntroScene : Scene
    {
        public IntroScene(GameDirector director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight)
        {
            Director = director;
            IntroTiles = new LayerObject[12];
        }

        public override void Initialize()
        {
            base.Initialize();

            TilesLayer = new Layer(this, Layer.LayerTypes.Interactive, Vector2.Zero, 0, true);
            AddLayer(TilesLayer);

            Texture sampleTileTexture = Director.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Intro1);

            for (int i = 0; i < 12; i++)
            {
                IntroTiles[i] = new LayerObject(TilesLayer, Director.CurrentResourcesManager.GetCachedTexture(
                    (int)Enum.Parse(typeof(GameDirector.TextureAssets), "Intro" + (i + 1), true)),
                    new Vector2((i % NumberOfColumns) * sampleTileTexture.Width - (i % NumberOfColumns), i / NumberOfColumns * sampleTileTexture.Height - i / NumberOfColumns),
                    Vector2.Zero);

                IntroTiles[i].IsTouchEnabled = true;

                IntroTiles[i].OnTap += (sender, pos) =>
                {
                    Close();
                };

                IntroTiles[i].AttachToParentLayer();
            }

            TapToExitLayer = new Layer(this, Layer.LayerTypes.Static, Vector2.Zero, 1, false);
            AddLayer(TapToExitLayer);

            Texture titleTexture = Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.MainTitle);

            LayerObject titleLayerObject = new LayerObject(TilesLayer, titleTexture, new Vector2(200, titleTexture.Height / 4 - titleTexture.Height / 4));
            titleLayerObject.Rotation = -10;
            titleLayerObject.SetScale(LayerObject.ScaleFactors.Half);
            titleLayerObject.AttachToParentLayer();

            TapToExitText = new Text(
                TapToExitLayer,
                new Vector2(353, Height - 42),
                Director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.IntroLetters),
                "tap to skip",
                Color.White, Color.Black);

            TapToExitText.AttachToParentLayer();

            Vector2 storyBoardSplinePointsHeightOffset = new Vector2(0, -200);

            List<SplinePoint> storyBoardSplinePoints = new List<SplinePoint>();
            storyBoardSplinePoints.Add(new SplinePoint(IntroTiles[0].Position - new Vector2(sampleTileTexture.Width - 150, 0) + storyBoardSplinePointsHeightOffset));
            for (int i = 0; i < 12; i++)
            {
                if (i == 7 || i == 10)
                {
                    continue;
                }

                if (i == 6 || i == 9)
                {
                    storyBoardSplinePoints.Add(new SplinePoint(IntroTiles[i].Position + storyBoardSplinePointsHeightOffset));
                    storyBoardSplinePoints.Add(new SplinePoint(IntroTiles[i].Position + storyBoardSplinePointsHeightOffset + new Vector2(sampleTileTexture.Width, 0)));
                }
                else
                {
                    storyBoardSplinePoints.Add(new SplinePoint(IntroTiles[i].Position + storyBoardSplinePointsHeightOffset - new Vector2(100, 0)));
                }
            }

            storyBoardSplinePoints.Add(new SplinePoint(IntroTiles[11].Position + storyBoardSplinePointsHeightOffset));
            storyBoardSplinePoints.Add(new SplinePoint(IntroTiles[11].Position + storyBoardSplinePointsHeightOffset));
            storyBoardSplinePoints.Add(new SplinePoint(IntroTiles[11].Position + storyBoardSplinePointsHeightOffset));

            StoryBoardSpline = new Spline(Spline.InterpolationType.CatmullRom, storyBoardSplinePoints.ToArray(), 3.5f);

            StoryBoardSpline.OnControlPointIndexReset += (sender, args) =>
            {
                Stop();
                Close();
            };

            Director.SoundManager.PlaySong(Director.CurrentResourcesManager.GetCachedSong((int)GameDirector.SongAssets.MainMenuTheme), true);
        }

        public void Close()
        {
            if (!IsClosing)
            {
                IsClosing = true;

                FadeDirectorTransition fadeDirectorTransition = new FadeDirectorTransition(Director, Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.BlankPixel), Director.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.Loading));
                Director.LoadSingleScene(
                    (int)GameDirector.ScenesTypes.MainMenuFirstScreen,
                    true,
                    fadeDirectorTransition,
                    () =>
                    {
                        Director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet(Path.Combine(Path.Combine("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.MainMenu1SpriteSheet.ToString()), (int)GameDirector.SpriteSheetAssets.MainMenu1SpriteSheet, typeof(GameDirector.TextureAssets), false);
                        Director.CurrentResourcesManager.CacheAllTexturesFromSpriteSheet(Path.Combine(Path.Combine("Sprites", "SpriteSheets"), GameDirector.SpriteSheetAssets.MainMenu2SpriteSheet.ToString()), (int)GameDirector.SpriteSheetAssets.MainMenu2SpriteSheet, typeof(GameDirector.TextureAssets), false);

                        GameDirector.Instance.CacheTutorialDependingOnDevice();
                    },
                    null);
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (!HasAnimationEnded)
            {
                base.Update(elapsedTime);

                float angle = 0;
                Camera.Position = StoryBoardSpline.GetNextPositionWithAngle(Camera.Position, (float)elapsedTime.TotalSeconds, out angle).Point;
            }
        }

        private bool IsClosing { get; set; }
        private bool HasAnimationEnded { get; set; }
        private Vector2 DragOffset { get; set; }

        private Spline StoryBoardSpline { get; set; }
        private Text TapToExitText { get; set; }
        private Layer TilesLayer { get; set; }
        private Layer TapToExitLayer { get; set; }
        private LayerObject[] IntroTiles { get; set; }

        private const int NumberOfColumns = 3;
    }
}
