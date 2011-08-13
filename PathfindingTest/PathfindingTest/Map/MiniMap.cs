using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PathfindingTest.Map;

public delegate void OnLoadTick(MiniMap source);
namespace PathfindingTest.Map
{
    public class MiniMap
    {
        public GameMap map { get; set; }
        public Texture2D miniMapTexture { get; set; }
        public Vector2 miniMapTileSize = new Vector2(3f, 3f);
        public OnLoadTick onLoadTickListeners { get; set; }

        public MiniMap(GameMap map)
        {
            this.map = map;
        }

        /// <summary>
        /// Creates a minimap texture (only call this once really, on load time)
        /// </summary>
        /// <param name="map">The map to make a minimap from.</param>
        /// <returns>The texture 2D</returns>
        /*public void CreateMiniMap()
        {
            Texture2D texture = new Texture2D(Game1.GetInstance().GraphicsDevice, 200, 200);

            Texture2D[,] scaledTextures = new Texture2D[this.map.mapTiles.GetLength(0), this.map.mapTiles.GetLength(1)];

            RenderTargetBinding[] oldBindings = null;
            RenderTarget2D newRenderTarget = null;
            this.SetRenderTargets(out oldBindings, out newRenderTarget);
            for (int i = 0; i < this.map.mapTiles.GetLength(0); i++)
            {
                for (int j = 0; j < this.map.mapTiles.GetLength(1); j++)
                {
                    scaledTextures[i, j] = GetScaledInstanceNoRenderTargetChange(this.map.mapTiles[i, j], miniMapTileSize, newRenderTarget);
                    if (onLoadTickListeners != null) onLoadTickListeners(this);
                }
                Console.Out.WriteLine("Scaled a texture: " + i);
            }
            this.ResetRenderTargets(oldBindings);

            this.miniMapTexture = MergeTextures(scaledTextures);
        }*/

        /// <summary>
        /// Creates a minimap - the new way!
        /// </summary>
        /// <param name="dummy">Just a dummy</param>
        public void CreateMiniMap(Boolean dummy)
        {
            int targetWidth = (int)(this.map.mapTiles.GetLength(0) * (int)this.miniMapTileSize.X);
            int targetHeight = (int)(this.map.mapTiles.GetLength(1) * (int)this.miniMapTileSize.Y);

            RenderTargetBinding[] OldRenderTargetBindings = Game1.GetInstance().GraphicsDevice.GetRenderTargets();
            Game1.GetInstance().GraphicsDevice.SetRenderTargets(null);
            // Get the Texture with the screen drawn on it
            // Create the Render Target to draw the scaled Texture to 
            RenderTarget2D newRenderTarget =
                new RenderTarget2D(Game1.GetInstance().GraphicsDevice, targetWidth, targetHeight);

            // Set the given Graphics Device to draw to the new Render Target 
            Game1.GetInstance().GraphicsDevice.SetRenderTarget(newRenderTarget);

            // Clear the scene 
            Game1.GetInstance().GraphicsDevice.Clear(Color.Transparent);

            // Create the new SpriteBatch that will be used to scale the Texture 
            SpriteBatch sb = new SpriteBatch(Game1.GetInstance().GraphicsDevice);

            // Draw the scaled Texture 
            sb.Begin();
            for (int i = 0; i < this.map.mapTiles.GetLength(0); i++)
            {
                for (int j = 0; j < this.map.mapTiles.GetLength(1); j++)
                {
                    sb.Draw(this.map.mapTiles[i, j],
                        new Rectangle(i * (int)this.miniMapTileSize.X, j * (int)this.miniMapTileSize.Y,
                            (int)this.miniMapTileSize.X, (int)this.miniMapTileSize.Y), 
                        Color.White);
                }
            }
            // End it
            sb.End();

            // Clear Graphics Device render target
            Game1.GetInstance().GraphicsDevice.SetRenderTarget(null);
            // Restore the given Graphics Device's Render Target Bindings
            Game1.GetInstance().GraphicsDevice.SetRenderTargets(OldRenderTargetBindings);

            this.miniMapTexture = new Texture2D(Game1.GetInstance().GraphicsDevice, targetWidth, targetHeight);
            int[] data = new int[targetWidth * targetHeight];
            newRenderTarget.GetData(data);
            this.miniMapTexture.SetData(data);
            // Set the Texture To Return to the scaled Texture 
        }

