using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    public class AttemptRepository : DbContext {

        public AttemptRepository() : base("SW9_Project") { }
        public DbSet<Attempt> Attempts { get; set; }
    }
}
