using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSetGenerator
{
    public class Validity
    {
        private int _participantID;
        private GestureDirection _direction;
        private GestureType _type;
        private int _invalidAttempts;
        private int _timeErrors;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The participant id</param>
        /// <param name="line">Line to be parsed from file</param>
        public Validity(string id, string line)
        {
            string[] info = line.Split(',');

            // direction,type,invalidAttempts,timeErrors
            // push,pinch,3,4
            _participantID = Int32.Parse(id);
            switch (info[0])
            {
                case "push":
                    _direction = GestureDirection.Push;
                    break;
                case "pull":
                    _direction = GestureDirection.Pull;
                    break;
                default:
                    throw new Exception("Direction must be either: push, pull");
            }
            switch (info[1])
            {
                case "pinch":
                    _type = GestureType.Pinch;
                    break;
                case "grab":
                    _type = GestureType.Pinch;
                    break;
                case "swipe":
                    _type = GestureType.Swipe;
                    break;
                case "throw":
                    _type = GestureType.Throw;
                    break;
                case "tilt":
                    _type = GestureType.Tilt;
                    break;
                default:
                    throw new Exception("Technique must be either: pinch (or grab), swipe, throw, tilt");
            }
            _invalidAttempts = Int32.Parse(info[2]);
            _timeErrors = Int32.Parse(info[3]);

        }

        public static void ValidateAttempts()
        {
            //get number of invalid attempts from validity file
            var validities = ValidityFilesList();
                

            //Get all miss attempts
            var missedAttempts = AttemptRepository.GetMissedAttempts(DataSource.Old);
            Console.WriteLine(missedAttempts.Count);

            var changedAttempts = new List<Attempt>();
            foreach (var validity in validities)
            {
                var tattempt = missedAttempts.Where(x => x.ID == validity._participantID.ToString() && x.Type == validity._type && x.Direction == validity._direction).ToList();
                for (int i = 0; i < validity._invalidAttempts; i++)
                {
                    tattempt[i].Valid = false;
                    changedAttempts.Add(tattempt[i]);
                }
            }

            AttemptRepository.UpdateAttempts(changedAttempts);


            Console.WriteLine(missedAttempts.Count);

            //Apply false to valid property in attempt class on database for for furthest attempts 
        }

        /// <summary>
        /// Parses the .invalid files to a List
        /// </summary>
        /// <returns>A list of validities with id, type, direction, number of invalid attempts, and time errors</returns>
        public static List<Validity> ValidityFilesList()
        {
            List<Validity> results = new List<Validity>();

            string[] files = Directory.GetFiles(DataGenerator.TestFileDirectory(DataSource.Target), "/invalidity/*.invalidity");
            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                StreamReader sr = new StreamReader(file);
                using (sr)
                {
                    string line = "";
                    while ((line = sr.ReadLine()) != null)
                    {
                        Validity invalid = new Validity(filename, line);
                        results.Add(invalid);
                    }
                }
            }

            return results;

        } 
    }
}