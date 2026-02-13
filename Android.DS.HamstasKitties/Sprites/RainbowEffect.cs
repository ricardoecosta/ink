using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProjectMercury;
using Microsoft.Xna.Framework;
using GameLibrary.Logic;
using Microsoft.Xna.Framework.Graphics;
using GameLibrary.UI;
using GameLibrary.Utils;
using HnK.Constants;

namespace HnK.Sprites
{
    public class RainbowEffect : ParticleEffect, GameLibrary.UI.IUpdateable
    {
        public RainbowEffect(Layer parentLayer, ParticleEffect particleEffect)
            : base()
        {
            ParticleEffect = particleEffect;

            List<SplinePoint> randomSplinePoints = new List<SplinePoint>();
            for (int i = 0; i < 16; i++)
            {
                randomSplinePoints.Add(new SplinePoint(new Vector2(i % 2 == 0 ? GlobalConstants.DefaultSceneWidth / 4 : 3 * (GlobalConstants.DefaultSceneWidth / 4), GlobalConstants.DefaultSceneHeight - (i * GlobalConstants.DefaultSceneHeight / 16))));
            }

            Path = new Spline(Spline.InterpolationType.CatmullRom, randomSplinePoints.ToArray(), 1.8f);
            Path.OnControlPointIndexReset += (sender, args) =>
            {
                parentLayer.DetachParticleEffect(particleEffect);
            };

            ParticleEffect.Initialise();
            parentLayer.AttachParticleEffect(ParticleEffect);
        }

        public void Update(TimeSpan elapsedTime)
        {
            float angle = 0;
            AbsolutePosition = Path.GetNextPositionWithAngle(AbsolutePosition, (float)elapsedTime.TotalSeconds, out angle).Point;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            ParticleEffect.Trigger(AbsolutePosition);
        }

        private Spline Path { get; set; }
        private Vector2 AbsolutePosition { get; set; }
        private ParticleEffect ParticleEffect { get; set; }
    }
}
