using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevWebApp
{
    public class WordPair
    {

        public string SourceWord { get; set; }
        public string TargetWord { get; set; }
        [System.ComponentModel.Browsable(false)]
        public int TargetID { get; set; }
        [System.ComponentModel.Browsable(false)]
        public int SourceID { get; set; }

        public decimal InitialScore { get; set; }

        public bool scored { get; set; }

        public int levdistance { get; set; }

        public int sourcedistance { get; set; }

        public int targetdistance { get; set; }

        public int totaldistance { get; set; }

        public bool excluded { get; set; }




    }
}
