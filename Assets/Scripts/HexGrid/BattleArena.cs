using System.Collections.Generic;
using TestTurnBasedCombat.Managers;
using UnityEngine;
using UnityEngine.Assertions;


namespace TestTurnBasedCombat.HexGrid
{
    /// <summary>
    /// Component that represents the battle arena object.
    /// </summary>
    public class BattleArena : MonoBehaviour
    {
        #region Private fields
        /// <summary>Hex grid holder game object.</summary>
        [SerializeField] private GameObject hexGridObject;
        /// <summary>Width of the hex grid (number of hex rows).</summary>
        [SerializeField] private int gridWidth;
        /// <summary>Height of the hex grid (number of hex columns).</summary>
        [SerializeField] private int gridHeight;
        /// <summary>Prefab of the hex cell object.</summary>
        [SerializeField] private GameObject hexPrefab;
        /// <summary>Length of the hex side.</summary>
        [SerializeField] private float hexSide = 1f;
        /// <summary>Obstacles holder game object.</summary>
        [SerializeField] private GameObject obstaclesObject;
        /// <summary>Prefab of the obstacle object.</summary>
        [SerializeField] private GameObject obstaclePrefab;
        /// <summary>Number of the obstacles.</summary>
        [SerializeField] private int obstaclesNumber = 2;
        /// <summary>Width of the hex cell.</summary>
        private float hexWidth;
        /// <summary>Height of the hex cell.</summary>
        private float hexHeight;
        /// <summary>Table of the hex cells.</summary>
        private GameObject[][] hexCells;
        /// <summary>Table of the obstacles.</summary>
        private GameObject[] obstacles;
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            // do asstertions:
            Assert.IsNotNull(hexGridObject);
            Assert.IsNotNull(hexPrefab);
            Assert.IsNotNull(obstaclesObject);
            Assert.IsNotNull(obstaclePrefab);
            // set up hexWidth and hexHeight:
            hexWidth = hexSide * Mathf.Sqrt(3f);
            hexHeight = 2 * hexSide;
        }

        // Use this for initialization
        void Start()
        {
            CreateHexGrid();
            AddObstacles();
            SetUpArmiesPosition();
        }

