using System;

namespace Hairibar.HFSM
{
    /// <summary>
    /// A class used to determine whether the state machine should transition to another state
    /// </summary>
    public class ConditionalPolymorphicTransition<TMachine, TOwner> : PolymorphicTransition<TMachine, TOwner> where TOwner : TMachine
    {
        public override bool ShouldTransition => condition?.Invoke(this) ?? true;

        readonly Func<ConditionalPolymorphicTransition<TMachine, TOwner>, bool> condition;

        /// <summary>
        /// Initialises a new instance of the Transition class
        /// </summary>
        /// <param name="from">The name / identifier of the active state</param>
        /// <param name="to">The name / identifier of the next state</param>
        /// <param name="condition">A function that returns true if the state machine 
        /// 	should transition to the <c>to</c> state</param>
        /// <param name="forceInstantly">Ignores the needsExitTime of the active state if forceInstantly is true 
        /// 	=> Forces an instant transition</param>
        public ConditionalPolymorphicTransition(
                string from,
                string to,
                Func<ConditionalPolymorphicTransition<TMachine, TOwner>, bool> condition = null,
                bool forceInstantly = false) : base(from, to, forceInstantly)
        {
            this.condition = condition;
        }
    }
}
