using TestTurnBasedCombat.Managers;
using UnityEngine;


namespace TestTurnBasedCombat.Game
{
    /// <summary>
    /// Component that holds player's units game objects.
    /// </summary>
    public class PlayerArmyHolder : MonoBehaviour
    {
        #region Private fields
        /// <summary>Player tag.</summary>
        [SerializeField] private Players playerTag;
        #endregion


        #region MonoBehaviour methods
        // Use this for initialization
        void Start()
        {
            // create units game objects from player's army data:
            Player player = null;
            foreach(var item in GameManager.instance.Players)
            {
                if (item.PlayerTag == playerTag)
                {
                    player = item;
                    break;
                }
            }
            // create game objects for each unit data:
            if (player != null)
            {
                PriorityQueue<int, Unit> queue = new PriorityQueue<int, Unit>();
                foreach (var unitData in player.Army)
                {
                    GameObject go = Instantiate(Resources.Load("Units/" + unitData.PrefabCode) as GameObject);
                    go.AddComponent<Unit>();
                    Unit unit = go.GetComponent<Unit>();
                    unit.UnitData = new UnitData(unitData);
                    // elements in PriorityQUeue are sorted from lowest to highest
                    // this simple hack (priority * -1) will reverse this
                    queue.Enqueue(-unit.UnitData.ActionPoints, unit);
                    go.transform.SetParent(gameObject.transform);
                }
                // update player's Units list:
                while (!queue.IsEmpty) player.Units.Add(queue.Dequeue());
            }
        }
        #endregion
    }
}
