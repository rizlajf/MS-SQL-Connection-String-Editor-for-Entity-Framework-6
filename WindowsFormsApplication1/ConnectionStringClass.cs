using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApplication1
{
    public class ConnectionStringClass
    {
        public IList<connectionString> connectionStrings;

    }
    public class connectionString
    {
        public string Name { get; set; }
        public string Provider { get; set; }
        public string ConnectionString { get; set; }
        public string Metadata { get; set; }
    }
}
