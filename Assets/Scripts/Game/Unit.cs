using UnityEngine;
using System.Collections;
using TestTurnBasedCombat.Managers;
using TestTurnBasedCombat.HexGrid;


namespace TestTurnBasedCombat.Game
{
    /// <summary>
    /// Component that represents single unit from player's army.
    /// </summary>
    public class Unit : MonoBehaviour
    {
        #region Public fields & properties
        /// <summary>Health points of the unit.</summary>
        public int HealthPoints;
        /// <summary>Action points of the unit.</summary>
        public int ActionPoints;
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
            foreach(Hex hex in hexPath)
            {
                newPos = hex.gameObject.transform.position;
                newPos.y = transform.position.y;
                transform.position = newPos;
                yield return new WaitForSeconds(0.5f);
            }
            GameManager.instance.ActionInProgress = false;
            yield return null;
        }
        #endregion
    }
}
