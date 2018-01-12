using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace TestTurnBasedCombat.HexGrid
{
    /// <summary>
    /// Component that creates tha hex grid game object.
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
        /// <summary>Table of hex cells.</summary>
        private GameObject[][] hexCells;
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            Assert.IsNotNull(hexPrefab);
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
            hexCells = new GameObject[gridWidth][];
            for(int i = 0; i < gridWidth; i++)
            {
                hexCells[i] = new GameObject[gridHeight];
                for (int j = 0; j < gridHeight; j++)
                {
                    GameObject hex = Instantiate(hexPrefab) as GameObject;
                    hex.transform.Translate(i * 0.5f, j * 0.5f, 0f);
                    hex.transform.SetParent(transform);
                    hexCells[i][j] = hex;
                }
            }
        }
        #endregion
    }
}
