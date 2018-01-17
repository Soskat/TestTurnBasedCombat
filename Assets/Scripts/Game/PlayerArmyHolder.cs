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
        [SerializeField] private PlayerTags playerTag;
        #endregion


        #region MonoBehaviour methods
        // Use this for initialization
        void Start()
        {
            // sign up for actions:
            GameManager.instance.RestartGame += () =>
            {
                // remove all remaining units from winner's army:
                //foreach(var player in GameManager.instance.Players) player.Units.Clear();
                // remove all children:
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).GetComponent<Unit>().AssignedHex.SetOccupyingObject(null);
                    Destroy(transform.GetChild(i).gameObject);
                }
                // create units game objects once again:
                CreateUnitsGameObjects();

                // inform that player is ready for battle:
                GameManager.instance.PrepareForBattle();
            };

            // create units game objects:
            CreateUnitsGameObjects();
            // inform that player is ready for battle:
            GameManager.instance.PrepareForBattle();
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Creates units' game objects.
        /// </summary>
        private void CreateUnitsGameObjects()
        {
            // create units game objects from player's army data:
            Player player = null;
            foreach (var item in GameManager.instance.Players)
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
                    // create and initialize unit's game object:
                    GameObject go = Instantiate(Resources.Load("Units/" + unitData.PrefabCode) as GameObject);
                    go.AddComponent<Unit>();
                    Unit unit = go.GetComponent<Unit>();
                    unit.UnitData = new UnitData(unitData);
                    unit.IsDead += () => {
                        player.Units.Remove(unit);
                        if (player.Units.Count == 0) GameManager.instance.GameIsOver(player.PlayerTag);
                    };
                    // elements in PriorityQUeue are sorted from lowest to highest
                    // this simple hack (priority * -1) will reverse this
                    queue.Enqueue(-unit.UnitData.MaxActionPoints, unit);
                    go.transform.SetParent(gameObject.transform);
                }
                // update player's Units list:
                player.Units.Clear();
                while (!queue.IsEmpty) player.Units.Add(queue.Dequeue());
            }
        }
        #endregion
    }
}
