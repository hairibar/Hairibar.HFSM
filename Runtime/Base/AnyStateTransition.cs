using System;

namespace Hairibar.HFSM
{
    public class AnyStateTransition<TMachine> : Transition<TMachine>
    {
        Predicate<AnyStateTransition<TMachine>> condition;

        public override bool ShouldTransition => fsm.ActiveState.name != To && condition(this);

        public AnyStateTransition(string to, Predicate<AnyStateTransition<TMachine>> condition, bool forceInstantly = false) : base(to, forceInstantly)
        {
            this.condition = condition;
        }
    }
}
