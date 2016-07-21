using ffb.Modelling.Reality;
using SafetySharp.Modeling;

namespace ffb.Modelling.Crossing
{
    public class SensorGate : Component
    {
        public virtual bool Closed { get; private set; }

        public virtual bool Open { get; private set; }

        public extern GateState GateState { get; }

        public override void Update()
        {
            Closed = GateState == GateState.Closed;
            Open = GateState == GateState.Open;
        }

        public readonly Fault SensorGateDefectOpen = new TransientFault();

        [FaultEffect(Fault = nameof(SensorGateDefectOpen))]
        public class DeffectOpenEffect : SensorGate
        {
            public override bool Open { get { return !base.Open; } }
        }

        public readonly Fault SensorGateDefectClosed = new TransientFault();

        [FaultEffect(Fault = nameof(SensorGateDefectClosed))]
        public class DeffectClosedEffect : SensorGate
        {
            public override bool Closed { get { return !base.Closed; } }
        }
    }
}
