using ffb.Modelling.Crossing;
using ffb.Modelling.Reality;
using ffb.Modelling.Train;
using SafetySharp.Modeling;

namespace ffb.Modelling
{
    public class Model : ModelBase
    {
        public const int C = 4;
        public const int T = 6;
        public const int A = 1;
        public const int Z = 12;
        public const int DistToSP = 12;

        public Model()
        {
            // Initialise Components
            VirtualTrain = new VirtualTrain
            {
                RadioModule = new TrainRadioModule(),
                Odometer = new Odometer(),
                SpeedMeasurement = new SpeedMeasurement()
            };

            Crossing = new Crossing.Crossing
            {
                RadioModule = new CrossingRadioModule(),
                Timer = new CrossingTimer(),
                SensorGate = new SensorGate()
            };

            RealTrain = new RealTrain
            {
                DriveTrain = new DriveTrain()
            };

            SensorSp = new SensorSP();
            Gates = new Gates();

            // Do binding

            // Reality ->
            Bind(nameof(SpeedMeasurement.Speed), nameof(DriveTrain.Speed));
            Bind(nameof(Odometer.Distance), nameof(RealTrain.Distance));
            Bind(nameof(SensorSp.TrainOnSP), nameof(RealTrain.OnSP));
            Bind(nameof(Crossing.SensorSPActive), nameof(SensorSp.Active));
            Bind(nameof(SensorGate.GateState), nameof(Gates.State));

            // Train ->
            Bind(nameof(DriveTrain.BreakCommand), nameof(VirtualTrain.BreakCommand));
            Bind(nameof(TrainRadioModule.RequestIn), nameof(VirtualTrain.Request));
            Bind(nameof(CrossingRadioModule.RequestIn), nameof(TrainRadioModule.RequestOut));

            // Crossing ->
            Bind(nameof(TrainRadioModule.ResponseIn), nameof(CrossingRadioModule.ResponseOut));
            Bind(nameof(CrossingRadioModule.ResponseIn), nameof(Crossing.Response));
            Bind(nameof(Gates.CrossingState), nameof(Crossing.State));
            Bind(nameof(CrossingTimer.CrossingState), nameof(Crossing.State));

        }

        [Root(RootKind.Controller)]
        public VirtualTrain VirtualTrain { get; private set; }

        public Odometer Odometer { get { return VirtualTrain.Odometer; } }

        public SpeedMeasurement SpeedMeasurement { get { return VirtualTrain.SpeedMeasurement; } }

        public TrainRadioModule TrainRadioModule { get { return VirtualTrain.RadioModule; } }

        [Root(RootKind.Controller)]
        public Crossing.Crossing Crossing { get; private set; }

        public CrossingTimer CrossingTimer { get { return Crossing.Timer; } }

        public SensorGate SensorGate { get { return Crossing.SensorGate; } }

        public CrossingRadioModule CrossingRadioModule { get { return Crossing.RadioModule; } }

        [Root(RootKind.Plant)]
        public RealTrain RealTrain { get; private set; }

        public DriveTrain DriveTrain { get { return RealTrain.DriveTrain; } }

        [Root(RootKind.Plant)]
        public SensorSP SensorSp { get; private set; }

        [Root(RootKind.Plant)]
        public Gates Gates { get; private set; }
    }
}
