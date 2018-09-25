using System;
using System.Collections.Generic;
using System.Linq;

namespace Shpora.WordSearcher
{
    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override bool Equals(object obj)
        {
            var point = obj as Point;
            return X == point.X && Y == point.Y;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return X * 1023 + Y;
            }
        }
    }

    public class SlidingWindow
    {
        public HashSet<Point> Points;

        public bool IsLeftZeros { get; private set; } = true;  
        public bool IsLeftBorder { get; private set; } = true;
        public bool IsLeftQuater { get; private set; } = true;

        public bool IsUpZeros { get; private set; } = true;
        public bool IsUpBorder { get; private set; } = true;

        public bool IsRightZeros { get; private set; } = true;
        public bool IsRightBorder { get; private set; } = true;

        public bool IsDownZeros { get; private set; } = true;
        public bool IsDownBorder { get; private set; } = true;

        public SlidingWindow(string content)
        {
            var lines = content.Split(new string[] {"\r\n"}, StringSplitOptions.None);
            Points = new HashSet<Point>();
            InitWindow(lines);
            CheckBorders();
        }

        public void UpdateWindow(string content, Directions dir)
        {
            var lines = content.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            switch (dir)
            {
                case Directions.Up:
                    UpdateWindowTop(lines);
                    break;
                case Directions.Down:
                    UpdateBottomEdge(lines);
                    break;
                case Directions.Right:
                    UpdateRightEdge(lines);
                    break;
                case Directions.Left:
                    UpdateLeftEdge(lines);
                    break;
            }
            CheckBorders();
        }

        private void InitWindow(string[] lines)
        {
            var rowsCount = lines.Count();
            var columnsCount = lines[0].Length;
            for (var i = 0; i < rowsCount; i++)
            {
                var line = lines[i].Select(c => c == '1').ToArray();
                for (var j = 0; j < columnsCount; j++)
                    if (line[j]) Points.Add(new Point(i, j));
            }
        }

        private void CheckBorders()
        {
            IsUpZeros = Points.All(x => x.X > 0);
            IsUpBorder = Points.All(x => x.X > 2);

            IsDownZeros = Points.All(x => x.X < 4);
            IsDownBorder = Points.All(x => x.X < 2);

            IsRightZeros = Points.All(x => x.Y < 10);
            IsRightBorder = Points.All(x => x.Y < 8);

            IsLeftZeros = Points.All(x => x.Y > 0);
            IsLeftBorder = Points.All(x => x.Y > 2);
            IsLeftQuater = Points.All(x => x.Y > 3);
        }


        private void UpdateWindowTop(string[] lines)
        {
            Points.RemoveWhere(x => x.X == 4);
            foreach (var point in Points)
                point.X += 1;
            var line = lines.First().Select(c => c == '1').ToArray();
            for (var j = 0; j < line.Length; j++)
                if (line[j]) Points.Add(new Point(0, j));
        }

        private void UpdateBottomEdge(string[] lines)
        {
            Points.RemoveWhere(x => x.X == 0);
            foreach (var point in Points)
                point.X -= 1;
            var lastRow = lines.Count();
            var line = lines.Last().Select(c => c == '1').ToArray();
            for (var j = 0; j < line.Length; j++)
                if (line[j]) Points.Add(new Point(lastRow, j));
        }

        private void UpdateLeftEdge(string[] lines)
        {
            var lastColumn = lines.First().Length;
            Points.RemoveWhere(x => x.Y == lastColumn);
            foreach (var point in Points)
                point.Y += 1;
            for (var i = 0; i < lines.Count(); i++)
                if (lines[i].First() == '1') Points.Add(new Point(i, 0));
        }

        private void UpdateRightEdge(string[] lines)
        {
            Points.RemoveWhere(x => x.Y == 0);
            foreach (var point in Points)
                point.Y -= 1;
            var lastColumn = lines.First().Length;
            for (var i = 0; i < lines.Count(); i++)
                if (lines[i].Last() == '1')
                    Points.Add(new Point(i, lastColumn));
        }

        public void AddRow(HashSet<Point> currentWindow, Directions direction)
        {
            if (direction == Directions.Down)
            {
                var newPoints = currentWindow.Where(x => x.X == 4 && x.Y < 7);
                var row = Points.Last().X + 1;
                foreach (var point in newPoints)
                    Points.Add(new Point(row, point.Y));
            }
            else if (direction == Directions.Up)
            {
                var newPoints = currentWindow.Where(x => x.X == 0 && x.Y < 7);
                foreach (var point in Points)
                    point.X += 1;
                foreach (var point in newPoints)
                    Points.Add(point);
            }
        }

        public HashSet<Point> SortPoints()
        {
            return new HashSet<Point>(Points.OrderBy(x => x.X));
        }

        public HashSet<Point> CropLetter()
        {
            foreach (var point in Points)
                point.X -= 1;
            return new HashSet<Point>(Points.Where(x => x.X < 8 && x.Y < 7));
        }

    }
}
