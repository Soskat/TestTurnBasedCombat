using System;
using System.Collections.Generic;
using TestTurnBasedCombat.Game;
using TestTurnBasedCombat.HexGrid;
using UnityEngine;


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
        [SerializeField] private int playerIndex;
        /// <summary>Number of players ready to the battle.</summary>
        [SerializeField] private int playersReadyCount;
        /// <summary>Index of current active attack on unit's attacks list.</summary>
        [SerializeField] private int currentAttackIndex;
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
        /// <summary>Is game paused?</summary>
        public bool GameIsPaused;
        /// <summary>Last active path.</summary>
        public Hex[] LastPath;
        /// <summary>Ring of hexes around range of current attack.</summary>
        public Hex[] RangeOfAttackHexes;
        /// <summary>Hexes whitin range of attack.</summary>
        public Hex[] DamageRangeHexes;
        /// <summary>Doeas SelectedHex contain an enemy of SelectedUnit?</summary>
        public bool IsSelectedHexContainsEnemy
        {
            get
            {
                if (SelectedUnit != null && 
                    SelectedHex != null && SelectedHex.IsOccupied && 
                    SelectedHex.OccupyingObject.GetComponent<Unit>() != null)
                {
                    return SelectedUnit.UnitData.Leader != SelectedHex.OccupyingObject.GetComponent<Unit>().UnitData.Leader;
                }
                else return false;
            }
        }
        /// <summary>List of players.</summary>
        public List<Player> Players { get { return players; } }
        /// <summary>Current player.</summary>
        public Player CurrentPlayer { get { return players[playerIndex]; } }
        #region Actions
        /// <summary>Informs that units game objects have been created.</summary>
        public Action CreatedUnits { get; set; }
        /// <summary>Informs that game must be restarted.</summary>
        public Action ResetUnitsData { get; set; }
        /// <summary>Informs that game is over.</summary>
        public Action<PlayerTags> GameIsOver { get; set; }
        /// <summary>Informs that SelectedHex has changed.</summary>
        public Action UpdateSelectedHex { get; set; }
        /// <summary>Informs that SelectedUnit has changed.</summary>
        public Action UpdateSelectedUnit { get; set; }
        /// <summary>Informs that SelectedUnit action points has changed.</summary>
        public Action UpdateSelectedUnitAP { get; set; }
        #endregion
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
                GameIsPaused = false;
                CreatedUnits += () => {
#if UNITY_EDITOR
                    Debug.Log("[GM]: created units");
#endif
                };
                GameIsOver += (pt) => {
                    playersReadyCount = 0;
                    //Debug.Log("[GM]: set GameIsPaused to " + true);
                    GameIsPaused = true;
                };
                ResetUnitsData += () => {
#if UNITY_EDITOR
                    Debug.Log("[GM]: Reset units data");
#endif
                };
                UpdateSelectedUnit += () => { };
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
            // start the game:
            ResetUnitsData();
        }

        // Update is called once per frame
        void Update()
        {
            if (ActionInProgress || GameIsPaused) return;

            // left mouse button is down:
            if (Input.GetMouseButtonDown(0))
            {
                if (SelectedHex != null && CurrentAttack != null)
                {
                    // current attack need an enemy to launch:
                    if (CurrentAttack.NeedEnemyToLaunch)
                    {
                        // SelectedHex contains an enemy:
                        if (IsSelectedHexContainsEnemy)
                        {
                            // enemy is in range of attack:
                            if (HexOperations.GetDistanceBetweenHexes(SelectedUnitHex, SelectedHex) <= CurrentAttack.RangeOfAttack)
                            {
                                // attack:
                                CurrentAttack.PerformAttack(DamageRangeHexes);
                            }
                            // enemy is out of range of attack:
                            else
                            {
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
                        // SelectedHex is in range of attack:
                        if (HexOperations.GetDistanceBetweenHexes(SelectedUnitHex, SelectedHex) <= CurrentAttack.RangeOfAttack)
                        {
                            // attack:
                            CurrentAttack.PerformAttack(DamageRangeHexes);
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

            // right mouse button is down:
            if (Input.GetMouseButtonDown(1))
            {
                // reset current attack to the basic one:
                currentAttackIndex = 0;
                // unselects last damage range:
                HexOperations.UnselectRangeOfHexes(DamageRangeHexes);
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
        /// Highlights current selected hex.
        /// </summary>
        /// <param name="hex">Current selected hex</param>
        public void HighlightSelectedHex(Hex hex)
        {
            if (hex == null)
            {
                if (SelectedHex != null)
                {
                    // unselect all highlighted hexes:
                    HexOperations.UnselectRangeOfHexes(DamageRangeHexes);
                    HexOperations.UnselectPath(LastPath);
                    SelectedHex.Unselect();
                }
                SelectedHex = null;
                // inform that SelectedHex has changed:
                UpdateSelectedHex();
            }
            else if (SelectedHex != hex)
            {
                if (SelectedHex != null) SelectedHex.Unselect();
                SelectedHex = hex;
                if (IsSelectedHexContainsEnemy) SelectedHex.Select(AssetManager.instance.HexEnemy);
                else SelectedHex.Select();
                // inform that SelectedHex has changed:
                UpdateSelectedHex();
            }
        }

        /// <summary>
        /// Decrease selected unit's action points by delta AP.
        /// </summary>
        /// <param name="dAP">Delta action points</param>
        public void DecreaseActionPoint(int dAP)
        {
            if (SelectedUnit != null)
            {
                SelectedUnit.UnitData.CurrentActionPoints -= dAP;
                UpdateSelectedUnitAP();
                if (SelectedUnit.UnitData.CurrentActionPoints <= 0)
                {
                    // reset action points:
                    SelectedUnit.UnitData.ResetActionPoints();
                    // end turn if second player still has some units:
                    if (players[(playerIndex + 1) % players.Count].Units.Count > 0) EndTurn();
                }
            }
        }
        #endregion


        #region Game flow methods
        /// <summary>
        /// Sets current attack index.
        /// </summary>
        /// <param name="index">New index of current attack</param>
        public void SetCurrentAttackIndex(int index)
        {
            currentAttackIndex = index;
        }

        /// <summary>
        /// Prepares for the battle.
        /// </summary>
        public void PrepareForBattle()
        {
            playersReadyCount++;
            if (playersReadyCount == players.Count)
            {
                // inform BattleArena that all units have been created:
                CreatedUnits();
            }
        }

        /// <summary>
        /// Starts the battle.
        /// </summary>
        public void StartTheBattle()
        {
            // start the battle:
            playerIndex = -1;
            EndTurn();
        }

        /// <summary>
        /// Ends turn of the current player.
        /// </summary>
        public void EndTurn()
        {
            // reset CurrentAttack index:
            currentAttackIndex = 0;
            // reset LastPath, DamageRangeHexes and RangeOfAttackHexes:
            HexOperations.UnselectPath(LastPath);
            LastPath = null;
            HexOperations.UnselectRangeOfHexes(DamageRangeHexes);
            DamageRangeHexes = null;
            RangeOfAttackHexes = null;

            // switch to other player's next unit:
            playerIndex = ++playerIndex % players.Count;
            if (SelectedUnit != null)
            {
                SelectedUnit.UnitData.PrepareForNextTurn();
                SelectedUnitHex.Select(AssetManager.instance.HexIdle);
            }
            if (players[playerIndex].Units.Count > 0)
            {
                SelectedUnit = players[playerIndex].Units.Next();
                SelectedUnit.UnitData.ResetActionPoints();
            }
            if (SelectedUnitHex != null) SelectedUnitHex.Select(AssetManager.instance.HexSelectedUnit);
            // inform that SelectedUnit has changed:
            UpdateSelectedUnit();

            // set down all flags:
            ActionInProgress = false;
            GameIsPaused = false;
        }
        #endregion
    }
}
