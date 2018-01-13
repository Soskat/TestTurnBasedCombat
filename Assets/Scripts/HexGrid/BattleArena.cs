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
        }

        // Update is called once per frame
        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // check if mouse cursor is pointing at a hex cell:
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("HexGrid")))
            {
                GameManager.instance.SelectHexCell(hit.collider.gameObject.GetComponent<Hex>());
            }
            else
            {
                GameManager.instance.SelectHexCell(null);
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
                        break;
                    }
                }
            }
        }
        #endregion
    }
}
