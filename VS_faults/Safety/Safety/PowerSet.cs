using System;
using System.Collections.Generic;
using System.Linq;

namespace Safety
{
    class PowerSet
    {
        public enum Errors
        {
            CRM_Outage, 
            CRM_WrongMessage,
            DriveTrain,
            Gates,
            Odometer,
            SensorGate,
            SensorSP,
            SpeedMeasurement,
            TRM_Outage
        };

        public static void Main()
        {
            var errors = new List<Errors>
            {
                Errors.CRM_Outage,
                Errors.CRM_WrongMessage,
                Errors.DriveTrain,
                Errors.Gates,
                Errors.SensorGate,
                Errors.SpeedMeasurement,
                Errors.TRM_Outage
            };

            var hazards = new List<List<Errors>>();
            hazards.Add(new List<Errors> { Errors.SensorSP });
            hazards.Add(new List<Errors> { Errors.Odometer});
            hazards.Add(new List<Errors> { Errors.DriveTrain, Errors.CRM_Outage});
            hazards.Add(new List<Errors>{Errors.DriveTrain, Errors.TRM_Outage});
            hazards.Add(new List<Errors>{Errors.DriveTrain, Errors.Gates});
            hazards.Add(new List<Errors>{Errors.Gates, Errors.CRM_WrongMessage});
            hazards.Add(new List<Errors>{Errors.CRM_Outage, Errors.SpeedMeasurement});
            hazards.Add(new List<Errors>{Errors.Gates, Errors.SensorGate});
            hazards.Add(new List<Errors>{Errors.Gates, Errors.SpeedMeasurement});
            hazards.Add(new List<Errors>{Errors.SpeedMeasurement, Errors.TRM_Outage});
            hazards.Add(new List<Errors>{ Errors.CRM_WrongMessage, Errors.DriveTrain, Errors.SensorGate});
            hazards.Add(new List<Errors>{Errors.CRM_WrongMessage, Errors.DriveTrain, Errors.SpeedMeasurement});
            hazards.Add(new List<Errors>{Errors.DriveTrain, Errors.SensorGate, Errors.SpeedMeasurement});



            var result = GetPowerSet(errors);
            result = result.Where(r => r.Count() == 3 && !ContainsAny(r, hazards));

            Console.Write(string.Join(Environment.NewLine,
                result.Select(subset =>
                    string.Join(",", subset.Select(clr => clr.ToString()).ToArray())).ToArray()));

            Console.WriteLine();
            Console.WriteLine();
        }

        public static bool ContainsAny<T>(IEnumerable<T> elem, List<List<T>> all)
        {
            foreach (var reference in all)
            {
                if (reference.All(r => elem.Contains(r)))
                {
                    return true;
                }
            }

            return false;
        }

        public static IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
        {
            return from m in Enumerable.Range(0, 1 << list.Count)
                   select
                       from i in Enumerable.Range(0, list.Count)
                       where (m & (1 << i)) != 0
                       select list[i];
        }
    }
}
