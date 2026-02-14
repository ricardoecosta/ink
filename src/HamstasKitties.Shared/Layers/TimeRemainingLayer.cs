using System;
using System.Collections.Generic;
using HamstasKitties.Animation;
using HamstasKitties.Core;
using HamstasKitties.Core.Interfaces;
using HamstasKitties.UI;
using HamstasKitties.Mechanics;
using HamstasKitties.GameModes;
using HamstasKitties.Management;
using HamstasKitties.Scenes.GameModes;
using HamstasKitties.Sprites;
using HamstasKitties.Extras;
using Microsoft.Xna.Framework;
using HamstasKitties.Scenes.Menus;
using HamstasKitties.Constants;
using HamstasKitties.Extensions;
using HamstasKitties.Animation.Tween;
using HamstasKitties.Utils;
using static HamstasKitties.Utils.Utils;

namespace HamstasKitties.Layers
{
    public class TimeRemainingLayer : Layer
    {
        public TimeRemainingLayer(CountdownMode countdownModeLevel)
            : base(countdownModeLevel, LayerTypes.Static, new Vector2(0, countdownModeLevel.Height - HeightOffset), 100, false)
        {
            CountdownModeLevel = countdownModeLevel;
            Width = 250;
            Height = 120;
            TransformationMatrix = Matrix.CreateTranslation(new Vector3((-Width / 2), -(countdownModeLevel.Height - Height) - Height / 2, 0)) * Matrix.CreateRotationZ(Rotation.ToRadians()) * Matrix.CreateTranslation(new Vector3(Width / 2, countdownModeLevel.Height - HeightOffset + Height / 2, 0));
            IsInHurryUpMode = false;
        }

        public override void Initialize()
        {
            base.Initialize();

            CurrentTimeBitmapText = new BitmapText(this, GetCountdownCharacters(), new DateTime(CountdownModeLevel.CurrentLevelTime.Ticks).ToString("mm:ss"), TextPosition, BitmapText.AlignmentTypes.Left, 45);
            if (CountdownModeLevel.IsInHurryTimeMode)
            {
                CurrentTimeBitmapText.Color = Color.Red;
            }

            CurrentTimeBitmapText.AttachToParentLayer();
            CurrentTimeBitmapText.UpdateText(new DateTime(CountdownModeLevel.CurrentLevelTime.Ticks).ToString("mm:ss"));

            Texture timeBackTexture = GameDirector.Instance.CurrentResourcesManager.GetCachedTexture((int)GameDirector.TextureAssets.TimeBack);
            new LayerObject(this, timeBackTexture, new Vector2(-WidthOffset, 0), Vector2.Zero).AttachToParentLayer();

            CountdownModeLevel.CountdownTimer.OnStart += (sender, e) =>
            {
                CurrentTimeBitmapText.UpdateText(new DateTime(CountdownModeLevel.CurrentLevelTime.Ticks).ToString("mm:ss"));
            };

            CountdownModeLevel.CountdownTimer.OnFinished += (sender, e) =>
            {
                if (CountdownModeLevel.CurrentLevelTime.TotalSeconds <= GlobalConstants.CountdownModeHurryTimeInSeconds)
                {
                    if (!IsInHurryUpMode)
                    {
                        IsInHurryUpMode = true;
                        CurrentTimeBitmapText.Color = Color.Red;
                        HurryUpTweener.Start();
                    }
                }
                else
                {
                    if (IsInHurryUpMode)
                    {
                        IsInHurryUpMode = false;
                        CurrentTimeBitmapText.Color = Color.White;
                    }
                }

                if (CountdownModeLevel.CurrentLevelTime.Ticks >= 0)
                {
                    if (CountdownModeLevel.CurrentLevelTime.TotalSeconds < GlobalConstants.CountdownModeHurryTimeInSeconds)
                    {
                        HurryUpTweener = new Tweener(1, 1.2f, 0.15f, Sinusoidal.EaseOut, true);
                        HurryUpTweener.OnUpdate += (value) =>
                        {
                            Matrix scaleMatrix = Matrix.CreateScale(new Vector3(value, value, 0));

                            TransformationMatrix = Matrix.CreateTranslation(
                                new Vector3(
                                    (-Width / 2), -(CountdownModeLevel.Height - Height) - Height / 2, 0)) *
                                    Matrix.CreateRotationZ(Rotation.ToRadians()) * scaleMatrix *
                                    Matrix.CreateTranslation(new Vector3((Width / 2), CountdownModeLevel.Height - HeightOffset + Height / 2, 0));
                        };

                        HurryUpTweener.Start();
                    }

                    if (CountdownModeLevel.CurrentLevelTime.Ticks == 0)
                    {
                        HurryUpTweener.Stop();
                    }

                    CurrentTimeBitmapText.UpdateText(new DateTime(CountdownModeLevel.CurrentLevelTime.Ticks).ToString("mm:ss"));
                }
            };

            HurryUpTweener = new Tweener(1, 1.3f, 0.2f, Sinusoidal.EaseOut, true);
            HurryUpTweener.OnUpdate += (value) =>
            {
                Matrix scaleMatrix = Matrix.CreateScale(new Vector3(value, value, 0));
                TransformationMatrix = Matrix.CreateTranslation(new Vector3(-(Width / 2), -(CountdownModeLevel.Height - Height) - Height / 2, 0)) *
                    Matrix.CreateRotationZ(Rotation.ToRadians()) * scaleMatrix *
                    Matrix.CreateTranslation(new Vector3((Width / 2), CountdownModeLevel.Height - HeightOffset + Height / 2, 0));
            };

            CurrentTimeBitmapText.UpdateText(new DateTime(CountdownModeLevel.CurrentLevelTime.Ticks).ToString("mm:ss"));
        }

        public override void Update(TimeSpan elapsedTime)
        {
            base.Update(elapsedTime);
            HurryUpTweener.Update(elapsedTime);

            if (CountdownModeLevel.CurrentLevelTime.TotalSeconds > GlobalConstants.CountdownModeHurryTimeInSeconds && CurrentTimeBitmapText.Color != Color.White)
            {
                CurrentTimeBitmapText.Color = Color.White;
                CurrentTimeBitmapText.UpdateText(new DateTime(CountdownModeLevel.CurrentLevelTime.Ticks).ToString("mm:ss"));
            }

            // TODO: Check if this sort affects performance.
            LayerObjects.Sort((obj1, obj2) =>
            {
                int zOrderCompareResult = obj1.ZOrder.CompareTo(obj2.ZOrder);
                return zOrderCompareResult == 0 ? obj1.UniqueID.CompareTo(obj2.UniqueID) : zOrderCompareResult;
            });
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            // API FIXME: each layer should have its own transformation matrix
            spriteBatch.Begin(Microsoft.Xna.Framework.Graphics.SpriteSortMode.Immediate, null, null, null, null, null, TransformationMatrix);

            for (int i = 0; i < LayerObjects.Count; i++)
            {
                LayerObjects[i].Draw(spriteBatch);
            }

            spriteBatch.End();
        }

        private bool IsInHurryUpMode { get; set; }
        private CountdownMode CountdownModeLevel { get; set; }
        public BitmapText CurrentTimeBitmapText { get; set; }
        private Matrix TransformationMatrix { get; set; }
        private Tweener HurryUpTweener { get; set; }

        private const int HeightOffset = 108;
        private const int WidthOffset = 35;
        private const float Rotation = -5f;
        private readonly Vector2 TextPosition = new Vector2(10, 60);
    }
}
