using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using CustomLists.Lists;

namespace PathfindingTest.Audio
{
    public class SoundManager
    {

        public float sfxVolume { get; set; }
        public float bgmVolume { get; set; }

        public Boolean bgmPlaying { get; set; }
        public Boolean menuSoundPlaying { get; set; }
        public Boolean silencePlayed { get; set; }

        public CustomArrayList<SoundEffectInstance> arrowSounds { get; set; }
        public CustomArrayList<SoundEffectInstance> swordSounds { get; set; }
        public CustomArrayList<SoundEffectInstance> lanceSounds { get; set; }

        public Song menuSound { get; set; }
        public Song silence { get; set; }
        public CustomArrayList<Song> ingameSounds { get; set; }

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
            /*
            sfxVolume = 0.45f;
            bgmVolume = 1f;*/

            bgmPlaying = false;
            silencePlayed = false;

            arrowSounds = new CustomArrayList<SoundEffectInstance>();
            swordSounds = new CustomArrayList<SoundEffectInstance>();
            lanceSounds = new CustomArrayList<SoundEffectInstance>();

            ingameSounds = new CustomArrayList<Song>();

            InitiateSound();
        }

        ~SoundManager()
        {
            this.Dispose();
        }

        public void InitiateSound()
        {
            menuSound = Game1.GetInstance().Content.Load<Song>("Sounds/Menu/Menu1");

            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame1"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame2"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame3"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame4"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame5"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame6"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame7"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame8"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame9"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame10"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame11"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame12"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame13"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame14"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame15"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame16"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame17"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame18"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame19"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame20"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame21"));
            ingameSounds.AddLast(Game1.GetInstance().Content.Load<Song>("Sounds/Ingame/Ingame22"));

            silence = Game1.GetInstance().Content.Load<Song>("Sounds/Misc/Silence");

            arrowSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Arrow/Arrow1").CreateInstance());
            arrowSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Arrow/Arrow2").CreateInstance());

            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword/Sword1").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword/Sword2").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword/Sword3").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword/Sword4").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword/Sword5").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword/Sword6").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword/Sword7").CreateInstance());
            swordSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Sword/Sword8").CreateInstance());

            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance/Lance1").CreateInstance());
            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance/Lance2").CreateInstance());
            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance/Lance3").CreateInstance());
            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance/Lance4").CreateInstance());
            lanceSounds.AddLast(Game1.GetInstance().Content.Load<SoundEffect>("Sounds/Lance/Lance5").CreateInstance());
        }

        public void Update()
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                bgmPlaying = false;
            }
            else if (MediaPlayer.State == MediaState.Playing)
            {
                bgmPlaying = true;
            }
        }

        public void PlaySFX(CustomArrayList<SoundEffectInstance> source)
        {
            SoundEffectInstance sound = source.ElementAt(new Random().Next(source.Count()));
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
                        MediaPlayer.Volume = bgmVolume;
                        MediaPlayer.IsRepeating = false;
                        MediaPlayer.Play(menuSound);
                        menuSoundPlaying = true;
                        bgmPlaying = true;
                    }
                    break;

                case BGMType.InGame:
                    if (menuSoundPlaying)
                    {
                        MediaPlayer.Stop();
                        menuSoundPlaying = false;
                        bgmPlaying = false;
                    }
                    if (!bgmPlaying)
                    {
                        if (!silencePlayed)
                        {
                            MediaPlayer.Volume = bgmVolume;
                            MediaPlayer.IsRepeating = false;
                            MediaPlayer.Play(silence);
                            silencePlayed = true;
                            bgmPlaying = true;
                        }
                        else
                        {
                            MediaPlayer.Volume = bgmVolume;
                            MediaPlayer.IsRepeating = false;
                            MediaPlayer.Play(ingameSounds.ElementAt(new Random().Next(ingameSounds.Count())));
                            bgmPlaying = true;
                            silencePlayed = false;
                        }
                    }
                    break;

                default:
                    break;
            }
        }

        public void Dispose()
        {
            try
            {
                MediaPlayer.Stop();
            }
            catch (Exception e) { }
        }
    }
}
