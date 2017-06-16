using System;
using System.Linq;

namespace SeqAlignTest
{
    public class NeedlemanWunsch
    {
        private string firstSequence { get; set; }
        private string secondSequence { get; set; }
        public int[] initialSize { get; set; }

        public int firstSequenceCounter { get; set; }
        public int secondSequenceCounter { get; set; }
        public double percScore { get; set; }
        public int[,] scoreMatrix { get; set; }
        public string alignmentFirstSequence { get; set; }
        public string alignmentSecondSequence { get; set; }

        public double executionTime { get; set; }

        public NeedlemanWunsch(string firstSequenceFile, string secondSequenceFile)
        {
            firstSequence = Util.ReadFasta(firstSequenceFile);
            secondSequence = Util.ReadFasta(secondSequenceFile);

            initialSize = new int[2] { firstSequence.Count(), secondSequence.Count() };
            
            firstSequenceCounter = firstSequence.Length + 1;
            secondSequenceCounter = secondSequence.Length + 1;
            percScore = 0.0;

            scoreMatrix = new int[secondSequenceCounter, firstSequenceCounter];
            
            alignmentFirstSequence = string.Empty;
            alignmentSecondSequence = string.Empty;
        }

        public void InitializeScoreMatrix()
        {
            for (int i = 0; i < secondSequenceCounter; i++)
            {
                for (int j = 0; j < firstSequenceCounter; j++)
                    scoreMatrix[i, j] = 0;
            }
        }

        public void FillScoreMatrix()
        {
            DateTime initialTime = DateTime.Now;
            for (int i = 1; i < secondSequenceCounter; i++)
            {
                for (int j = 1; j < firstSequenceCounter; j++)
                {
                    int scroeDiag = 0;
                    if (firstSequence.Substring(j - 1, 1) == secondSequence.Substring(i - 1, 1))
                        scroeDiag = scoreMatrix[i - 1, j - 1] + 2;                    // +2 para Match
                    else
                        scroeDiag = scoreMatrix[i - 1, j - 1] + -1;                   // -1 pra Missmatch

                    int scroeLeft = scoreMatrix[i, j - 1] - 2;                        // -2 pra Gap
                    int scroeUp = scoreMatrix[i - 1, j] - 2;

                    int maxScore = Math.Max(Math.Max(scroeDiag, scroeLeft), scroeUp);

                    scoreMatrix[i, j] = maxScore;
                }
            }
            executionTime = (DateTime.Now - initialTime).TotalSeconds;
        }

        public void DisplayScoreMatrix()
        {
            for (int i = 0; i < secondSequenceCounter; i++)
            {
                Console.Write("  ");
                for (int j = 0; j < firstSequenceCounter; j++)
                {
                    if (scoreMatrix[i, j] >= 0)
                        Console.Write(" ");
                    Console.Write(" " + scoreMatrix[i, j]);
                }
                Console.Write(Environment.NewLine);
            }
        }

        public void Traceback()
        {
            DateTime initialTime = DateTime.Now;
            char[] firstSequenceTracebackArray = secondSequence.ToCharArray();
            char[] secondSequenceTracebackArray = firstSequence.ToCharArray();

            int m = secondSequenceCounter - 1;
            int n = firstSequenceCounter - 1;
            while (m > 0 || n > 0)
            {
                int scroeDiag = 0;

                if (m == 0 && n > 0)
                {
                    alignmentFirstSequence = secondSequenceTracebackArray[n - 1] + alignmentFirstSequence;
                    alignmentSecondSequence = "-" + alignmentSecondSequence;
                    n = n - 1;
                }
                else if (n == 0 && m > 0)
                {
                    alignmentFirstSequence = "-" + alignmentFirstSequence;
                    alignmentSecondSequence = firstSequenceTracebackArray[m - 1] + alignmentSecondSequence;
                    m = m - 1;
                }
                else
                {
                    if (firstSequenceTracebackArray[m - 1] == secondSequenceTracebackArray[n - 1])
                        scroeDiag = 2;
                    else
                        scroeDiag = -1;

                    if (m > 0 && n > 0 && scoreMatrix[m, n] == scoreMatrix[m - 1, n - 1] + scroeDiag)
                    {
                        percScore += 1;
                        alignmentFirstSequence = secondSequenceTracebackArray[n - 1] + alignmentFirstSequence;
                        alignmentSecondSequence = firstSequenceTracebackArray[m - 1] + alignmentSecondSequence;
                        m = m - 1;
                        n = n - 1;
                    }
                    else if (n > 0 && scoreMatrix[m, n] == scoreMatrix[m, n - 1] - 2)
                    {
                        alignmentFirstSequence = secondSequenceTracebackArray[n - 1] + alignmentFirstSequence;
                        alignmentSecondSequence = "-" + alignmentSecondSequence;
                        n = n - 1;
                    }
                    else //if (m > 0 && scoringMatrix[m, n] == scoringMatrix[m - 1, n] + -2)
                    {
                        alignmentFirstSequence = "-" + alignmentFirstSequence;
                        alignmentSecondSequence = firstSequenceTracebackArray[m - 1] + alignmentSecondSequence;
                        m = m - 1;
                    }
                }
            }
            executionTime = (DateTime.Now - initialTime).TotalSeconds + executionTime;
        }

        public void PrintResult()
        {
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Needleman Wusch");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Tempo de execucao:");
            Console.WriteLine($"{ executionTime } segundos");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Tamanho da sequencia antes do alinhamento:");
            Console.WriteLine($"1a Sequencia - { initialSize[0] }   |   2a Sequencia - { initialSize[1] }");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Tamanho da sequencia apos alinhamento:");
            Console.WriteLine($"{ alignmentFirstSequence.Count() }");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"Semelhanca entre as sequencias:");
            Console.WriteLine($"{ percScore * 100.0 / (alignmentFirstSequence.Count() + 1) } %");
            //Console.WriteLine($"{ percScore } - { (alignmentFirstSequence.Count() + 1) } %");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"Score:");
            Console.WriteLine($"{ scoreMatrix[secondSequenceCounter - 1, firstSequenceCounter - 1] }");
            Console.WriteLine("----------------------------------------------------");
        }

    }
}
