using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units.Projectiles;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Audio;
using Microsoft.Xna.Framework.Audio;

namespace PathfindingTest.Units.Damage
{
    public class ArrowManager
    {
        public LinkedList<Projectile> projectiles { get; set; }

        public ArrowManager()
        {
            Console.WriteLine("new ArrowManager made");
            projectiles = new LinkedList<Projectile>();
        }

        public void UpdateProjectiles(KeyboardState ks, MouseState ms)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles.ElementAt(i).Update(ks, ms);
            }
        }

        public void DrawProjectiles(SpriteBatch sb)
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles.ElementAt(i).Draw(sb);
            }
        }

        public void AddProjectile(Projectile projectile)
        {
            this.projectiles.AddLast(projectile);
            SoundManager.GetInstance().PlaySound(SoundManager.GetInstance().arrowSounds);
        }

        public void RemoveProjectile(Projectile projectile)
        {
            this.projectiles.Remove(projectile);
        }
    }
}
