using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace LevWebApp
{
    internal class Calculator
    {


        public WordPair[] CalcLevDistance(WordPair[] wordPairs)
        {
            foreach (WordPair pair in wordPairs)
            {
                pair.levdistance = LevenshteinRecursive(pair.SourceWord.ToLower(), pair.TargetWord.ToLower(), pair.SourceWord.Length, pair.TargetWord.Length);

            }

            return wordPairs;


        }

        static int LevenshteinRecursive(string str1, string str2, int m, int n)
        {
            // If str1 is empty, the distance is the length of str2
            if (m == 0)
            {
                return n;
            }

            // If str2 is empty, the distance is the length of str1
            if (n == 0)
            {
                return m;
            }

            // If the last characters of the strings are the same
            if (str1[m - 1] == str2[n - 1])
            {
                return LevenshteinRecursive(str1, str2, m - 1, n - 1);
            }

            // Calculate the minimum of three operations:
            // Insert, Remove, and Replace
            return 1 + Math.Min(
                Math.Min(
                    // Insert
                    LevenshteinRecursive(str1, str2, m, n - 1),
                    // Remove
                    LevenshteinRecursive(str1, str2, m - 1, n)
                ),
                // Replace
                LevenshteinRecursive(str1, str2, m - 1, n - 1)
            );
        }

        public WordPair[] CalcDistances(WordPair[] wordPairs)
        {
            foreach (WordPair pair in wordPairs)
            {
                pair.sourcedistance = Math.Min(pair.levdistance, pair.SourceWord.Length);
                pair.targetdistance = Math.Min(pair.levdistance, pair.TargetWord.Length);
                pair.totaldistance = pair.targetdistance + pair.sourcedistance;
            }


            return wordPairs;
        }
        public WordPair[] CalcIndividualScore(WordPair[] wordPairs)
        {

            foreach (WordPair pair in wordPairs)
            {
                decimal firstscore = 0;
                decimal td = pair.totaldistance;
                decimal tw = pair.TargetWord.Length;
                decimal sw = pair.SourceWord.Length;
                if (td == (decimal)0 || tw == (decimal)0 || sw == (decimal)0)
                {
                    firstscore = 0;
                }
                else
                {
                    firstscore = (td / (tw + sw));
                }

                pair.InitialScore = Convert.ToDecimal(1) - firstscore;

            }
            return wordPairs;
        }

        public WordPair[] FindScored(WordPair[] wordPairs, int totalScored)
        {
            List<WordPair> list = wordPairs.ToList();

            var qry = from w in list
                      orderby w.InitialScore
                      select w;

            wordPairs = qry.ToArray();
            wordPairs.Reverse();
            List<int> usedsource = new List<int>();
            List<int> usedtarget = new List<int>();
            int k = wordPairs.Length - 1;
            for (int i = 0; i < wordPairs.Length; i++)
            {
                if (usedsource.Contains(wordPairs[k].SourceID) == false && usedtarget.Contains(wordPairs[k].TargetID) == false && wordPairs[k].excluded == false)
                {
                    wordPairs[k].scored = true;
                    usedtarget.Add(wordPairs[k].TargetID);
                    usedsource.Add(wordPairs[k].SourceID);

                }

                k--;

            }

            return wordPairs;


        }

        public (decimal, int, int) FindTotalScore(WordPair[] wordPairs)

        {
            int S = 0;
            int D = 0;
            decimal totalScore;
            List<WordPair> ScorePairs = new List<WordPair>();
            foreach (WordPair wordPair in wordPairs)
            {
                if (wordPair.scored == true)
                {
                    ScorePairs.Add(wordPair);
                }

            }


            foreach (WordPair wordPair in ScorePairs)
            {
                S = S + wordPair.TargetWord.Length + wordPair.SourceWord.Length;
                D = D + wordPair.totaldistance;
            }


            totalScore = (S - D) * 100;
            if (S == 0)
            {
                S = 1;
            }
            totalScore = totalScore / S;



            return (totalScore, S, D);
        }
    }
}
