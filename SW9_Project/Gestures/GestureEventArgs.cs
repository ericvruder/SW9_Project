using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project
{
    /// <summary>
    /// The gesture event arguments
    /// </summary>
    public class GestureEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GestureEventArgs"/> class.
        /// </summary>
        /// <param name="type">The gesture type.</param>
        /// <param name="trackingID">The tracking ID.</param>
        public GestureEventArgs(string name, int trackingId, SkeletonPoint pos)
        {
            this.TrackingId = trackingId;
            this.GestureName = name;
            this.Position = pos;
            this.Time = DateTime.Now;
        }

        /// <summary>
        /// Gets or sets the type of the gesture.
        /// </summary>
        /// <value>
        /// The name of the gesture.
        /// </value>
        public string GestureName { get; set; }

        /// <summary>
        /// Gets or sets the tracking ID.
        /// </summary>
        /// <value>
        /// The tracking ID.
        /// </value>
        public int TrackingId { get; set; }

        public SkeletonPoint Position { get; set; }

        public DateTime Time { get; set; }
    }
}
