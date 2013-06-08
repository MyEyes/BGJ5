using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Bacon_Game_Jam_5
{
    public class Tile
    {
        public Rectangle SelectRect;
        public Rectangle TargetRect;

        public Tile(int tileID, int x, int y)
        {
            SelectRect = new Rectangle(tileID * Map.TileSize, 0, Map.TileSize, Map.TileSize);
            TargetRect = new Rectangle(x * Map.TileSize, y * Map.TileSize, Map.TileSize, Map.TileSize);
        }
    }

    class Map
    {
        Texture2D _tileSet;
        Tile[][,] _tiles;
        public const int SizeX = 300;
        public const int SizeY = 300;

        public Lightmap lightMap;

        public const int TileSize = 32;

        public List<GameObject> Objects;

        public Map(ContentManager Content)
        {
            _tileSet = Content.Load<Texture2D>("TileSet");
            _tiles = new Tile[2][,];
            _tiles[0] = new Tile[SizeX, SizeY];
            _tiles[1] = new Tile[SizeX, SizeY];
            Random rand = new Random();

            for (int x = 0; x < SizeX; x++)
                for (int y = 0; y < SizeY; y++)
                {
                    _tiles[0][x,y] = new Tile(0, x, y);
                    if (rand.NextDouble()>0.95)
                        _tiles[1][x, y] = new Tile(1, x, y);
                }

            Objects = new List<GameObject>();
        }

        public void Draw(Camera cam, SpriteBatch batch)
        {
            int startX = cam.ViewSpace.X / TileSize;
            if (startX < 0) startX = 0;
            if (startX >= SizeX) startX = SizeX - 1;

            int startY = cam.ViewSpace.Y / TileSize;
            if (startY < 0) startY = 0;
            if (startY >= SizeY) startY = SizeY - 1;

            int endX = cam.ViewSpace.Width / TileSize + startX;
            if (endX > SizeX) endX = SizeX - 1;

            int endY = cam.ViewSpace.Height / TileSize + startY;
            if (endY > SizeY) endY = SizeY - 1;

            batch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, null, DepthStencilState.None, null, null, cam.ViewMatrix);

            for (int layer = 0; layer < 2; layer++)
                for (int x = startX; x <= endX; x++)
                    for (int y = startY; y <= endY; y++)
                        if (_tiles[layer][x, y] != null)
                            batch.Draw(_tileSet, _tiles[layer][x, y].TargetRect, _tiles[layer][x, y].SelectRect, Color.White);
            for (int x = 0; x < Objects.Count; x++)
                Objects[x].Draw(batch);
            batch.End();
        }

        public void Update(float seconds)
        {
            for (int x = 0; x < Objects.Count; x++)
                Objects[x].Update(seconds);
        }

        public bool Collides(GameObject obj)
        {
            if (obj.BoundingRect.X < 0 || obj.BoundingRect.Y < 0 || obj.BoundingRect.Right > TileSize * SizeX || obj.BoundingRect.Bottom > TileSize * SizeY)
                return true;
            int startX = obj.BoundingRect.X / TileSize-1;
            if (startX < 0) startX = 0;
            if (startX >= SizeX) startX = SizeX - 1;

            int startY = obj.BoundingRect.Y / TileSize-1;
            if (startY < 0) startY = 0;
            if (startY >= SizeY) startY = SizeY - 1;

            int endX = obj.BoundingRect.Width / TileSize + startX+1;
            if (endX > SizeX) endX = SizeX - 1;

            int endY = obj.BoundingRect.Height / TileSize + startY+1;
            if (endY > SizeY) endY = SizeY - 1;
            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                    if (_tiles[1][x, y] != null && obj.BoundingRect.Intersects(_tiles[1][x, y].TargetRect))
                        return true;
            return false;
        }
    }
}
