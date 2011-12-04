using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomLists.Lists;
using Microsoft.Xna.Framework.Graphics;
using ParticleEngine.Particle;
using Microsoft.Xna.Framework;
using ParticleEngine.Gametime;

namespace ParticleEngine.Emitter
{
    public abstract class ParticleEmitter
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }

        public Boolean alive { get; set; }
        public Boolean stopSpawningParticles { get; set; }

        public float particleRandomX { get; set; }
        public float particleRandomY { get; set; }

        public float particleSpeedX { get; set; }
        public float particleSpeedY { get; set; }

        public float particleRandomSpeedX { get; set; }
        public float particleRandomSpeedY { get; set; }

        public float particleScale { get; set; }
        public float particleScaleRandom { get; set; }

        public float particleRadianRotation { get; set; }
        public float particleRadianRotationRandom { get; set; }
        public float particleRadianRotationSpeed { get; set; }
        public float particleRadianRotationSpeedRandom { get; set; }

        public int ticksPerSecond { get; set; }
        public double lastTick { get; set; }
        public int particlesPerTick { get; set; }
        public int maxParticles { get; set; }

        public float particleLifespanMS { get; set; }
        public float particleLifespanRandomMS { get; set; }

        public double lifespanMS { get; set; }
        public double creationTimeMS { get; set; }

        public Color particleColor { get; set; }

        public CustomArrayList<AbstractParticle> particles { get; set; }

        public Boolean fadeAccordingToLifespan { get; set; }
        public Boolean inverseFade { get; set; }

        public Boolean scaleAccordingToLifespan { get; set; }
        public Boolean inverseScale { get; set; }

        public BlendState blendState = BlendState.Additive;
        public SpriteBatch sb { get; set; }

        public Texture2D particleTexture { get; set; }

        public ParticleEmitter(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            this.particleSpeedX = 1f;
            this.particleSpeedY = 1f;

            this.particleColor = Color.White;

            this.particleScale = 1f;
            this.particleLifespanMS = float.MaxValue;
            this.particles = new CustomArrayList<AbstractParticle>();

            this.ticksPerSecond = 60;
            this.creationTimeMS = GameTimeManager.GetInstance().currentUpdateStartMS;

            this.lifespanMS = Double.MaxValue;

            this.alive = true;

            ParticleManager.GetInstance().emitterList.AddLast(this);
        }

        public virtual void Update(float time_step)
        {
            ParticleManager manager = ParticleManager.GetInstance();
            // Update
            if (manager.screenBounds != Rectangle.Empty)
            {
                for (int i = 0; i < this.particles.Count(); i++)
                {
                    AbstractParticle particle = this.particles.ElementAt(i);
                    if (!particle.Update(time_step) || manager.screenBounds.Contains(particle.GetLocation()))
                    {
                        this.particles.Remove(particle);
                    }
                }
            }
            else
            {
                for (int i = 0; i < this.particles.Count(); i++)
                {
                    AbstractParticle particle = this.particles.ElementAt(i);
                    if (!particle.Update(time_step))
                    {
                        this.particles.Remove(particle);
                    }
                }
            }

            double currTime = GameTimeManager.GetInstance().currentUpdateStartMS;
            if (currTime - this.creationTimeMS > this.lifespanMS) this.alive = false;

            if (alive)
            {
                if (!stopSpawningParticles)
                {
                    if (currTime - lastTick > (1000 / this.ticksPerSecond))
                    {
                        for (int i = this.particlesPerTick; i >= 0; i--)
                            this.CreateParticle();
                        this.lastTick = currTime;
                    }
                }
            }
            else if (this.particles.Count() == 0) ParticleManager.GetInstance().emitterList.Remove(this);
        }

        public virtual void Draw(GraphicsDevice device)
        {
            if (sb == null) sb = new SpriteBatch(device);
            sb.Begin(SpriteSortMode.FrontToBack, this.blendState);
            for (int i = 0; i < this.particles.Count(); i++)
            {
                this.particles.ElementAt(i).Draw(sb);
            }
            sb.End();
        }

        /// <summary>
        /// Forces this effect to die quickly.
        /// </summary>
        /// <param name="timeLeftMS">The time left in MS before this effect is completely gone</param>
        public void QuicklyDie(int timeLeftMS)
        {
            this.alive = false;
            for (int i = 0; i < this.particles.Count(); i++)
            {
                AbstractParticle part = this.particles.ElementAt(i);
                part.lifespanMS = (int)(GameTimeManager.GetInstance().currentUpdateStartMS - part.creationTime) + timeLeftMS;
                part.fadeAccordingToLifespan = true;
            }
        }

        protected abstract void CreateParticle();
    }
}
