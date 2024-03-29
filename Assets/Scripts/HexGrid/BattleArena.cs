﻿using TestTurnBasedCombat.Managers;
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
        /// <summary>Player 1 units game objects holder.</summary>
        [SerializeField] private GameObject player1UnitsHolder;
        /// <summary>Player 2 units game objects holder.</summary>
        [SerializeField] private GameObject player2UnitsHolder;
        /// <summary>Width of the hex cell.</summary>
        private float hexWidth;
        /// <summary>Height of the hex cell.</summary>
        private float hexHeight;
        /// <summary>Table of the hex cells.</summary>
        private GameObject[][] hexCells;
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
            Assert.IsNotNull(player1UnitsHolder);
            Assert.IsNotNull(player2UnitsHolder);
            // set up hexWidth and hexHeight:
            hexWidth = hexSide * Mathf.Sqrt(3f);
            hexHeight = 2 * hexSide;

            // sign up for actions:
            GameManager.instance.CreatedUnits += () =>
            {
                // recreate units before next battle:
                if (hexCells != null)
                {
                    // recreate obstacles:
                    for (int i = 0; i < obstaclesObject.transform.childCount; i++)
                    {
                        Destroy(obstaclesObject.transform.GetChild(i).gameObject);
                    }
                    AddObstacles();
                    SetUpArmiesPosition();

                    // start the battle:
                    GameManager.instance.StartTheBattle();
                }
            };
            GameManager.instance.UpdateSelectedHex += () =>
            {
                // update hexes materials when SelectedHex changes:
                if (GameManager.instance.SelectedHex != null &&
                    GameManager.instance.SelectedUnitHex != null &&
                    GameManager.instance.CurrentAttack != null)
                {
                    // current attack need an enemy to launch:
                    if (GameManager.instance.CurrentAttack.NeedEnemyToLaunch)
                    {
                        // check if SelectedHex contains an enemy:
                        if (GameManager.instance.IsSelectedHexContainsEnemy)
                        {
                            // enemy is in range of attack:
                            if (HexOperations.GetDistanceBetweenHexes(GameManager.instance.SelectedUnitHex, GameManager.instance.SelectedHex)
                                <= GameManager.instance.CurrentAttack.RangeOfAttack)
                            {
                                // unselects the last path:
                                HexOperations.UnselectPath(GameManager.instance.LastPath);
                                // mark hexes whitin the damage range of attack as vulnerable:
                                HexOperations.UnselectRangeOfHexes(GameManager.instance.DamageRangeHexes);
                                GameManager.instance.DamageRangeHexes = HexOperations.GetHexesInDamageRange(GameManager.instance.SelectedHex,
                                                                                                      GameManager.instance.CurrentAttack.DamageRange,
                                                                                                      hexCells);
                                HexOperations.SelectRangeOfHexes(GameManager.instance.DamageRangeHexes, AssetManager.instance.HexEnemyToAttack);
                            }
                            // enemy is out of range of attack:
                            else
                            {
                                // unselects the last path:
                                HexOperations.UnselectPath(GameManager.instance.LastPath);
                                // find a new path to the destination (with limited path length):
                                GameManager.instance.LastPath = HexOperations.FindPathUsingAStar(GameManager.instance.SelectedUnitHex,
                                                                                                 GameManager.instance.SelectedHex,
                                                                                                 hexCells,
                                                                                                 GameManager.instance.SelectedUnit.UnitData.CurrentActionPoints);
                                HexOperations.SelectPath(GameManager.instance.LastPath);
                            }
                        }
                        // there's no enemy on SelectedHex:
                        else
                        {
                            // unselect last path:
                            HexOperations.UnselectPath(GameManager.instance.LastPath);
                            // find a new path to the destination (with limited path length):
                            GameManager.instance.LastPath = HexOperations.FindPathUsingAStar(GameManager.instance.SelectedUnitHex,
                                                                                             GameManager.instance.SelectedHex,
                                                                                             hexCells,
                                                                                             GameManager.instance.SelectedUnit.UnitData.CurrentActionPoints);
                            HexOperations.SelectPath(GameManager.instance.LastPath);
                        }
                    }
                    // current attack doesn't need an enemy to launch:
                    else
                    {
                        // SelectedHex is in range of attack:
                        if (HexOperations.GetDistanceBetweenHexes(GameManager.instance.SelectedUnitHex, GameManager.instance.SelectedHex)
                            <= GameManager.instance.CurrentAttack.RangeOfAttack)
                        {
                            // unselects the last path:
                            HexOperations.UnselectPath(GameManager.instance.LastPath);
                            // mark hexes whitin the range of attack as vulnerable:
                            HexOperations.UnselectRangeOfHexes(GameManager.instance.DamageRangeHexes);
                            GameManager.instance.DamageRangeHexes = HexOperations.GetHexesInDamageRange(GameManager.instance.SelectedHex,
                                                                                              GameManager.instance.CurrentAttack.DamageRange,
                                                                                              hexCells);
                            HexOperations.SelectRangeOfHexes(GameManager.instance.DamageRangeHexes, AssetManager.instance.HexEnemyToAttack);
                        }
                        // SelectedHex is out of range of attack:
                        else
                        {
                            // unselects last damage range:
                            HexOperations.UnselectRangeOfHexes(GameManager.instance.DamageRangeHexes);
                            // unselects the last path:
                            HexOperations.UnselectPath(GameManager.instance.LastPath);
                            // find a new path to the destination (with limited path length):
                            GameManager.instance.LastPath = HexOperations.FindPathUsingAStar(GameManager.instance.SelectedUnitHex,
                                                                                             GameManager.instance.SelectedHex,
                                                                                             hexCells,
                                                                                             GameManager.instance.SelectedUnit.UnitData.CurrentActionPoints);
                            HexOperations.SelectPath(GameManager.instance.LastPath);
                        }
                    }
                }
            };
        }

        // Use this for initialization
        void Start()
        {
            // set up battle arena for the first time:
            CreateHexGrid();
            AddObstacles();
            SetUpArmiesPosition();
            
            // start the battle:
            GameManager.instance.StartTheBattle();
        }

        // Update is called once per frame
        void Update()
        {
            if (GameManager.instance.ActionInProgress || GameManager.instance.GameIsPaused || GameManager.instance.MenuIsOn) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // check if mouse cursor is pointing at a hex cell:
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << LayerMask.NameToLayer("HexGrid")))
            {
                GameManager.instance.HighlightSelectedHex(hit.collider.gameObject.GetComponent<Hex>());
            }
            else
            {
                GameManager.instance.HighlightSelectedHex(null);
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
            int childCount, spaceLeft, startIndex;
            // set up an army of player 1:
            childCount = player1UnitsHolder.transform.childCount;
            spaceLeft = gridHeight - childCount;
            if (spaceLeft > childCount) startIndex = 1;
            else startIndex = 0;
            for(int y = startIndex, index = 0; y < gridHeight && index < childCount; y++, index++)
            {
                Vector3 newPosition = hexCells[0][y].transform.position;
                player1UnitsHolder.transform.GetChild(index).position = newPosition;
                if (spaceLeft > 0)
                {
                    y++;
                    spaceLeft--;
                }
            }
            // set up an army of player 2:
            childCount = player2UnitsHolder.transform.childCount;
            spaceLeft = gridHeight - childCount;
            if (spaceLeft > childCount) startIndex = 1;
            else startIndex = 0;
            for (int y = startIndex, index = 0; y < gridHeight && index < childCount; y++, index++)
            {
                Vector3 newPosition = hexCells[gridWidth - 1][y].transform.position;
                player2UnitsHolder.transform.GetChild(index).position = newPosition;
                if (spaceLeft > 0)
                {
                    y++;
                    spaceLeft--;
                }
            }
        }
        #endregion
    }
}
