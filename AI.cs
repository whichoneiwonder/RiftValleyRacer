using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Project
{
    public class AI
    {
        static Vector2 start;
        static Vector2 end;
        static Vector2 lastNode;

        // Stores path cost between the start and a point in the array.
        static int[,] g;

        // Stores heuristic calculation between a point in the array and the goal point.
        static int[,] h;

        // Stores the sum of the two, to figure out the cost of the most efficient path to a goal.
        static int[,] f;

        // Stores optimal paths up to a point.
        static Vector2[,] prev;

        // Stores the terrain as a whole.
        static double[,] map;
        static int width;
        static int height;

        // Find a path over a map given a start and end point to move towards.
        public static List<Vector2> findPath(Vector2 s, Vector2 e, double[,] m)
        {
            // INITIALISE ALL THE THINGS
            start = s;
            end = e;
            map = m;

            width = map.GetLength(0);
            height = map.GetLength(1);

            g = new int[width, height];
            h = new int[width, height];
            f = new int[width, height];
            prev = new Vector2[width, height];

            List<Vector2> open = new List<Vector2>();
            List<Vector2> closed = new List<Vector2>();

            open.Add(start);
            g[(int)start.X, (int)start.Y] = 0;
            h[(int)start.X, (int)start.Y] = calculateHeuristic(start);
            f[(int)start.X, (int)start.Y] = h[(int)start.X, (int)start.Y];
            prev[(int)start.X, (int)start.Y] = -Vector2.One;

            while (open.Count > 0)
            {
                
                int min = -1;
                Vector2 point = -Vector2.One;

                // Get the most optimal point in the open set.
                foreach (Vector2 p in open)
                {
                    int distance = 0;

                    if (prev[(int)p.X, (int)p.Y] != -Vector2.One && prev[(int)p.X, (int)p.Y] != new Vector2(width, height))
                        distance = g[(int)prev[(int)p.X, (int)p.Y].X, (int)prev[(int)p.X, (int)p.Y].Y]
                            + calculateDistance(p, prev[(int)p.X, (int)p.Y]) + calculateHeuristic(p);

                    if (distance <= min || min == -1)
                    {
                        min = distance;
                        point = p;
                    }
                }
                //Debug.WriteLine("P: " + point.X + ", " + point.Y + " | E: " + end.X + ", " + end.Y);
                // Return a path if we reached the end.
                if (calculateDistance(point, end) <= 5)
                    return generatePath(point);

                open.Remove(point);
                closed.Add(point);

                List<Vector2> neighbours = getNeighbours(point);
                foreach (Vector2 n in neighbours)
                {
                    // Ignore if this point is already explored.
                    if (closed.Contains(n))
                        continue;

                    // Store a temporary distance containing the path
                    // cost and the distance between the two points.
                    int t = g[(int)point.X, (int)point.Y] + calculateDistance(n, point);

                    // If either the current neighbour is unexplored, or if there is a
                    // more optimal path to reach that neighbour, update the neighbour's
                    // utility values accordingly.
                    if (!open.Contains(n) || (open.Contains(n) && t < g[(int)n.X, (int)n.Y]))
                    {
                        // If the neighbour is unexplored, add it to the open set.
                        if (!open.Contains(n))
                            open.Add(n);
                        
                        // Update utility values as necessary.
                        prev[(int)n.X, (int)n.Y] = point;
                        g[(int)n.X, (int)n.Y] = t;
                        h[(int)n.X, (int)n.Y] = calculateHeuristic(n);
                        f[(int)n.X, (int)n.Y] = t + h[(int)n.X, (int)n.Y];
                    }
                }

                lastNode = point;
            }

            return null;
        }

        // Calculate the straight-line heuristic value between the current point and the goal.
        private static int calculateHeuristic(Vector2 p)
        {
            return (int)(Math.Abs(p.X - end.X) + Math.Abs(p.Y - end.Y));
        }

        // Calculates the distance between one point and another.
        private static int calculateDistance(Vector2 a, Vector2 b)
        {
            Vector3 i = new Vector3(a.X, (float)map[(int)a.X, (int)a.Y], a.Y);
            //Debug.WriteLine(b);
            //Debug.WriteLine((float)map[(int)b.X, (int)b.Y]);
            Vector3 j = new Vector3(b.X, (float)map[(int)b.X, (int)b.Y], b.Y);
            return (int)Math.Round(Math.Sqrt(Math.Pow(i.X - j.X, 2) + Math.Pow(i.Y - j.Y, 2) + Math.Pow(i.Z - j.Z, 2)));
        }

        // Get all neighbours for a certain point on the map.
        private static List<Vector2> getNeighbours(Vector2 p)
        {
            List<Vector2> neighbours = new List<Vector2>();
            bool left = p.X > 0 && map[(int)p.X - 1, (int)p.Y] > 0;
            bool right = p.X < width - 1 && map[(int)p.X + 1, (int)p.Y] > 0;
            bool up = p.Y > 0 && map[(int)p.X, (int)p.Y - 1] > 0;
            bool down = p.Y < height - 1 && map[(int)p.X, (int)p.Y + 1] > 0;

            if (up) neighbours.Add(new Vector2((int)p.X, (int)p.Y - 1));
            if (right) neighbours.Add(new Vector2((int)p.X + 1, (int)p.Y));
            if (down) neighbours.Add(new Vector2((int)p.X, (int)p.Y + 1));
            if (left) neighbours.Add(new Vector2((int)p.X - 1, (int)p.Y));
            if (up && right) neighbours.Add(new Vector2((int)p.X + 1, (int)p.Y - 1));
            if (up && left) neighbours.Add(new Vector2((int)p.X - 1, (int)p.Y - 1));
            if (down && right) neighbours.Add(new Vector2((int)p.X + 1, (int)p.Y + 1));
            if (down && left) neighbours.Add(new Vector2((int)p.X - 1, (int)p.Y + 1));/**/
            return neighbours;
        }

        // Recursively walk the grid and find the shortest path back to the start.
        private static List<Vector2> generatePath(Vector2 p)
        {
            List<Vector2> path = new List<Vector2>();

            // If we have not reached the termination point
            // for the path, continue generating the path.
            if (p.X > -1 && p.Y > -1)
                path = generatePath(prev[(int)p.X, (int)p.Y]);

            path.Add(p);

            return path;
        }
    }
}
