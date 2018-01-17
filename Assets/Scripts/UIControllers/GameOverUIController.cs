using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Assertions;
using TestTurnBasedCombat.Managers;
using TestTurnBasedCombat.Game;

namespace TestTurnBasedCombat.UIControllers
{
    /// <summary>
    /// Component that manages behaviour of the game over panel UI.
    /// </summary>
    public class GameOverUIController : MonoBehaviour
    {
        #region Private fields
        /// <summary>Game over UI panel.</summary>
        [SerializeField] GameObject gameOverPanel;
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
            Assert.IsNotNull(gameOverPanel);
            Assert.IsNotNull(restartGameButton);
            Assert.IsNotNull(quitGameButton);
            Assert.IsNotNull(winnerText);
        }

        // Use this for initialization
        void Start()
        {
            // set up buttons:
            restartGameButton.onClick.AddListener(() => {
                GameManager.instance.RestartGame();
                gameOverPanel.SetActive(false);
            });
            quitGameButton.onClick.AddListener(() => { Application.Quit(); });
            // sign up for events:
            GameManager.instance.GameIsOver += (looserTag) =>
            {
                PlayerTags winnerTag = PlayerTags.Player1;
                foreach(var player in GameManager.instance.Players)
                {
                    if (player.PlayerTag != looserTag)
                    {
                        winnerTag = player.PlayerTag;
                        break;
                    }
                }
                winnerText.text = winnerTag.ToString();
                GameManager.instance.GameIsPaused = true;
                gameOverPanel.SetActive(true);
            };
            // start with hidden panel:
            gameOverPanel.SetActive(false);
        }
        #endregion
    }
}
