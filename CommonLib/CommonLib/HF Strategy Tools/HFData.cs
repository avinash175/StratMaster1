using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CommonLib
{
    public class HFData
    {
        public List<TimeSeries> InputData { get; set; }
        public TypeOfData DataType { get; set; }
        public List<string> SecName { get; set; }
    }
}
