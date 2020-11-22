namespace Hairibar.HFSM
{
    public abstract class FromToTransition<TMachine> : Transition<TMachine>
    {
        public string From { get; }


        /// <summary>
        /// Called when the state machine enters the "from" state
        /// </summary>
        public virtual void OnEnterFromState() { }


        protected FromToTransition(string from, string to, bool forceInstantly) : base(to, forceInstantly)
        {
            From = from;
        }
    }

}
