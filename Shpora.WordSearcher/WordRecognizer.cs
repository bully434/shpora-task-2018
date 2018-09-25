using System;
using System.Collections.Generic;
using System.Linq;

namespace Shpora.WordSearcher
{
    class WordRecognizer
    {
        private readonly GameSession GameSession;
        private readonly Dictionary<HashSet<Point>, char> Alphabet;

        public WordRecognizer(GameSession requests, Dictionary<HashSet<Point>, char> alphabet)
        {
            GameSession = requests;
            Alphabet = alphabet;
        }

        public string ScanWord()
        {
            var recognizedLetters = new List<char>();
            while (true)
            {
                var letter = ScanLetterDown();
                if (letter == default(char))
                    break;
                recognizedLetters.Add(letter);

                for (var i = 0; i < 8; i++)
                    GameSession.Move(Directions.Right);

                letter = ScanLetterUp();
                if (letter == default(char))
                    break;
                recognizedLetters.Add(letter);

                for (var i = 0; i < 8; i++)
                    GameSession.Move(Directions.Right);
            }
            return string.Concat(recognizedLetters);
        }

        private char ScanLetterDown()
        {
            GameSession.Move(Directions.Down);
            while (!GameSession.CurrentWindow.IsUpZeros)
                GameSession.Move(Directions.Up, true);
            var scannedPattern = GameSession.CurrentWindow;
            while (!GameSession.CurrentWindow.IsDownZeros)
            {
                GameSession.Move(Directions.Down, true);
                scannedPattern.AddRow(GameSession.CurrentWindow.Points, Directions.Down);
            }
            return RecognizeLetter(scannedPattern);
        }

        private char ScanLetterUp()
        {
            GameSession.Move(Directions.Up, true);
            var scannedPattern = GameSession.CurrentWindow;
            while (!GameSession.CurrentWindow.IsUpZeros)
            {
                GameSession.Move(Directions.Up, true);
                scannedPattern.AddRow(GameSession.CurrentWindow.Points, Directions.Up);
            }
            return RecognizeLetter(scannedPattern);
        }

        private char RecognizeLetter(SlidingWindow pattern)
        {
            pattern.SortPoints();
            var scannedLetter = pattern.CropLetter();
            Alphabet.TryGetValue(scannedLetter, out var letter);
            return letter;
        }

    }
}
