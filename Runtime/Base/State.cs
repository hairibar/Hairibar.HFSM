namespace Hairibar.HFSM
{
    /// <summary>
    /// The base class of all states
    /// </summary>
    public abstract class State<TMachine>
    {
        public abstract bool HasExitRules { get; }
        public virtual bool CanExit => true;

        public string name;

        public StateMachine<TMachine> fsm;
        public virtual TMachine Owner
        {
            get => _machineTypeOwner;
            set => _machineTypeOwner = value;
        }
        TMachine _machineTypeOwner;


        public bool IsActive { get; private set; }


        public void Enter()
        {
            IsActive = true;
            OnEnter();
        }

        public void Exit()
        {
            IsActive = false;
            OnExit();
        }

        protected virtual void OnEnter()
        {

        }

        /// <summary>
        /// Called while this state is active
        /// </summary>
        public virtual void Update()
        {

        }

        protected virtual void OnExit()
        {

        }
    }
}
