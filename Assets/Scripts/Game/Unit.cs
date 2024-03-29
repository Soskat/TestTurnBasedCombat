﻿using System;
using System.Collections;
using TestTurnBasedCombat.HexGrid;
using TestTurnBasedCombat.Managers;
using UnityEngine;


namespace TestTurnBasedCombat.Game
{
    /// <summary>
    /// Component that represents single unit from player's army.
    /// </summary>
    public class Unit : MonoBehaviour
    {
        #region Public fields & properties
        /// <summary>The unit specification.</summary>
        public UnitData UnitData;
        /// <summary>The hex cell that unit stands on.</summary>
        public Hex AssignedHex;
        /// <summary>Informs that the unit is dead.</summary>
        public Action IsDead { get; set; }
        #endregion


        #region MonoBehaviour methods
        // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Hex")
            {
                if (GameManager.instance.SelectedUnit == this)
                {
                    if (AssignedHex != null) AssignedHex.Select(AssetManager.instance.HexIdle);
                    AssignedHex = collision.gameObject.GetComponent<Hex>();
                    AssignedHex.Select(AssetManager.instance.HexSelectedUnit);
                }
                else AssignedHex = collision.gameObject.GetComponent<Hex>();
            }
        }

        // OnGUI is called for rendering and handling GUI events.
        // Source: Unit.cs from project HexGridby Daniel Carrier (https://www.assetstore.unity3d.com/en/#!/content/27440)
        private void OnGUI()
        {
            if (GameManager.instance.GameIsPaused || GameManager.instance.MenuIsOn) return;

            int hpBarWidth = 50;
            int hpBarHeight = 15;
            Vector3 coordinates = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, -1.5f, 0) + 0.5f * Camera.main.transform.up);
            coordinates.y = Screen.height - coordinates.y;
            // set up colors:
            Texture2D green = new Texture2D(1, 1);
            green.SetPixel(0, 0, new Color(0f, 0.85f, 0.13f));
            green.wrapMode = TextureWrapMode.Repeat;
            green.Apply();
            Texture2D darkRed = new Texture2D(1, 1);
            darkRed.SetPixel(0, 0, new Color(0.9f, 0f, 0f));
            darkRed.wrapMode = TextureWrapMode.Repeat;
            darkRed.Apply();
            // draw unit's health bar:
            GUI.DrawTexture(new Rect(coordinates.x - hpBarWidth / 2f,
                                     coordinates.y + hpBarHeight / 2f,
                                     hpBarWidth,
                                     hpBarHeight),
                            darkRed);
            GUI.DrawTexture(new Rect(coordinates.x - hpBarWidth / 2f,
                                     coordinates.y + hpBarHeight / 2f,
                                     hpBarWidth * UnitData.CurrentHealthPoints / UnitData.MaxHealthPoints,
                                     hpBarHeight),
                            green);
            GUIStyle centered = new GUIStyle();
            centered.alignment = TextAnchor.MiddleCenter;
            GUI.Label(new Rect(coordinates.x - hpBarWidth / 2f,
                               coordinates.y + hpBarHeight / 2f,
                               hpBarWidth,
                               hpBarHeight),
                      UnitData.CurrentHealthPoints.ToString(),
                      centered);
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Move the unit along the given path.
        /// </summary>
        /// <param name="hexPath">Path</param>
        /// <returns></returns>
        public IEnumerator Move(Hex[] hexPath)
        {
            GameManager.instance.ActionInProgress = true;
            Vector3 newPos;
            for (int i = 0; i < hexPath.Length; i++)
            {
                yield return new WaitForSeconds(0.1f);
                newPos = hexPath[i].gameObject.transform.position;
                newPos.y = transform.position.y;
                transform.position = newPos;
                // update unit's current action points number:
                GameManager.instance.DecreaseActionPoint(1);
            }
            GameManager.instance.ActionInProgress = false;
            yield return null;
        }

        /// <summary>
        /// Deal with the damage.
        /// </summary>
        /// <param name="damage">Damage that unit has received</param>
        public void GotHit(int damage)
        {
            // calculate received damage:
            UnitData.CurrentHealthPoints -= damage;
            UnitData.CurrentHealthPoints = (UnitData.CurrentHealthPoints < 0) ? 0 : UnitData.CurrentHealthPoints;
            // unit is dead:
            if (UnitData.CurrentHealthPoints == 0)
            {
                // remove this unit from player's army:
                IsDead();
                // destroy this game object:
                Destroy(gameObject);
            }
        }
        #endregion
    }
}
