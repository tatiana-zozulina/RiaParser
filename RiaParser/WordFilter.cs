using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RiaParser
{
    class WordFilter
    {
        private List<string> prepositions;
        private List<string> pronouns;
        private List<string> conjunctions;

        public WordFilter()
        {
            prepositions = ReadWords("Prepositions.txt");
            pronouns = ReadWords("Pronouns.txt");
            conjunctions = ReadWords("Conjunctions.txt");
        }

        public static bool IsUseless(string word)
        {
            var filter = new WordFilter();
            if (word.Length <= 2 
                || word.All(x => char.IsDigit(x)) 
                || filter.prepositions.Contains(word)
                || filter.pronouns.Contains(word)
                || filter.conjunctions.Contains(word))
            {
                return true;
            }
            return false;
        }

        private List<string> ReadWords(string fileName)
        {
            return File.ReadAllLines(fileName).ToList();
        }


    }
}
