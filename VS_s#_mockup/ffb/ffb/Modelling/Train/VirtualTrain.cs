using SafetySharp.Modeling;

namespace ffb.Modelling.Train
{
    public class VirtualTrain : Component
    {
        private enum State
        {
            BeforeEP, OnEP, EPtoAP, OnAP, APtoBEP, OnBEP, BEPtoGP, OnGP, AfterGP, OnSP, AfterSP
        }

        private readonly StateMachine<State> _stateMachine = State.BeforeEP;

        [Hidden, Subcomponent]
        public Odometer Odometer { get; set; }

        [Hidden, Subcomponent]
        public SpeedCalculator SpeedCalculator { get; set; }

        [Hidden, Subcomponent]
        public TrainRadioModule RadioModule { get; set; }

        public virtual BreakCommand BreakCommand { get; private set; }

        private int MeasuredDistance { get { return Odometer.MeasuredDistance; } }

        private int MeasuredSpeed { get { return SpeedCalculator.MeasuredSpeed; } }

        public virtual  Request Request { get; private set; }

        private Response Response { get { return RadioModule.ResponseOut; } }

        public VirtualTrain()
        {
            BreakCommand = BreakCommand.DoNotBreak;
        }

        public override void Update()
        {
            Update(Odometer, SpeedCalculator, RadioModule);

            _stateMachine.
                Transition(
                    from: State.BeforeEP,
                    to: State.OnEP,
                    guard:
                        MeasuredDistance >=
                        -1*Model.Z - MeasuredSpeed*MeasuredSpeed/2/Model.A - 2*Model.C*MeasuredSpeed -
                        MeasuredSpeed*(Model.T + Model.C)).
                Transition(
                    from: State.OnEP,
                    to: State.EPtoAP,
                    action: () => { Request = Request.EP_Request; }).
                Transition(
                    from: State.EPtoAP, 
                    to: State.OnAP,
                    guard: MeasuredDistance >= -1*Model.Z - MeasuredSpeed*MeasuredSpeed/2/Model.A - 2*Model.C*MeasuredSpeed).
                Transition(
                    from: State.OnAP, 
                    to: State.APtoBEP,
                    action: () => { Request = Request.AP_Request; }).
                Transition(
                    from: State.APtoBEP,
                    to: State.OnBEP,
                    guard: MeasuredDistance >= -1*Model.Z - MeasuredSpeed*MeasuredSpeed/2/Model.A).
                Transition(
                    from: State.OnBEP, 
                    to: State.BEPtoGP,
                    guard: Response == Response.Protected).
                Transition(
                    from: State.OnBEP,
                    to: State.BEPtoGP,
                    guard: Response != Response.Protected,
                    action: () => { BreakCommand = BreakCommand.Break;}).
                Transition(
                    from: State.BEPtoGP, 
                    to: State.OnGP,
                    guard: MeasuredDistance >= 0).
                Transition(
                    from: State.OnGP, 
                    to: State.AfterGP).
                Transition(
                    from: State.AfterGP, 
                    to: State.OnSP,
                    guard: MeasuredDistance >= Model.DistToSP).
                Transition(
                    from: State.OnSP, 
                    to: State.AfterSP);
        }
    }
}
