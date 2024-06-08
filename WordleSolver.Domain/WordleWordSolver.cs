using Microsoft.Extensions.Logging;
using System.Globalization;
using WordleSolver.Domain.Models;

namespace WordleSolver.Domain
{
    public interface IWordleWordSolver
    {
        public List<string> GetWordleWords(List<string> wordList, string wordleString);
    }

    public class WordleWordSolver : IWordleWordSolver
    {
        private readonly ILogger<WordleWordSolver> _logger;

        public WordleWordSolver(ILogger<WordleWordSolver> logger)
        {
            _logger = logger;  
        }

        public List<string> GetWordleWords(List<string> wordList, string wordleString)
        {
            var matchGrid = ParseMatches(wordleString);

            if (matchGrid == null)
            {
                return null;
            }

            if (matchGrid.Matches.Count > 6 || matchGrid.Matches.Any(e => e.Count >5))
            {
                return null;
            }

            foreach (var matches in matchGrid.Matches)
            {
                _logger.LogInformation(String.Join("", matches));
            }

            _logger.LogInformation("====================================================");

            var wordlist = GetMatchingWords(matchGrid, wordList);

            return wordlist;
        }

        private List<string> GetMatchingWords(MatchGrid matchGrid, List<string> wordList)
        {
            matchGrid.Matches.RemoveRange(matchGrid.Matches.Count - 1, 1);

            var indexList = GetIndexList(matchGrid).OrderByDescending(e => e.Count).ToList();

            foreach (var index in indexList.Where(e => e.Any()))
            {
                wordList = GetMatchingWordsAtIndex(index.OrderByDescending(e => e).ToList(), wordList);
            }

            return wordList;
        }

        private string GetLettersAtIndexes(List<int> indexes, string word)
        {
            try
            {
                string wordResult = string.Empty;

                foreach (var index in indexes)
                {
                    wordResult += word[index];
                }
                return wordResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Indexes:{@indexes}, Word: {@word}", indexes, word);
                throw;
            }
         
        }

        private List<List<int>> GetIndexList(MatchGrid matchGrid)
        {
            int count = 0;
            var indexList = new List<List<int>>();
            var indexes = new List<int>();

            foreach (var matches in matchGrid.Matches)
            {
                foreach (var match in matches)
                {
                    if (match == "🟩")
                    {
                        indexes.Add(count);
                    }
                    count++;
                }
                indexList.Add(indexes);
                count = 0;
                indexes = new List<int>();
            }

            return indexList;
        }

        private List<string> GetMatchingWordsAtIndex(List<int> indexes, List<string> wordList)
        {
            var words = new List<string>();
            bool matches = false;

            foreach (var word in wordList)
            {
                foreach (var word2 in wordList)
                {
                    if (word != word2)
                    {
                        if (GetLettersAtIndexes(indexes, word) == GetLettersAtIndexes(indexes, word2))
                        {
                            matches = true;
                        }
                        else
                        {
                            matches = false;
                        }

                        if (matches)
                        {
                            words.Add(word);
                            matches = false;
                        }
                    }
                }
            }

            return words.Distinct().ToList();
        }

        private MatchGrid ParseMatches(string text)
        {
            const string newLine = "\n";

            int startIndex = text.IndexOf("Wordle 233 6/6\n\n");
            int endIndex = text.LastIndexOf("🟩");

            startIndex = startIndex + 4;

            if ((startIndex <= 0) || (endIndex <= 0))
            {
                return null;
            }

            string grid = text.Substring(startIndex, endIndex - (startIndex - 2));

            MatchGrid matchGrid = new MatchGrid();
            matchGrid.Matches = new List<List<string>>();
            List<string> row = new List<string>();

            var gridElements = SplitIntoTextElements(grid);

            foreach (var item in gridElements)
            {
                switch (item)
                {
                    case "⬛":
                        row.Add("⬛");
                        break;
                    case "⬜":
                        row.Add("⬛");
                        break;
                    case "⬜️":
                        row.Add("⬛");
                        break;
                    case "\U0001f7e9":
                        row.Add("🟩");
                        break;
                    case "\U0001f7e8":
                        row.Add("🟨");
                        break;
                    case newLine:
                        {
                            matchGrid.Matches.Add(row);
                            row = new List<string>();
                        }
                        break;
                }
            }

            matchGrid.Matches.Add(row);
            matchGrid.Matches.RemoveAll(e => e.Count == 0);

            return matchGrid;
        }

        private string[] SplitIntoTextElements(string input)
        {
            IEnumerable<string> Helper()
            {
                for (var en = StringInfo.GetTextElementEnumerator(input); en.MoveNext();)
                    yield return en.GetTextElement();
            }
            return Helper().ToArray();
        }
    }
}
