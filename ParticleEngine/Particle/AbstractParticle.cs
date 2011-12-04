using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using ParticleEngine.Emitter;
using ParticleEngine.Gametime;

namespace ParticleEngine.Particle
{
    public abstract class AbstractParticle
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public float speedX { get; set; }
        public float speedY { get; set; }

        public float scale { get; set; }

        public float lifespanMS { get; set; }
        public double creationTime { get; set; }

        public float radianRotation { get; set; }
        public float radianRotationSpeed { get; set; }

        public Boolean alive = true;

        public Texture2D texture { get; set; }

        public Color color { get; set; }

        public ParticleEmitter emitter { get; set; }

        private Vector2 drawOffset { get; set; }

        public Boolean fadeAccordingToLifespan { get; set; }
        public Boolean inverseFade { get; set; }

        public Boolean scaleAccordingToLifespan { get; set; }
        public Boolean inverseScale { get; set; }

        public AbstractParticle(ParticleEmitter emitter)
        {
            long ticks = DateTime.UtcNow.Ticks;
            this.creationTime = new TimeSpan(ticks).TotalMilliseconds;

            if (emitter.particleTexture == null)
                this.texture = ParticleManager.DEFAULT_TEXTURE;
            else this.texture = emitter.particleTexture;

            Random random = new Random();
            this.x = emitter.x + ((random.Next(0, 10000) / 10000f) * emitter.particleRandomX);
            this.y = emitter.y + ((random.Next(0, 10000) / 10000f) * emitter.particleRandomY);
            this.z = emitter.z;

            this.scale = emitter.particleScale + ((random.Next(0, 10000) / 10000f) * emitter.particleScaleRandom);

            this.speedX = emitter.particleSpeedX + ((random.Next(0, 10000) / 10000f) * emitter.particleRandomSpeedX);
            this.speedY = emitter.particleSpeedY + ((random.Next(0, 10000) / 10000f) * emitter.particleRandomSpeedY);

            this.lifespanMS = emitter.particleLifespanMS + ((random.Next(0, 10000) / 10000f) * emitter.particleLifespanRandomMS);

            this.radianRotation = emitter.particleRadianRotation + ((random.Next(0, 10000) / 10000f) * emitter.particleRadianRotationRandom);
            this.radianRotationSpeed = emitter.particleRadianRotationSpeed + ((random.Next(0, 10000) / 10000f) * emitter.particleRadianRotationSpeedRandom);

            this.color = emitter.particleColor;

            this.fadeAccordingToLifespan = emitter.fadeAccordingToLifespan;
            this.inverseFade = emitter.inverseFade;

            this.scaleAccordingToLifespan = emitter.scaleAccordingToLifespan;
            this.inverseScale = emitter.inverseScale;

            this.emitter = emitter;

            this.emitter.particles.AddLast(this);
        }


        /// <summary>
        /// Determines if the particle should die.
        /// </summary>
        /// <returns>Yes or no. Chose. Wisely.</returns>
        public Boolean ShouldDie()
        {
            return GameTimeManager.GetInstance().currentUpdateStartMS - this.creationTime > lifespanMS;
        }

        /// <summary>
        /// Updates the particle.
        /// </summary>
        /// <param name="time_step">FPS timestep</param>
        /// <returns>True if the update was successful, false if the particle should be removed!</returns>
        public Boolean Update(float time_step)
        {
            if (!alive) return alive;
            // Check if we should die
            if( this.ShouldDie() ) {
                // Yes
                this.alive = false;
            }
            else
            {
                // No
                this.x += (speedX * time_step);
                this.y += (speedY * time_step);

                this.radianRotation += (radianRotationSpeed * time_step);
            }
            // Return
            return alive;
        }

        /// <summary>
        /// Gets the percentage of the lifespan the particle
        /// </summary>
        /// <returns></returns>
        public int GetPercentageOfLifespan()
        {
            return (int)(((GameTimeManager.GetInstance().currentUpdateStartMS - this.creationTime) / this.lifespanMS) * 100f);
        }

        public void Draw(SpriteBatch sb)
        {
            if (!alive) return;
            Color drawColor = this.color;
            if (fadeAccordingToLifespan)
            {
                int percentageOfLifespan = this.GetPercentageOfLifespan();
                if( inverseFade )
                    drawColor = new Color(this.color.R, this.color.G, this.color.B, (int)(percentageOfLifespan * 2.55f));
                else
                    drawColor = new Color(this.color.R, this.color.G, this.color.B, 255 - (int)(percentageOfLifespan * 2.55f));
            }
            float newScale = this.scale;
            if (scaleAccordingToLifespan)
            {
                int percentageOfLifespan = this.GetPercentageOfLifespan();
                if( inverseScale )
                    newScale = this.scale * (1f - (percentageOfLifespan / 100f));
                else
                    newScale = this.scale * (percentageOfLifespan / 100f);
            }

            drawOffset = new Vector2(((this.texture.Width * newScale) / 2f),
                ((this.texture.Height * newScale) / 2f));

            sb.Draw(this.texture, new Rectangle((int)(this.x - this.drawOffset.X), (int)(this.y - this.drawOffset.Y),
                (int)(texture.Width * newScale), (int)(texture.Height * newScale)), null, drawColor,
                this.radianRotation, Vector2.Zero /*this.drawOffset*/, SpriteEffects.None, this.z);
        }

        /// <summary>
        /// Gets the location of this particle on the screen.
        /// </summary>
        /// <returns>The point.</returns>
        public Point GetLocation()
        {
            return new Point((int)x, (int)y);
        }
    }
}
