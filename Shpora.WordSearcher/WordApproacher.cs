namespace Shpora.WordSearcher
{
    class WordApproacher
    {
        private readonly GameSession GameSession;
        public WordApproacher(GameSession gameSession)
        {
            GameSession = gameSession;
        }

        public void MakeApproach()
        {
            if (!GameSession.CurrentWindow.IsLeftBorder && GameSession.CurrentWindow.IsRightZeros)
                ApproachLeft();
            else if (GameSession.CurrentWindow.IsUpZeros && !GameSession.CurrentWindow.IsDownZeros)
                ApproachDown();
            else if (GameSession.CurrentWindow.IsLeftBorder && !GameSession.CurrentWindow.IsRightBorder)
                ApproachRight();
            else if ((!GameSession.CurrentWindow.IsUpZeros || !GameSession.CurrentWindow.IsUpBorder) && GameSession.CurrentWindow.IsDownZeros)
                ApproachUp();
            else if (!GameSession.CurrentWindow.IsUpZeros && (!GameSession.CurrentWindow.IsLeftBorder || !GameSession.CurrentWindow.IsRightBorder))
                ApproachMiddle();
        }

        private void ApproachDown()
        {
            while (GameSession.CurrentWindow.IsUpZeros && !GameSession.CurrentWindow.IsDownZeros)
                GameSession.Move(Directions.Down);
            while (GameSession.CurrentWindow.IsLeftZeros)
                GameSession.Move(Directions.Right);
            ApproachTopLeftCorner();
        }

        private void ApproachMiddle()
        {
            while (!GameSession.CurrentWindow.IsLeftBorder)
                GameSession.Move(Directions.Left);
            while (!GameSession.CurrentWindow.IsUpZeros)
                GameSession.Move(Directions.Up);
            ApproachTopLeftCorner();
        }

        private void ApproachUp()
        {
            while (!GameSession.CurrentWindow.IsUpZeros)
                GameSession.Move(Directions.Up);
            while (GameSession.CurrentWindow.IsLeftBorder)
                GameSession.Move(Directions.Right);
            GameSession.Move(Directions.Down);
            ApproachTopLeftCorner();
        }

        private void ApproachRight()
        {
            while (GameSession.CurrentWindow.IsLeftZeros)
                GameSession.Move(Directions.Right);
            while (GameSession.CurrentWindow.IsUpZeros)
                GameSession.Move(Directions.Down);
            while (!GameSession.CurrentWindow.IsUpZeros)
                GameSession.Move(Directions.Up);
            ApproachTopLeftCorner();
        }

        private void ApproachLeft()
        {
            while (!GameSession.CurrentWindow.IsLeftBorder)
                GameSession.Move(Directions.Left);
            if (GameSession.CurrentWindow.IsDownZeros)
                while (!GameSession.CurrentWindow.IsUpZeros)
                    GameSession.Move(Directions.Up);
            else if (GameSession.CurrentWindow.IsUpZeros)
                while (GameSession.CurrentWindow.IsUpZeros)
                    GameSession.Move(Directions.Down);
            ApproachTopLeftCorner();
        }

        private void ApproachTopLeftCorner()
        {
            while (!GameSession.CurrentWindow.IsLeftQuater)
                GameSession.Move(Directions.Left);
            while (!GameSession.CurrentWindow.IsDownZeros)
                GameSession.Move(Directions.Down);

            if (GameSession.CurrentWindow.IsLeftQuater)
            {
                while (!GameSession.CurrentWindow.IsUpZeros)
                    GameSession.Move(Directions.Up);
                for (var i = 0; i < 4; i++)
                    GameSession.Move(Directions.Right);
                return;
            }

            while (!GameSession.CurrentWindow.IsLeftBorder)
                GameSession.Move(Directions.Left);
            while (GameSession.CurrentWindow.IsLeftZeros)
                GameSession.Move(Directions.Right);
            while (!GameSession.CurrentWindow.IsUpZeros)
                GameSession.Move(Directions.Up);
            GameSession.Move(Directions.Down);
        }
    }
}
