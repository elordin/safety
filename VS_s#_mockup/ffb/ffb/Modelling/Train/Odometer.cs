using System;
using SafetySharp.Modeling;

namespace ffb.Modelling.Train
{
    public class Odometer : Component
    {
        public virtual int MeasuredDistance { get; private set; }

        public extern int Distance { get; }

        public override void Update()
        {
            MeasuredDistance = Distance;
        }

        public readonly Fault WrongMeasurements = new TransientFault();

        [FaultEffect(Fault = nameof(WrongMeasurements))]
        public class WrongMeasurementsEffect : Odometer
        {
            public override void Update()
            {
                MeasuredDistance = new Random().Next(-96, 24);
            }
        }

        public readonly Fault NoMeasurements = new TransientFault();

        [FaultEffect(Fault = nameof(NoMeasurements))]
        public class NoMeasurementsEffect : Odometer
        {
            public override void Update()
            {
                // Do nothing
            }
        }
    }
}