        #region Scale Functions
        /// <summary>
        /// Sets the render target for scaling.
        /// </summary>
        /// <param name="oldBindings">The old bindings that you need to reapply in the ResetRenderTargets function</param>
        /// <param name="newRenderTarget">The newly created render target on the Graphics Card</param>
        private void SetRenderTargets(out RenderTargetBinding[] oldBindings, out RenderTarget2D newRenderTarget)
        {
            oldBindings = Game1.GetInstance().GraphicsDevice.GetRenderTargets();
            Game1.GetInstance().GraphicsDevice.SetRenderTargets(null);
            // Get the Texture with the screen drawn on it
            // Create the Render Target to draw the scaled Texture to 
            newRenderTarget =
                new RenderTarget2D(Game1.GetInstance().GraphicsDevice, (int)this.miniMapTileSize.X, (int)this.miniMapTileSize.Y);
            // Set the given Graphics Device to draw to the new Render Target 
            Game1.GetInstance().GraphicsDevice.SetRenderTarget(newRenderTarget);
        }

        private void ResetRenderTargets(RenderTargetBinding[] oldBindings)
        {
            // Restore the given Graphics Device's Render Target 
            Game1.GetInstance().GraphicsDevice.SetRenderTargets(oldBindings);
        }

        /// <summary>
        /// Merges textures together to a big texture.
        /// </summary>
        /// <param name="toMerge">The textures to merge, THEY MUST BE THE SAME SIZE.</param>
        /// <returns>The new texture</returns>
        public Texture2D MergeTextures(Texture2D[,] toMerge)
        {
            int targetWidth = 0;
            int targetHeight = 0;
            

            
            // Check if we can merge the new texture; if the sizes aren't equal, we cant do it, for now
            foreach (Texture2D tex in toMerge)
            {
                if (toMerge[0,0].Width != tex.Width || toMerge[0,0].Height != tex.Height)
                {
                    throw new Exception("Texture sizes aren't equal; cannot merge!");
                }
            }

            int textureWidth = toMerge[0, 0].Width;
            int textureHeight = toMerge[0, 0].Height;

            targetWidth = toMerge[0, 0].Width * toMerge.GetLength(0);
            targetHeight = toMerge[0, 0].Height * toMerge.GetLength(1);

            int[] result = new int[targetWidth * targetHeight];
            for (int i = 0; i < toMerge.GetLength(0); i++)
            {
                for (int j = 0; j < toMerge.GetLength(1); j++)
                {
                    int[] tempData = new int[toMerge[i, j].Width * toMerge[i, j].Height];
                    toMerge[i, j].GetData(tempData);

                    for (int k = 0; k < tempData.Length; k++)
                    {
                        // if tempData[k] == 0, it will bring row from -1 to 0
                        int xoffset = k % textureWidth;
                        int startPoint = (j * toMerge[i, j].Height * targetWidth) + i * toMerge[i, j].Width;
                        int yoffset = (k / textureWidth) * targetWidth;

                        result[startPoint + xoffset + yoffset] = tempData[k];
                    }
                    if (onLoadTickListeners != null) onLoadTickListeners(this);
                }
            }


            Texture2D resultTex = new Texture2D(Game1.GetInstance().graphics.GraphicsDevice, targetWidth, targetHeight);
            resultTex.SetData(result);
            return resultTex;
        }


        /// <summary>
        /// Gets a scaled instance of a texture.
        /// </summary>
        /// <param name="originalTexture"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public Texture2D GetScaledInstance(Texture2D originalTexture, float factor)
        {            
            int newWidth = (int)(originalTexture.Width * factor);
            int newHeight = (int)(originalTexture.Height * factor);
            RenderTargetBinding[] oldRenderTargets = Game1.GetInstance().GraphicsDevice.GetRenderTargets();
            Game1.GetInstance().GraphicsDevice.SetRenderTargets(null);
            // Get the Texture with the screen drawn on it
            // Create the Render Target to draw the scaled Texture to 
            RenderTarget2D newRenderTarget = 
                new RenderTarget2D(Game1.GetInstance().GraphicsDevice, newWidth, newHeight);

            // Set the given Graphics Device to draw to the new Render Target 
            Game1.GetInstance().GraphicsDevice.SetRenderTarget(newRenderTarget);

            // Clear the scene 
            Game1.GetInstance().GraphicsDevice.Clear(Color.Transparent);

            // Create the new SpriteBatch that will be used to scale the Texture 
            SpriteBatch cSpriteBatch = new SpriteBatch(Game1.GetInstance().GraphicsDevice);

            // Draw the scaled Texture 
            cSpriteBatch.Begin();
            cSpriteBatch.Draw(originalTexture, new Rectangle(0, 0, newWidth, newHeight), Color.White);
            cSpriteBatch.End();

            // Restore the given Graphics Device's Render Target 
            Game1.GetInstance().GraphicsDevice.SetRenderTargets(oldRenderTargets);

            Texture2D result = new Texture2D(Game1.GetInstance().GraphicsDevice, newWidth, newHeight);
            int[] data = new int[newWidth * newHeight];
            newRenderTarget.GetData(data);
            result.SetData(data);
            // Set the Texture To Return to the scaled Texture 
            return result;
        }

