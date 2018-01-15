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
        /// <summary>List of all units from player's army.</summary>
        public List<UnitData> Army;
        #endregion


        #region Constructors
        /// <summary>
        /// Default constructor of <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
            Army = new List<UnitData>();
        }
        #endregion
    }
}
