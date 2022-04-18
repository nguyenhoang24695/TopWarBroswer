using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoBroswer.Entity
{
    class Token
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public int TotalRally { get; set; }
        public int Slot { get; set; }
        public bool IsRunning { get; set; }
        public string StringToken { get; set; }
    }
}
