namespace Hairibar.HFSM
{
    public abstract class PolymorphicTransition<TMachine, TOwner> : FromToTransition<TMachine> where TOwner : TMachine
    {
        public override TMachine Owner
        {
            get => base.Owner;
            set
            {
                CastOwner = (TOwner) value;
                base.Owner = value;
            }
        }

        public TOwner CastOwner { get; private set; }


        protected PolymorphicTransition(string from, string to, bool forceInstantly) : base(from, to, forceInstantly) { }
    }
}
