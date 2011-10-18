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
using Utility;

namespace PixelArt
{
    class SelfAvoidingWalk
    {
        int LINE_THICKNESS = 1; //thickness of the lines; 2 default
        int ITERATIONS_PER_FRAME = 15; //increase to increase the speed; 7 default
        static int SCALE = 15; //lower number = more grid points; 15 default
        float JITTERINESS = .67f; //number between 0f and 1f; 0 is more rigid, 1 is more jittery; (greater than 1f causes extreme jitter) .67f default
        bool JITTER = !true;
        int JITTER_SPEED = 1; //lower number = faster. between 1 and infinity
        bool DRAW_POINTS = false; //whether or not to draw points; false default
        float MIN_ALPHA = .5f;// minimum alpha value. between 0 and 1
        Color[] colors = new[] //colors it cycles through, add as many as you want; White then ROYGBV default
        {
           Color.White,
           Color.Red,
           Color.Orange,
           Color.Yellow,
           Color.Green,
           Color.Blue,
           Color.Violet,
        };

        public const int UNVISITED = 0;
        public const int VISITED = 1;
        

        public static int GridWidth = Config.ResolutionWidth / SCALE;
        public static int GridHeight = Config.ResolutionHeight / SCALE;
        public static Random Random = new Random();
        private Vector2 gridOffset = new Vector2(SCALE / 2, SCALE / 2);
        private Point gridOffsetp = new Point(SCALE / 2, SCALE / 2);
        public int[,] GridData = new int[GridWidth, GridHeight];
        public Point[,] GridOffsets = new Point[GridWidth, GridHeight];
        private Dictionary<int, Point> directionMap = new Dictionary<int, Point>();
        Point current = new Point(Config.ResolutionWidth / SCALE / 2, Config.ResolutionHeight / SCALE / 2);
        private Stack<Point> theStack = new Stack<Point>();
        private Queue<Point> history = new Queue<Point>();
        private Stack<Point> thePath = new Stack<Point>();

        TimerCollection timers = new TimerCollection();
        InterpolatorCollection interpolators = new InterpolatorCollection();

        Color color = Color.White;
        
        int currentColor = 0;

        private void Interpolate()
        {
            interpolators.Create(
               0f,//start
               1f,//end
               1f,//length of time to perform interpolation
               InterpolatorStep,
               i => currentColor = (currentColor + 1) % colors.Length);
        }
        private void InterpolatorStep(Interpolator i)
        {
            color = Color.Lerp(
               colors[currentColor],
               colors[(currentColor + 1) % colors.Length],
               i.Value);
        }


        public SelfAvoidingWalk()
        {
            for (int x = 0; x < GridData.GetLength(0); x++)
            {
                for (int y = 0; y < GridData.GetLength(1); y++)
                {
                    GridData[x, y] = 0;
                    GridOffsets[x, y] = new Point(Random.Next((int)(JITTERINESS * SCALE)), Random.Next((int)(JITTERINESS * SCALE)));
                }
            }

            directionMap.Add(0, Directions.UP);
            directionMap.Add(1, Directions.RIGHT);
            directionMap.Add(2, Directions.LEFT);
            directionMap.Add(3, Directions.DOWN);

            SetValueAtPoint(current, VISITED);

            theStack.Push(current);
            //history.Enqueue(current);
            thePath.Push(current);
            timers.Create(1f, true, timer => Interpolate());
        }

        public Point GetRandomPoint()
        {
            return new Point(Random.Next(GridWidth), Random.Next(GridHeight));
        }

        public Point GetRandomDirection()
        {
            return directionMap[Random.Next(directionMap.Count)];
        }

