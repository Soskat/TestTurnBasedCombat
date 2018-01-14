using System;
using UnityEngine;


namespace TestTurnBasedCombat.Game
{
    /// <summary>
    /// Component that represents single attack.
    /// </summary>
    [Serializable]
    public class Attack
    {
        #region Public fields & properties
        /// <summary>Name of the attack.</summary>
        public string AttackName = "Single hit";
        /// <summary>Info about the attack.</summary>
        public string AttackInfo = "Performs a single melee hit";
        /// <summary>Area where the attack can be performed.</summary>
        public int RangeOfAttack = 1;
        /// <summary>Area of the attack damage.</summary>
        public int DamageRange = 1;
        /// <summary>Damage dealt by the attack.</summary>
        public int Damage = 15;
        /// <summary>Cost of the attack.</summary>
        public int AttackCost = 1;
        /// <summary>Number of turns of the attack cooldown.</summary>
        public int Cooldown = 1;
        #endregion


        #region Constructors
        /// <summary>
        /// Default constructor of <see cref="Attack"/> class.
        /// </summary>
        public Attack()
        {
            AttackName = "Single hit";
            AttackInfo = "Performs a single melee hit";
            RangeOfAttack = 1;
            DamageRange = 1;
            Damage = 15;
            AttackCost = 1;
            Cooldown = 1;
        }
        #endregion


        #region Public 
        /// <summary>
        /// Performs the attack on the given unit.
        /// </summary>
        /// <param name="unit">Unit to attack</param>
        public void PerformAttack(Unit unit)
        {
            Debug.Log("Do the " + AttackName);
        }
        #endregion
    }
}
