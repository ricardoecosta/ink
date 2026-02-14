using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HamstasKitties.UI;
using HamstasKitties.Core;
using HamstasKitties.Layers;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using HamstasKitties.GameModes;
using HamstasKitties.Animation;
using HamstasKitties.Social.Achievements;
using Microsoft.Xna.Framework;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Constants;

namespace HamstasKitties.Scenes
{
    public class AchievementPopup : Scene
    {
        public AchievementPopup(GameDirector director)
            : base(director, GlobalConstants.DefaultSceneWidth, GlobalConstants.DefaultSceneHeight)
        {
            Director = director;
        }

        public override void Initialize()
        {
            base.Initialize();

            Level level = Director.UnderlyingScene as Level;
            if (level == null)
            {
                throw new ArgumentNullException("Creating an achievement popup but Level.CurrentAchievement is null!");
            }

            Achievement achievement = level.CurrentAchievement;

            AchievementPopupBGLayer = new AchievementPopupBGLayer(this, 0);
            AchievementPopupButtonsLayer = new AchievementPopupButtonsLayer(this, 1, achievement);
            Layer achievementPopupDetailsLayer = new Layer(this, Layer.LayerTypes.Static, Vector2.Zero, 2, false);

            AchievementPopupBGLayer.Initialize();
            AchievementPopupButtonsLayer.Initialize();

            AddLayer(AchievementPopupBGLayer);
            AddLayer(AchievementPopupButtonsLayer);
            AddLayer(achievementPopupDetailsLayer);

            List<StringBuilder> titleLines = Utils.GetTextLines(
                achievement.Data.Name,
                Director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.BigAchievementEntryTitle),
                375, 1);

            List<StringBuilder> descriptionLines = Utils.GetTextLines(
                achievement.Data.Description,
                Director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.BigAchievementEntryDescription),
                375, 3);

            int cursorYPos = 340;

            Text currentline;
            for (int i = 0; i < titleLines.Count; i++)
            {
                currentline = new Text(
                    achievementPopupDetailsLayer,
                    new Vector2(60, cursorYPos),
                    Director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.BigAchievementEntryTitle),
                    titleLines[i].ToString(),
                    new Color(255, 94, 0),
                    Color.Black);
                currentline.OutlineThickness = 3;

                currentline.AttachToParentLayer();

                if (i < titleLines.Count - 1)
                {
                    cursorYPos += SpaceBetweenTitleLines;
                }
            }

            cursorYPos += SpaceBetweenTitleAndDescription;

            for (int i = 0; i < descriptionLines.Count; i++)
            {
                currentline = new Text(
                    achievementPopupDetailsLayer,
                    new Vector2(60, cursorYPos),
                    Director.GlobalResourcesManager.GetCachedFont((int)GameDirector.FontsAssets.BigAchievementEntryDescription),
                    descriptionLines[i].ToString().Trim(),
                    Color.Black,
                    Color.White);
                currentline.OutlineThickness = 3;

                currentline.AttachToParentLayer();

                if (i < descriptionLines.Count - 1)
                {
                    cursorYPos += SpaceBetweenDescriptionLines;
                }
            }

            CreateCoolnessHamsta(achievementPopupDetailsLayer);
        }

        private void CreateCoolnessHamsta(Layer parentLayer)
        {
            Texture coolHamsta = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.AchivementUnlockedHamsta);
            Texture coolHamstaBlink = GameDirector.Instance.GlobalResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.HamstaBlink);

            LayerObject coolHamstaLayerObject = new LayerObject(parentLayer, coolHamsta, new Vector2(Width - coolHamsta.Width, Height), Vector2.Zero);
            LayerObject coolHamstaBlinkLayerObject = new LayerObject(parentLayer, coolHamstaBlink, new Vector2(445, 710));

            coolHamstaLayerObject.AttachToParentLayer();

            PopupTweener = new Tweener(Height, Height - coolHamsta.Height, 1f, (t, b, c, d) => Elastic.EaseInOut(t, b, c, d), false);
            PopupTweener.OnUpdate += (value) =>
            {
                coolHamstaLayerObject.Position = new Vector2(coolHamstaLayerObject.Position.X, value);
            };

            PopupTweener.OnFinished += (sender, e) =>
            {
                BlinkTweener.Start();
                coolHamstaBlinkLayerObject.AttachToParentLayer();
            };

            BlinkTweener = new Tweener(0.6f, 1, 1.8f, (t, b, c, d) => Sinusoidal.EaseInOut(t, b, c, d), true);
            BlinkTweener.OnUpdate += (value) =>
            {
                coolHamstaBlinkLayerObject.Alpha = value;
            };

            BlinkTweener.OnFinished += (sender, e) =>
            {
                BlinkTweener.Reverse();
                BlinkTweener.Start();
            };

            PopupTweener.Start();

            Director.SoundManager.PlaySound(Director.CurrentResourcesManager.GetCachedSoundEffect((int)GameDirector.SoundEffectsAssets.Yeah));
        }

        public void Close()
        {
            Level level = Director.UnderlyingScene as Level;
            if (level != null)
            {
                level.MarkCurrentAchievementPopupAsShown();
                level.ReleaseAnyDraggingBlocks();
            }

            Director.PopCurrentScene();
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);

            PopupTweener.Update(elapsedTime);
            BlinkTweener.Update(elapsedTime);
        }

        private AchievementPopupBGLayer AchievementPopupBGLayer { get; set; }
        private AchievementPopupButtonsLayer AchievementPopupButtonsLayer { get; set; }

        private Tweener PopupTweener { get; set; }
        private Tweener BlinkTweener { get; set; }

        private const int SpaceBetweenTitleLines = 50;
        private const int SpaceBetweenDescriptionLines = 38;
        private const int SpaceBetweenTitleAndDescription = 55;
    }
}
