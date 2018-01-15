using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        public List<Unit> Army;
        #endregion


        #region Constructors
        /// <summary>
        /// Default constructor of <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
            Army = new List<Unit>();
        }
        #endregion
    }
}
