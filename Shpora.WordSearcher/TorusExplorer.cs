using System;
using System.Collections.Generic;

namespace Shpora.WordSearcher
{
    public enum Directions
    {
        Left,
        Right,
        Up,
        Down
    }

    class TorusExplorer
    {
        private readonly GameSession GameSession;
        private readonly WordApproacher Approacher;
        private readonly IEnumerable<Directions> DirectionGenerator;

        public TorusExplorer(GameSession gameSession)
        {
            GameSession = gameSession;
            Approacher = new WordApproacher(gameSession);
            DirectionGenerator = GenerateDirection();
        }

        public void FindWord()
        {
            FindPattern();
            Approacher.MakeApproach();
        }

        public void MoveFromWord()
        {
            for (var i = 0; i < 8; i++)
                GameSession.Move(Directions.Up);
        }

        private void FindPattern()
        {
            while (GameSession.CurrentWindow.Points.Count == 0)
                foreach (var direction in DirectionGenerator)
                {
                    for (var i = 0; i < 30; i++)
                    {
                        if (GameSession.CurrentWindow.Points.Count == 0)
                            GameSession.Move(direction);
                        else return;
                    }
                }
        }

        private IEnumerable<Directions> GenerateDirection()
        {
            for (var i = 0; i < 5; i++)
                yield return Directions.Right;
            yield return Directions.Up;
        }

    }
}
