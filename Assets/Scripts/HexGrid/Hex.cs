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


        #region Public methods
        /// <summary>
        /// Gets the hex cell coordinates in the hex grid.
        /// </summary>
        /// <returns>Hex cell coordinates</returns>
        public Vector2 GetCoords()
        {
            return new Vector2(ColumnIndex, RowIndex);
        }


        public void Select()
        {
            GetComponent<MeshRenderer>().material = AssetManager.instance.HexHighlight;
        }

        public void Unselect()
        {
            // uncomment later:
            //GetComponent<MeshRenderer>().material = AssetManager.instance.HexIdle;

            // Debug <----------------------------------------------- remove later
            if (!IsOccupied) GetComponent<MeshRenderer>().material = AssetManager.instance.HexIdle;
            else GetComponent<MeshRenderer>().material = AssetManager.instance.HexOccupied;
            // /Debug <----------------------------------------------- remove later
        }
        #endregion
    }
}
