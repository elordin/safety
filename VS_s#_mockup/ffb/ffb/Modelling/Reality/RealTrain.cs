using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SafetySharp.Modeling;

namespace ffb.Modelling.Reality
{
    public class RealTrain : Component
    {
        private enum State
        {
            BeforeGP, OnGP, AfterGP, OnSP, AfterSP
        }

        private const int Tick = 1;
        private readonly StateMachine<State> StateMachine = State.BeforeGP;

        [Hidden, Subcomponent]
        public DriveTrain DriveTrain { get; set; }

        public RealTrain()
        {
            Distance = -96;
        }

        public int Distance { get; private set; }

        public bool OnSP {get { return StateMachine == State.OnSP; }}

        public override void Update()
        {
            Update(DriveTrain);

            Action action = () => { Distance += DriveTrain.Speed*Tick; };

            StateMachine.
                Transition(
                    from: State.BeforeGP,
                    to: State.BeforeGP,
                    guard: Distance < 0,
                    action: action).
                Transition(
                    from: State.BeforeGP,
                    to: State.OnGP,
                    guard: Distance >= -1*DriveTrain.Speed*Tick,
                    action: action).
                Transition(
                    from: State.OnGP, 
                    to: State.AfterGP,
                    action: action).
                Transition(
                    from: State.AfterGP, 
                    to: State.AfterGP,
                    guard: Distance < Model.DistToSP,
                    action: action).
                Transition(
                    from: State.AfterGP, 
                    to: State.OnSP,
                    guard: Distance >= Model.DistToSP - DriveTrain.Speed*Tick,
                    action: action).
                Transition(
                    from: State.OnSP, 
                    to: State.AfterSP,
                    action: action);

        }
    }
}
