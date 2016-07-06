using System;
using SafetySharp.Modeling;

namespace ffb.Modelling.Reality
{
    public class DriveTrain : Component
    {
        public DriveTrain()
        {
            Speed = 4;
        }

        public virtual int Speed { get; private set; }

        public extern BreakCommand BreakCommand { get; }

        public override void Update()
        {
            if (BreakCommand == BreakCommand.Break)
            {
                Speed = Math.Max(Speed - 2, 0);
            }
        }

        public readonly Fault BreaksBroken = new TransientFault();

        [FaultEffect(Fault = nameof(BreaksBroken))]
        public class BreaksBrokenEffect : DriveTrain
        {
            public override void Update()
            {
                // Do nothing
            }
        }
    }
}
