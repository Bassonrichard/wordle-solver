using System.Text.Json;
using WordleSolver.Domain;
using WordleSolver.Domain.Models;

namespace WordleSolver.WorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IWordleWordSolver _wordleWordSolver;
        private readonly ITwitterService _twitterService;

        public Worker(ILogger<Worker> logger, IWordleWordSolver wordleWordSolver, ITwitterService twitterService)
        {
            _logger = logger;
            _wordleWordSolver = wordleWordSolver;
            _twitterService = twitterService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var wordList = WordleWords.words;
            List<string> words = new List<string>();
            int viableMatches = 0;
            const string wordleWordNumber = "234";
            const string wordleWordAnswer = "frame";

            var data = await _twitterService.GetTwitterSearch($"Wordle%20{wordleWordNumber}%206%2F6");

            foreach (var dataItems in data.Data)
            {
                _logger.LogInformation("---------------");
                _logger.LogInformation(dataItems.Id);
                _logger.LogInformation("---------------");
                words = _wordleWordSolver.GetWordleWords(wordList, dataItems.Text);
                if (words != null && words.Any())
                {
                    wordList = wordList.Intersect(words).ToList();
                    viableMatches++;

                    if (wordList.Contains(wordleWordAnswer))
                        _logger.LogInformation("Contains Answer");

                    _logger.LogInformation("Word Count: {@WordCount}", wordList.Count);
                }
            }

            if (wordList.Contains(wordleWordAnswer))
                _logger.LogInformation("Contains Answer");

            foreach (var word in wordList.OrderBy(e => e))
            {
                _logger.LogInformation(word);
            }

            _logger.LogInformation($"WordCount: {wordList.Count}");
            _logger.LogInformation($"Viable Matches: {viableMatches}");
        }
    }
}