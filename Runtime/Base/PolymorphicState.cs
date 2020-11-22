namespace Hairibar.HFSM
{
    public abstract class PolymorphicState<TMachine, TOwner> : State<TMachine> where TOwner : TMachine
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
    }
}
