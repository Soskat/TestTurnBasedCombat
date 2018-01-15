using System.Collections.Generic;
using System.IO;
using TestTurnBasedCombat.Game;
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
        /// <summary>Game configuration data.</summary>
        private GameConfig config;
        #region Materials
        /// <summary>Basic hex cell material.</summary>
        private Material hexIdle;
        /// <summary>Selected unit hex cell material.</summary>
        private Material hexSelectedUnit;
        /// <summary>Path hex cell material.</summary>
        private Material hexPath;
        /// <summary>Highlighted by cursor hex cell material.</summary>
        private Material hexCursor;
        /// <summary>Enemy hex cell material.</summary>
        private Material hexEnemy;
        #endregion
        /// <summary>Armies specification.</summary>
        private Dictionary<Players, ArmySpec> armiesSpec;
        /// <summary>Table of the neighbours directions of the hex in the hex grid in cube coordinates.</summary>
        private Vector3Int[] cubeDirections;
        #endregion


        #region Public properties
        #region Materials
        /// <summary>Basic hex cell material.</summary>
        public Material HexIdle { get { return hexIdle; } }
        /// <summary>Occupied hex cell material.</summary>
        public Material HexSelectedUnit { get { return hexSelectedUnit; } }
        /// <summary>Path hex cell material.</summary>
        public Material HexPath { get { return hexPath; } }
        /// <summary>Highlighted by cursor hex cell material.</summary>
        public Material HexCursor { get { return hexCursor; } }
        /// <summary>Enemy hex cell material.</summary>
        public Material HexEnemy { get { return hexEnemy; } }
        #endregion
        /// <summary>Armies specification.</summary>
        public Dictionary<Players, ArmySpec> ArmiesSpec { get { return armiesSpec; } }
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
                // load configuration data:
                LoadGameConfig();
                // load or set up all assets:
                hexIdle = Resources.Load("Materials/HexMaterial") as Material;
                hexSelectedUnit = Resources.Load("Materials/HexOccupiedMaterial") as Material;
                hexPath = Resources.Load("Materials/HexPathMaterial") as Material;
                hexCursor = Resources.Load("Materials/HexHighlightMaterial") as Material;
                hexEnemy = Resources.Load("Materials/HexHighlightEnemyMaterial") as Material;
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


        #region Private methods
        /// <summary>
        /// Loads game configuration data.
        /// </summary>
        private void LoadGameConfig()
        {
            config = LoadGameConfigFromFile();
            if (config == null)
            {
                // create new game configuration data:
                config = CreateNewGameConfiguration();
                SaveGameConfigToFile(config);
            }
            // create armies spec dictionary:
            armiesSpec = new Dictionary<Players, ArmySpec>();
            foreach (var item in config.ArmiesSpec) armiesSpec.Add(item.Player, item);
        }

        /// <summary>
        /// Creates new game configuration data.
        /// </summary>
        /// <returns>New game configuration data</returns>
        private GameConfig CreateNewGameConfiguration()
        {
            GameConfig newConfig = new GameConfig();
            newConfig.ArmiesSpec = CreateArmiesSpecification();
#if UNITY_EDITOR
            Debug.Log("[AssetManager]: Created new game configuration data...");
#endif
            return newConfig;
        }

        /// <summary>
        /// Creates hard-coded units specifications.
        /// </summary>
        /// <returns>Dictionary with new armies specification</returns>
        private List<ArmySpec> CreateArmiesSpecification()
        {
            List<ArmySpec> newArmiesSpec = new List<ArmySpec>();
            ArmySpec army1 = new ArmySpec(Players.Player1);
            army1.Units.Add(new UnitData("Blue Knight", 50, 5, null, Players.Player1, "soldierBlue"));
            army1.Units.Add(new UnitData("Royal archer", 40, 6, null, Players.Player1, "archerBlue"));
            army1.Units.Add(new UnitData("Mage", 30, 3, null, Players.Player1, "wizardBlue"));
            newArmiesSpec.Add(army1);
            ArmySpec army2 = new ArmySpec(Players.Player2);
            army2.Units.Add(new UnitData("Blood Mercenary", 50, 5, null, Players.Player2, "soldierRed"));
            army2.Units.Add(new UnitData("Poacher", 40, 6, null, Players.Player2, "archerRed"));
            army2.Units.Add(new UnitData("Sorcerer", 30, 3, null, Players.Player2, "wizardRed"));
            newArmiesSpec.Add(army2);
            return newArmiesSpec;
        }

        #region File IO operations methods
        /// <summary>
        /// Saves game configuration data to a file.
        /// </summary>
        /// <param name="armiesSpec">Game configuration data to save</param>
        private void SaveGameConfigToFile(GameConfig config)
        {
            string dataAsJson = JsonUtility.ToJson(config, true);
            string filePath = Application.dataPath + "/Resources/Config/game_config.json";
            File.WriteAllText(filePath, dataAsJson);
#if UNITY_EDITOR
            Debug.Log("[AssetManager]: Saved game configuration data to a file...");
#endif
        }

        /// <summary>
        /// Loads game configuration data.
        /// </summary>
        /// <returns>Game configuration data</returns>
        private GameConfig LoadGameConfigFromFile()
        {
            TextAsset file = Resources.Load("Config/game_config") as TextAsset;
            if (file != null)
            {
                GameConfig config = JsonUtility.FromJson<GameConfig>(file.text);
#if UNITY_EDITOR
                Debug.Log("[AssetManager]: Loaded game configuration data from a file...");
#endif
                return config;
            }
            else return null;
        }
        #endregion
        #endregion
    }
}
