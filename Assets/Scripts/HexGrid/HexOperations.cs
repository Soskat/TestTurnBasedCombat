using System.Collections.Generic;
using TestTurnBasedCombat.Managers;
using UnityEngine;


namespace TestTurnBasedCombat.HexGrid
{
    /// <summary>
    /// Class that aggregates common hex cell and hex grid operations.
    /// </summary>
    public static class HexOperations
    {
        #region Public static methods
        /// <summary>
        /// Calculates the sum of two cube coordinates.
        /// </summary>
        /// <param name="a">First cube coordinates</param>
        /// <param name="b">Secont cube coordinates</param>
        /// <returns>Sum of the first and second cube coordinates</returns>
        public static Vector3Int AddTwoCubeCoords(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// Mark as selected all the hex cells from the given path.
        /// </summary>
        /// <param name="path">Path of hex cells</param>
        public static void SelectPath(Hex[] path)
        {
            if (path == null || path.Length < 1) return;
            int i;
            for (i = 0; i < path.Length - 1; i++) path[i].Select(AssetManager.instance.HexPath);
            if (path[i] != GameManager.instance.SelectedHex) path[i].Select(AssetManager.instance.HexPath);
        }

        /// <summary>
        /// Unselects all the hex cells from the given path.
        /// </summary>
        /// <param name="path">Path of hex cells</param>
        public static void UnselectPath(Hex[] path)
        {
            if (path == null || path.Length < 1) return;
            foreach (var hex in path) hex.Unselect();
            if (GameManager.instance.SelectedHex != null) GameManager.instance.SelectedHex.Select();
        }

        /// <summary>
        /// Converts offset coordinates of the hex cell to the cube coordinates.
        /// Based on: https://www.redblobgames.com/grids/hexagons/#conversions-offset
        /// </summary>
        /// <param name="hex">Hex cell</param>
        /// <returns>Cube coordinates</returns>
        public static Vector3Int OffsetToCubeCoords(Hex hex)
        {
            Vector3Int cube = Vector3Int.zero;
            cube.x = hex.ColumnIndex - (hex.RowIndex - (hex.RowIndex & 1)) / 2; // use bitwise AND to determine if hex.RowIndex is even(0) or odd(1)
            cube.z = hex.RowIndex;
            cube.y = -cube.x - cube.z;
            return cube;
        }

        /// <summary>
        /// Converts cube coordinates to the offset coordinates.
        /// Based on: https://www.redblobgames.com/grids/hexagons/#conversions-offset
        /// </summary>
        /// <param name="cube">Cube coordinates</param>
        /// <returns>Offset coordinates</returns>
        public static Vector2Int CubeToOffsetCoords(Vector3Int cube)
        {
            Vector2Int offset = Vector2Int.zero;
            offset.x = cube.x + (cube.z - (cube.z & 1)) / 2; // use bitwise AND to determine if cube.z is even(0) or odd(1)
            offset.y = cube.z;
            return offset;
        }

        /// <summary>
        /// Gets distance between two hexes with cube coordinates.
        /// Source: https://www.redblobgames.com/grids/hexagons/#distances
        /// </summary>
        /// <param name="a">First hex cell</param>
        /// <param name="b">Second hex cell</param>
        /// <returns>Distance between first and second hexes</returns>
        public static int GetDistanceBetweenHexes(Hex a, Hex b)
        {
            Vector3Int cubeA = OffsetToCubeCoords(a);
            Vector3Int cubeB = OffsetToCubeCoords(b);
            return Mathf.Max(Mathf.Abs(cubeA.x - cubeB.x), Mathf.Abs(cubeA.y - cubeB.y), Mathf.Abs(cubeA.z - cubeB.z));
        }

        /// <summary>
        /// Gets hex cells within a specific range around given hex.
        /// </summary>
        /// <param name="center">Hex that is a center of range</param>
        /// <param name="range">Max distance from center hex</param>
        /// <param name="hexGrid">Grid of hex cells</param>
        /// <returns></returns>
        public static Hex[] GetHexesInRange(Hex center, int range, GameObject[][] hexGrid)
        {
            List<Hex> hexesInRange = new List<Hex>() { center };
            if (range == 1) return hexesInRange.ToArray();
            Vector3Int cubeC = OffsetToCubeCoords(center);
            /*
            var results = []
            for each - N ≤ dx ≤ N:
                for each max(-N, -dx - N) ≤ dy ≤ min(N, -dx + N):
                    var dz = -dx - dy
                    results.append(cube_add(center, Cube(dx, dy, dz))) */
            int gridWidth = hexGrid.Length;
            int gridHeight = hexGrid[1].Length;
            int rMin, rMax, dz;
            for (int dx = -range; dx < range; dx++)
            {
                rMin = Mathf.Min(-range, -dx - range);
                rMax = Mathf.Max(range, dx + range);
                for (int dy = rMin; dy < rMax; dy++)
                {
                    dz = -dx - dy;
                    Vector2Int offset = CubeToOffsetCoords(new Vector3Int(cubeC.x + dx, cubeC.y + dy, cubeC.z + dz));
                    if (offset.x > 0 && offset.x < gridWidth && offset.y > 0 && offset.y < gridHeight)
                    {
                        hexesInRange.Add(hexGrid[offset.x][offset.y].GetComponent<Hex>());
                    }
                }
            }

            return hexesInRange.ToArray();
        }

        /// <summary>
        /// Finds all neighbours of the given hex cell.
        /// Based on: https://www.redblobgames.com/grids/hexagons/#neighbors
        /// </summary>
        /// <param name="hex">Hex cell to find the neighbours</param>
        /// <param name="HexGrid">Hex grid</param>
        /// <returns>List of the neighbours of the hex</returns>
        public static Hex[] GetNeighbours(Hex hex, GameObject[][] hexGrid)
        {
            Vector3Int cube = OffsetToCubeCoords(hex);
            Vector2Int offset = Vector2Int.zero;
            List<Hex> neighbours = new List<Hex>();
            int gridWidth = hexGrid.Length;
            int gridHeight = hexGrid[0].Length;
            // find all neighbours of the hex:
            foreach (Vector3Int coords in AssetManager.instance.CubeDirections)
            {
                offset = CubeToOffsetCoords(AddTwoCubeCoords(cube, coords));
                if (offset.x >= 0 && offset.x < gridWidth &&
                    offset.y >= 0 && offset.y < gridHeight)
                {
                    // neighbour exists -> add it to the list:
                    neighbours.Add(hexGrid[offset.x][offset.y].GetComponent<Hex>());
                }
            }
            return neighbours.ToArray();
        }

        /// <summary>
        /// Return heuristic value of the distance between start and goal hex cells.
        /// Based on: https://www.redblobgames.com/grids/hexagons/#distances
        /// </summary>
        /// <param name="start">Start hex</param>
        /// <param name="goal">Final hex</param>
        /// <returns>Heuristic value</returns>
        public static float Heuristic(Hex start, Hex goal)
        {
            Vector3Int a = OffsetToCubeCoords(start);
            Vector3Int b = OffsetToCubeCoords(goal);
            return (Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y) + Mathf.Abs(a.z - b.z));
        }

