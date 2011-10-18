using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace PixelArt
{
    class Grid
    {
        public const int UNVISITED = 0;
        public const int VISITED = 1;
        public const int CURRENT = 2;
        public const int FLOOR = 3;
        public const int WALL = 4;

        public static int GridWidth = 1920/30;
        public static int GridHeight = 1080/30;
        public static Random Random = new Random();
        public int[,] GridData = new int[GridWidth, GridHeight];

        private Stack<Point> theStack = new Stack<Point>();
        private Dictionary<int, Point> directionMap = new Dictionary<int, Point>();

        Point current = new Point(0, 0);

        public Grid()
        {
            for (int x = 0; x < GridData.GetLength(0); x++)
            {
                for (int y = 0; y < GridData.GetLength(1); y++)
                {
                    GridData[x, y] = 0;
                }
            }
            //Point center = new Point(GridWidth / 2, GridHeight / 2);
            //GridData[center.X, center.Y] = 2;


            directionMap.Add(0, Directions.UP);
            directionMap.Add(1, Directions.RIGHT);
            directionMap.Add(2, Directions.LEFT);
            directionMap.Add(3, Directions.DOWN);

            //directionMap.Add(4, Directions.UPRIGHT);
            //directionMap.Add(5, Directions.DOWNRIGHT);
            //directionMap.Add(6, Directions.DOWNLEFT);
            //directionMap.Add(7, Directions.UPLEFT);


            SetValueAtPoint(current, 1);
        }

        

        public Point GetRandomPoint()
        {
            return new Point(Random.Next(GridWidth), Random.Next(GridHeight));
        }

        public Point GetRandomDirection()
        {
            return directionMap[Random.Next(4)];
        }

        public bool DoUnvisitedPointsExist()
        {
            for (int x = 0; x < GridData.GetLength(0); x++)
            {
                for (int y = 0; y < GridData.GetLength(1); y++)
                {
                    if(ValueAtPoint(new Point(x,y)) == UNVISITED)
                        return true;
                }
            }
            return false;
        }

        //returns the directions
        public List<Point> GetUnvisitedNeighbors(Point point)
        {
            List<Point> uneighbors = new List<Point>();

            for (int i = 0; i < directionMap.Count; i++)
            {
                if (ValueAtPoint(point + directionMap[i]*2) == UNVISITED) //point is unvisited
                {
                    uneighbors.Add(directionMap[i]);
                }
            }

            return uneighbors;
        }

        public int ValueAtPoint(Point p)
        {
            if (p.X >= 0 && p.X < GridWidth && p.Y >= 0 && p.Y < GridHeight)
            {
                return GridData[p.X, p.Y];
            }
            else
                return -1;
        }

        public void SetValueAtPoint(Point p, int value)
        {
            if (p.X >= 0 && p.X < GridWidth && p.Y >= 0 && p.Y < GridHeight)
            {
                GridData[p.X, p.Y] = value;
            }
        }

        //call by itself in update method
        public void Maze()
        {
            for (int i = 0; i < 50; i++)
            {
                if (DoUnvisitedPointsExist())
                {
                    List<Point> uneighbors = GetUnvisitedNeighbors(current);
                    if (uneighbors.Count > 0) //iff the current cell has any neighbors which hav enot been visited
                    {
                        Point chosen = uneighbors[Random.Next(uneighbors.Count)]; //choose a random unvisited neighbor

                        theStack.Push(current + chosen * 2); //push the chosen cell to the stack

                        SetValueAtPoint(current + chosen, FLOOR);//remove the wall between the current cell and the chosen cell


                        current = current + chosen * 2;
                        SetValueAtPoint(current, FLOOR);
                    }
                    else
                    {
                        if (theStack.Count > 0)
                            current = theStack.Pop();//pop a cell from the stack and make it the current cell
                        else
                        {
                            for (int x = 0; x < GridData.GetLength(0); x++)
                            {
                                for (int y = 0; y < GridData.GetLength(1); y++)
                                {
                                    Point currPoint = new Point(x, y);
                                    if (ValueAtPoint(currPoint) == UNVISITED)
                                        SetValueAtPoint(currPoint, WALL);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void Update()
        {
            Maze();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < GridData.GetLength(0); x++)
            {
                for (int y = 0; y < GridData.GetLength(1); y++)
                {
                    Color drawColor = Color.Black;
                    if (GridData[x, y] == UNVISITED)
                        drawColor = Color.Black;
                    else if (GridData[x, y] == VISITED)
                        drawColor = Color.White;
                    else if (GridData[x, y] == CURRENT)
                        drawColor = Color.Red;
                    else if (GridData[x, y] == FLOOR)
                        drawColor = Color.DarkGreen;
                    else if (GridData[x, y] == WALL)
                        drawColor = Color.Black;

                    if (GridData[x, y] != UNVISITED)
                    {
                        spriteBatch.Draw(Config.Pixel,
                            new Vector2(x * (Config.ResolutionWidth / GridWidth), y * (Config.ResolutionHeight / GridHeight)),
                            null,
                            drawColor,
                            0f,
                            Vector2.Zero,
                            new Vector2(Config.ResolutionWidth / GridWidth, Config.ResolutionHeight / GridHeight),
                            SpriteEffects.None,
                            1f);
                    }
                }
            }
        }
    }
}
