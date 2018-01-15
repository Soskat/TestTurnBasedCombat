using System;
using System.Collections.Generic;


namespace TestTurnBasedCombat.Game
{
    /// <summary>
    /// Class that holds all game configuration data.
    /// </summary>
    [Serializable]
    public class GameConfig
    {
        #region Public fields
        /// <summary>Armies specifications.</summary>
        public List<ArmySpec> ArmiesSpec;
        #endregion
    }
}
