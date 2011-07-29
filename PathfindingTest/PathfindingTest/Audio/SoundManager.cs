using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace PathfindingTest.Audio
{
    public class SoundManager
    {

        public LinkedList<SoundEffect> arrowSounds { get; set; }
        public LinkedList<SoundEffect> swordSounds { get; set; }

        private static SoundManager instance { get; set; }

        public static SoundManager GetInstance()
        {
            if (instance == null)
            {
                instance = new SoundManager();
            }
            return instance;
        }

        public SoundManager()
        {
            arrowSounds = new LinkedList<SoundEffect>();
            swordSounds = new LinkedList<SoundEffect>();

            InitiateSound();
        }

        public void InitiateSound()
        {
            arrowSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Arrow1"));
            arrowSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Arrow2"));

            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword1"));
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword2"));
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword3"));
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword4"));
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword5"));
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword6"));
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword7"));
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword8"));
        }

        public void PlaySound(LinkedList<SoundEffect> source)
        {
            SoundEffect play = source.ElementAt(new Random().Next(source.Count));
            SoundEffectInstance playInstance = play.CreateInstance();

            playInstance.Volume = 0.45f;
            playInstance.Play();
        }
    }
}