        //returns the directions
        public List<Point> GetUnvisitedNeighbors(Point point)
        {
            List<Point> uneighbors = new List<Point>();

            for (int i = 0; i < directionMap.Count; i++)
            {
                if (ValueAtPoint(point + directionMap[i] ) == UNVISITED)// || ValueAtPoint(point + directionMap[i]) == VISITED) //point is unvisited
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
        bool update = true;
        public void Update(GameTime gameTime)
        {
            if (update)
            {
                //Lightning.UpdateAll(gameTime);
                timers.Update(gameTime);
                interpolators.Update(gameTime);

                for (int i = 0; i < ITERATIONS_PER_FRAME; i++)
                {
                    SAW();
                }

                if (JITTER)
                    Jitter();
            }
            if (Keyboard.GetState().IsKeyDown(Keys.F9))
                update = true;
        }

        long c = 0;
        private void Jitter()
        {
            if (c % JITTER_SPEED == 0)
            {
                for (int x = 0; x < GridData.GetLength(0); x++)
                {
                    for (int y = 0; y < GridData.GetLength(1); y++)
                    {
                        if (ValueAtPoint(new Point(x, y)) == VISITED)
                            GridOffsets[x, y] = new Point(Random.Next((int)(JITTERINESS * SCALE)), Random.Next((int)(JITTERINESS * SCALE)));
                    }
                }
            }
            c++;
        }

        public void Reset()
        {
            for (int x = 0; x < GridData.GetLength(0); x++)
            {
                for (int y = 0; y < GridData.GetLength(1); y++)
                {
                    GridData[x, y] = 0;
                    GridOffsets[x, y] = new Point(Random.Next((int)(JITTERINESS * SCALE)), Random.Next((int)(JITTERINESS * SCALE)));
                }
            }
            current = GetRandomPoint();
            theStack.Clear();
            history.Clear();
            thePath.Clear();
            RandomizeParameters();
        }

        private void RandomizeParameters()
        {
            List<int> thicknesses = new List<int>();
            for (int i = 1; i < 7; i++)
            {
                for (int j = 0; j < (7-i); j++)
                {
                    thicknesses.Add(i);
                }
            }
            thicknesses.Shuffle();
            LINE_THICKNESS = thicknesses[Random.Next(thicknesses.Count)]; //thickness of the lines; 2 default

            ITERATIONS_PER_FRAME = (int)RandomRange(1, 50); //increase to increase the speed; 7 default
            SCALE = (int)RandomRange(15, 30); ; //lower number = more grid points; 15 default
            JITTERINESS = RandomRange(0f, .7f); ; //number between 0f and 1f; 0 is more rigid, 1 is more jittery; (greater than 1f causes extreme jitter) .67f default
            int choice = Random.Next(100);
            if (choice < 25)
            {
                JITTER = true;
            }
            else
                JITTER = false;
        }
        bool justPopped = false;

        private void SAW()
        {
            List<Point> uneighbors = GetUnvisitedNeighbors(current);

            if (uneighbors.Count > 0) //if the current cell has any neighbors which hav enot been visited
            {
                if (justPopped)
                {
                }
                
                AddCurrentPointToThePathIfItsANeighborOfPreviousPointThatWasAddedToTheStack(current);
                Point chosen = uneighbors[Random.Next(uneighbors.Count)]; //choose a random unvisited neighbor
                //Point chosen = uneighbors[uneighbors.Count - 1]; //choose last neighbor
                Point nextPoint = current + chosen;

                theStack.Push(nextPoint);
                SetValueAtPoint(nextPoint, VISITED);

                current = nextPoint;

            }
            else //stuck
            {
                if (theStack.Count > 1)
                {
                    
                    current = theStack.Pop();
                    AddCurrentPointToThePathIfItsANeighborOfPreviousPointThatWasAddedToTheStack(current);
                    current = theStack.Peek();
                    justPopped = true;
                }
                else
                {
                    Reset();
                }
            }
            
        }

        private void AddCurrentPointToThePathIfItsANeighborOfPreviousPointThatWasAddedToTheStack(Point current)
        {
            if (thePath.Count > 0)
            {
                Point lastAdded = thePath.Peek();
                for (int i = 0; i < directionMap.Count; i++)
                {
                    if ((lastAdded + directionMap[i]).Equals(current) || lastAdded.Equals(current))
                    {
                        thePath.Push(current);
                        return;
                    }
                }
            }

            thePath.Push(current); //makes diagonals
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int x = 0; x < GridData.GetLength(0); x++)
            {
                for (int y = 0; y < GridData.GetLength(1); y++)
                {
                    Color drawColor = Color.Black;
                    if (GridData[x, y] == UNVISITED)
                        drawColor = Color.White;
                    else if (GridData[x, y] == VISITED)
                        drawColor = color;

                    if (GridData[x, y] != -1 && DRAW_POINTS)
                    {

                        spriteBatch.Draw(Config.Pixel,
                            new Vector2(x * (Config.ResolutionWidth / GridWidth), y * (Config.ResolutionHeight / GridHeight)) + gridOffset + new Vector2(GridOffsets[x,y].X, GridOffsets[x,y].Y),
                            null,
                            drawColor,
                            0f,
                            Vector2.Zero,
                            new Vector2(1f,1f),//new Vector2(Config.ResolutionWidth / GridWidth, Config.ResolutionHeight / GridHeight),
                            SpriteEffects.None,
                            1f);
                    }
                }
            }

            DrawPath(spriteBatch);
            //Lightning.DrawAll(spriteBatch);
        }

        private void DrawPath(SpriteBatch spriteBatch)
        {
            List<Point> points = new List<Point>(thePath);
            for (int i = 1; i < points.Count; i++)
            {
                Point adjustedA = new Point(points[i].X * (Config.ResolutionWidth / GridWidth), points[i].Y * (Config.ResolutionHeight / GridHeight)) + gridOffsetp + GridOffsets[points[i].X, points[i].Y];
                Point adjustedB = new Point(points[i - 1].X * (Config.ResolutionWidth / GridWidth), points[i - 1].Y * (Config.ResolutionHeight / GridHeight)) + gridOffsetp + GridOffsets[points[i-1].X, points[i-1].Y];
                float alpha = Math.Max(MIN_ALPHA, 1f-((float)i / (float)thePath.Count));
                Line.DrawLine(spriteBatch, color * alpha, adjustedA, adjustedB, 0f, LINE_THICKNESS);
                
                //Vector2 vA = new Vector2(adjustedA.X, adjustedA.Y);
                //Vector2 vB = new Vector2(adjustedB.X, adjustedB.Y);
                //Lightning.CreateBolt(vA, vB, 10, 5, 5);
            }            
        }

        public static float RandomRange(float min, float max)
        {
            return (float)Random.NextDouble() * (max - min) + min;
        }
    }

    public static class Shuffler
    {
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
