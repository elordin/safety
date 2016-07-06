using SafetySharp.Modeling;

namespace ffb.Modelling.Train
{
    public class SpeedMeasurement : Component
    {
        public virtual int MeasuredSpeed { get; private set; }

        public extern int Speed { get; }

        public override void Update()
        {
            MeasuredSpeed = Speed;
        }

        public readonly Fault WrongMeasurements = new TransientFault();

        [FaultEffect(Fault = nameof(WrongMeasurements))]
        public class WrongMeasurementsEffect : SpeedMeasurement
        {
            public override void Update()
            {
                base.Update();
                MeasuredSpeed += 2;
            }
        }
    }
}
