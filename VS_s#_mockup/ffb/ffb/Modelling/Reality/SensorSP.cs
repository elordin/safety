using SafetySharp.Modeling;

namespace ffb.Modelling.Reality
{
    public class SensorSP : Component
    {
        public extern bool TrainOnSP { get; }

        public virtual bool Active { get; private set; }

        public override void Update()
        {
            Active = TrainOnSP;
        }
    }
}
