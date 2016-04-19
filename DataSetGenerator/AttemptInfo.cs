using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    public class AttemptInfo {

        public Dictionary<GestureType, double[]> HitPercentage { get; set; } = new Dictionary<GestureType, double[]>();
        public Dictionary<GestureType, double[]> TimeTaken { get; set; } = new Dictionary<GestureType, double[]>();
        public Dictionary<GestureType, double[]> Accuracy { get; set; } = new Dictionary<GestureType, double[]>();

        public AttemptInfo(Test test, GestureDirection direction) : this(new List<Test>() {test}, direction) { }

        public AttemptInfo(List<Test> tests, GestureDirection direction) {

            foreach (var t in DataGenerator.AllTechniques) {
                HitPercentage.Add(t, new double[18]);
                TimeTaken.Add(t, new double[18]);
                Accuracy.Add(t, new double[18]);
            }

            foreach (var technique in DataGenerator.AllTechniques) {
                foreach (var test in tests) {
                    var attempts = test.Attempts[technique].Where(x => x.Direction == direction).ToList();
                    attempts.Sort((x, y) => x.AttemptNumber.CompareTo(y.AttemptNumber));
                    int count = 0;
                    foreach (var attempt in attempts) {
                        HitPercentage[technique][count] += attempt.Hit ? 1 : 0;
                        TimeTaken[technique][count] += attempt.Time.TotalSeconds;
                        Accuracy[technique][count] += MathHelper.DistanceToTargetCell(attempt);
                        count++;
                    }
                }
            }

            foreach (var t in DataGenerator.AllTechniques) {
                for (int i = 0; i < 18; i++) {
                    HitPercentage[t][i] /= tests.Count;
                    TimeTaken[t][i] /= tests.Count;
                    Accuracy[t][i] /= tests.Count;
                }
            }
        }
    }
}
