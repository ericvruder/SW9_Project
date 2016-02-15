using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SW9_Project {
    class BulletinBoard : CanvasWindow {

        /// <summary>
        /// I skal hold styr på background, om det skal være et billede eller farve skal ordnes ellers forsvinder griddet ikke
        /// </summary>
        public BulletinBoard() : base(false) {
            canvas.Background = Brushes.Tomato;
        }

        /// <summary>
        /// This gets called every draw cycle, IE every time the kinect captures a frame. 
        /// </summary>
        /// <param name="gesture">Any gestures that were captured. Can be null</param>
        public override void ExtendedDraw(KinectGesture gesture) {

        }
    }


}
