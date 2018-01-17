using System;
using System.Collections.Generic;


namespace TestTurnBasedCombat.Game
{
    /// <summary>
    /// Class that represents the player.
    /// </summary>
    [Serializable]
    public class Player
    {
        #region Public fields & properties
        /// <summary>Player tag.</summary>
        public PlayerTags PlayerTag;
        /// <summary>List of all units data from player's army.</summary>
        public List<UnitData> Army;
        /// <summary>Circular queue with all units on the battle arena.</summary>
        public CicrularQueue<Unit> Units;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="Player"/> class.
        /// </summary>
        /// <param name="tag">Player tag</param>
        public Player(PlayerTags tag)
        {
            PlayerTag = tag;
            Army = new List<UnitData>();
            Units = new CicrularQueue<Unit>();
        }
        #endregion
    }
}