        /// <summary>
        /// Gets a scaled instance of a texture.
        /// </summary>
        /// <param name="originalTexture">The original texture</param>
        /// <param name="targetSize">The vector target size.</param>
        /// <returns>The scaled texture instance</returns>
        public Texture2D GetScaledInstance(Texture2D originalTexture, Vector2 targetSize)
        {
            RenderTargetBinding[] oldRenderTargets = Game1.GetInstance().GraphicsDevice.GetRenderTargets();
            // Get the Texture with the screen drawn on it                  
            // Create the Render Target to draw the scaled Texture to 
            RenderTarget2D newRenderTarget =
                new RenderTarget2D(Game1.GetInstance().GraphicsDevice, (int)targetSize.X, (int)targetSize.Y);

            // Set the given Graphics Device to draw to the new Render Target 
            Game1.GetInstance().GraphicsDevice.SetRenderTarget(newRenderTarget);

            // Clear the scene 
            Game1.GetInstance().GraphicsDevice.Clear(Color.Transparent);

            // Create the new SpriteBatch that will be used to scale the Texture 
            SpriteBatch cSpriteBatch = new SpriteBatch(Game1.GetInstance().GraphicsDevice);

            // Draw the scaled Texture 
            cSpriteBatch.Begin();
            cSpriteBatch.Draw(originalTexture, new Rectangle(0, 0, (int)targetSize.X, (int)targetSize.Y), Color.White);
            cSpriteBatch.End();

            // Restore the given Graphics Device's Render Target 
            Game1.GetInstance().GraphicsDevice.SetRenderTargets(oldRenderTargets);

            Texture2D result = new Texture2D(Game1.GetInstance().GraphicsDevice, (int)targetSize.X, (int)targetSize.Y);
            int[] data = new int[(int)targetSize.X * (int)targetSize.Y];
            newRenderTarget.GetData(data);
            result.SetData(data);
            // Set the Texture To Return to the scaled Texture 
            return result;
        }

        /// <summary>
        /// Gets a scaled instance of a texture.
        /// </summary>
        /// <param name="originalTexture">The original texture</param>
        /// <param name="targetSize">The vector target size.</param>
        /// <returns>The scaled texture instance</returns>
        private Texture2D GetScaledInstanceNoRenderTargetChange(Texture2D originalTexture, Vector2 targetSize, RenderTarget2D newRenderTarget)
        {
            // Clear the scene 
            Game1.GetInstance().GraphicsDevice.Clear(Color.Transparent);

            // Create the new SpriteBatch that will be used to scale the Texture 
            SpriteBatch cSpriteBatch = new SpriteBatch(Game1.GetInstance().GraphicsDevice);

            // Draw the scaled Texture 
            cSpriteBatch.Begin();
            cSpriteBatch.Draw(originalTexture, new Rectangle(0, 0, (int)targetSize.X, (int)targetSize.Y), Color.White);
            cSpriteBatch.End();

            Texture2D result = new Texture2D(Game1.GetInstance().GraphicsDevice, (int)targetSize.X, (int)targetSize.Y);
            int[] data = new int[(int)targetSize.X * (int)targetSize.Y];
            Game1.GetInstance().GraphicsDevice.SetRenderTarget(null);
            newRenderTarget.GetData(data);
            Game1.GetInstance().GraphicsDevice.SetRenderTarget(newRenderTarget);
            result.SetData(data);
            // Set the Texture To Return to the scaled Texture 
            return result;
        }
        #endregion

        /// <summary>
        /// Draws the minimap on the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw on.</param>
        /// <param name="targetRectangle">The target rectangle the minimap will appear on.</param>
        public void Draw(SpriteBatch sb, Rectangle targetRectangle)
        {
            sb.Draw(this.miniMapTexture, targetRectangle, Color.White);
        }
    }
}
