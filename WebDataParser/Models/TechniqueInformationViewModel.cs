using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DataSetGenerator;
using Newtonsoft.Json;

namespace WebDataParser.Models {
    public class TechniqueInformationViewModel {

        public float[][] PushTime { get; set; }
        public float[][] PushHitRate { get; set; }
        public float[][] PushAccuracy { get; set; }

        public float[][] PullTime { get; set; }
        public float[][] PullHitRate { get; set; }
        public float[][] PullAccuracy { get; set; }

        public TechniqueInformationViewModel(IEnumerable<Attempt> attempts) {

            IEnumerable<Attempt> pushAttempts = from attempt in attempts
                                                where attempt.Direction == GestureDirection.Push
                                                select attempt;
            IEnumerable<Attempt> pullAttempts = from attempt in attempts
                                                where attempt.Direction == GestureDirection.Pull
                                                select attempt;

            PushTime = GetTimeInformation(pushAttempts);
            PullTime = GetTimeInformation(pullAttempts);

            PullHitRate = GetHitRateInformation(pullAttempts); 
            PushHitRate = GetHitRateInformation(pullAttempts);

            PullAccuracy = GetAccuracyInformation(pullAttempts);
            PushAccuracy = GetAccuracyInformation(pullAttempts);

        }

        private float[][] GetHitRateInformation(IEnumerable<Attempt> attempts) {

            //HPM = (float)attempts.Sum(attemtp => attemtp.Hit ? 1 : 0) / (float)attempts.Count;
            //HPSTD = (float)Math.Sqrt(attempts.Sum(attempt => Math.Pow((attempt.Hit ? 1 : 0) - HPM, 2)) / attempts.Count);

            float[][] hitRateInfo = new float[4][];
            foreach (var technique in DataGenerator.AllTechniques) {
                var techAttempts = attempts.Where(attempt => attempt.Type == technique);

                float tNum = (float)technique + 1;
                float tMean = (float)techAttempts.Sum(attemtp => attemtp.Hit ? 1 : 0) / techAttempts.Count();
                float tStd = (float)Math.Sqrt(techAttempts.Sum(attempt => Math.Pow((attempt.Hit ? 1 : 0) - tMean, 2)) / techAttempts.Count());
                hitRateInfo[(int)technique] = new float[] { tNum, tMean, tStd };
            }
            return hitRateInfo;

        }
        

        private float[][] GetAccuracyInformation(IEnumerable<Attempt> attempts) {


            //ACCM = (float)attempts.Sum(attempt => attempt.Hit ? 0 : MathHelper.DistanceToTargetCell(attempt)) / (float)attempts.Count;
            //ACCSTD = (float)Math.Sqrt(attempts.Sum(attempt => Math.Pow(MathHelper.DistanceToTargetCell(attempt) - ACCM, 2)) / attempts.Count);

            float[][] accuracyInfo = new float[4][];
            foreach (var technique in DataGenerator.AllTechniques) {
                var techAttempts = attempts.Where(attempt => attempt.Type == technique);

                float tNum = (float)technique + 1;
                float tMean = (float)techAttempts.Sum(attempt => attempt.Hit ? 0 : MathHelper.DistanceToTargetCell(attempt)) / techAttempts.Count();
                float tStd = (float)Math.Sqrt(techAttempts.Sum(attempt => Math.Pow(MathHelper.DistanceToTargetCell(attempt) - tMean, 2)) / techAttempts.Count());
                accuracyInfo[(int)technique] = new float[] { tNum, tMean, tStd };
            }
            return accuracyInfo;
        }

        private float[][] GetTimeInformation(IEnumerable<Attempt> attempts) {

            //TTM = (float)attempts.Sum(attempt => attempt.Time.TotalSeconds) / (float)attempts.Count;
            //TTSTD = (float)Math.Sqrt(attempts.Sum(attempt => Math.Pow(attempt.Time.TotalSeconds - TTM, 2)) / attempts.Count);

            float[][] timeInfo = new float[4][];
            foreach(var technique in DataGenerator.AllTechniques) {
                var techAttempts = attempts.Where(attempt => attempt.Type == technique);

                float tNum = (float)technique + 1;
                float tMean = (float)techAttempts.Sum(attempt => attempt.Time.TotalSeconds) / techAttempts.Count();
                float tStd = (float)Math.Sqrt(techAttempts.Sum(attempt => Math.Pow(attempt.Time.TotalSeconds - tMean, 2)) / techAttempts.Count());
                timeInfo[(int)technique] = new float[]{ tNum, tMean, tStd };
            }
            return timeInfo;
        }
    }
}