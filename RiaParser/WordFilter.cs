using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace RiaParser
{
    class WordFilter
    {
        private HashSet<string> prepositions;
        private HashSet<string> pronouns;
        private HashSet<string> conjunctions;

        public WordFilter()
        {
            prepositions = ReadWords("Prepositions.txt");
            pronouns = ReadWords("Pronouns.txt");
            conjunctions = ReadWords("Conjunctions.txt");
        }

        public bool IsUseless(string word)
        {
            if (word.Length <= 2 
                || word.All(x => char.IsDigit(x)) 
                || prepositions.Contains(word)
                || pronouns.Contains(word)
                || conjunctions.Contains(word))
            {
                return true;
            }
            return false;
        }

        private HashSet<string> ReadWords(string fileName)
        {
            return new HashSet<string>(File.ReadAllLines(fileName));
        }


    }
}
