using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SeqAlignTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nw = new NeedlemanWunsch("../../rattus_norvegicus.zfp563.faa", "../../mus_musculus.zfp563.faa");
            nw.InitializeScoreMatrix();
            nw.FillScoreMatrix();
            nw.Traceback();
            nw.PrintResult();
        }

    }
}
