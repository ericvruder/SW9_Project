using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;


namespace SW9_Project {
    interface IDrawingBoard {
        void PointAt(double xFromMid, double yFromMid);
        Point GetPoint(double xFromMid, double yFromMid);
        Cell GetCell(Point p);
        void CreatePushTarget(Target target);
        void CreatePullTargets(Target t1, Target t2);
        void CreateGrid(GridSize size);
        void StopTest();
    }
}
