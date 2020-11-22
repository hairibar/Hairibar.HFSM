using System;
using System.Collections.Generic;

namespace Hairibar.HFSM
{
    /// <summary>
    /// A finite state machine
    /// </summary>
    public class StateMachine<T> : State<T>
    {
        public State<T> ActiveState { get; private set; }
        public string PreviousState { get; private set; }

        public override bool HasExitRules => ActiveState.HasExitRules;
        public override bool CanExit => ActiveState.CanExit;

        public State<T> this[string stateName]
        {
            get
            {
                return states[stateName];
            }
        }

        string startState;
        string pendingState;
        IReadOnlyCollection<Transition<T>> activeStateTransitions;

        readonly Dictionary<string, State<T>> states = new Dictionary<string, State<T>>();
        readonly Dictionary<string, List<FromToTransition<T>>> fromToTransitions = new Dictionary<string, List<FromToTransition<T>>>();

        readonly List<AnyStateTransition<T>> anyStateTransitions = new List<AnyStateTransition<T>>();

        #region Building
        public void AddState(string name, State<T> state)
        {
            state.fsm = this;
            state.name = name;
            state.Owner = Owner;

            states[name] = state;

            if (states.Count == 1 && startState == null)
            {
                SetStartState(name);
            }
        }

        /// <summary>
        /// Defines the entry point of the state machine.
        /// </summary>
        /// <param name="name">The name / identifier of the start state.</param>
        public void SetStartState(string name)
        {
            startState = name;
        }

        public void AddTransition(Transition<T> transition)
        {
            transition.fsm = this;
            transition.Owner = Owner;

            if (transition is FromToTransition<T> fromToTransition)
            {
                AddFromToTransition(fromToTransition);
            }
            else if (transition is AnyStateTransition<T> anyStateTransition)
            {
                AddAnyStateTransition(anyStateTransition);
            }
        }

        void AddFromToTransition(FromToTransition<T> transition)
        {
            if (!fromToTransitions.ContainsKey(transition.From))
            {
                fromToTransitions[transition.From] = new List<FromToTransition<T>>();
            }

            fromToTransitions[transition.From].Add(transition);
        }

        void AddAnyStateTransition(AnyStateTransition<T> transition)
        {
            anyStateTransitions.Add(transition);
        }
        #endregion

        #region State changing
        /// <summary>
        /// Request a state change, respecting the exit rules of the active state
        /// </summary>
        /// <param name="name">The name / identifier of the target state</param>
        /// <param name="forceInstantly">Overrides the exit rules of the active state if true,
        /// therefore forcing an immediate state change</param>
        public void RequestStateChange(string name, bool forceInstantly = false)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException("Tried to change states in an inactive StateMachine.");
            }

            if (StateExitIsValid(forceInstantly))
            {
                ChangeState(name);
            }
            else
            {
                pendingState = name;
            }
        }

        bool StateExitIsValid(bool forceInstantly = false)
        {
            return !ActiveState.HasExitRules || ActiveState.CanExit || forceInstantly;
        }

        /// <summary>
        /// Instantly changes to the target state
        /// </summary>
        /// <param name="name">The name / identifier of the active state</param>
        void ChangeState(string name)
        {
            State<T> newState = GetState(name);

            ActiveState?.Exit();
            PreviousState = ActiveState?.name ?? "";

            ActiveState = newState;
            ActiveState.Enter();

            if (fromToTransitions.TryGetValue(name, out List<FromToTransition<T>> newActiveTransitions))
            {
                activeStateTransitions = newActiveTransitions;
                foreach (FromToTransition<T> transition in activeStateTransitions)
                {
                    transition.OnEnterFromState();
                }
            }
            else
            {
                activeStateTransitions = System.Array.Empty<Transition<T>>();
            }
        }

        State<T> GetState(string name)
        {
            try
            {
                return states[name];
            }
            catch (KeyNotFoundException e)
            {
                throw new KeyNotFoundException($"There is no state {name}.", e);
            }
        }
        #endregion


        protected override void OnEnter()
        {
            ChangeState(startState);
        }

        public override void Update()
        {
            if (ActiveState == null)
            {
                throw new System.InvalidOperationException("The FSM has not been initialised yet! "
                    + "Call fsm.SetStartState(...) and fsm.OnEnter() to initialise.");
            }

            if (pendingState != null && StateExitIsValid())
            {
                ChangeState(pendingState);
                pendingState = null;
            }

            bool usedAFromToTransition = false;
            foreach (FromToTransition<T> transition in activeStateTransitions)
            {
                if (transition.ShouldTransition && StateExitIsValid(transition.ForceInstantly))
                {
                    ChangeState(transition.To);
                    usedAFromToTransition = true;
                    break;
                }
            }

            if (!usedAFromToTransition)
            {
                foreach (AnyStateTransition<T> transition in anyStateTransitions)
                {
                    if (transition.ShouldTransition && StateExitIsValid(transition.ForceInstantly))
                    {
                        ChangeState(transition.To);
                    }
                }
            }

            ActiveState.Update();
        }

        protected override void OnExit()
        {
            ActiveState.Exit();
        }


        public StateMachine(T owner)
        {
            this.Owner = owner;
        }
    }
}
