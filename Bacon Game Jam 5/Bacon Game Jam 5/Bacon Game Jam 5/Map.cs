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

    public class Map
    {
        Texture2D _tileSet;
        Tile[][,] _tiles;
        public const int SizeX = 150;
        public const int SizeY = 150;

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
                    if (rand.NextDouble() > 0.95 && x != 1 && x != 2 && y != 1 && y != 2)
                        _tiles[1][x, y] = new Tile(1, x, y);
                }

            Objects = new List<GameObject>();
        }

        public void Draw(Camera cam, SpriteBatch batch, BlendState blend)
        {
            int startX = cam.ViewSpace.X / TileSize;
            if (startX < 0) startX = 0;
            if (startX >= SizeX) startX = SizeX - 1;

            int startY = cam.ViewSpace.Y / TileSize;
            if (startY < 0) startY = 0;
            if (startY >= SizeY) startY = SizeY - 1;

            int endX = cam.ViewSpace.Width / TileSize + startX + 2;
            if (endX >= SizeX) endX = SizeX - 1;

            int endY = cam.ViewSpace.Height / TileSize + startY + 2;
            if (endY >= SizeY) endY = SizeY - 1;

            batch.Begin(SpriteSortMode.Immediate, blend, null, DepthStencilState.Default, null, null, cam.ViewMatrix);

            for (int layer = 0; layer < 2; layer++)
                for (int x = startX; x <= endX; x++)
                    for (int y = startY; y <= endY; y++)
                        if (_tiles[layer][x, y] != null)
                            batch.Draw(_tileSet, _tiles[layer][x, y].TargetRect, _tiles[layer][x, y].SelectRect, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.55f - layer * 0.1f);
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

            int endX = obj.BoundingRect.Width / TileSize + startX+2;
            if (endX >= SizeX) endX = SizeX - 1;

            int endY = obj.BoundingRect.Height / TileSize + startY+2;
            if (endY >= SizeY) endY = SizeY - 1;
            for (int x = startX; x <= endX; x++)
                for (int y = startY; y <= endY; y++)
                    if (_tiles[1][x, y] != null && obj.BoundingRect.Intersects(_tiles[1][x, y].TargetRect))
                        return true;
            return false;
        }

        public int CreateShadowGeometry(Vector2 pos, Rectangle viewSpace, VertexBuffer shadowVB)
        {
            Vector2 center = pos;
            Rectangle drawRect = viewSpace;
            //Calculate minimal and maximal tile index to consider for shadow calculation
            int minX = (int)((pos.X - drawRect.Width / 2) / TileSize);
            minX = minX >= 0 ? minX : 0;
            int minY = (int)((pos.Y-drawRect.Height/2) / TileSize);
            minY = minY >= 0 ? minY : 0;
            int maxX = (drawRect.Width + drawRect.X) / TileSize + 1;
            maxX = maxX < _tiles[1].GetLength(0) ? maxX : _tiles[1].GetLength(0);
            int maxY = (drawRect.Height + drawRect.Y) / TileSize + 1;
            maxY = maxY < _tiles[1].GetLength(1) ? maxY : _tiles[1].GetLength(1);
            //Count vertices to make sure we fit everything into our vertex buffer
            int vertexCount = 0;
            //Iterate over all tiles we need to consider, this method is slower than using a BSP tree or something
            //But it is sufficient for this game
            //48,24
            for (int x = minX; x < maxX; x++)
                for (int y = minY; y < maxY; y++)
                    //Check if there's a tile here and wether we have drawn this rect already
                    if (_tiles[1][x, y] != null)
                    {
                        //Set up helper values
                        //Offset for the diagonal of the rectangle
                        Vector2 tileSizeVec = Vector2.Zero;// new Vector2(_tiles[1][x, y].Geometry.BoundingRect.Width * 0.5f, _tiles[1][x, y].Geometry.BoundingRect.Height * 0.5f);
                        //mid point of the rectangle, this way we can directly compute the corners over the diagonals
                        Vector2 midPoint = Vector2.Zero;// new Vector2(_tiles[1][x, y].Geometry.BoundingRect.Center.X, _tiles[1][x, y].Geometry.BoundingRect.Center.Y);
                        //vector from light source to rectangles center to calculate which edges are facing away from light source

                            tileSizeVec = new Vector2(_tiles[1][x, y].TargetRect.Width * 0.5f, _tiles[1][x, y].TargetRect.Height * 0.5f);
                            midPoint = new Vector2(_tiles[1][x, y].TargetRect.Center.X, _tiles[1][x, y].TargetRect.Center.Y);
                        
                        Vector3 diff = new Vector3(midPoint - center, 0);
                        Vector3 edge1, edge2, edge3, edge4;
                        //Find a sensible order for the 4 edges, sort them so that if you only take the first 3 edges the left out side is closest to the center of the rectangle
                        //This simplifies the shadow geometry calculation a lot
                        //Note: This seems like a lot of code, but it's just 2 branches and 4 vector 3 creators

                        /*        2*                         3*
                         * 
                         *                                              
                         * 
                         * 
                         *        1*                         4*
                         *                      x
                         *                      
                         * diff.X/tileSizeVec.X > diff.Y/tileSizeVec.Y
                         */

                        float depth = 0.5f;
                        if (diff.X * tileSizeVec.Y > diff.Y * tileSizeVec.X)
                        {
                            if (diff.X * tileSizeVec.Y > -diff.Y * tileSizeVec.X)
                            {

                                edge1 = new Vector3(midPoint + new Vector2(-tileSizeVec.X, -tileSizeVec.Y), depth);//1
                                edge2 = new Vector3(midPoint + new Vector2(tileSizeVec.X, -tileSizeVec.Y), depth);//2
                                edge3 = new Vector3(midPoint + new Vector2(tileSizeVec.X, tileSizeVec.Y), depth);//3
                                edge4 = new Vector3(midPoint + new Vector2(-tileSizeVec.X, tileSizeVec.Y), depth);//4
                            }
                            else
                            {
                                edge1 = new Vector3(midPoint + new Vector2(-tileSizeVec.X, tileSizeVec.Y), depth);//4
                                edge2 = new Vector3(midPoint + new Vector2(-tileSizeVec.X, -tileSizeVec.Y), depth);//1
                                edge3 = new Vector3(midPoint + new Vector2(tileSizeVec.X, -tileSizeVec.Y), depth);//2
                                edge4 = new Vector3(midPoint + new Vector2(tileSizeVec.X, tileSizeVec.Y), depth);//3
                            }
                        }
                        else
                        {
                            if (diff.X * tileSizeVec.Y > -diff.Y * tileSizeVec.X)
                            {
                                edge1 = new Vector3(midPoint + new Vector2(tileSizeVec.X, -tileSizeVec.Y), depth);//2
                                edge2 = new Vector3(midPoint + new Vector2(tileSizeVec.X, tileSizeVec.Y), depth);//3
                                edge3 = new Vector3(midPoint + new Vector2(-tileSizeVec.X, tileSizeVec.Y), depth);//4
                                edge4 = new Vector3(midPoint + new Vector2(-tileSizeVec.X, -tileSizeVec.Y), depth);//1
                            }
                            else
                            {
                                edge1 = new Vector3(midPoint + new Vector2(tileSizeVec.X, tileSizeVec.Y), depth);//3
                                edge2 = new Vector3(midPoint + new Vector2(-tileSizeVec.X, tileSizeVec.Y), depth);//4
                                edge3 = new Vector3(midPoint + new Vector2(-tileSizeVec.X, -tileSizeVec.Y), depth);//1
                                edge4 = new Vector3(midPoint + new Vector2(tileSizeVec.X, -tileSizeVec.Y), depth);//2
                            }
                        }
                        Vector3 dir1;
                        Vector3 dir2;
                        //Calculate some (not accurate) stretch value to make sure the shadow geometry does not end mid screen
                        //We definitely draw way too much, but everything outside of the screen barely costs fillrate
                        float stretch = tileSizeVec.X * tileSizeVec.Y * 200;

                        //Check which of the 3 interesting sides are facing away from the light center and write appropriate
                        //Geometry into the dynamic vertex buffer
                        if (vertexCount < shadowVB.VertexCount)// && Vector3.Dot((edge1 + edge2) / 2 - new Vector3(center, 0), new Vector3(-(edge2 - edge1).Y, (edge2 - edge1).X, 0)) < 0)
                        {
                            dir1 = edge1 - new Vector3(center, depth);
                            dir1.Normalize();
                            dir2 = edge2 - new Vector3(center, depth);
                            dir2.Normalize();
                            //Some basic math basically we put in 2 triangles
                            //The idea is this, you draw a straight line from the center of the light to the corners and elongate that line
                            //until it is out of the screen, this way with the 2 corner points you get a trapezoid
                            //this trapezoid is your geometry, so you split it into 2 triangles and send them into the VBO
                            shadowVB.SetData<VertexPositionColor>(vertexCount * VertexPositionColor.VertexDeclaration.VertexStride, new VertexPositionColor[]{
                            new VertexPositionColor(edge1+dir1*stretch,Color.White),
                            new VertexPositionColor(edge1+Vector3.Transform(dir1,Lightmap.rotateRight)*stretch, Color.Black),
                            new VertexPositionColor(edge2+Vector3.Transform(dir2,Lightmap.rotateLeft)*stretch, Color.Black),
                            new VertexPositionColor(edge2+dir2*stretch,Color.White),
                            new VertexPositionColor(edge1,Color.Black),
                            new VertexPositionColor(edge2,Color.Black)},
                            0, 6, VertexPositionColor.VertexDeclaration.VertexStride);
                            vertexCount += 6;
                        }

                        //This is the backwards facing side, which will always be facing away, so we only need to check that the geometry fits
                        if (vertexCount < shadowVB.VertexCount)// && Vector3.Dot((edge2+edge3)/2 - new Vector3(center, 0), new Vector3(-(edge3 - edge2).Y, (edge3 - edge2).X, 0)) < 0)
                        {
                            dir1 = edge2 - new Vector3(center, depth);
                            dir1.Normalize();
                            dir2 = edge3 - new Vector3(center, depth);
                            dir2.Normalize();
                            shadowVB.SetData<VertexPositionColor>(vertexCount * VertexPositionColor.VertexDeclaration.VertexStride, new VertexPositionColor[]{
                            new VertexPositionColor(edge2+dir1*stretch,Color.White),
                            new VertexPositionColor(edge2+Vector3.Transform(dir1,Lightmap.rotateRight)*stretch, Color.Black),
                            new VertexPositionColor(edge3+Vector3.Transform(dir2,Lightmap.rotateLeft)*stretch, Color.Black),
                            new VertexPositionColor(edge3+dir2*stretch,Color.White),
                            new VertexPositionColor(edge2,Color.Black),
                            new VertexPositionColor(edge3,Color.Black)},
                            0, 6, VertexPositionColor.VertexDeclaration.VertexStride);
                            vertexCount += 6;
                        }
                        else
                        {

                        }

                        if (vertexCount < shadowVB.VertexCount)// && Vector3.Dot((edge3 + edge4) / 2 - new Vector3(center, 0), new Vector3(-(edge4 - edge3).Y, (edge4 - edge3).X, 0)) < 0)
                        {
                            dir1 = edge3 - new Vector3(center, depth);
                            dir1.Normalize();
                            dir2 = edge4 - new Vector3(center, depth);
                            dir2.Normalize();
                            shadowVB.SetData<VertexPositionColor>(vertexCount * VertexPositionColor.VertexDeclaration.VertexStride, new VertexPositionColor[]{
                            new VertexPositionColor(edge3+dir1*stretch,Color.White),
                            new VertexPositionColor(edge3+Vector3.Transform(dir1,Lightmap.rotateRight)*stretch, Color.Black),
                            new VertexPositionColor(edge4+Vector3.Transform(dir2,Lightmap.rotateLeft)*stretch, Color.Black),
                            new VertexPositionColor(edge4+dir2*stretch,Color.White),
                            new VertexPositionColor(edge3,Color.Black),
                            new VertexPositionColor(edge4,Color.Black),
                            },
                            0, 6, VertexPositionColor.VertexDeclaration.VertexStride);
                            vertexCount += 6;
                        }
                        if (vertexCount < shadowVB.VertexCount)// && Vector3.Dot((edge4 + edge1) / 2 - new Vector3(center, 0), new Vector3(-(edge1 - edge4).Y, (edge1 - edge4).X, 0)) < 0)
                        {
                            dir1 = edge4 - new Vector3(center, depth);
                            dir1.Normalize();
                            dir2 = edge1 - new Vector3(center, depth);
                            dir2.Normalize();
                            shadowVB.SetData<VertexPositionColor>(vertexCount * VertexPositionColor.VertexDeclaration.VertexStride, new VertexPositionColor[]{
                            new VertexPositionColor(edge4+dir1*stretch,Color.White),
                            new VertexPositionColor(edge4+Vector3.Transform(dir1,Lightmap.rotateRight)*stretch, Color.Black),
                            new VertexPositionColor(edge1+Vector3.Transform(dir2,Lightmap.rotateLeft)*stretch, Color.Black),
                            new VertexPositionColor(edge1+dir2*stretch,Color.White),
                            new VertexPositionColor(edge4,Color.Black),
                            new VertexPositionColor(edge1,Color.Black),
                            },
                            0, 6, VertexPositionColor.VertexDeclaration.VertexStride);
                            vertexCount += 6;
                        }
                    }
            return vertexCount;
        }
    }
}
