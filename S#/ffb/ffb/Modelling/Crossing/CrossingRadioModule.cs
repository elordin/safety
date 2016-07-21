using System;
using SafetySharp.Modeling;

namespace ffb.Modelling.Crossing
{
    public class CrossingRadioModule : RadioModule
    {
        public readonly Fault CrossingRadioOutage = new TransientFault();

        [FaultEffect(Fault = nameof(CrossingRadioOutage))]
        public class OutageEffect : CrossingRadioModule
        {
            public override void Update()
            {
                //Do nothing
            }
        }

        public readonly Fault CrossingRadioWrongMessage = new TransientFault();

        [FaultEffect(Fault = nameof(CrossingRadioWrongMessage))]
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
