using System.IO;
using System.Text;

namespace SeqAlignTest
{
    public static class Util
    {
        public static string ReadFasta(string fileName)
        {
            var readerFileOne = new StreamReader(fileName);
            string stringBuilder = "";

            while (true)
            {
                var line = readerFileOne.ReadLine();
                if (string.IsNullOrEmpty(line))
                    break;
                if (line.StartsWith(">"))
                    continue;
                else
                    stringBuilder += line;
            }

            return stringBuilder;
        }
    }
}
