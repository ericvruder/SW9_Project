using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW9_Project.Drawing
{
    class Target
    {
        public Cell targetCell { get; set; }

        public Target(Cell cell) { targetCell = cell; }
    }
}
