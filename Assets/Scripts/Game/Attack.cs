using System;
using TestTurnBasedCombat.HexGrid;


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
        /// <summary>Does this attack need selected enemy to launch?</summary>
        public bool NeedEnemyToLaunch = true;
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
            NeedEnemyToLaunch = true;
            AttackCost = 1;
            Cooldown = 1;
        }

        /// <summary>
        /// Creates an instance of <see cref="Attack"/> class.
        /// </summary>
        /// <param name="attackName">Name of the attack</param>
        /// <param name="attackInfo">Info about the attack</param>
        /// <param name="rangeOfAttack">Area where the attack can be performed</param>
        /// <param name="damageRange">Area of the attack damage</param>
        /// <param name="damage">Damage dealt by the attack</param>
        /// <param name="needEnemy">Does this attack need selected enemy to launch?</param>
        /// <param name="attackCost">Cost of the attack</param>
        /// <param name="cooldown">Number of turns of the attack cooldown</param>
        public Attack(string attackName, string attackInfo, int rangeOfAttack, int damageRange, int damage, bool needEnemy, int attackCost, int cooldown)
        {
            AttackName = attackName;
            AttackInfo = attackInfo;
            RangeOfAttack = rangeOfAttack;
            DamageRange = damageRange;
            Damage = damage;
            NeedEnemyToLaunch = needEnemy;
            AttackCost = attackCost;
            Cooldown = cooldown;
        }
        #endregion


        #region Public 
        /// <summary>
        /// Performs the attack on the given unit.
        /// </summary>
        /// <param name="unit">Unit to attack</param>
        public void PerformAttack(Hex[] range)
        {
            foreach(Hex hex in range)
            {
                if (hex.OccupyingObject != null && hex.OccupyingObject.gameObject.tag == "Unit")
                {
                    hex.OccupyingObject.GetComponent<Unit>().GotHit(Damage);
                }
            }
        }
        #endregion
    }
}
