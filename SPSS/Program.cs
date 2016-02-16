using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spss;
using System.IO;

namespace SPSSTest
{
    /// <summary>
    /// Example of auto generated sav files for SPSS using
    /// SPSS.net (https://github.com/AArnott/SPSS.NET)
    /// </summary>
    class SPSSExample
    {
        static void Main(string[] args)
        {
            if (File.Exists(@"example.sav"))
            {
                File.Delete(@"example.sav");
            }
            using (SpssDataDocument doc = SpssDataDocument.Create("example.sav"))
            {
                CreateMetaData(doc);
                CreateData(doc);
            }

        }

        public static void CreateMetaData(SpssDataDocument doc)
        {
            SpssStringVariable v1 = new SpssStringVariable();
            v1.Name = "v1";
            v1.Label = "What is your name?";
            doc.Variables.Add(v1);
            SpssStringVariable v2 = new SpssStringVariable();
            v2.Name = "v2";
            v2.Label = "How old are you?";
            doc.Variables.Add(v2);
            SpssStringVariable v3 = new SpssStringVariable();
            v3.Name = "v3";
            v3.Label = "What is your gender?";
            v3.ValueLabels.Add("1", "Male");
            v3.ValueLabels.Add("2", "Female");
            doc.Variables.Add(v3);
            SpssStringVariable v4 = new SpssStringVariable();
            v4.Name = "v4";
            v4.Label = "What is your birthday?";
            v4.Length = 20; // Seems like this needs to be here or it won't compile
            doc.Variables.Add(v4);
            doc.CommitDictionary();
        }

        public static void CreateData(SpssDataDocument doc)
        {
            SpssCase case1 = doc.Cases.New();
            case1["v1"] = "Bjarke";
            case1["v2"] = "26";
            case1["v3"] = "1";
            case1["v4"] = "09/08/1989";
            case1.Commit();
        }
    }
}
