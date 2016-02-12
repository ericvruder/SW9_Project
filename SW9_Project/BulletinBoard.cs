using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SW9_Project {
    class BulletinBoard : CanvasWindow {

        public BulletinBoard() : base(false) {
            //I skal hold styr på background, om det skal være et billede eller farve skal ordnes ellers forsvinder griddet ikke
            canvas.Background = Brushes.Tomato;
        }

        public override void ExtendedDraw() {

        }
    }


}
