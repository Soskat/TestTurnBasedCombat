using System;


namespace TestTurnBasedCombat.Game
{
    /// <summary>
    /// Represents unit specification data.
    /// </summary>
    [Serializable]
    public class UnitData
    {
        #region Public fields & properties
        /// <summary>Unit name.</summary>
        public string Name;
        /// <summary>Health points of the unit.</summary>
        public int HealthPoints;
        /// <summary>Action points of the unit.</summary>
        public int ActionPoints;
        /// <summary>Unit basic attack.</summary>
        public Attack BasicAttack;
        /// <summary>Leader (player) of the unit.</summary>
        public Players Leader;
        /// <summary>Unit prefab code.</summary>
        public string PrefabCode;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="UnitData"/> class.
        /// </summary>
        /// <param name="name">Unit name</param>
        /// <param name="hp">Unit health points</param>
        /// <param name="ap">Unit action points</param>
        /// <param name="attack">Unit basic attack</param>
        /// <param name="leader">Unit leader (player)</param>
        /// <param name="prefabCode">Unit prefab code</param>
        public UnitData(string name, int hp, int ap, Attack attack, Players leader, string prefabCode)
        {
            Name = name;
            HealthPoints = hp;
            ActionPoints = ap;
            BasicAttack = attack;
            Leader = leader;
            PrefabCode = prefabCode;
        }

        /// <summary>
        /// Copy constructor. Creates an instance of <see cref="UnitData"/> class.
        /// </summary>
        /// <param name="unit">Unit data to copy</param>
        public UnitData(UnitData unit)
        {
            Name = unit.Name;
            HealthPoints = unit.HealthPoints;
            ActionPoints = unit.ActionPoints;
            BasicAttack = unit.BasicAttack;
            Leader = unit.Leader;
            PrefabCode = unit.PrefabCode;
        }
        #endregion
    }
}
