using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace SW9_Project {
    interface IDrawingBoard {
        void ReceiveShape(Shape shapeToMove, double xFromMid, double yFromMid);
        void PointAt(double xFromMid, double yFromMid);
        void PullShape(double xFromMid, double yFromMid);
        void DrawNotice(string message, int secondsToShow);

    }
}
