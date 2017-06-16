using System;
using System.Linq;
using System.Text;

namespace SeqAlignTest
{
    public class SmithWaterman
    {
        private string firstSequence { get; set; }
        private string secondSequence { get; set; }
        private int gapPenalty { get; set; }
        private int matchScore { get; set; }
        private int mismatchScore { get; set; }
        public int[] initialSize { get; set; }

        public int firstSequenceCounter { get; set; }
        public int secondSequenceCounter { get; set; }
        public double percScore { get; set; }
        public int[] score { get; set; }
        public int[,] scoreMatrix { get; set; }
        public string alignmentFirstSequence { get; set; }
        public string alignmentSecondSequence { get; set; }

        public double executionTime { get; set; }

        public SmithWaterman(string firstSequenceFile, string secondSequenceFile)
        {
            firstSequence = Util.ReadFasta(firstSequenceFile);
            secondSequence = Util.ReadFasta(secondSequenceFile);

            initialSize = new int[2] { firstSequence.Count(), secondSequence.Count() };

            scoreMatrix = new int[firstSequence.Length + 1, secondSequence.Length + 1];

            firstSequenceCounter = firstSequence.Length + 1;
            secondSequenceCounter = secondSequence.Length + 1;

            score = new int[2];
            percScore = 0.0;

            gapPenalty = -1;
            matchScore = 2;
            mismatchScore = -1;

            alignmentFirstSequence = string.Empty;
            alignmentSecondSequence = string.Empty;
        }

        public void DisplayScoreMatrix()
        {
            Console.Write("        ");
            foreach (var c in firstSequence)
            {
                if(c != '-')
                    Console.Write(c + "  ");
            }
            Console.WriteLine();
            for (int i = 0; i < secondSequenceCounter; i++)
            {
                if(i != 0)
                    Console.Write(secondSequence[i - 1]);
                else
                    Console.Write(" ");

                Console.Write("  ");
                for (int j = 0; j < firstSequenceCounter; j++)
                {
                    if (scoreMatrix[j, i] >= 0)
                        Console.Write(" ");
                    Console.Write(" " + scoreMatrix[j, i]);
                }
                Console.Write(Environment.NewLine);
            }
        }

        private int ScoreFunction(char a, char b, int matchScore, int mismatchScore)
        {
            return a == b ? matchScore : mismatchScore;
        }
        
        public void FillScoreMatrix()
        {
            DateTime initialTime = DateTime.Now;
            char[,] tracebackMatrix = new char[firstSequence.Length + 1, secondSequence.Length + 1];
            scoreMatrix[0, 0] = 0;
            
            for (int i = 1; i < firstSequence.Length + 1; i++)
            {
                scoreMatrix[i, 0] = 0;
                tracebackMatrix[i, 0] = '0';
            }
            
            for (int i = 1; i < secondSequence.Length + 1; i++)
            {
                scoreMatrix[0, i] = 0;
                tracebackMatrix[0, i] = '0';
            }


            for (int i = 1; i < firstSequence.Length + 1; i++)
            {
                for (int j = 1; j < secondSequence.Length + 1; j++)
                {
                    int diagonal = scoreMatrix[i - 1, j - 1] + ScoreFunction(firstSequence[i - 1], secondSequence[j - 1], matchScore, mismatchScore);
                    int links = scoreMatrix[i - 1, j] + gapPenalty;
                    int oben = scoreMatrix[i, j - 1] + gapPenalty;

                    scoreMatrix[i, j] = Math.Max(Math.Max(oben, Math.Max(links, diagonal)),0);

                    if (scoreMatrix[i,j] > scoreMatrix[score[0],score[1]])
                    {
                        score[0] = i;
                        score[1] = j;
                    }

                    if (scoreMatrix[i, j] == diagonal && i > 0 && j > 0)
                    {
                        tracebackMatrix[i, j] = 'D';
                    }
                    else if (scoreMatrix[i, j] == links)
                    {
                        tracebackMatrix[i, j] = 'L';
                    }
                    else if (scoreMatrix[i, j] == oben)
                    {
                        tracebackMatrix[i, j] = 'U';
                    }
                    else if (scoreMatrix[i, j] == 0)
                    {
                        tracebackMatrix[i, j] = '0';
                    }
                }
            }

            TraceBack(tracebackMatrix, score, firstSequence, secondSequence);
            executionTime = (DateTime.Now - initialTime).TotalSeconds;
        }

        private void TraceBack(char[,] tracebackMatrix, int[] score, string sequenzA, string sequenzB)
        {
            int i = score[0];
            int j = score[1];

            StringBuilder alignedSeqA = new StringBuilder();
            StringBuilder alignedSeqB = new StringBuilder();
            
            while (tracebackMatrix[i, j] != '0')
            {
                switch (tracebackMatrix[i, j])
                {
                    case 'D':
                        alignedSeqA.Append(sequenzA[i - 1]);
                        alignedSeqB.Append(sequenzB[j - 1]);
                        i--;
                        j--;
                        break;
                    case 'U':
                        alignedSeqA.Append("-");
                        firstSequence += "-";
                        alignedSeqB.Append(sequenzB[j - 1]);
                        j--;
                        break;
                    case 'L':
                        alignedSeqA.Append(sequenzA[i - 1]);
                        alignedSeqB.Append("-");
                        i--;
                        break;

                }
            }

            alignmentFirstSequence = new string(alignedSeqA.ToString().Reverse().ToArray());
            alignmentSecondSequence = new string(alignedSeqB.ToString().Reverse().ToArray());
            percScore = alignmentSecondSequence.Where(c => c != '-').Count();

        }


        public void PrintResult()
        {
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Smith Waterman");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Tempo de execucao:");
            Console.WriteLine($"{ executionTime } segundos");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Tamanho da sequencia antes do alinhamento:");
            Console.WriteLine($"1a Sequencia - { initialSize[0] }   |   2a Sequencia - { initialSize[1] }");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine("Tamanho da sequencia apos alinhamento:");
            Console.WriteLine($"{ (firstSequence.Count() + 1) }");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"Semelhanca entre as sequencias:");
            Console.WriteLine($"{ percScore * 100.0 / (firstSequence.Count() + 1) } %");
            //Console.WriteLine($"{ percScore } - { (firstSequence.Count() + 1) } %");
            Console.WriteLine("----------------------------------------------------");
            Console.WriteLine($"Score:");
            Console.WriteLine($"{ scoreMatrix[score[0], score[1]] }");
            Console.WriteLine("----------------------------------------------------");
        }

    }
}
