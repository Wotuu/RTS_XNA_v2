using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using AStarCollisionMap.Collision;
using Microsoft.Xna.Framework;

namespace AStarCollisionMap.QuadTree
{
    public class CollisionTexture
    {
        public Quad quad { get; set; }
        private Texture2D _texture { get; set; }
        public Texture2D texture { 
            get{
                return _texture;
            }
            set
            {
                this._texture = value;
                this.textureData = this.TextureToBoolean();
            }
        }

        private Boolean[] textureData { get; set; }

        /// <summary>
        /// Returns the index of a certain point.
        /// </summary>
        /// <param name="p">The point.</param>
        /// <returns>The index.</returns>
        public int PointToIndex(int x, int y)
        {
            return x + y * this.texture.Width;
        }



        /// <summary>
        /// Updates the texture of this collisiontexture.
        /// </summary>
        private void UpdateTexture()
        {
            int[] intData = new int[textureData.Length];
            Texture2D texture = new Texture2D(this.texture.GraphicsDevice, this.texture.Width, this.texture.Height);
            
            for( int i = 0; i < intData.Length; i++ )
            {
                if (textureData[i]) intData[i] = (int)Color.Black.PackedValue;
                else intData[i] = 0;
            }
            texture.SetData(intData);
            this.texture = texture;
        }

        /// <summary>
        ///  Updates the collision of this texture, and update the texture as well.
        /// </summary>
        /// <param name="rect">The rectangle that is to be updated.</param>
        /// <param name="add">Whether to add or remove the rectangle.</param>
        public void UpdateCollision(Rectangle rect, Boolean add)
        {
            // Update collisionmap data
            for (int i = rect.Left; i < rect.Right; i++)
            {
                for (int j = rect.Top; j < rect.Bottom; j++)
                {
                    this.textureData[PointToIndex(i, j)] = add;
                }
            }

            UpdateTexture();
        }


        /// <summary>
        /// Sets collision data at this point.
        /// </summary>
        /// <param name="i">The point to put it at.</param>
        /// <param name="value">The value to place it at.</param>
        public void SetCollisionAt(int i, Boolean value)
        {
            this.textureData[i] = value;
        }

        /// <summary>
        /// Checks whether there is collision at a certain point.
        /// </summary>
        /// <param name="i">The index of the point to check at. Note that the index has to fall within
        /// the bounds of this texture, i.e., it's only as large as this.rectangle.Width * this.rectangle.Height.</param>
        /// <returns></returns>
        public Boolean CollisionAt(int i)
        {
            return this.textureData[i];
        }

        /// <summary>
        /// Converts a texture to a boolean array
        /// </summary>
        /// <returns>The boolean array of the texture of this quad</returns>
        public Boolean[] TextureToBoolean()
        {
            Boolean[] data = new Boolean[this.texture.Width * this.texture.Height];
            int[] intData = new int[this.texture.Width * this.texture.Height];
            this.texture.GetData(intData);
            for (int i = 0; i < intData.Length; i++)
            {
                //if (i == 0) Console.Out.WriteLine(intData[i] + "");
                // 0 = no collision, 1 = collision
                data[i] = (intData[i] != 0);
                //if (i == 0) Console.Out.WriteLine(data[i] + "");
            }
            return data;
        }

        public CollisionTexture(Quad quad, Texture2D texture)
        {
            this.quad = quad;
            this.texture = texture;
        }
    }
}
