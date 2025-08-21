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
            foreach (var pair in wordPairs)
            {
                // normalize once so comparisons are case-insensitive like before
                var s = pair.SourceWord?.ToLowerInvariant() ?? string.Empty;
                var t = pair.TargetWord?.ToLowerInvariant() ?? string.Empty;

                pair.levdistance = LevenshteinMemo(s, t);
            }
            return wordPairs;


        }

        private static int LevenshteinMemo(string a, string b)
        {
            if (a is null) a = string.Empty;
            if (b is null) b = string.Empty;

            int m = a.Length, n = b.Length;

            // Optional very quick outs
            if (m == 0) return n;
            if (n == 0) return m;
            if (ReferenceEquals(a, b) || a == b) return 0;

            // -1 means "not computed"
            var memo = new int[m + 1, n + 1];
            for (int i = 0; i <= m; i++)
                for (int j = 0; j <= n; j++)
                    memo[i, j] = -1;

            return LevenshteinMemoRec(a, b, m, n, memo);
        }

        private static int LevenshteinMemoRec(string a, string b, int i, int j, int[,] memo)
        {
            // already computed?
            int cached = memo[i, j];
            if (cached >= 0) return cached;

            int res;
            if (i == 0) res = j;
            else if (j == 0) res = i;
            else if (a[i - 1] == b[j - 1])
                res = LevenshteinMemoRec(a, b, i - 1, j - 1, memo);
            else
            {
                int insert = LevenshteinMemoRec(a, b, i, j - 1, memo);
                int remove = LevenshteinMemoRec(a, b, i - 1, j, memo);
                int replace = LevenshteinMemoRec(a, b, i - 1, j - 1, memo);
                res = 1 + Math.Min(Math.Min(insert, remove), replace);
            }

            memo[i, j] = res;
            return res;
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
