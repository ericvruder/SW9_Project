﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDataParser;
using Newtonsoft.Json;

namespace DataSetGenerator {
    public class TechniqueInfo {

        public TechniqueInfo(List<Attempt> attempts) {

            HPM = (float)attempts.Sum(attemtp => attemtp.Hit ? 1 : 0) / (float)attempts.Count;
            TTM = (float)attempts.Sum(attempt => attempt.Time.TotalSeconds) / (float)attempts.Count;
            ACCM = (float)attempts.Sum(attempt => attempt.Hit ? 0 : DataGenerator.DistanceToTargetCell(attempt)) / (float)attempts.Count;

            HPSTD = (float)Math.Sqrt(attempts.Sum(attempt => Math.Pow((attempt.Hit ? 1 : 0) - HPM, 2)) / attempts.Count);
            TTSTD = (float)Math.Sqrt(attempts.Sum(attempt => Math.Pow(attempt.Time.TotalSeconds - TTM, 2)) / attempts.Count);
            ACCSTD = (float)Math.Sqrt(attempts.Sum(attempt => Math.Pow(DataGenerator.DistanceToTargetCell(attempt) - ACCM, 2)) / attempts.Count);

        }

        public string ToJson() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        public float HPM { get; set; }
        public float HPSTD { get; set; }
        public float TTM { get; set; }
        public float TTSTD { get; set; }
        public float ACCM { get; set; }
        public float ACCSTD { get; set; }

    }
}
