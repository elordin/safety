using ffb.Modelling.Crossing;
using SafetySharp.Modeling;

namespace ffb.Modelling.Reality
{
    public class Gates : Component
    {
        private int _angle;
        private readonly StateMachine<GateState> _stateMachine = GateState.Open;

        public GateState State
        {
            get { return _stateMachine.State; }
        }

        public extern CrossingState CrossingState { get; }

        public Gates()
        {
            _angle = 0;
        }

        public override void Update()
        {
            _stateMachine.
                Transition(
                    from: GateState.Open, 
                    to: GateState.Closing,
                    guard: CrossingState == CrossingState.ProtectionPhase).
                Transition(
                    from: GateState.Closing, 
                    to: GateState.Closing,
                    guard: _angle < 2,
                    action: () => { _angle++; }).
                Transition(
                    from: GateState.Closing,
                    to: GateState.Closed,
                    guard: _angle == 2).
                Transition(
                    from: GateState.Closed, 
                    to: GateState.Opening,
                    guard: CrossingState == CrossingState.OpeningPhase).
                Transition(
                    from: GateState.Opening, 
                    to: GateState.Opening,
                    guard: _angle > 0,
                    action: () => { _angle--; }).
                Transition(
                    from: GateState.Opening, 
                    to: GateState.Open,
                    guard: _angle == 0);
        }
    }
}
