﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PathfindingTest.Map;
using XNAInputHandler.MouseInput;
using PathfindingTest.Pathfinding;
using PathfindingTest.Players;
using PathfindingTest.Units;
using PathfindingTest.Buildings;

public delegate void OnLoadTick(MiniMap source);
namespace PathfindingTest.Map
{
    public class MiniMap
    {
        public GameMap map { get; set; }
        public Texture2D miniMapTexture { get; set; }
        public Vector2 miniMapTileSize = new Vector2(3f, 3f);
        public OnLoadTick onLoadTickListeners { get; set; }
        public float z { get; set; }
        public Game1 game;


        public RenderTarget2D miniLightTarget;
        public RenderTarget2D miniMainTarget;

        public Rectangle currentDrawRectangle { get; set; }

        public Color rectangleColor { get; set; }

        public MiniMap(GameMap map)
        {
            this.map = map;
            MouseManager.GetInstance().mouseClickedListeners += this.OnMouseClicked;
            MouseManager.GetInstance().mouseDragListeners += this.OnMouseDrag;

            // Just above the HUD
            this.z = 0.09999f;
            rectangleColor = Color.Red;
            game = Game1.GetInstance();
        }

        public void OnMouseDrag(MouseEvent e)
        {
            if (this.currentDrawRectangle.Contains(e.location) && Game1.CURRENT_PLAYER.selectBox == null)
            {
                Point miniMapLocation = new Point(e.location.X - this.currentDrawRectangle.X, e.location.Y - this.currentDrawRectangle.Y);
                ActionOnMiniMap(miniMapLocation);
            }
        }

        public void OnMouseClicked(MouseEvent e)
        {
            if (this.currentDrawRectangle.Contains(e.location))
            {
                Point miniMapLocation = new Point(e.location.X - this.currentDrawRectangle.X, e.location.Y - this.currentDrawRectangle.Y);
                ActionOnMiniMap(miniMapLocation);
            }
        }

        /// <summary>
        /// Perform an action on the given point
        /// </summary>
        /// <param name="miniMapLocation">The point to perform the action on</param>
        private void ActionOnMiniMap(Point miniMapLocation)
        {
            Point mapLocation = MiniMapToMap(miniMapLocation);
            Point centerSize =
                new Point(Game1.GetInstance().GetScreenBounds().Width / 2, Game1.GetInstance().GetScreenBounds().Height / 2);
            Game1.GetInstance().drawOffset = new Vector2(mapLocation.X - centerSize.X, mapLocation.Y - centerSize.Y);
        }

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

        #region Slow minimap creation ( +/- 1 minute )
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
        #endregion

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
                if (toMerge[0, 0].Width != tex.Width || toMerge[0, 0].Height != tex.Height)
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
        /// Converts map coordinates to mini map coordinates
        /// </summary>
        /// <param name="mapCoordinates">The map coordinates you want to convert</param>
        /// <returns>The mini map coordinates</returns>
        public Point MapToMiniMap(Point mapCoordinates)
        {
            float unscaledFactorX = (this.miniMapTexture.Width / (float)this.map.collisionMap.mapWidth);
            float unscaledFactorY = (this.miniMapTexture.Height / (float)this.map.collisionMap.mapHeight);

            float scaledFactorX = (this.currentDrawRectangle.Width / (float)this.miniMapTexture.Width);
            float scaledFactorY = (this.currentDrawRectangle.Height / (float)this.miniMapTexture.Height);

            return new Point((int)(mapCoordinates.X * unscaledFactorX * scaledFactorX),
                (int)(mapCoordinates.Y * unscaledFactorY * scaledFactorY));
        }

        /// <summary>
        /// Converts mini map to map coordinates
        /// </summary>
        /// <param name="miniMapCoordinates">The mini map coordinates you want to convert</param>
        /// <returns>The point in map coordinates</returns>
        public Point MiniMapToMap(Point miniMapCoordinates)
        {
            float scaledFactorX = (this.miniMapTexture.Width / (float)this.currentDrawRectangle.Width);
            float scaledFactorY = (this.miniMapTexture.Height / (float)this.currentDrawRectangle.Height);

            float unscaledFactorX = (this.map.collisionMap.mapWidth / (float)this.miniMapTexture.Width);
            float unscaledFactorY = (this.map.collisionMap.mapHeight / (float)this.miniMapTexture.Height);


            return new Point((int)(miniMapCoordinates.X * unscaledFactorX * scaledFactorX),
                (int)(miniMapCoordinates.Y * unscaledFactorY * scaledFactorY));
        }

