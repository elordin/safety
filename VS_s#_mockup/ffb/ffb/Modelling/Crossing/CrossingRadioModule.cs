using System;
using SafetySharp.Modeling;

namespace ffb.Modelling.Crossing
{
    public class CrossingRadioModule : RadioModule
    {
        public readonly Fault Outage = new TransientFault();

        [FaultEffect(Fault = nameof(Outage))]
        public class OutageEffect : CrossingRadioModule
        {
            public override void Update()
            {
                //Do nothing
            }
        }

        public readonly Fault WrongMessage = new TransientFault();

        public class WrongMessageEffect : CrossingRadioModule
        {
            public override void Update()
            {
                base.Update();
                if (ResponseOut == Response.Unprotected)
                {
                    ResponseOut = Response.Protected;
                }
            }
        }
    }
}
