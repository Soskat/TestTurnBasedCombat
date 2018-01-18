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
        #region Colors
        /// <summary>Color of the enabled button.</summary>
        private Color buttonEnabled;
        /// <summary>Color of the disabled button.</summary>
        private Color buttonDisabled;
        #endregion
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
        /// <summary>Enemy hex cell in range of attack material.</summary>
        private Material hexEnemyToAttack;
        #endregion
        /// <summary>Units attacks specifications.</summary>
        private Dictionary<string, Attack> attacksSpec;
        /// <summary>Armies specification.</summary>
        private Dictionary<PlayerTags, ArmySpec> armiesSpec;
        /// <summary>Table of the neighbours directions of the hex in the hex grid in cube coordinates.</summary>
        private Vector3Int[] cubeDirections;
        #endregion


        #region Public properties
        #region Colors
        /// <summary>Color of the enabled button.</summary>
        public Color ButtonEnabled { get { return buttonEnabled; } }
        /// <summary>Color of the disabled button.</summary>
        public Color ButtonDisabled { get { return buttonDisabled; } }
        #endregion
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
        /// <summary>Enemy hex cell in range of attack material.</summary>
        public Material HexEnemyToAttack { get { return hexEnemyToAttack; } }
        #endregion
        /// <summary>Armies specification.</summary>
        public Dictionary<PlayerTags, ArmySpec> ArmiesSpec { get { return armiesSpec; } }
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
                buttonEnabled = Color.white;
                buttonDisabled = new Color(0.59f, 0.59f, 0.59f);
                hexIdle = Resources.Load("Materials/HexMaterial") as Material;
                hexSelectedUnit = Resources.Load("Materials/HexOccupiedMaterial") as Material;
                hexPath = Resources.Load("Materials/HexPathMaterial") as Material;
                hexCursor = Resources.Load("Materials/HexHighlightMaterial") as Material;
                hexEnemy = Resources.Load("Materials/HexHighlightEnemyMaterial") as Material;
                hexEnemyToAttack = Resources.Load("Materials/HexHighlightEnemyToAttackMaterial") as Material;
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
            // create attacks spec dictionary:
            attacksSpec = new Dictionary<string, Attack>();
            foreach (var item in config.AttacksSpec) attacksSpec.Add(item.AttackName, item);
            // create armies spec dictionary:
            armiesSpec = new Dictionary<PlayerTags, ArmySpec>();
            foreach (var item in config.ArmiesSpec) armiesSpec.Add(item.Player, item);
        }

        /// <summary>
        /// Creates new game configuration data.
        /// </summary>
        /// <returns>New game configuration data</returns>
        private GameConfig CreateNewGameConfiguration()
        {
            GameConfig newConfig = new GameConfig();
            Dictionary<string, Attack> attacks = CreateAttacksSpecification();
            List<Attack> attacksList = new List<Attack>();
            foreach (var item in attacks.Values) attacksList.Add(item);
            newConfig.AttacksSpec = attacksList;
            newConfig.ArmiesSpec = CreateArmiesSpecification(attacks);
#if UNITY_EDITOR
            Debug.Log("[AssetManager]: Created new game configuration data...");
#endif
            return newConfig;
        }

        /// <summary>
        /// Creates attacks specifications.
        /// </summary>
        /// <returns>Dictionary with attacks specifications</returns>
        private Dictionary<string, Attack> CreateAttacksSpecification()
        {
            Dictionary<string, Attack> attacks = new Dictionary<string, Attack>();
            // soldier attacks:
            attacks.Add("Punch", new Attack("Punch", "One punch, one bruise", 1, 1, 15, true, false, 1, 0));    // basic attack
            attacks.Add("Stab", new Attack("Stab", "Meet Mr. Pointy", 1, 1, 30, true, false, 2, 2));
            attacks.Add("Hammer_punch", new Attack("Hammer_punch", "Whole ground is shaking", 1, 2, 40, false, false, 3, 3));
            // archer attacks:
            attacks.Add("Shoot", new Attack("Shoot", "An arrow to the knee", 10, 1, 10, true, false, 1, 0));    // basic attack
            attacks.Add("Grenade", new Attack("Grenade", "An egg with surprise", 4, 2, 20, false, true, 2, 3));
            attacks.Add("Rain_of_arrows", new Attack("Rain_of_arrows", "It's raining iron!", 5, 3, 30, false, true, 2, 3));
            // wizard attacks:
            attacks.Add("Magic_missle", new Attack("Magic_missle", "A shiny star of pain", 10, 1, 15, true, false, 1, 0));  // basic attack
            attacks.Add("Lightning_bolt", new Attack("Lightning_bolt", "Lightning bolt! Lightning bolt! Lightning bolt!", 10, 1, 25, true, false, 2, 2));
            attacks.Add("Meteorite", new Attack("Meteorite", "Mind your head", 7, 3, 45, false, true, 3, 4));
            return attacks;
        }

        /// <summary>
        /// Creates hard-coded units specifications.
        /// </summary>
        /// <returns>Dictionary with new armies specification</returns>
        private List<ArmySpec> CreateArmiesSpecification(Dictionary<string, Attack> attacks)
        {
            List<ArmySpec> newArmiesSpec = new List<ArmySpec>();
            // army of player 1 spec:
            ArmySpec army1 = new ArmySpec(PlayerTags.Player1);
            army1.Units.Add(new UnitData("Blue Knight",
                                         120,
                                         5,
                                         new Attack[] { attacks["Punch"], attacks["Stab"], attacks["Hammer_punch"] },
                                         PlayerTags.Player1,
                                         "soldierBlue",
                                         "soldierBlue_image"));
            army1.Units.Add(new UnitData("Royal archer",
                                         90,
                                         6,
                                         new Attack[] { attacks["Shoot"], attacks["Grenade"], attacks["Rain_of_arrows"] },
                                         PlayerTags.Player1,
                                         "archerBlue",
                                         "archerBlue_image"));
            army1.Units.Add(new UnitData("Mage",
                                         75,
                                         3,
                                         new Attack[] { attacks["Magic_missle"], attacks["Lightning_bolt"], attacks["Meteorite"] },
                                         PlayerTags.Player1,
                                         "wizardBlue",
                                         "wizardBlue_image"));
            newArmiesSpec.Add(army1);
            // army of player 2 spec:
            ArmySpec army2 = new ArmySpec(PlayerTags.Player2);
            army2.Units.Add(new UnitData("Blood Mercenary",
                                         120,
                                         5,
                                         new Attack[] { attacks["Punch"], attacks["Stab"], attacks["Hammer_punch"] },
                                         PlayerTags.Player2,
                                         "soldierRed",
                                         "soldierRed_image"));
            army2.Units.Add(new UnitData("Poacher",
                                         90,
                                         6,
                                         new Attack[] { attacks["Shoot"], attacks["Greaade"], attacks["Rain_of_arrows"] },
                                         PlayerTags.Player2,
                                         "archerRed",
                                         "archerRed_image"));
            army2.Units.Add(new UnitData("Sorcerer",
                                         75,
                                         3,
                                         new Attack[] { attacks["Magic_missle"], attacks["Lightning_bolt"], attacks["Meteorite"] },
                                         PlayerTags.Player2,
                                         "wizardRed",
                                         "wizardRed_image"));
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
