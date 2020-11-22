using System;

namespace Hairibar.HFSM
{
    public class AnonState<TMachine> : State<TMachine>
    {
        public override bool HasExitRules => _needsExitTime;
        readonly bool _needsExitTime;

        public override bool CanExit => canExit?.Invoke(this) ?? true;

        readonly Action<AnonState<TMachine>> onEnter;
        readonly Action<AnonState<TMachine>> update;
        readonly Action<AnonState<TMachine>> onExit;
        readonly Func<AnonState<TMachine>, bool> canExit;


        protected override void OnEnter()
        {
            onEnter?.Invoke(this);
        }

        public override void Update()
        {
            update?.Invoke(this);
        }

        protected override void OnExit()
        {
            onExit?.Invoke(this);
        }


        /// <summary>
        /// Initialises a new instance of the State class
        /// </summary>
        /// <param name="onEnter">A function that is called when the state machine enters this state</param>
        /// <param name="update">A function that is called by the logic function of the state machine if this state is active</param>
        /// <param name="onExit">A function that is called when the state machine exits this state</param>
        /// <param name="canExit">(Only if needsExitTime is true):
        /// 	Called when a state transition from this state to another state should happen.
        /// 	If it can exit, it should call fsm.StateCanExit()
        /// 	and if it can not exit right now, later in OnLogic() it should call fsm.StateCanExit()</param>
        /// <param name="needsExitTime">Determines if the state is allowed to instantly
        /// 	exit on a transition (false), or if the state machine should wait until the state is ready for a
        /// 	state change (true)</param>
        public AnonState(
                Action<AnonState<TMachine>> onEnter = null,
                Action<AnonState<TMachine>> update = null,
                Action<AnonState<TMachine>> onExit = null,
                Func<AnonState<TMachine>, bool> canExit = null,
                bool needsExitTime = false)
        {
            this.onEnter = onEnter;
            this.update = update;
            this.onExit = onExit;
            this.canExit = canExit;

            this._needsExitTime = needsExitTime;
        }
    }
}
