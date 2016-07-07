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

        private readonly StateMachine<State> StateMachine = State.BeforeGP;
        
                
        [Hidden, Subcomponent]
        public DriveTrain DriveTrain { get; set; }

        public RealTrain()
        {
            Distance = Model.StartDistance;
        }

        [Range(Model.EndDistance, Model.StartDistance, OverflowBehavior.Clamp)]
        public int Distance { get; private set; }

        public bool OnSP { get { return StateMachine == State.OnSP; } }
        public bool OnGP { get { return StateMachine == State.OnGP; } }

        public override void Update()
        {
            Update(DriveTrain);

            Action action = () => { Distance -= DriveTrain.Speed * Model.Tick; };

            StateMachine.
                Transition(
                    from: State.BeforeGP,
                    to: State.BeforeGP,
                    guard: Distance > 0,
                    action: action).
                Transition(
                    from: State.BeforeGP,
                    to: State.OnGP,
                    guard: Distance <= 0,
                    action: action).
                Transition(
                    from: State.OnGP, 
                    to: State.AfterGP,
                    action: action).
                Transition(
                    from: State.AfterGP, 
                    to: State.AfterGP,
                    guard: Distance > Model.DistToSP,
                    action: action).
                Transition(
                    from: State.AfterGP, 
                    to: State.OnSP,
                    guard: Distance < Model.DistToSP + DriveTrain.Speed * Model.Tick,
                    action: action).
                Transition(
                    from: State.OnSP, 
                    to: State.AfterSP,
                    action: action);

        }
    }
}
