using SafetySharp.Modeling;

namespace ffb.Modelling.Train
{
    public class TrainRadioModule : RadioModule
    {
        public readonly Fault Outage = new TransientFault();

        [FaultEffect(Fault = nameof(Outage))]
        public class OutageEffect : TrainRadioModule
        {
            public override void Update()
            {
                // Do nothing
            }
        }
    }
}
