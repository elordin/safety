using SafetySharp.Modeling;

namespace ffb.Modelling.Crossing
{
    public class Crossing : Component
    {
        private readonly StateMachine<CrossingState> _stateMachine = CrossingState.Open;

        [Hidden, Subcomponent]
        public CrossingTimer Timer { get; set; }

        [Hidden, Subcomponent]
        public SensorGate SensorGate { get; set; }

        [Hidden, Subcomponent]
        public CrossingRadioModule RadioModule { get; set; }

        public virtual CrossingState State
        {
            get { return _stateMachine.State; }
        }

        public virtual Response Response { get; private set; }

        private Request Request { get { return RadioModule.RequestOut; } }

        private bool GatesClosed { get { return SensorGate.Closed; } }

        private bool GatesOpen { get { return SensorGate.Open; } }

        private bool TimerFinished { get { return Timer.Finished; } }

        public bool Protected { get { return _stateMachine.State == CrossingState.Protected; } }

        public extern bool SensorSPActive { get; }

        public override void Update()
        {
            Update(RadioModule, Timer, SensorGate);

            _stateMachine.
                Transition(
                    from: CrossingState.Open, 
                    to: CrossingState.ProtectionPhase,
                    guard: Request == Request.EP_Request).
                Transition(
                    from: CrossingState.ProtectionPhase, 
                    to: CrossingState.Protected,
                    guard: GatesClosed).
                Transition(
                    from: CrossingState.Protected, 
                    to: CrossingState.OpeningPhase,
                    guard: SensorSPActive || TimerFinished).
                Transition(
                    from: CrossingState.OpeningPhase, 
                    to: CrossingState.Open,
                    guard: GatesOpen);

            if (Request == Request.AP_Request)
            {
                Response = _stateMachine.State == CrossingState.Protected ? Response.Protected : Response.Unprotected;
            }
            else
            {
                Response = Response.Unknown;
            }
        }
    }
}
