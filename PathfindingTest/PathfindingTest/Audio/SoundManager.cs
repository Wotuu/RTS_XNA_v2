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
        public Boolean silencePlayed { get; set; }

        public LinkedList<SoundEffectInstance> arrowSounds { get; set; }
        public LinkedList<SoundEffectInstance> swordSounds { get; set; }
        public LinkedList<SoundEffectInstance> lanceSounds { get; set; }

        public SoundEffectInstance menuSound { get; set; }
        public SoundEffectInstance silence { get; set; }
        public LinkedList<SoundEffectInstance> ingameSounds { get; set; }

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
            silencePlayed = false;

            arrowSounds = new LinkedList<SoundEffectInstance>();
            swordSounds = new LinkedList<SoundEffectInstance>();
            lanceSounds = new LinkedList<SoundEffectInstance>();

            ingameSounds = new LinkedList<SoundEffectInstance>();

            InitiateSound();
        }

        public void InitiateSound()
        {
            menuSound = Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Menu1").CreateInstance();

            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame1").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame2").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame3").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame4").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame5").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame6").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame7").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame8").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame9").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame10").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame11").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame12").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame13").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame14").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame15").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame16").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame17").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame18").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame19").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame20").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame21").CreateInstance());
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Ingame22").CreateInstance());

            silence = Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Silence").CreateInstance();

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
            if (menuSound.State == SoundState.Stopped && silence.State == SoundState.Stopped)
            {
                bgmPlaying = false;

                foreach (SoundEffectInstance sound in ingameSounds)
                {
                    if (sound.State == SoundState.Playing)
                    {
                        bgmPlaying = true;
                        break;
                    }
                }
            }
        }

        public void PlaySFX(LinkedList<SoundEffectInstance> source)
        {
            SoundEffectInstance sound = source.ElementAt(new Random().Next(source.Count));
            sound.Volume = sfxVolume;
            sound.Play();
        }

        public void PlayBGM(BGMType type)
        {
            switch (type)
            {
                case BGMType.Menu:
                    if (!bgmPlaying)
                    {
                        menuSound.Volume = bgmVolume;
                        menuSound.Play();
                        bgmPlaying = true;
                    }
                    break;

                case BGMType.InGame:
                    if (menuSound.State == SoundState.Playing)
                    {
                        menuSound.Stop();
                        bgmPlaying = false;
                    }
                    if (!bgmPlaying)
                    {
                        if (!silencePlayed)
                        {
                            silence.Volume = 0f;
                            silence.Play();
                            silencePlayed = true;
                            bgmPlaying = true;
                        }
                        else
                        {
                            SoundEffectInstance sound = ingameSounds.ElementAt(new Random().Next(ingameSounds.Count));
                            sound.Volume = bgmVolume;
                            sound.Play();
                            bgmPlaying = true;
                            silencePlayed = false;
                        }
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
