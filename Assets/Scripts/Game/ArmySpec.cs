using System;
using System.Collections.Generic;


namespace TestTurnBasedCombat.Game
{
    /// <summary>
    /// Class that represents an army specification.
    /// </summary>
    [Serializable]
    public class ArmySpec
    {
        #region Public fields
        /// <summary>Player that owns the army.</summary>
        public Players Player;
        /// <summary>Units available in this army.</summary>
        public List<UnitData> Units;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="ArmySpec"/> class.
        /// </summary>
        /// <param name="player">Player that owns the army</param>
        public ArmySpec(Players player)
        {
            Player = player;
            Units = new List<UnitData>();
        }
        #endregion
    }
}