        // Update is called once per frame
        void Update()
        {
            if (!GameManager.instance.ActionInProgress)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                // check if mouse cursor is pointing at a hex cell:
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("HexGrid")))
                {
                    GameManager.instance.SelectHexCell(hit.collider.gameObject.GetComponent<Hex>());
                    // find the path from SelectedUnit to SelectedHex:
                    if (GameManager.instance.SelectedUnit != null)
                    {
                        // unselect last path:
                        UnselectPath(GameManager.instance.LastPath);
                        // find a new path to the destination (with limited path length):
                        GameManager.instance.LastPath = FindPathUsingAStar(GameManager.instance.SelectedUnitHex,
                                                                           GameManager.instance.SelectedHex,
                                                                           GameManager.instance.SelectedUnit.UnitData.ActionPoints);

                        //// find a new path to the destination (with unlimited path length):
                        //GameManager.instance.LastPath = FindPathUsingAStar(GameManager.instance.SelectedUnitHex,
                        //                                                   GameManager.instance.SelectedHex);
                        SelectPath(GameManager.instance.LastPath);
                    }
                }
                else
                {
                    GameManager.instance.SelectHexCell(null);
                }
            }
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Creates grix of hexes.
        /// </summary>
        private void CreateHexGrid()
        {
            float xOffset = 0.5f * hexWidth;
            float yOffset = 0.75f * hexHeight;
            Vector3 startPos = Vector3.zero;
            startPos.x = transform.position.x - ((gridWidth * hexWidth) / 2f - xOffset / 2f);
            startPos.z = transform.position.z - (((gridHeight - 1) * yOffset) / 2f);
            // create the hex grid:
            hexCells = new GameObject[gridWidth][];
            for (int x = 0; x < gridWidth; x++)
            {
                hexCells[x] = new GameObject[gridHeight];
                for (int y = 0; y < gridHeight; y++)
                {
                    GameObject hex = Instantiate(hexPrefab) as GameObject;
                    // calculate object position:
                    if (y % 2 == 0)
                        hex.transform.Translate(startPos.x + x * hexWidth, startPos.z + y * yOffset, 0f);
                    else
                        hex.transform.Translate(startPos.x + xOffset + x * hexWidth, startPos.z + y * yOffset, 0f);
                    hex.transform.SetParent(hexGridObject.transform);
                    // set up Hex component:
                    hex.GetComponent<Hex>().RowIndex = y;
                    hex.GetComponent<Hex>().ColumnIndex = x;
                    hexCells[x][y] = hex;
                }
            }
        }

        /// <summary>
        /// Adds obstacles to the battle arena.
        /// </summary>
        private void AddObstacles()
        {
            System.Random rand = new System.Random();
            int halfWidth = gridWidth / 2;
            int xOffset = (int)(0.25f * gridWidth);
            int x, y;
            for (int i = 0; i < obstaclesNumber; i++)
            {
                while (true)
                {
                    x = rand.Next(halfWidth - xOffset, halfWidth + xOffset);
                    y = rand.Next(0, gridHeight);
                    if (!hexCells[x][y].GetComponent<Hex>().IsOccupied)
                    {
                        // create new obstacle:
                        GameObject go = Instantiate(obstaclePrefab) as GameObject;
                        Vector3 goPosition = hexCells[x][y].transform.position;
                        goPosition.y = 0.5f;
                        go.transform.SetPositionAndRotation(goPosition, go.transform.rotation);
                        go.transform.SetParent(obstaclesObject.transform);
                        hexCells[x][y].GetComponent<Hex>().IsOccupied = true;
                        hexCells[x][y].GetComponent<Hex>().SetOccupyingObject(go);
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Set up armies positions.
        /// </summary>
        private void SetUpArmiesPosition()
        {

        }

        /// <summary>
        /// Calculates the sum of two cube coordinates.
        /// </summary>
        /// <param name="a">First cube coordinates</param>
        /// <param name="b">Secont cube coordinates</param>
        /// <returns>Sum of the first and second cube coordinates</returns>
        private Vector3Int AddCubeCoords(Vector3Int a, Vector3Int b)
        {
            return new Vector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        /// <summary>
        /// Mark as selected all the hex cells from the given path.
        /// </summary>
        /// <param name="path">Path of hex cells</param>
        private void SelectPath(Hex[] path)
        {
            if (path == null) return;
            foreach (Hex hex in path)
            {
                if (hex != GameManager.instance.SelectedHex) hex.Select(AssetManager.instance.HexPath);
            }
        }

        /// <summary>
        /// Unselects all the hex cells from the given path.
        /// </summary>
        /// <param name="path">Path of hex cells</param>
        private void UnselectPath(Hex[] path)
        {
            if (path == null) return;
            foreach (Hex hex in path)
            {
                if (hex != GameManager.instance.SelectedHex) hex.Unselect();
            }
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Converts offset coordinates of the hex cell to the cube coordinates.
        /// Based on: https://www.redblobgames.com/grids/hexagons/#conversions-offset
        /// </summary>
        /// <param name="hex">Hex cell</param>
        /// <returns>Cube coordinates</returns>
        public Vector3Int OffsetToCubeCoords(Hex hex)
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
        public Vector2Int CubeToOffsetCoords(Vector3Int cube)
        {
            Vector2Int offset = Vector2Int.zero;
            offset.x = cube.x + (cube.z - (cube.z & 1)) / 2; // use bitwise AND to determine if cube.z is even(0) or odd(1)
            offset.y = cube.z;
            return offset;
        }

        /// <summary>
        /// Finds all neighbours of the given hex cell.
        /// Based on: https://www.redblobgames.com/grids/hexagons/#neighbors
        /// </summary>
        /// <param name="hex">hex cell to find the neighbours</param>
        /// <returns>List of the neighbours of the hex</returns>
        public Hex[] GetNeighbours(Hex hex)
        {
            Vector3Int cube = OffsetToCubeCoords(hex);
            Vector2Int offset = Vector2Int.zero;
            List<Hex> neighbours = new List<Hex>();
            // find all neighbours of the hex:
            foreach(Vector3Int coords in AssetManager.instance.CubeDirections)
            {
                offset = CubeToOffsetCoords(AddCubeCoords(cube, coords));
                if (offset.x >= 0 && offset.x < gridWidth &&
                    offset.y >= 0 && offset.y < gridHeight)
                {
                    // neighbour exists -> add it to the list:
                    neighbours.Add(hexCells[offset.x][offset.y].GetComponent<Hex>());
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
        public float Heuristic(Hex start, Hex goal)
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
        /// <param name="maxSteps">Maximum number of steps in the path</param>
        /// <returns>The best path</returns>
        public Hex[] FindPathUsingAStar(Hex start, Hex goal, int maxSteps = int.MaxValue)
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
                Hex[] neighbours = GetNeighbours(current);
                foreach(Hex neighbour in neighbours)
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
            path.Reverse();
            // return path:
            if (maxSteps < int.MaxValue && maxSteps < path.Count) return path.GetRange(0, maxSteps).ToArray();
            else return path.ToArray();
        }
        #endregion
    }
}
