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
        /// <summary>Highlighted hex cell material.</summary>
        private Material hexHighlight;
        /// <summary>Highlighted hex cell with an enemy material.</summary>
        private Material hexEnemyHighlight;
        #endregion


        #region Public properties
        /// <summary>Basic hex cell material.</summary>
        public Material HexIdle { get { return hexIdle; } }
        /// <summary>Occupied hex cell material.</summary>
        public Material HexOccupied { get { return hexOccupied; } }
        /// <summary>Highlighted hex cell material.</summary>
        public Material HexHighlight { get { return hexHighlight; } }
        /// <summary>Highlighted hex cell with an enemy material.</summary>
        public Material HexEnemyHighlight { get { return hexEnemyHighlight; } }
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                // load all assets:
                hexIdle = Resources.Load("Materials/HexMaterial") as Material;
                hexOccupied = Resources.Load("Materials/HexOccupiedMaterial") as Material;
                hexHighlight = Resources.Load("Materials/HexHighlightMaterial") as Material;
                hexEnemyHighlight = Resources.Load("Materials/HexHighlightEnemyMaterial") as Material;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
