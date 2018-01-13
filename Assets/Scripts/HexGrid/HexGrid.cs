using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;


namespace TestTurnBasedCombat.HexGrid
{
    /// <summary>
    /// Component that creates the hex grid game object.
    /// </summary>
    public class HexGrid : MonoBehaviour
    {
        #region Private fields
        /// <summary>Width of the hex grid (number of hex rows).</summary>
        [SerializeField] private int gridWidth;
        /// <summary>Height of the hex grid (number of hex columns).</summary>
        [SerializeField] private int gridHeight;
        /// <summary>Prefab of the hex cell object.</summary>
        [SerializeField] private GameObject hexPrefab;
        /// <summary>Length of the hex side.</summary>
        [SerializeField] private float hexSide = 1f;
        /// <summary>Width of the hex cell.</summary>
        private float hexWidth;
        /// <summary>Height of the hex cell.</summary>
        private float hexHeight;
        /// <summary>Table of hex cells.</summary>
        private GameObject[][] hexCells;
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            Assert.IsNotNull(hexPrefab);
            // set up hexWidth and hexHeight:
            hexWidth = hexSide * Mathf.Sqrt(3f);
            hexHeight = 2 * hexSide;
        }

        // Use this for initialization
        void Start()
        {
            CreateHexGrid();
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
                    hex.transform.SetParent(transform);
                    // set up Hex component:
                    hex.GetComponent<Hex>().RowIndex = y;
                    hex.GetComponent<Hex>().ColumnIndex = x;
                    hexCells[x][y] = hex;
                }
            }
        }
        #endregion
    }
}
