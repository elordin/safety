using SafetySharp.Modeling;

namespace ffb.Modelling.Train
{
    public class SpeedCalculator : Component
    {
        public virtual int MeasuredSpeed { get; private set; }

        public extern int Speed { get; }

        public override void Update()
        {
            MeasuredSpeed = Speed;
        }
    }
}
