using System.Collections.Generic;
using TestTurnBasedCombat.Managers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace TestTurnBasedCombat.UIControllers
{
    /// <summary>
    /// Component that manages behaviour of the help UI panel.
    /// </summary>
    public class HelpUIController : MonoBehaviour
    {
        #region Private fields
        /// <summary>Help UI panel game object.</summary>
        [SerializeField] private GameObject helpUIPanel;
        /// <summary>Close help UI panel button.</summary>
        [SerializeField] private Button closeButton;
        /// <summary>Go to the next panel button.</summary>
        [SerializeField] private Button nextButton;
        /// <summary>Go to the previous panel button.</summary>
        [SerializeField] private Button backButton;
        /// <summary>Show help panel button.</summary>
        [SerializeField] private Button showHelpPanelButton;
        /// <summary>List of all panels.</summary>
        [SerializeField] private List<GameObject> panels;
        /// <summary>Index of the currently viewed panel.</summary>
        [SerializeField] private int currentPanelIndex;
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            Assert.IsNotNull(helpUIPanel);
            Assert.IsNotNull(closeButton);
            Assert.IsNotNull(nextButton);
            Assert.IsNotNull(backButton);
            Assert.IsNotNull(showHelpPanelButton);
        }

        // Use this for initialization
        void Start()
        {
            // set up buttons behaviour:
            showHelpPanelButton.onClick.AddListener(() =>
            {
                ShowHelpPanel();
            });
            closeButton.onClick.AddListener(() =>
            {
                // hide help panel:
                helpUIPanel.SetActive(false);
                GameManager.instance.MenuIsOn = false;
            });
            nextButton.onClick.AddListener(() =>
            {
                // switch panels:
                panels[currentPanelIndex].SetActive(false);
                currentPanelIndex++;
                panels[currentPanelIndex].SetActive(true);
                // update buttons visibility:
                backButton.gameObject.SetActive(true);
                if (currentPanelIndex == panels.Count - 1) nextButton.gameObject.SetActive(false);
            });
            backButton.onClick.AddListener(() =>
            {
                // switch panels:
                panels[currentPanelIndex].SetActive(false);
                currentPanelIndex--;
                panels[currentPanelIndex].SetActive(true);
                // update buttons visibility:
                nextButton.gameObject.SetActive(true);
                if (currentPanelIndex == 0) backButton.gameObject.SetActive(false);
            });

            // show help UI panel on start:
            ShowHelpPanel();
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Shows help UI panel.
        /// </summary>
        public void ShowHelpPanel()
        {
            foreach (var panel in panels) panel.SetActive(false);
            // show the first panel:
            currentPanelIndex = 0;
            panels[currentPanelIndex].SetActive(true);
            backButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
            // update MenuIsOn flag:
            GameManager.instance.MenuIsOn = true;
            // show help panel:
            helpUIPanel.SetActive(true);
        }
        #endregion
    }
}
