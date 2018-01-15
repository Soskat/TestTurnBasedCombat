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
        public Players PlayerTag;
        /// <summary>List of all units from player's army.</summary>
        public List<UnitData> Army;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="Player"/> class.
        /// </summary>
        /// <param name="tag">Player tag</param>
        public Player(Players tag)
        {
            PlayerTag = tag;
            Army = new List<UnitData>();
        }
        #endregion
    }
}
