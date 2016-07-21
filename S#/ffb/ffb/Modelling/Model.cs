using ffb.Modelling.Crossing;
using ffb.Modelling.Reality;
using ffb.Modelling.Train;
using SafetySharp.Analysis;
using SafetySharp.Modeling;

namespace ffb.Modelling
{
    public class Model : ModelBase
    {
        public const int C = 4; // seconds
        public const int T = 30; // seconds
        public const int A = 8; // m/s²
        public const int Z = 150; // m
        public const int DistToSP = 32; // m
        public const int StartDistance = 512; // m
        public const int EndDistance = -64; // m
        public const int MaxSpeed = 45; // m/s
        public const int Tick = 1; // s
        public const int GateSpeed = 45; // deg/s

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

        public Formula Hazard => ! Gates.Closed && RealTrain.OnGP;
        public Formula NoHazard => Gates.Closed || ! RealTrain.OnGP; 
    }

}
