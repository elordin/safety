using SafetySharp.Modeling;
using System;

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

        public readonly Fault SpeedWrongMeasurements = new TransientFault();

        [FaultEffect(Fault = nameof(SpeedWrongMeasurements))]
        public class WrongMeasurementsEffect : SpeedMeasurement
        {
            public override void Update()
            {
                base.Update();
                MeasuredSpeed = (new Random()).Next(0, Model.MaxSpeed);
            }
        }
    }
}