        /// <summary>
        /// Draws the screen rectangle
        /// </summary>
        /// <param name="sb">SpriteBatch to draw on.</param>
        private void DrawScreenRectangle(SpriteBatch sb)
        {
            Point screenTopLeftOnMinimap = this.MapToMiniMap(Game1.GetInstance().GetScreenLocation());
            Point screenSize = this.MapToMiniMap(
                new Point(Game1.GetInstance().GetScreenBounds().Width, Game1.GetInstance().GetScreenBounds().Height));
            // The percentage of the screen 
            Rectangle screenRect = new Rectangle(this.currentDrawRectangle.X + screenTopLeftOnMinimap.X,
                this.currentDrawRectangle.Y + screenTopLeftOnMinimap.Y,
                screenSize.X,
                screenSize.Y
                /*(int)((this.map.collisionMap.windowSize.Width / (float)this.map.collisionMap.mapWidth) * this.miniMapTexture.Width),
                (int)((this.map.collisionMap.windowSize.Height / (float)this.map.collisionMap.mapHeight) * this.miniMapTexture.Height)*/);
            DrawUtil.DrawClearRectangle(sb, screenRect, 1, this.rectangleColor, this.z - 0.00002f);
        }

        /// <summary>
        /// Draws the minimap on the screen
        /// </summary>
        /// <param name="sb">SpriteBatch to draw on.</param>
        /// <param name="targetRectangle">The target rectangle the minimap will appear on.</param>
        public void Draw(SpriteBatch sb, Rectangle targetRectangle)
        {

            if (game.isFoggy)
            {
                if (miniMainTarget == null)
                {
                    miniMainTarget = game.CreateRenderTarget(targetRectangle.Width, targetRectangle.Height);
                }
                if (miniLightTarget == null)
                {
                    miniLightTarget = game.CreateRenderTarget(targetRectangle.Width, targetRectangle.Height);
                }

                game.GraphicsDevice.SetRenderTarget(miniMainTarget);
                game.GraphicsDevice.Clear(Color.Black);
            }

            currentDrawRectangle = targetRectangle;

            //sb.Draw(this.miniMapTexture, targetRectangle, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, this.z);
            sb.Draw(this.miniMapTexture, new Rectangle(0,0,targetRectangle.Width,targetRectangle.Height), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, this.z);

            for (int i = 0; i < game.players.Count(); i++)
            {
                Player p = game.players.ElementAt(i);
                for (int j = 0; j < p.units.Count(); j++)
                {
                    Unit unit = p.units.ElementAt(j);
                    Point miniMapPoint = this.MapToMiniMap(unit.GetLocation());
                    sb.Draw(DrawUtil.lineTexture, new Rectangle(miniMapPoint.X - 1,
                        miniMapPoint.Y - 1,
                        2, 2), null, p.color, 0f, Vector2.Zero, SpriteEffects.None, this.z - 0.00001f);
                }

                for (int j = 0; j < p.buildings.Count(); j++)
                {
                    Building building = p.buildings.ElementAt(j);
                    if (building.state == Building.State.Preview) continue;
                    Point miniMapPoint = this.MapToMiniMap(building.GetLocation());
                    sb.Draw(DrawUtil.lineTexture, new Rectangle(miniMapPoint.X - 2,
                        miniMapPoint.Y - 2,
                        4, 4), null, p.color, 0f, Vector2.Zero, SpriteEffects.None, this.z - 0.00001f);
                }
            }

            sb.End();

            if (game.isFoggy)
            {
                try
                {
                    game.GraphicsDevice.SetRenderTarget(miniLightTarget);
                    game.GraphicsDevice.Clear(Color.Black);

                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                    Game1.CURRENT_PLAYER.DrawMiniLights(sb, this);

                    sb.End();

                    game.GraphicsDevice.SetRenderTarget(null);

                    Texture2D miniMainTex = miniMainTarget;
                    Texture2D miniLightTex = miniLightTarget;

                    game.basicMiniFogOfWarEffect.Parameters["LightsTexture"].SetValue(miniLightTex);

                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                    foreach (EffectPass effect in game.basicMiniFogOfWarEffect.CurrentTechnique.Passes)
                    {
                        effect.Apply();
                    }

                    sb.Draw(
                        miniMainTex,
                        targetRectangle,
                        Color.White
                    );

                    sb.End();

                }
                catch (Exception e) {
                    Console.WriteLine(e.Message);
                }
            }

            game.GraphicsDevice.SetRenderTarget(null);

            sb.Begin(SpriteSortMode.FrontToBack, null);


            this.DrawScreenRectangle(sb);

        }
    }
}
