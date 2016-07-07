using SafetySharp.Modeling;

namespace ffb.Modelling.Crossing
{
    public class CrossingTimer : Component
    {
        private enum State
        {
            Idle, Counting, Finished
        }

        private int Counter { get; set; }
        private readonly StateMachine<State> _stateMachine = State.Idle;

        public virtual bool Finished
        {
            get { return _stateMachine.State == State.Finished; }
        }

        public extern CrossingState CrossingState { get; }

        public CrossingTimer()
        {
            Counter = 0;
        }

        public override void Update()
        {
            _stateMachine.
                Transition(
                    from: State.Idle, 
                    to: State.Counting,
                    guard: CrossingState == CrossingState.Protected,
                    action: () => { Counter = 0; }).
                Transition(
                    from: State.Counting, 
                    to: State.Counting,
                    guard: CrossingState == CrossingState.Protected && Counter < 16,
                    action: () => { Counter++; }).
                Transition(
                    from: State.Counting, 
                    to: State.Idle,
                    guard: CrossingState != CrossingState.Protected).
                Transition(
                    from: State.Counting,
                    to: State.Finished,
                    guard: CrossingState == CrossingState.Protected && Counter == 16).
                Transition(
                    from: State.Finished, 
                    to: State.Idle);
        }
    }
}
