using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    delegate void Method();

    public class PerformanceTest
    {
        public static void DoTest()
        {
            var rand = new Random();
            var receptions = Enumerable.Range(1, 500000).SelectMany(pid => Enumerable.Range(1,
            rand.Next(0, 100)).Select(rid => new {
                PatientId = pid,
                ReceptionStart = new
            DateTime(2017, 06, 30).AddDays(-rand.Next(1, 500))
            })).ToList();
            var patients = Enumerable.Range(1, 500000).Select(pid => new {
                Id = pid,
                Surname =
            string.Format("Иванов{0}", pid)
            }).ToList();

            DateTime delimeterDate = new DateTime(2017, 01, 01);

            Stopwatch sw = new Stopwatch();

            Method method1 = () =>
            {
                var patientIDs = receptions.Where(r => r.ReceptionStart < delimeterDate).Select(r => r.PatientId).Distinct();
                var earlyPatients = patients.Where(p => patientIDs.Any(id => id == p.Id));
                Console.WriteLine(earlyPatients.Count());
            };

            Method method2 = () =>
            {
                var patientIDs = receptions.Where(r => r.ReceptionStart < delimeterDate).Select(r => r.PatientId).Distinct().ToList();
                var earlyPatients = patients.Where(p => patientIDs.Any(id => id == p.Id));
                Console.WriteLine(earlyPatients.Count());
            };

            Method method3 = () =>
            {
                var patientIDs = receptions.Where(r => r.ReceptionStart < delimeterDate).Select(r => r.PatientId).Distinct().AsEnumerable();
                var earlyPatients = patients.Where(p => patientIDs.Any(id => id == p.Id));
                Console.WriteLine(earlyPatients.Count());
            };

            Method method4 = () =>
            {
                var patientIDs = receptions.Where(r => r.ReceptionStart < delimeterDate).Select(r => r.PatientId).Distinct().AsQueryable();
                var earlyPatients = patients.Where(p => patientIDs.Any(id => id == p.Id));
                Console.WriteLine(earlyPatients.Count());
            };

            Method method5 = () =>
            {
                var earlyPatients = patients.Where(p => receptions.Where(r => r.ReceptionStart < delimeterDate).Select(r => r.PatientId).Distinct().Any(id => id == p.Id));
                Console.WriteLine(earlyPatients.Count());
            };

            Method method6 = () =>
            {
                var patientIDs = receptions.Where(r => r.ReceptionStart < delimeterDate).Select(r => r.PatientId).Distinct();
                var earlyPatients = patients.Join(patientIDs, p => p.Id, id => id, (p, id) => p);
                Console.WriteLine(earlyPatients.Count());
            };

            Method method7 = () =>
            {
                var patientIDs = receptions.Where(r => r.ReceptionStart < delimeterDate).Select(r => r.PatientId).Distinct().ToList();
                var earlyPatients = patients.Join(patientIDs, p => p.Id, id => id, (p, id) => p);
                Console.WriteLine(earlyPatients.Count());
            };

            Method method8 = () =>
            {
                var earlyPatients = patients.Join(receptions, p => p.Id, r => r.PatientId, (p, r) => new { Id = p.Id, ReceptionStart = r.ReceptionStart }).Where(m => m.ReceptionStart < delimeterDate).Select(m => m.Id).Distinct();
                Console.WriteLine(earlyPatients.Count());
            };

            Method[] methods = new Method[] { method6, method7, method8, method2 };

            for (int i = 0; i < methods.Length; ++i)
            {
                sw.Reset();
                sw.Start();
                methods[i]();
                sw.Stop();
                Console.WriteLine($"Method {i + 1}: {sw.ElapsedMilliseconds}");
                Console.WriteLine();
            }
        }
    }
}
