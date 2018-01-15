﻿using System.Collections;
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
        #endregion


        #region MonoBehaviour methods
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

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
            for (int i = 1; i < hexPath.Length; i++)
            {
                yield return new WaitForSeconds(0.1f);
                newPos = hexPath[i].gameObject.transform.position;
                newPos.y = transform.position.y;
                transform.position = newPos;
            }
            GameManager.instance.ActionInProgress = false;
            yield return null;
        }

        /// <summary>
        /// Deal with the damage.
        /// </summary>
        /// <param name="damage">Damage that unit has received</param>
        /// <returns></returns>
        public IEnumerator GotHit(int damage)
        {
            GameManager.instance.ActionInProgress = true;
            // animate the unit reaction to being hit:
            gameObject.transform.localScale = Vector3.one * 0.9f;
            yield return new WaitForSeconds(0.2f);
            gameObject.transform.localScale = Vector3.one;
            yield return new WaitForSeconds(0.1f);
            // calculate received damage:
            UnitData.HealthPoints -= damage;
            UnitData.HealthPoints = (UnitData.HealthPoints < 0) ? 0 : UnitData.HealthPoints;
            // unit is dead:
            if (UnitData.HealthPoints == 0) StartCoroutine(Die());
            GameManager.instance.ActionInProgress = false;
            yield return null;
        }

        /// <summary>
        /// Deal with the unit death.
        /// </summary>
        /// <returns></returns>
        public IEnumerator Die()
        {
            GameManager.instance.ActionInProgress = true;
            // animate unit death:
            for (float i = 1f; i > 0.0f; i -= 0.1f)
            {
                gameObject.transform.localScale = Vector3.one * i;
                yield return new WaitForSeconds(0.01f);
            }
            // remove this unit from player's army:
            // ...
            GameManager.instance.ActionInProgress = false;
            // destroy this game object:
            Destroy(gameObject);
        }
        #endregion
    }
}
