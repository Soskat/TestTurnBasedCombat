using System;
using TestTurnBasedCombat.HexGrid;
using TestTurnBasedCombat.Managers;


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
        /// <summary>Can hurt yourself with this attack?</summary>
        public bool AutoDamageOn = false;
        /// <summary>Cost of the attack.</summary>
        public int AttackCost = 1;
        /// <summary>Number of turns of the attack cooldown.</summary>
        public int Cooldown = 1;
        /// <summary>Turns left to cooldown the attack.</summary>
        public int TurnsLeft = 0;
        #endregion


        #region Constructors
        /// <summary>
        /// Default constructor of <see cref="Attack"/> class.
        /// </summary>
        public Attack()
        {
            AttackName = "Attack";
            AttackInfo = "Attack info";
            RangeOfAttack = 1;
            DamageRange = 1;
            Damage = 15;
            NeedEnemyToLaunch = true;
            AutoDamageOn = false;
            AttackCost = 1;
            Cooldown = 1;
            TurnsLeft = 0;
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
        /// <param name="autoDamage">Can hurt yourself with this attack?</param>
        /// <param name="attackCost">Cost of the attack</param>
        /// <param name="cooldown">Number of turns of the attack cooldown</param>
        public Attack(string attackName,
                      string attackInfo,
                      int rangeOfAttack,
                      int damageRange,
                      int damage,
                      bool needEnemy,
                      bool autoDamage,
                      int attackCost,
                      int cooldown)
        {
            AttackName = attackName;
            AttackInfo = attackInfo;
            RangeOfAttack = rangeOfAttack;
            DamageRange = damageRange;
            Damage = damage;
            NeedEnemyToLaunch = needEnemy;
            AutoDamageOn = autoDamage;
            AttackCost = attackCost;
            Cooldown = cooldown;
            TurnsLeft = 0;
        }
        #endregion


        #region Public 
        /// <summary>
        /// Performs the attack on the given unit.
        /// </summary>
        /// <param name="unit">Unit to attack</param>
        public void PerformAttack(Hex[] range)
        {
            if (range == null) return;

            foreach(Hex hex in range)
            {
                if (hex.OccupyingObject != null && hex.OccupyingObject.gameObject.tag == "Unit")
                {
                    // if SelectedUnit is in damage range, but auto-damage is off, skip:
                    if (GameManager.instance.SelectedUnit == hex.OccupyingObject.GetComponent<Unit>() && !AutoDamageOn) continue;
                    // calculate damage:
                    hex.OccupyingObject.GetComponent<Unit>().GotHit(Damage);
                }
            }
            // set up turns left (attack cooldown counter):
            TurnsLeft = Cooldown;
            // reset current attack index:
            GameManager.instance.SetCurrentAttackIndex(0);
            // unselects last damage range:
            HexOperations.UnselectRangeOfHexes(GameManager.instance.DamageRangeHexes);
            // restore highlight of the SelectedHex:
            GameManager.instance.HighlightSelectedHex(GameManager.instance.SelectedHex);
            // update unit's current action points number:
            GameManager.instance.DecreaseActionPoint(AttackCost);
        }
        #endregion
    }
}
