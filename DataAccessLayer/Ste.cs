using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class Ste
    {
        public int ID { get; set; }
        public string Barcode { get; set; }
        public string ProductCode { get; set; }
        public DateTime DateTime { get; set; }
        public int QualityID { get; set; }
        public string Quality { get; set; }
        public int FaultID { get; set; }
        public string Fault { get; set; }
        public int QualityPersonalID { get; set; }
        public string QualityPersonal { get; set; }
    }
}
