using System.Collections;
using System.Collections.Generic;
using TestTurnBasedCombat.Game;
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
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }




        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        #endregion
    }
}
