using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PathfindingTest.State
{
    public class GameTimeManager
    {
        public int targetFPS = 60;
        private double msPerFrame { get; set; }

        public double currentUpdateStartMS { get; set; }
        public double currentDrawStartMS { get; set; }

        public double previousUpdateStartMS { get; set; }
        public double previousDrawStartMS { get; set; }

        /// <summary>
        /// If you want your objects to move frame-independantly, multiply your movement speed variable by this one.
        /// </summary>
        public double time_step { get; set; }


        private static GameTimeManager instance = null;

        private GameTimeManager() { msPerFrame = (1000.0 / targetFPS); }

        /// <summary>
        /// Gets the amount of MS that is left to update this frame.
        /// </summary>
        /// <returns>The amount of MS left</returns>
        public double UpdateMSLeftThisFrame()
        {
            return this.msPerFrame - (new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds - this.previousUpdateStartMS);
        }

        /// <summary>
        /// Gets the amount of MS that is left to draw this frame.
        /// </summary>
        /// <returns>The amount of MS left</returns>
        public double DrawMSLeftThisFrame()
        {
            return this.msPerFrame - (new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds - this.previousDrawStartMS);
        }

        public static GameTimeManager GetInstance()
        {
            if (instance == null) instance = new GameTimeManager();
            return instance;
        }


        /// <summary>
        /// Updates all the variables in the manager. Should only be called at the very beginning of the Update loop!
        /// </summary>
        public void OnStartUpdate()
        {
            this.previousUpdateStartMS = this.currentUpdateStartMS;
            this.currentUpdateStartMS = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;

            double msTakenLastFrame = this.currentUpdateStartMS - this.previousUpdateStartMS;
            // if (Game1.GetInstance().frames % 120 == 0) 
                // Console.Out.WriteLine(msTakenLastFrame);



            time_step = (msTakenLastFrame / this.msPerFrame);
        }

        /// <summary>
        /// Updates all the variables in the manager. Should only be called at the very beginning of the Draw loop!
        /// </summary>
        public void OnStartDraw()
        {
            this.previousDrawStartMS = this.currentDrawStartMS;
            this.currentDrawStartMS = new TimeSpan(DateTime.UtcNow.Ticks).TotalMilliseconds;
        }
    }
}
