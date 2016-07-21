using System;
using SafetySharp.Modeling;

namespace ffb.Modelling.Train
{
    public class Odometer : Component
    {
        [Range(Model.EndDistance, Model.StartDistance, OverflowBehavior.Clamp)]
        public virtual int MeasuredDistance { get; private set; }

        public extern int Distance { get; }

        public override void Update()
        {
            MeasuredDistance = Distance;
        }

        public readonly Fault OdometerWrongMeasurements = new TransientFault();

        [FaultEffect(Fault = nameof(OdometerWrongMeasurements))]
        public class WrongMeasurementsEffect : Odometer
        {
            public override void Update()
            {
                MeasuredDistance = new Random().Next(Model.EndDistance, Model.StartDistance);
            }
        }

        public readonly Fault OdometerNoMeasurements = new TransientFault();

        [FaultEffect(Fault = nameof(OdometerNoMeasurements))]
        public class NoMeasurementsEffect : Odometer
        {
            public override void Update()
            {
                // Do nothing
            }
        }
    }
}
