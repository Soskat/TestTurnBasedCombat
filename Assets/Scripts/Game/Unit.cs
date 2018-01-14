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
                AssignedHex = collision.gameObject.GetComponent<Hex>();
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
            //foreach(Hex hex in hexPath)
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
        #endregion
    }
}
