using TestTurnBasedCombat.Game;
using TestTurnBasedCombat.Managers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace TestTurnBasedCombat.UIControllers
{
    /// <summary>
    /// Component that manages behaviour of the all of game menu UI panels.
    /// </summary>
    public class MenuUIController : MonoBehaviour
    {
        #region Private fields
        /// <summary>In-game menu UI panel.</summary>
        [SerializeField] private GameObject inGameMenu;
        /// <summary>Resume game button.</summary>
        [SerializeField] private Button resumeGameButton;
        /// <summary>Quit game button.</summary>
        [SerializeField] private Button quitGameMenuButton;
        /// <summary>Game over UI panel.</summary>
        [SerializeField] private GameObject gameOverPanel;
        /// <summary>Restart game button.</summary>
        [SerializeField] private Button restartGameButton;
        /// <summary>Quit game button.</summary>
        [SerializeField] private Button quitGameButton;
        /// <summary>Winner text.</summary>
        [SerializeField] private Text winnerText;
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            Assert.IsNotNull(inGameMenu);
            Assert.IsNotNull(resumeGameButton);
            Assert.IsNotNull(quitGameMenuButton);
            Assert.IsNotNull(gameOverPanel);
            Assert.IsNotNull(restartGameButton);
            Assert.IsNotNull(quitGameButton);
            Assert.IsNotNull(winnerText);
        }

        // Use this for initialization
        void Start()
        {
            #region MIn-game menu UI panel
            // set up buttons:
            resumeGameButton.onClick.AddListener(() => {
                inGameMenu.SetActive(false);
                GameManager.instance.MenuIsOn = false;
            });
            quitGameMenuButton.onClick.AddListener(() => { Application.Quit(); });
            // start with hidden panel:
            inGameMenu.SetActive(false);
            #endregion

            #region Game over UI panel
            // set up buttons:
            restartGameButton.onClick.AddListener(() => {
                GameManager.instance.ResetUnitsData();
                gameOverPanel.SetActive(false);
            });
            quitGameButton.onClick.AddListener(() => { Application.Quit(); });
            // sign up for events:
            GameManager.instance.GameIsOver += (looserTag) =>
            {
                PlayerTags winnerTag = PlayerTags.Player1;
                foreach (var player in GameManager.instance.Players)
                {
                    if (player.PlayerTag != looserTag)
                    {
                        winnerTag = player.PlayerTag;
                        break;
                    }
                }
                winnerText.text = winnerTag.ToString();
                gameOverPanel.SetActive(true);
            };
            // start with hidden panel:
            gameOverPanel.SetActive(false);
            #endregion
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.instance.MenuIsOn = GameManager.instance.MenuIsOn ? false : true;
                if (GameManager.instance.MenuIsOn) inGameMenu.SetActive(true);
                else inGameMenu.SetActive(false);
            }
        }
        #endregion
    }
}
