namespace Hairibar.HFSM
{
    /// <summary>
    /// The base class of all transitions
    /// </summary>
    public abstract class Transition<TMachine>
    {
        public string To { get; }

        public bool ForceInstantly { get; }

        public virtual bool ShouldTransition => true;

        [System.NonSerialized] public StateMachine<TMachine> fsm;
        public virtual TMachine Owner
        {
            get => _machineTypeOwner;
            set => _machineTypeOwner = value;
        }

        TMachine _machineTypeOwner;



        public virtual void OnExecutedTransition() { }


        protected Transition(string to, bool forceInstantly)
        {
            To = to;

            ForceInstantly = forceInstantly;
        }
    }
}
