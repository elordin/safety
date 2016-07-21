using SafetySharp.Modeling;

namespace ffb.Modelling.Train
{
    public class TrainRadioModule : RadioModule
    {
        public readonly Fault TrainRadioOutage = new TransientFault();

        [FaultEffect(Fault = nameof(TrainRadioOutage))]
        public class OutageEffect : TrainRadioModule
        {
            public override void Update()
            {
                // Do nothing
            }
        }
    }
}
