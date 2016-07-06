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
    }
}
