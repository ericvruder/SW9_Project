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
        /// At kalde base med false fortæller canvas window at det er en bulletin board der arbejder, den sætter variable targetPractice = false
        /// så bare bruge det hvis i skal ændre nogen ting i canvaswindow.cs
        /// </summary>
        public BulletinBoard() : base(false) {
            canvas.Background = Brushes.Gray;
        }

        /// <summary>
        /// This gets called every draw cycle, IE every time the kinect captures a frame. 
        /// </summary>
        /// <param name="gesture">Any gestures that were captured. Can be null</param>
        public override void ExtendedDraw(KinectGesture gesture) {

        }
    }


}
