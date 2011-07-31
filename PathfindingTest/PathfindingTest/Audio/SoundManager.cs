using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace PathfindingTest.Audio
{
    public class SoundManager
    {

        public float sfxVolume { get; set; }
        public float bgmVolume { get; set; }

        public Boolean bgmPlaying { get; set; }

        public LinkedList<SoundEffectInstance> arrowSounds { get; set; }
        public LinkedList<SoundEffectInstance> swordSounds { get; set; }
        public LinkedList<SoundEffectInstance> lanceSounds { get; set; }

        public SoundEffectInstance menuSound { get; set; }

        public enum BGMType
        {
            Menu,
            InGame
        }

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
            sfxVolume = 0.45f;
            bgmVolume = 1f;

            bgmPlaying = false;

            arrowSounds = new LinkedList<SoundEffectInstance>();
            swordSounds = new LinkedList<SoundEffectInstance>();
            lanceSounds = new LinkedList<SoundEffectInstance>();

            InitiateSound();
        }

        public void InitiateSound()
        {
            menuSound = Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Menu1").CreateInstance();

            arrowSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Arrow1").CreateInstance());
            arrowSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Arrow2").CreateInstance());

            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword1").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword2").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword3").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword4").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword5").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword6").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword7").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword8").CreateInstance());

            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance1").CreateInstance());
            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance2").CreateInstance());
            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance3").CreateInstance());
            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance4").CreateInstance());
            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance5").CreateInstance());
        }

        public void Update()
        {
            if (menuSound.State == SoundState.Stopped)
            {
                bgmPlaying = false;
            }
        }

        public void PlaySFX(LinkedList<SoundEffectInstance> source)
        {
            SoundEffectInstance sound = source.ElementAt(new Random().Next(source.Count));
            sound.Volume = 0.45f;
            sound.Play();
        }

        public void PlayBGM(BGMType type)
        {
            switch (type)
            {
                case BGMType.Menu:
                    if (!bgmPlaying)
                    {
                        menuSound.Volume = 1f;
                        menuSound.Play();
                        bgmPlaying = true;
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
