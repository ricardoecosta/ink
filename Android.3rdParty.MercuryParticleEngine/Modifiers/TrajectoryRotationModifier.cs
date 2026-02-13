/*  
 Copyright © 2009 Project Mercury Team Members (http://mpe.codeplex.com/People/ProjectPeople.aspx)

 This program is licensed under the Microsoft Permissive License (Ms-PL).  You should 
 have received a copy of the license along with the source code.  If not, an online copy
 of the license can be found at http://mpe.codeplex.com/license.
*/

namespace ProjectMercury.Modifiers
{
    using System;
    using System.ComponentModel;

    /// <summary>
    /// Defines a Modifier which adjusts the rotation of a Particle to follow its trajectory.
    /// </summary>
    /// <remarks>Ideally this modifier should be added *after* any other physics modifiers.</remarks>
#if WINDOWS
    [TypeConverter("ProjectMercury.Design.Modifiers.TrajectoryRotationModifierTypeConverter, ProjectMercury.Design")]
#endif
    public sealed class TrajectoryRotationModifier : Modifier
    {
        /// <summary>
        /// The rotation offset to add to the calculated trajectory rotation.
        /// </summary>
        public float RotationOffset;

        /// <summary>
        /// Returns a deep copy of the Modifier implementation.
        /// </summary>
        /// <returns></returns>
        public override Modifier DeepCopy()
        {
            return new TrajectoryRotationModifier
            {
                RotationOffset = this.RotationOffset
            };
        }

        /// <summary>
        /// Processes the particles.
        /// </summary>
        /// <param name="dt">Elapsed time in whole and fractional seconds.</param>
        /// <param name="particle">A pointer to an array of particles.</param>
        /// <param name="count">The number of particles which need to be processed.</param>
        protected internal override void Process(float dt, Particle[] particleArray, int count)
        {
            Particle previousParticle = particleArray[0];

            for (int i = 0; i < count; i++)
            {
                if (particleArray[i].Momentum == previousParticle.Momentum)
                {
                    particleArray[i].Rotation = previousParticle.Rotation;

                    continue;
                }

                float rads = Calculator.Atan2(particleArray[i].Momentum.Y, particleArray[i].Momentum.X);

                particleArray[i].Rotation = (rads + this.RotationOffset);

                if (particleArray[i].Rotation > Calculator.Pi)
                    particleArray[i].Rotation -= Calculator.TwoPi;

                else if (particleArray[i].Rotation < -Calculator.Pi)
                    particleArray[i].Rotation += Calculator.TwoPi;

                previousParticle = particleArray[i];
            }
        }
    }
}