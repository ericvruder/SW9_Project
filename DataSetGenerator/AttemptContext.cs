using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSetGenerator {
    public class AttemptContext : DbContext {

        public AttemptContext() : base("SW9_Project") { }
        public DbSet<Attempt> Attempts { get; set; }
    }
}
