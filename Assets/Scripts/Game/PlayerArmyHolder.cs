using System.Collections.Generic;
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
        /// <summary>Player object.</summary>
        [SerializeField] private Player player;
        #endregion


        #region MonoBehaviour methods
        // Use this for initialization
        void Start()
        {
            // sign up for actions:
            GameManager.instance.ResetUnitsData += () =>
            {
                // remove all remaining units from player's army:
                //player.Units.Clear();
                //foreach (var player in GameManager.instance.Players) player.Units.Clear();
                // remove all children from transform:
                var children = new List<GameObject>();
                foreach (Transform child in transform) children.Add(child.gameObject);
                transform.DetachChildren();
                foreach(GameObject child in children) Destroy(child);
                // preapare units for battle:
                PrepareUnitsForBattle();
            };

            player = null;
            // preapare units for battle:
            PrepareUnitsForBattle();
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Prepares units for the battle.
        /// </summary>
        private void PrepareUnitsForBattle()
        {
            // create units game objects:
            CreateUnitsGameObjects();
            // inform that player is ready for battle:
            GameManager.instance.PrepareForBattle();
        }

        /// <summary>
        /// Creates game objects for units.
        /// </summary>
        private void CreateUnitsGameObjects()
        {
            // get reference to assigned player:
            if (player == null)
            {
                foreach (var item in GameManager.instance.Players)
                {
                    if (item.PlayerTag == playerTag)
                    {
                        player = item;
                        break;
                    }
                }
            }
            // create game object for each unit data:
            //if (player != null)
            {
                PriorityQueue<int, Unit> queue = new PriorityQueue<int, Unit>();
                foreach (var unitData in player.Army)
                {
                    // create and initialize unit's game object:
                    GameObject go = Instantiate(Resources.Load("Units/" + unitData.PrefabCode) as GameObject);
                    go.AddComponent<Unit>();
                    Unit unit = go.GetComponent<Unit>();
                    // reset attacks:
                    foreach (var attack in unitData.Attacks) attack.TurnsLeft = 0;
                    unit.UnitData = new UnitData(unitData);
                    unit.IsDead += () => {
                        player.Units.Remove(unit);
                        if (player.Units.Count == 0)
                        {
                            //Debug.Log("PAH: I am ending this GAME!");
                            // end game if all units from player's army are dead:
                            GameManager.instance.GameIsOver(player.PlayerTag);
                        }
                        else
                        {
                            // end turn if SelectedUnit just died:
                            if (GameManager.instance.SelectedUnit == unit)
                            {
                                //Debug.Log("PAH: I am ending this turn!");
                                GameManager.instance.EndTurn();
                            }
                        }
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
