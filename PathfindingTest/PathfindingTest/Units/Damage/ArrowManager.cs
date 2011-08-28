using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathfindingTest.Units.Projectiles;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using PathfindingTest.Audio;
using Microsoft.Xna.Framework.Audio;
using CustomLists.Lists;

namespace PathfindingTest.Units.Damage
{
    public class ArrowManager
    {
        public CustomArrayList<Projectile> projectiles { get; set; }

        public ArrowManager()
        {
            projectiles = new CustomArrayList<Projectile>();
        }

        public void UpdateProjectiles(KeyboardState ks, MouseState ms)
        {
            for (int i = 0; i < projectiles.Count(); i++)
            {
                projectiles.ElementAt(i).Update(ks, ms);
            }
        }

        public void DrawProjectiles(SpriteBatch sb)
        {
            for (int i = 0; i < projectiles.Count(); i++)
            {
                projectiles.ElementAt(i).Draw(sb);
            }
        }

        public void AddProjectile(Projectile projectile)
        {
            this.projectiles.AddLast(projectile);
            SoundManager.GetInstance().PlaySFX(SoundManager.GetInstance().arrowSounds);
        }

        public void RemoveProjectile(Projectile projectile)
        {
            this.projectiles.Remove(projectile);
        }
    }
}
