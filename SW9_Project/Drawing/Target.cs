using SW9_Project.Logging;

namespace SW9_Project.Drawing
{
    class Target
    {
        public Cell targetCell { get; set; }
        Logger logger = new Logger();

        public Target(Cell cell) { targetCell = cell; }

        ~Target() { logger.LogMessage("TARGET DESTROYED"); }
    }
}
