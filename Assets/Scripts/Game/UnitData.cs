using System;
using UnityEngine;


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
        /// <summary>Max health points of the unit.</summary>
        public int MaxHealthPoints;
        /// <summary>Current health points of the unit.</summary>
        public int CurrentHealthPoints;
        /// <summary>Max action points of the unit.</summary>
        public int MaxActionPoints;
        /// <summary>Current action points of the unit.</summary>
        public int CurrentActionPoints;
        /// <summary>List of unit's attacks.</summary>
        public Attack[] Attacks;
        /// <summary>Leader (player) of the unit.</summary>
        public PlayerTags Leader;
        /// <summary>Unit prefab code.</summary>
        public string PrefabCode;
        /// <summary>Unit image code.</summary>
        public string ImageCode;
        /// <summary>Unit image.</summary>
        public Texture2D Image;
        #endregion


        #region Constructors
        /// <summary>
        /// Creates an instance of <see cref="UnitData"/> class.
        /// </summary>
        /// <param name="name">Unit name</param>
        /// <param name="hp">Unit health points</param>
        /// <param name="ap">Unit action points</param>
        /// <param name="attack">List of unit's attacks</param>
        /// <param name="leader">Unit leader (player)</param>
        /// <param name="prefabCode">Unit prefab code</param>
        /// <param name="prefabCode">Unit image code</param>
        public UnitData(string name, int hp, int ap, Attack[] attack, PlayerTags leader, string prefabCode, string imageCode)
        {
            Name = name;
            MaxHealthPoints = hp;
            CurrentHealthPoints = MaxHealthPoints;
            MaxActionPoints = ap;
            CurrentActionPoints = MaxActionPoints;
            Attacks = attack;
            Leader = leader;
            PrefabCode = prefabCode;
            ImageCode = imageCode;
            Image = Resources.Load("Images/" + ImageCode) as Texture2D;
        }

        /// <summary>
        /// Copy constructor. Creates an instance of <see cref="UnitData"/> class.
        /// </summary>
        /// <param name="unit">Unit data to copy</param>
        public UnitData(UnitData unit)
        {
            Name = unit.Name;
            MaxHealthPoints = unit.MaxHealthPoints;
            CurrentHealthPoints = MaxHealthPoints;
            MaxActionPoints = unit.MaxActionPoints;
            CurrentActionPoints = MaxActionPoints;
            Attacks = unit.Attacks;
            Leader = unit.Leader;
            PrefabCode = unit.PrefabCode;
            ImageCode = unit.ImageCode;
            Image = unit.Image;
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Resets current action points.
        /// </summary>
        public void ResetActionPoints()
        {
            CurrentActionPoints = MaxActionPoints;
        }

        /// <summary>
        /// Actions performed before next turn.
        /// </summary>
        public void PrepareForNextTurn()
        {
            // update attaks' turns left counters:
            foreach (var attack in Attacks)
            {
                if (attack.TurnsLeft > 0) attack.TurnsLeft--;
            }
        }
        #endregion
    }
}
