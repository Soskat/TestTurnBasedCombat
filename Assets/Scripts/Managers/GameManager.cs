﻿using System.Collections.Generic;
using TestTurnBasedCombat.Game;
using TestTurnBasedCombat.HexGrid;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace TestTurnBasedCombat.Managers
{
    /// <summary>
    /// Comonent that manages main game logic.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Public static fields
        /// <summary>Reference to the active <see cref="GameManager"/> class.</summary>
        public static GameManager instance;
        #endregion


        #region Private fields
        /// <summary>Current active game phase.</summary>
        [SerializeField] private GamePhase currentPhase;
        /// <summary>List of players.</summary>
        [SerializeField] private List<Player> players;
        #endregion


        #region Public fields & properties
        /// <summary>Current active game phase.</summary>
        public GamePhase CurrentPhase { get { return currentPhase; } }
        /// <summary>Currently selected hex.</summary>
        public Hex SelectedHex;
        /// <summary>Currently selected unit.</summary>
        public Unit SelectedUnit;
        /// <summary>Hex of currently selected hex.</summary>
        public Hex SelectedUnitHex
        {
            get
            {
                if (SelectedUnit != null) return SelectedUnit.AssignedHex;
                else return null;
            }
        }
        /// <summary>Is there any action in progress?</summary>
        public bool ActionInProgress;
        /// <summary>Last active path.</summary>
        public Hex[] LastPath;
        /// <summary>Doeas SelectedHex contain an enemy of SelectedUnit?</summary>
        public bool IsSelectedHexContainsEnemy
        {
            get
            {
                if (SelectedUnit != null && 
                    SelectedHex != null && SelectedHex.OccupyingObject != null && 
                    SelectedHex.OccupyingObject.GetComponent<Unit>() != null)
                {
                    return SelectedUnit.UnitData.Leader != SelectedHex.OccupyingObject.GetComponent<Unit>().UnitData.Leader;
                }
                else return false;
            }
        }
        /// <summary>List of players.</summary>
        public List<Player> Players { get { return players; } }
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                // initialize all things:
                currentPhase = GamePhase.StartOfGame;
                ActionInProgress = false;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Use this for initialization
        void Start()
        {
            // initialize players:
            InitializePlayers();
        }

            // Update is called once per frame
            void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (SelectedHex != null)
                {
                    // SelectedHex contains an unit:
                    if (SelectedHex.IsOccupied &&
                        SelectedHex.OccupyingObject.tag == "Unit" &&
                        SelectedUnit != SelectedHex.OccupyingObject.GetComponent<Unit>())
                    {
                        // unit is an ally - update SelectedUnit:
                        if (!IsSelectedHexContainsEnemy)
                        {
                            SelectedUnit = SelectedHex.OccupyingObject.GetComponent<Unit>();
                        }
                        // unit is an enemy - attack:
                        else
                        {
                            // check if can perform an attack:
                            // ...

                            // if so, attack:
                            // ...
                        }
                    }
                    // SelectedHex is unoccupieed:
                    else
                    {
                        // move the unit:

                        Debug.Log("Distance: " + BattleArena.GetDistanceBetweenHexes(SelectedHex, SelectedUnitHex));

                        // play movement animation:
                        if (LastPath != null) StartCoroutine(SelectedUnit.Move(LastPath));
                    }
                }
            }
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Initialize players.
        /// </summary>
        private void InitializePlayers()
        {
            players = new List<Player>();
            foreach(var value in AssetManager.instance.ArmiesSpec.Values)
            {
                Player newPlayer = new Player(value.Player);
                foreach (var item in value.Units) newPlayer.Army.Add(item);
                players.Add(newPlayer);
            }
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Save info about current selected hex cell.
        /// </summary>
        /// <param name="hex">Current selected hex</param>
        public void SelectHexCell(Hex hex)
        {
            if (hex == null)
            {
                if (SelectedHex != null) SelectedHex.Unselect();
                SelectedHex = null;
            }
            else if (SelectedHex != hex)
            {
                if (SelectedHex != null) SelectedHex.Unselect();
                SelectedHex = hex;
                if (IsSelectedHexContainsEnemy) SelectedHex.Select(AssetManager.instance.HexEnemyHighlight);
                else SelectedHex.Select();
            }
        }

        /// <summary>
        /// Starts the battle.
        /// </summary>
        public void StartTheBattle()
        {
#if UNITY_EDITOR
            Debug.Log("START THE BATTLE");
#endif
            // things to do before loading next scene:
            // ...

            // load next scene:
            SceneManager.LoadScene(1); // 1 -> BattleArena
        }
        #endregion
    }
}
