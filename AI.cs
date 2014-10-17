using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project1
{
    class AI
    {
        public static List<Vector2> Pathfind(Vector2 start, Vector2 end, double[,] pointMap)
        {
            // Defines points that have already been explored.
            var closed = new List<Vector2>();
            // Defines points that haven't been explored.
            var open = new List<Vector2> { start };
            // Just a way for us to figure out how to best figure out an optimal path of a point given its origin.
            var prev = new Dictionary<Vector2, Vector2>();

            // Get a point with its optimal distance
            var dist = new Dictionary<Vector2, int>();

            // A dictionary for a point and its straight-line heuristic value.
            var calcDistance = new Dictionary<Vector2, float>();

            // Add in the origin. There should be no weight on it.
            dist.Add(start, 0);

            // Similarly, add in the origin and its corresponding heuristic value.
            calcDistance.Add(start, Math.Abs(start.X - end.X) + Math.Abs(start.Y - end.Y));

            while (open.Count > 0)
            {
                // Find nodes with the lowest cost to the end.
                var current = start;
                foreach (var p in calcDistance.Keys)
                {
                    if (calcDistance[p] < calcDistance[current])
                    {
                        //Console.WriteLine(current.X + ", " + current.Y + " | " + p.X + ", " + p.Y);
                        current = p;
                    }
                }


                // Return a path if we reached the end.
                if ((int)current.X == (int)end.X && (int)current.Y == (int)end.Y)
                    return generatePath(prev, end);

                open.Remove(current);
                closed.Add(current);

                // Search through a point's surrounding neighbours.
                foreach (var neighbour in getNeighbours(current, pointMap))
                {
                    var tempDistance = dist[current] + 1;

                    // Ignore if there is a faster way to get to that neighbour.
                    if (closed.Contains(neighbour)
                        && tempDistance >= dist[neighbour])
                        continue;

                    // Otherwise, if there isn't any, store this route instead.
                    if (!closed.Contains(neighbour)
                        || tempDistance < dist[neighbour])
                    {
                        // If we find that we found a faster way to get to the neighbour, modify accordingly.
                        if (prev.ContainsKey(neighbour))
                            prev[neighbour] = current;
                        else
                            prev.Add(neighbour, current);

                        // Set weights and utility values as well.
                        dist[neighbour] = tempDistance;
                        calcDistance[neighbour] = dist[neighbour] + Math.Abs(neighbour.X - end.X) + Math.Abs(neighbour.Y - end.Y);

                        // If we haven't explored this node before,  add it to the open set.
                        if (!open.Contains(neighbour))
                            open.Add(neighbour);
                    }
                }
            }

            // If for some reason we break out of the loop, throw an exception saying we couldn't find a path.
            throw new Exception("Could not find a path.");
        }

        // Fetch all of the neighbours of a point.
        private static List<Vector2> getNeighbours(Vector2 p, double[,] pointMap)
        {
            var neighbours = new List<Vector2>();

            //Console.WriteLine("X: " + p.X + ", Y: " + p.Y);
            // Add a point going up.
            if (pointMap[(int)p.X, (int)p.Y - 1] > 0)
                neighbours.Add(new Vector2(p.X, p.Y - 1));

            // Add a point going right.
            if (pointMap[(int)p.X + 1, (int)p.Y] > 0)
                neighbours.Add(new Vector2(p.X + 1, p.Y));

            // Add a point going down.
            if (pointMap[(int)p.X, (int)p.Y + 1] > 0)
                neighbours.Add(new Vector2(p.X, p.Y + 1));

            // Add a point going left.
            if (pointMap[(int)p.X - 1, (int)p.Y] > 0)
                neighbours.Add(new Vector2(p.X - 1, p.Y));

            return neighbours;
        }

        // Recursively walk the grid and find the shortest path to the start.
        private static List<Vector2> generatePath(Dictionary<Vector2, Vector2> prev, Vector2 current)
        {
            if (!prev.ContainsKey(current))
                return new List<Vector2> { current };

            var path = generatePath(prev, prev[current]);
            path.Add(current);
            return path;
        }
    }
}
