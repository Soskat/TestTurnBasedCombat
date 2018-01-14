using UnityEngine;


namespace TestTurnBasedCombat.Managers
{
    /// <summary>
    /// Component that manages all of the game assets.
    /// </summary>
    public class AssetManager : MonoBehaviour
    {
        #region Public static fields
        /// <summary>Reference to the active <see cref="GameManager"/> class.</summary>
        public static AssetManager instance;
        #endregion


        #region Private fields
        /// <summary>Basic hex cell material.</summary>
        private Material hexIdle;
        /// <summary>Occupied hex cell material.</summary>
        private Material hexOccupied;
        /// <summary>Path hex cell material.</summary>
        private Material hexPath;
        /// <summary>Highlighted hex cell material.</summary>
        private Material hexHighlight;
        /// <summary>Highlighted hex cell with an enemy material.</summary>
        private Material hexEnemyHighlight;
        /// <summary>Table of the neighbours directions of the hex in the hex grid in cube coordinates.</summary>
        private Vector3Int[] cubeDirections;
        #endregion


        #region Public properties
        /// <summary>Basic hex cell material.</summary>
        public Material HexIdle { get { return hexIdle; } }
        /// <summary>Occupied hex cell material.</summary>
        public Material HexOccupied { get { return hexOccupied; } }
        /// <summary>Path hex cell material.</summary>
        public Material HexPath { get { return hexPath; } }
        /// <summary>Highlighted hex cell material.</summary>
        public Material HexHighlight { get { return hexHighlight; } }
        /// <summary>Highlighted hex cell with an enemy material.</summary>
        public Material HexEnemyHighlight { get { return hexEnemyHighlight; } }
        /// <summary>Table of the neighbours directions of the hex in the hex grid in cube coordinates.</summary>
        public Vector3Int[] CubeDirections { get { return cubeDirections; } }
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                // load or set up all assets:
                hexIdle = Resources.Load("Materials/HexMaterial") as Material;
                hexOccupied = Resources.Load("Materials/HexOccupiedMaterial") as Material;
                hexPath = Resources.Load("Materials/HexPathMaterial") as Material;
                hexHighlight = Resources.Load("Materials/HexHighlightMaterial") as Material;
                hexEnemyHighlight = Resources.Load("Materials/HexHighlightEnemyMaterial") as Material;
                cubeDirections = new Vector3Int[] { new Vector3Int(+1, -1, 0),
                                                    new Vector3Int(+1, 0, -1),
                                                    new Vector3Int(0, +1, -1),
                                                    new Vector3Int(-1, +1, 0),
                                                    new Vector3Int(-1, 0, +1),
                                                    new Vector3Int(0, -1, +1)};
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
