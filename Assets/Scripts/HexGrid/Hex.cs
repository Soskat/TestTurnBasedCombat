using TestTurnBasedCombat.Managers;
using UnityEngine;


namespace TestTurnBasedCombat.HexGrid
{
    /// <summary>
    /// Component that represents a single hex cell.
    /// </summary>
    public class Hex : MonoBehaviour
    {
        #region Public fields
        /// <summary>Row index of the hex cell in hex grid.</summary>
        public int RowIndex;
        /// <summary>Column index of the hex cell in hex grid.</summary>
        public int ColumnIndex;
        /// <summary>Is the hex cell occupied?</summary>
        public bool IsOccupied;
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
            if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Unit")
            {
                IsOccupied = true;

                // Debug <----------------------------------------------- remove later
                GetComponent<MeshRenderer>().material = AssetManager.instance.HexOccupied;
                // /Debug <----------------------------------------------- remove later
            }
        }

        // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Unit")
            {
                IsOccupied = false;

                // Debug <----------------------------------------------- remove later
                GetComponent<MeshRenderer>().material = AssetManager.instance.HexIdle;
                // /Debug <----------------------------------------------- remove later
            }
        }
        #endregion
    }
}
