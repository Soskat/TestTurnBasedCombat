using TestTurnBasedCombat.Game;
using UnityEngine;


namespace TestTurnBasedCombat.UIControllers
{
    /// <summary>
    /// Component that creates attack tooltip UI.
    /// </summary>
    public class AttackTooltip : MonoBehaviour
    {
        #region Private fields
        /// <summary>Assigned attack.</summary>
        [SerializeField] private Attack assignedAttack;
        /// <summary>Is tooltip on?</summary>
        [SerializeField] private bool tooltipOn;
        /// <summary>Do show unit's info?</summary>
        [SerializeField] private bool showUnitInfo;
        /// <summary>Is this panel left-sided?</summary>
        [SerializeField] private bool leftSideLayout;
        /// <summary>Tooltip display delay.</summary>
        [SerializeField] private float displayDelay = 0.5f;
        /// <summary>Can count to tooltip displaying?</summary>
        private bool canCount;
        /// <summary>Tooltip display start counter.</summary>
        private float displayStartCounter = 0f;
        /// <summary>Inner margin of the tooltip box.</summary>
        private float margin = 5f;
        /// <summary>Height of the line.</summary>
        private float lineHeight = 22f;
        /// <summary>Height of the tooltip box.</summary>
        private float tooltipHeight = 200f;
        /// <summary>Width of the tooltip box.</summary>
        private float tooltipWidth = 200f;
        /// <summary>Unit's name.</summary>
        private string unitName;
        #endregion
        

        #region MonoBehaviour methods
        // Use this for initialization
        void Start()
        {
            tooltipOn = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (canCount && !tooltipOn && displayDelay > displayStartCounter)
            {
                displayStartCounter += Time.deltaTime;
            }

            if (displayStartCounter >= displayDelay)
            {
                tooltipOn = true;
            }
        }

        // OnGUI is called for rendering and handling GUI events.
        private void OnGUI()
        {
            if (!tooltipOn) return;

            GUIStyle wordWrapStyle = new GUIStyle(GUI.skin.label) { wordWrap = true };
            GUIStyle boldLabelStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold };
            Vector2 mousePos = Input.mousePosition;
            Rect rect;
            // calculate tooltip GUI rect based on display side:
            if (leftSideLayout)
            {
                rect = new Rect(mousePos.x,
                                Screen.height - mousePos.y - tooltipHeight,
                                tooltipWidth,
                                tooltipHeight);
            }
            else
            {
                rect = new Rect(mousePos.x - tooltipWidth,
                                Screen.height - mousePos.y - tooltipHeight,
                                tooltipWidth,
                                tooltipHeight);
            }
            // display info about unit:
            if (showUnitInfo)
            {
                rect.height += 2 * (lineHeight + 2 * margin);
                // walkaround to get less transparent box:
                GUI.Box(rect, GUIContent.none);
                GUI.Box(rect, GUIContent.none);
                rect.y += margin;
                GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                          unitName.Replace("_", " "),
                          boldLabelStyle);
                rect.y += lineHeight + margin;
                GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                          "Basic attack:");
                rect.y += lineHeight + 2 * margin;
            }
            else
            {
                // walkaround to get less transparent box:
                GUI.Box(rect, GUIContent.none);
                GUI.Box(rect, GUIContent.none);
            }
            rect.y += margin;
            GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                      assignedAttack.AttackName.Replace("_", " "),
                      boldLabelStyle);
            rect.y += lineHeight - margin;
            GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, 2 * lineHeight),
                      assignedAttack.AttackInfo,
                      wordWrapStyle);
            rect.y += 2 * lineHeight;
            GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                      "Damage: " + assignedAttack.Damage.ToString());
            rect.y += lineHeight - margin;
            GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                      "Range of attack: " + assignedAttack.RangeOfAttack.ToString());
            rect.y += lineHeight - margin;
            GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                      "Range of damage: " + assignedAttack.DamageRange.ToString());
            rect.y += lineHeight - margin;
            GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                      "Need enemy to launch: " + assignedAttack.NeedEnemyToLaunch.ToString());
            rect.y += lineHeight - margin;
            GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                      "Auto-damage On: " + assignedAttack.AutoDamageOn.ToString());
            rect.y += lineHeight - margin;
            GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                      "Cost: " + assignedAttack.AttackCost.ToString());
            rect.y += lineHeight - margin;
            GUI.Label(new Rect(rect.x + margin, rect.y, tooltipWidth - 2 * margin, lineHeight),
                      "Cooldown: " + assignedAttack.Cooldown.ToString());
        }
        #endregion


        #region Public methods
        /// <summary>
        /// Sets up attack tooltip info.
        /// </summary>
        /// <param name="attack">Attack info</param>
        /// <param name="name">Unit's name</param>
        public void SetUpTooltip(Attack attack, string name = null)
        {
            assignedAttack = attack;
            unitName = name;
        }

        /// <summary>
        /// Turns on attack tooltip.
        /// </summary>
        public void TurnOnTooltip()
        {
            canCount = true;
        }

        /// <summary>
        /// Turns off attack tooltip.
        /// </summary>
        public void TurnOffToolTip()
        {
            tooltipOn = false;
            canCount = false;
            displayStartCounter = 0;
        }
        #endregion
    }
}
