using System;

namespace SeqAlignTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var nw = new NeedlemanWunsch("../../rattus_norvegicus.zfp563.faa", "../../mus_musculus.rn45s.faa");
            nw.InitializeScoreMatrix();
            nw.FillScoreMatrix();
            nw.Traceback();
            nw.PrintResult();

            Console.WriteLine();
            Console.WriteLine();

            var sw = new SmithWaterman("../../rattus_norvegicus.zfp563.faa", "../../mus_musculus.rn45s.faa");
            sw.FillScoreMatrix();
            sw.PrintResult();

            //nw.DisplayScoreMatrix();
            //sw.DisplayScoreMatrix();

            Console.ReadLine();
        }

    }
}
