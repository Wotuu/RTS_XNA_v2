using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using CustomLists.Lists;
using ParticleEngine.Emitter;
using ParticleEngine.Gametime;
using ParticleEngine.Particle;

namespace ParticleEngine
{
    public class ParticleManager
    {
        public CustomArrayList<ParticleEmitter> emitterList { get; set; }

        public static Texture2D DEFAULT_TEXTURE { get; set; }
        public static Texture2D RING_TEXTURE { get; set; }

        public Rectangle screenBounds { get; set; }

        private double pauseStartMS { get; set; }

        private static ParticleManager instance { get; set; }
        public static ParticleManager GetInstance()
        {
            if (instance == null) instance = new ParticleManager();
            return instance;
        }

        private ParticleManager()
        {
            this.emitterList = new CustomArrayList<ParticleEmitter>();
        }

        /// <summary>
        /// Pauses updating the particles, to be resumed by ResumeUpdating();
        /// </summary>
        public void PauseUpdating()
        {
            this.pauseStartMS = GameTimeManager.GetInstance().currentUpdateStartMS;
        }

        /// <summary>
        /// Resumes updating particles.
        /// </summary>
        /// <param name="currentTimeMS">The current time in milliseconds.</param>
        public void ResumeUpdating(double currentTimeMS)
        {
            double timePaused = currentTimeMS - this.pauseStartMS;
            for (int i = 0; i < this.emitterList.Count(); i++)
            {
                ParticleEmitter e = this.emitterList.ElementAt(i);
                e.creationTimeMS += timePaused;
                for (int j = 0; j < e.particles.Count(); j++)
                {
                    AbstractParticle ap = e.particles.ElementAt(j);
                    if (ap.alive) ap.creationTime += timePaused;
                }
            }
        }

        public void Update(float time_step)
        {
            GameTimeManager.GetInstance().OnStartUpdate();
            for (int i = 0; i < this.emitterList.Count(); i++)
            {
                this.emitterList.ElementAt(i).Update(time_step);
            }
        }

        public void Draw(GraphicsDevice device)
        {
            GameTimeManager.GetInstance().OnStartDraw();
            for (int i = 0; i < this.emitterList.Count(); i++)
            {
                this.emitterList.ElementAt(i).Draw(device);
            }
        }

        /// <summary>
        /// Gets the amount of particles currently on screen.
        /// </summary>
        public int GetParticleCount()
        {
            int result = 0;
            for (int i = 0; i < this.emitterList.Count(); i++)
            {
                result += this.emitterList.ElementAt(i).particles.Count();
            }
            return result;
        }

        /// <summary>
        /// Removes all emitters from the manager.
        /// </summary>
        /// <param name="timeToDieMS">The time in which all effects must be gone.</param>
        public void RemoveAllEmitters(int timeToDieMS)
        {
            for (int i = 0; i < this.emitterList.Count(); i++)
            {
                this.emitterList.ElementAt(i).QuicklyDie(timeToDieMS);
            }
        }
    }
}
