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
        /// <summary>List of players.</summary>
        [SerializeField] private List<Player> players;
        /// <summary>Index of current active player.</summary>
        private int playerIndex;
        /// <summary>Number of players ready to the battle.</summary>
        private int playersReadyCount;

        /// <summary>Index of current active attack on unit's attacks list.</summary>
        private int currentAttackIndex;
        #endregion


        #region Public fields & properties
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
        /// <summary>Index of current active attack on unit's attacks listUnit's current active attack.</summary>
        public Attack CurrentAttack
        {
            get
            {
                if (SelectedUnit != null) return SelectedUnit.UnitData.Attacks[currentAttackIndex];
                else return null;
            }
        }
        /// <summary>Is there any action in progress?</summary>
        public bool ActionInProgress;
        /// <summary>Last active path.</summary>
        public Hex[] LastPath;
        /// <summary>Hexes whitin range of attack.</summary>
        public Hex[] HexesInRange;
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
                ActionInProgress = false;
                // initialize players:
                InitializePlayers();
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Use this for initialization
        void Start() { }

        // Update is called once per frame
        void Update()
        {
            // got left mouse button down:
            if (Input.GetMouseButtonDown(0))
            {
                if (SelectedHex != null)
                {
                    // current attack need an enemy to launch:
                    if (CurrentAttack != null && CurrentAttack.NeedEnemyToLaunch)
                    {
                        // check if SelectedHex contains an enemy:
                        if (IsSelectedHexContainsEnemy)
                        {
                            // check if enemy is in range of attack:
                            if (HexOperations.GetDistanceBetweenHexes(SelectedUnitHex, SelectedHex) <= CurrentAttack.RangeOfAttack)
                            {
                                // attack:
                                Debug.Log(string.Format("{0} attacks {1}", SelectedUnit.gameObject.name,
                                                                           SelectedHex.OccupyingObject.gameObject.name));
                                CurrentAttack.PerformAttack(HexesInRange);
                            }
                            // enemy is out of range of attack:
                            else
                            {
                                Debug.Log(string.Format("{0} if out of range of {1}", SelectedHex.OccupyingObject.gameObject.name,
                                                                                      SelectedUnit.gameObject.name));
                                // move towards SelectedHex:
                                if (LastPath != null) StartCoroutine(SelectedUnit.Move(LastPath));
                            }
                        }
                        // there's no enemy on SelectedHex:
                        else
                        {
                            // move towards SelectedHex:
                            if (LastPath != null) StartCoroutine(SelectedUnit.Move(LastPath));
                        }
                    }
                    // current attack doesn't need an enemy to launch:
                    else
                    {
                        // check if SelectedHex is in range of attack:
                        if (HexOperations.GetDistanceBetweenHexes(SelectedUnitHex, SelectedHex) <= CurrentAttack.RangeOfAttack)
                        {
                            // attack:
                            CurrentAttack.PerformAttack(HexesInRange);
                        }
                        // SelectedHex is out of range of attack:
                        else
                        {
                            // move towards SelectedHex:
                            if (LastPath != null) StartCoroutine(SelectedUnit.Move(LastPath));
                        }
                    }
                }
            }

            // got right mouse button down:
            if (Input.GetMouseButtonDown(1))
            {
                // reset current attack to the basic one:
                currentAttackIndex = 0;
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
                if (IsSelectedHexContainsEnemy) SelectedHex.Select(AssetManager.instance.HexEnemy);
                else SelectedHex.Select();
            }
        }
        #endregion


        #region Game flow methods
        /// <summary>
        /// Prepares everything for the battle.
        /// </summary>
        public void PrepareTheBattle()
        {
#if UNITY_EDITOR
            Debug.Log("[GameManager]: START THE BATTLE");
#endif
            // things to do before loading next scene:
            playersReadyCount = 0;
            currentAttackIndex = 0;
            ActionInProgress = false;

            // load next scene:
            SceneManager.LoadScene(1); // 1 -> BattleArena
        }

        /// <summary>
        /// Starts the battle when everything is ready.
        /// </summary>
        public void ReadyForBattle()
        {
            playersReadyCount++;
            if (playersReadyCount == players.Count)
            {
                // start the battle:
                playerIndex = -1;
                EndTurn();
            }
        }

        /// <summary>
        /// Ends turn of the current player.
        /// </summary>
        public void EndTurn()
        {
            // reset CurrentAttack ref:
            currentAttackIndex = 0;
            ActionInProgress = false;
            // switch to other player's next unit:
            playerIndex = ++playerIndex % players.Count;
            if (SelectedUnit != null) SelectedUnitHex.Select(AssetManager.instance.HexIdle);
            SelectedUnit = players[playerIndex].Units.Next();
            SelectedUnit.UnitData.ResetActionPoints();
            if (SelectedUnitHex != null) SelectedUnitHex.Select(AssetManager.instance.HexSelectedUnit);
            // reset LastPath:
            HexOperations.UnselectPath(LastPath);
            LastPath = null;
#if UNITY_EDITOR
            Debug.Log(string.Format("[GameManager]: {0} turn", players[playerIndex].PlayerTag.ToString()));
#endif
        }

        /// <summary>
        /// Shows info about the winner of the battle.
        /// </summary>
        /// <param name="looser"></param>
        public void EndOfBattle(Players looser)
        {
            Debug.Log("--------------------> The looser is: " + looser.ToString());
            ActionInProgress = true;
        }
        #endregion
    }
}
