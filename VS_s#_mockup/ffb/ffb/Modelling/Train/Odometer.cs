﻿using SafetySharp.Modeling;

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
    }
}