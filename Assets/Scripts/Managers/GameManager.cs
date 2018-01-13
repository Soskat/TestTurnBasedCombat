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
        /// <summary>Current active game phase.</summary>
        [SerializeField] private GamePhase currentPhase;
        #endregion


        #region Public fields & properties
        /// <summary>Current active game phase.</summary>
        public GamePhase CurrentPhase { get { return currentPhase; } }
        /// <summary>Currently selected hex.</summary>
        public Hex SelectedHex;
        /// <summary>Currently selected unit.</summary>
        public Unit SelectedUnit;
        /// <summary>Is there any action in progress?</summary>
        public bool ActionInProgress;
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
        void Start() { }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (SelectedHex != null)
                {
                    // Debug <----------------------------------------------- remove later
                    Debug.Log("Left-clicked hex cell no. " + SelectedHex.GetCoords());
                    // /Debug <----------------------------------------------- remove later


                    // select unit
                    if (SelectedUnit == null ||
                        (SelectedHex.IsOccupied && SelectedHex.OccupyingObject.tag == "Unit" && SelectedUnit != SelectedHex.OccupyingObject.GetComponent<Unit>()))
                    {
                        if (SelectedHex.IsOccupied && SelectedHex.OccupyingObject.tag == "Unit")
                        {
                            SelectedUnit = SelectedHex.OccupyingObject.GetComponent<Unit>();
                            //Debug.Log("Change the selected unit");
                        }
                    }
                    // perform an action with selected unit:
                    else
                    {
                        // move unit to the selected hex:
                        if (!SelectedHex.IsOccupied)
                        {
                            StartCoroutine(SelectedUnit.Move(new Hex[] { SelectedHex }));
                            //Debug.Log("Move the selected");
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
