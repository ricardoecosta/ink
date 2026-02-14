/*
 Copyright  2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace HamstasKitties.Particles.Modifiers
{
    using System.ComponentModel;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Defines a modifier which changes the colour of particles based on a linear interpolation over three values.
    /// </summary>
#if WINDOWS
    [TypeConverter("ProjectMercury.Design.Modifiers.ColourInterpolatorModifierTypeConverter, ProjectMercury.Design")]
#endif
    public class ColourInterpolatorModifier : Modifier
    {
        /// <summary>
        /// Gets or sets the initial colour.
        /// </summary>
        /// <value>The initial colour.</value>
        public Vector3 InitialColour { get; set; }

        /// <summary>
        /// Gets or sets the middle colour.
        /// </summary>
        /// <value>The middle colour.</value>
        public Vector3 MiddleColour { get; set; }

        private float _middlePosition;

        /// <summary>
        /// Gets or sets the middle colour position.
        /// </summary>
        /// <value>The middle position.</value>
        public float MiddlePosition
        {
            get { return this._middlePosition; }
            set
            {
                Guard.ArgumentOutOfRange("MiddlePosition", value, 0f, 1f);

                this._middlePosition = value;
            }
        }

        /// <summary>
        /// Gets or sets the final colour.
        /// </summary>
        /// <value>The final colour.</value>
        public Vector3 FinalColour { get; set; }

        /// <summary>
        /// Returns a deep copy of the Modifier implementation.
        /// </summary>
        /// <returns></returns>
        public override Modifier DeepCopy()
        {
            return new ColourInterpolatorModifier
            {
                FinalColour = this.FinalColour,
                InitialColour = this.InitialColour,
                MiddleColour = this.MiddleColour,
                MiddlePosition = this.MiddlePosition,
            };
        }

        /// <summary>
        /// Processes the particles.
        /// </summary>
        /// <param name="elapsedSeconds">Elapsed time in whole and fractional seconds.</param>
        /// <param name="particleArray">A pointer to an array of particles.</param>
        /// <param name="count">The number of particles which need to be processed.</param>
        protected internal override void Process(float elapsedSeconds, Particle[] particleArray, int count)
        {
            float lerpPosition;
            Vector3 lerpColourA;
            Vector3 lerpColourB;

            Particle previousParticle = particleArray[0];
            for (int i = 0; i < count; i++)
            {
                // If the particle was released at the same time as the previous particle, there is no need to
                // interpolate the colour, so just copy it from the previous particle...
                if (particleArray[i].Age == previousParticle.Age)
                {
                    particleArray[i].Colour.X = previousParticle.Colour.X;
                    particleArray[i].Colour.Y = previousParticle.Colour.Y;
                    particleArray[i].Colour.Z = previousParticle.Colour.Z;
                }
                else
                {
                    // If the particle age is less than middle position, interpolate between InitialColour and MiddleColour...
                    if (particleArray[i].Age < this.MiddlePosition)
                    {
                        lerpColourA = this.InitialColour;
                        lerpColourB = this.MiddleColour;
                        lerpPosition = particleArray[i].Age / this.MiddlePosition;
                    }
                    // Otherwise, interpolate between MiddleColour and FinalColour...
                    else
                    {
                        lerpColourA = this.MiddleColour;
                        lerpColourB = this.FinalColour;
                        lerpPosition = (particleArray[i].Age - this.MiddlePosition) / (1f - this.MiddlePosition);
                    }

                    // Interpolate between the necessary colours...
                    particleArray[i].Colour.X = lerpColourA.X + ((lerpColourB.X - lerpColourA.X) * lerpPosition);
                    particleArray[i].Colour.Y = lerpColourA.Y + ((lerpColourB.Y - lerpColourA.Y) * lerpPosition);
                    particleArray[i].Colour.Z = lerpColourA.Z + ((lerpColourB.Z - lerpColourA.Z) * lerpPosition);
                }

                previousParticle = particleArray[i];
            }
        }
    }
}
