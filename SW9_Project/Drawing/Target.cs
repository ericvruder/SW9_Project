using SW9_Project.Logging;

namespace SW9_Project.Drawing
{
    class Target
    {
        public Cell targetCell { get; set; }

        public Target(Cell cell) { targetCell = cell; }

        ~Target() { }
    }
}
