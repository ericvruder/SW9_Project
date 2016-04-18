using System;
using System.Collections.Generic;

namespace DataSetGenerator
{
    public class Validity
    {
        private int _id;
        private GestureDirection _direction;
        private GestureType _type;
        private int _invalidAttempts;

        public void ValidateAttempts(Test t)
        {
            var id = t.ID;
            foreach (var tech in DataGenerator.AllTechniques)
            {
                //Get all miss attempts
                //get number of invalid attempts from validity file
                //Apply false to valid property in attempt class on database for for furthest attempts 
            }
        }

        public Validity(string id, string line)
        {
            string[] info = line.Split(':');

            // push,pinch:3
            _id = Int32.Parse(id);
            switch (info[0].Split(',')[0])
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
            switch (info[0].Split(',')[1])
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
            _invalidAttempts = Int32.Parse(info[1]);

        }
    }
}