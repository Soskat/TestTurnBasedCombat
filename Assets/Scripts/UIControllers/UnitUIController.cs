﻿using System.Collections.Generic;
using TestTurnBasedCombat.Game;
using TestTurnBasedCombat.Managers;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;


namespace TestTurnBasedCombat.UIControllers
{
    /// <summary>
    /// Component that manages behaviour of the unit UI.
    /// </summary>
    public class UnitUIController : MonoBehaviour
    {
        #region Private fields
        /// <summary>Unit UI panel.</summary>
        [SerializeField] private PlayerTags playerTag;
        /// <summary>Unit UI panel.</summary>
        [SerializeField] private GameObject unitUIPanel;
        /// <summary>Current action points label.</summary>
        [SerializeField] private Text actionPointsText;
        /// <summary>Image of the unit.</summary>
        [SerializeField] private RawImage unitImage;
        /// <summary>End turn button.</summary>
        [SerializeField] private Button endTurnButton;
        /// <summary>Attack 1 button.</summary>
        [SerializeField] private List<Button> attackButtons;
        #endregion


        #region MonoBehaviour methods
        // Awake is called when the script instance is being loaded.
        private void Awake()
        {
            Assert.IsNotNull(unitUIPanel);
            Assert.IsNotNull(actionPointsText);
            Assert.IsNotNull(unitImage);
            Assert.IsNotNull(endTurnButton);
            Assert.IsNotNull(attackButtons);
        }

        // Use this for initialization
        void Start()
        {
            UpdateUI();
            endTurnButton.onClick.AddListener(GameManager.instance.EndTurn);
            // sign up for events:
            GameManager.instance.GameIsOver += (pt) =>
            {
                unitUIPanel.SetActive(false);
            };
            GameManager.instance.UpdateSelectedUnit += () =>
            {
                UpdateUI();
            };
            GameManager.instance.UpdateSelectedUnitAP += () =>
            {
                UpdateUnitAPUI();
            };
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Updates UI.
        /// </summary>
        private void UpdateUI()
        {
            if (GameManager.instance.CurrentPlayer.PlayerTag == playerTag)
            {
                // update selected unit info:
                if (GameManager.instance.SelectedUnit.UnitData.Image != null)
                {
                    unitImage.texture = GameManager.instance.SelectedUnit.UnitData.Image;
                }
                else unitImage.texture = Resources.Load("Images/placeholder") as Texture2D;
                // reset attacks buttons:
                foreach (var attackButton in attackButtons) attackButton.gameObject.SetActive(false);
                UpdateUnitAPUI();
                unitUIPanel.SetActive(true);
            }
            else
            {
                unitUIPanel.SetActive(false);
            }
        }


        /// <summary>
        /// Updates unit's action points UI.
        /// </summary>
        private void UpdateUnitAPUI()
        {
            // update AP label:
            actionPointsText.text = GameManager.instance.SelectedUnit.UnitData.CurrentActionPoints.ToString();
            // update attacks buttons:
            for (int i = 1, j = 0; j < attackButtons.Count && i < GameManager.instance.SelectedUnit.UnitData.Attacks.Length; j++, i++)
            {
                Attack attack = GameManager.instance.SelectedUnit.UnitData.Attacks[i];
                if (attack.TurnsLeft > 0)
                {
                    attackButtons[j].GetComponentInChildren<Text>().text = attack.AttackName.Replace("_", " ") + string.Format(" ({0})", attack.TurnsLeft);
                    attackButtons[j].enabled = false;
                    attackButtons[j].GetComponent<Image>().color = AssetManager.instance.ButtonDisabled;
                }
                else
                {
                    attackButtons[j].GetComponentInChildren<Text>().text = attack.AttackName.Replace("_", " ");
                    if (attack.AttackCost > GameManager.instance.SelectedUnit.UnitData.CurrentActionPoints)
                    {
                        attackButtons[j].enabled = false;
                        attackButtons[j].GetComponent<Image>().color = AssetManager.instance.ButtonDisabled;
                    }
                    else
                    {
                        attackButtons[j].enabled = true;
                        attackButtons[j].GetComponent<Image>().color = AssetManager.instance.ButtonEnabled;
                        attackButtons[j].onClick.RemoveAllListeners();
                        int index = i;
                        attackButtons[j].onClick.AddListener(() => {
                            GameManager.instance.SetCurrentAttackIndex(index);
                        });
                    }
                }
                attackButtons[j].gameObject.SetActive(true);
            }
        }
        #endregion
    }
}
