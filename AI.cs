using SharpDX;
using System;
using System.Collections.Generic;

namespace Project1
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
            start = new Vector2(s.X, s.Y);
            end = new Vector2(e.X, e.Y);
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
            prev[(int)start.X, (int)start.Y] = new Vector2(-1, -1);

            while (open.Count > 0)
            {
                int min = -1;
                Vector2 point = new Vector2(-1, -1);

                // Get the most optimalest point in the open set.
                foreach (Vector2 p in open)
                {
                    int distance = 0;

                    if (prev[(int)p.X, (int)p.Y] != new Vector2(-1, -1))
                        distance = g[(int)prev[(int)p.X, (int)p.Y].X, (int)prev[(int)p.X, (int)p.Y].Y]
                            + calculateDistance(p, prev[(int)p.X, (int)p.Y]) + calculateHeuristic(p);

                    if (distance <= min || min == -1)
                    {
                        min = distance;
                        point = p;
                    }
                }

                // Return a path if we reached the end.
                if (point == end)
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
            return (int)Math.Round(Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2)));
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
            if (down && left) neighbours.Add(new Vector2((int)p.X - 1, (int)p.Y + 1));
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
