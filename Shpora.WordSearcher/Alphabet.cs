using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Shpora.WordSearcher
{
    class Alphabet
    {
        private HashSet<Point> ConvertToHashSet(string[] lines)
        {
            var points = new HashSet<Point>();
            var rowsCount = lines.Length;
            var columnsCount = lines[0].Length;
            for (var i = 0; i < rowsCount; i++)
            {
                var line = lines[i].Select(c => c == '1').ToArray();
                for (var j = 0; j < columnsCount; j++)
                    if (line[j])
                        points.Add(new Point(i, j));
            }
            return points;
        }

        public Dictionary<HashSet<Point>, char> LoadAlphabet()
        {
            var alphabet = new Dictionary<HashSet<Point>, char>(HashSet<Point>.CreateSetComparer());
            var directory = new DirectoryInfo("./Alphabet/");
            foreach (var file in directory.GetFiles("*.txt"))
            {
                using (StreamReader fi = File.OpenText(file.FullName))
                {
                    var content = fi.ReadToEnd();
                    var lines = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    var window = ConvertToHashSet(lines);
                    alphabet[window] = file.Name[0];
                }
            }
            return alphabet;
        }
    }
}

