using TestTurnBasedCombat.Managers;
using UnityEngine;


namespace TestTurnBasedCombat.HexGrid
{
    /// <summary>
    /// Component that represents a single hex cell.
    /// </summary>
    public class Hex : MonoBehaviour
    {
        #region Private fields
        /// <summary>Game object that occupies the hex cell.</summary>
        [SerializeField] private GameObject occupyingObject;
        #endregion

        #region Public fields & properties
        /// <summary>Row index of the hex cell in hex grid.</summary>
        public int RowIndex;
        /// <summary>Column index of the hex cell in hex grid.</summary>
        public int ColumnIndex;
        /// <summary>Is the hex cell occupied?</summary>
        public bool IsOccupied { get { return occupyingObject != null; } }
        /// <summary>Game object that occupies the hex cell.</summary>
        public GameObject OccupyingObject { get { return occupyingObject; } }
        #endregion


        #region MonoBehaviour methods
        // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider.
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Unit")
            {
                occupyingObject = collision.gameObject;
            }
        }

        // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider.
        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.tag == "Obstacle" || collision.gameObject.tag == "Unit")
            {
                occupyingObject = null;
            }
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Gets the hex cell offset coordinates in the hex grid.
        /// </summary>
        /// <returns>Hex cell offset coordinates</returns>
        public Vector3Int GetOffsetCoords()
        {
            return new Vector3Int(ColumnIndex, 0, RowIndex);
        }

        /// <summary>
        /// Sets the occupying game object.
        /// </summary>
        /// <param name="go">New occupying game object</param>
        public void SetOccupyingObject(GameObject go)
        {
            occupyingObject = go;
        }

        /// <summary>
        /// Selects the hex cell with the default color.
        /// </summary>
        public void Select()
        {
            GetComponent<MeshRenderer>().material = AssetManager.instance.HexCursor;
        }

        /// <summary>
        /// Selects the hex cell with given color.
        /// </summary>
        public void Select(Material mat)
        {
            GetComponent<MeshRenderer>().material = mat;
        }

        /// <summary>
        /// Unselects the hex cell.
        /// </summary>
        public void Unselect()
        {
            if (GameManager.instance.SelectedUnitHex == this) GetComponent<MeshRenderer>().material = AssetManager.instance.HexSelectedUnit;
            else GetComponent<MeshRenderer>().material = AssetManager.instance.HexIdle;
        }
        #endregion
    }
}
