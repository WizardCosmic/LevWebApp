using System.Runtime.ConstrainedExecution;

namespace LevWebApp.Models
{
    public class NamePair
    {
        public decimal FinalScore { get; set; }
        public string TargetName { get; set; }
        public string SourceName { get; set; }
        public int SumOfWords { get; set; }
        public int SumOfDistances { get; set; }
        public List<WordPair> WordPairs { get; set; } = new();
    }
}
