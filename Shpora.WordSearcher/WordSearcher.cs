using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Shpora.WordSearcher
{

    class WordSearcher
    {
        public static HashSet<string> RecognizedWords = new HashSet<string>();
        static void Main(string[] args)
        {
            //var uri = args[0];
            //var token = $"token {args[1]}";
            var uri = "http://shpora.skbkontur.ru";
            var token = "token eeccc7a9-a097-422e-8fb5-86cf05a37c4d";
            var alphabet = new Alphabet().LoadAlphabet();
            using (var gameSession = new GameSession(uri, token))
            {
                try
                {
                    gameSession.InitGameSession();
                    Console.WriteLine($"init {DateTime.Now}");
                    var explorer = new TorusExplorer(gameSession);
                    var recognizer = new WordRecognizer(gameSession, alphabet);
                    while (true)
                    {
                        explorer.FindWord();
                        var word = recognizer.ScanWord();
                        Console.WriteLine(word);
                        if (RecognizedWords.Contains(word))
                            break;
                        if (word != String.Empty && !RecognizedWords.Contains(word))
                            RecognizedWords.Add(word);
                        explorer.MoveFromWord();
                    }
                    Console.WriteLine(gameSession.GetGameStatistics());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}

