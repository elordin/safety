using ffb.Modelling.Crossing;
using SafetySharp.Modeling;

namespace ffb.Modelling.Reality
{
    public class Gates : Component
    {
        [Range(0, 90, OverflowBehavior.Clamp)]
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
                    guard: _angle < 90,
                    action: () => { _angle += Model.GateSpeed; }).
                Transition(
                    from: GateState.Closing,
                    to: GateState.Closed,
                    guard: _angle == 90).
                Transition(
                    from: GateState.Closed, 
                    to: GateState.Opening,
                    guard: CrossingState == CrossingState.OpeningPhase).
                Transition(
                    from: GateState.Opening, 
                    to: GateState.Opening,
                    guard: _angle > 0,
                    action: () => { _angle -= Model.GateSpeed; }).
                Transition(
                    from: GateState.Opening, 
                    to: GateState.Open,
                    guard: _angle == 0);
        }


        public bool Closed { get { return _stateMachine.State == GateState.Closed ; } }

        public readonly Fault GatesBroken = new PermanentFault();

        [FaultEffect(Fault = nameof(GatesBroken))]
        public class BrokenEffect : Gates
        {
            public override void Update()
            {
                // Do nothing
            }
        }
    }
}
