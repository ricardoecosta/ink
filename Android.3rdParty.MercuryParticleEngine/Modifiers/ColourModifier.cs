/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Modifiers
{
    using System.ComponentModel;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// Defines a Modifier which gradually changes the colour of a Particle over the course of its lifetime.
    /// </summary>
#if WINDOWS
    [TypeConverter("ProjectMercury.Design.Modifiers.ColourModifierTypeConverter, ProjectMercury.Design")]
#endif
    public sealed class ColourModifier : Modifier
    {
        /// <summary>
        /// Returns a deep copy of the Modifier implementation.
        /// </summary>
        /// <returns></returns>
        public override Modifier DeepCopy()
        {
            return new ColourModifier
            {
                InitialColour = this.InitialColour,
                UltimateColour = this.UltimateColour
            };
        }

        /// <summary>
        /// The initial colour of Particles when they are released.
        /// </summary>
        public Vector3 InitialColour;

        /// <summary>
        /// The ultimate colour of Particles when they are retired.
        /// </summary>
        public Vector3 UltimateColour;

        /// <summary>
        /// Processes the particles.
        /// </summary>
        /// <param name="elapsedSeconds">Elapsed time in whole and fractional seconds.</param>
        /// <param name="particle">A pointer to the first item in an array of particles.</param>
        /// <param name="count">The number of particles which need to be processed.</param>
        protected internal override void Process(float elapsedSeconds, Particle[] particleArray, int count)
        {
            particleArray[0].Colour.X = (this.InitialColour.X + ((this.UltimateColour.X - this.InitialColour.X) * particleArray[0].Age));
            particleArray[0].Colour.Y = (this.InitialColour.Y + ((this.UltimateColour.Y - this.InitialColour.Y) * particleArray[0].Age));
            particleArray[0].Colour.Z = (this.InitialColour.Z + ((this.UltimateColour.Z - this.InitialColour.Z) * particleArray[0].Age));

            Particle previousParticle = particleArray[0];

            for (int i = 1; i < count; i++)
            {
                if (particleArray[i].Age < previousParticle.Age)
                {
                    particleArray[i].Colour.X = (this.InitialColour.X + ((this.UltimateColour.X - this.InitialColour.X) * particleArray[i].Age));
                    particleArray[i].Colour.Y = (this.InitialColour.Y + ((this.UltimateColour.Y - this.InitialColour.Y) * particleArray[i].Age));
                    particleArray[i].Colour.Z = (this.InitialColour.Z + ((this.UltimateColour.Z - this.InitialColour.Z) * particleArray[i].Age));
                }
                else
                {
                    particleArray[i].Colour.X = previousParticle.Colour.X;
                    particleArray[i].Colour.Y = previousParticle.Colour.Y;
                    particleArray[i].Colour.Z = previousParticle.Colour.Z;
                }

                previousParticle = particleArray[i];
            }
        }
    }
}
