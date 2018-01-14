﻿using System.Collections.Generic;
using TestTurnBasedCombat.Game;
using TestTurnBasedCombat.HexGrid;
using UnityEngine;
using UnityEngine.Assertions;


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
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);

                // Debug <----------------------------------------------- remove later
                Assert.IsNotNull(SelectedUnit);
                // /Debug <----------------------------------------------- remove later





                // To do:
                // - add players to the players list and add units to their armies --------------------------------------------






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
        void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (SelectedHex != null)
                {
                    // select unit
                    if (SelectedUnit == null ||
                        (SelectedHex.IsOccupied && SelectedHex.OccupyingObject.tag == "Unit" && SelectedUnit != SelectedHex.OccupyingObject.GetComponent<Unit>()))
                    {
                        if (SelectedHex.IsOccupied && SelectedHex.OccupyingObject.tag == "Unit")
                        {
                            SelectedUnit = SelectedHex.OccupyingObject.GetComponent<Unit>();
                        }
                    }
                    // perform an action with selected unit:
                    else
                    {
                        // move unit to the selected hex:
                        if (!SelectedHex.IsOccupied)
                        {
                            // play movement animation:
                            if (LastPath != null) StartCoroutine(SelectedUnit.Move(LastPath));
                        }

                        // do other things
                        // ...
                    }
                }
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
                SelectedHex.Select();
            }
        }
        #endregion
    }
}
