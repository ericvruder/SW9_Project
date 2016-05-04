using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace DataSetGenerator {

    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public class Validity
    {
        public int ParticipantID { get; set; }
        public GestureDirection Direction { get; set; }
        public GestureType Type { get; set; }
        public int InvalidAttempts { get; set; }
        public int TimeErrors { get; set; }
        private string DebuggerDisplay { get { return $"ID = {ParticipantID} Direction = {Direction} Type = {Type} Number of invalid attempts = {InvalidAttempts }"; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">The participant id</param>
        /// <param name="line">Line to be parsed from file</param>
        public Validity(string id, string line)
        {
            if(line == String.Empty)
                return;

            line = Regex.Replace(line, @"\s", "");
            string[] info = line.Split(',');

            // direction,type,invalidAttempts,timeErrors
            // push,pinch,3,4
            ParticipantID = Int32.Parse(id);
            switch (info[0])
            {
                case "push":
                    Direction = GestureDirection.Push;
                    break;
                case "pull":
                    Direction = GestureDirection.Pull;
                    break;
                default:
                    throw new Exception("Direction must be either: push, pull");
            }
            switch (info[1])
            {
                case "pinch":
                    Type = GestureType.Pinch;
                    break;
                case "grab":
                    Type = GestureType.Pinch;
                    break;
                case "swipe":
                    Type = GestureType.Swipe;
                    break;
                case "throw":
                    Type = GestureType.Throw;
                    break;
                case "tilt":
                    Type = GestureType.Tilt;
                    break;
                default:
                    throw new Exception("Technique must be either: pinch (or grab), swipe, throw, tilt");
            }
            if(info.Length > 2)
                InvalidAttempts = Int32.Parse(info[2]);
            if(info.Length > 3)
                TimeErrors = Int32.Parse(info[3]);

        }
    }
}