        /// <summary>
        /// Finds the best path from the first hex of the path to the last hex of the path.
        /// Based on: https://www.redblobgames.com/pathfinding/a-star/implementation.html#python-astar
        /// </summary>
        /// <param name="start">First hex of the path</param>
        /// <param name="goal">Final hex of the path</param>
        /// <param name="hexGrid">Hex grid</param>
        /// <param name="maxSteps">Maximum number of steps in the path</param>
        /// <returns>The best path</returns>
        public static Hex[] FindPathUsingAStar(Hex start, Hex goal, GameObject[][] hexGrid, int maxSteps = int.MaxValue)
        {
            // initialize all variables:
            PriorityQueue<float, Hex> frontier = new PriorityQueue<float, Hex>();
            Dictionary<Hex, Hex> cameFrom = new Dictionary<Hex, Hex>();
            Dictionary<Hex, float> costSoFar = new Dictionary<Hex, float>();
            float newCost, priority;
            Hex current;
            frontier.Enqueue(0f, start);
            cameFrom.Add(start, null);
            costSoFar.Add(start, 0f);
            // while the queue is not empty:
            while (!frontier.IsEmpty)
            {
                // get the first element from the queue:
                current = frontier.Dequeue();

                // is the current element the goal?:
                if (current == goal) break;

                // examine all neighbours:
                Hex[] neighbours = GetNeighbours(current, hexGrid);
                foreach (Hex neighbour in neighbours)
                {
                    // skip hex cell if it is occupied:
                    if (neighbour.IsOccupied) continue;

                    // calculate new cost between current and neighbour:
                    newCost = costSoFar[current] + 1f;

                    // add neighbour to the lists if needed:
                    if (!costSoFar.ContainsKey(neighbour))
                    {
                        costSoFar.Add(neighbour, newCost);
                        priority = newCost + Heuristic(current, neighbour);
                        frontier.Enqueue(priority, neighbour);
                        cameFrom.Add(neighbour, current);
                    }
                }
            }
            // reconstruct the path:
            List<Hex> path = new List<Hex>();
            current = goal;
            path.Add(current);
            while (current != null)
            {
                if (cameFrom.ContainsKey(current))
                {
                    if (cameFrom[current] != null) path.Add(cameFrom[current]);
                    current = cameFrom[current];
                }
                else break;
            }
            path.RemoveAt(path.Count - 1);  // remove 
            path.Reverse();
            // return path:
            if (maxSteps < int.MaxValue && maxSteps < path.Count) return path.GetRange(0, maxSteps).ToArray();
            else return path.ToArray();
        }
        #endregion
    }
}
