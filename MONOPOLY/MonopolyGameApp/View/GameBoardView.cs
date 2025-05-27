using Microsoft.Maui.Graphics;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;

namespace Monopoly.MauiUI.Views
{
    public class GameBoardView : GraphicsView
    {
        public List<(string Name, int CellIndex, Color Color)> PlayerTokens { get; set; } = new();

        private Dictionary<string, Color> _playerColors = new();
        private readonly Color[] _availableColors = new[] { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow };
        private int _colorIndex = 0;
        public Dictionary<int, Color> OwnedCells { get; } = new();
        public GameBoardView()
        {
            Drawable = new GameBoardDrawable(this);
        }
        public void MarkCellOwned(int index, Color color)
        {
            OwnedCells[index] = color;
        }

        public Color GetPlayerColor(string playerName) => _playerColors[playerName];
        public void Refresh() => Invalidate();

        public void MovePlayer(string playerName, int newCellIndex)
        {
            if (!_playerColors.ContainsKey(playerName))
            {
                var color = _availableColors[_colorIndex % _availableColors.Length];
                _playerColors[playerName] = color;
                _colorIndex++;
            }

            var colorForPlayer = _playerColors[playerName];

            PlayerTokens.RemoveAll(p => p.Name == playerName);
            PlayerTokens.Add((playerName, newCellIndex, colorForPlayer));
            Refresh();
        }
    }

    class GameBoardDrawable : IDrawable
    {
        private readonly GameBoardView _view;
        private const int GridSize = 11; // ⬅⬅⬅ Меняем размер поля

        public GameBoardDrawable(GameBoardView view)
        {
            _view = view;
        }


        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float cellSize = Math.Min(dirtyRect.Width, dirtyRect.Height) / GridSize;

            for (int i = 0; i < 40; i++)
            {
                var (row, col) = GetCellCoordinates(i);
                float x = col * cellSize;
                float y = row * cellSize;

                // Светло-серый фон клетки
                canvas.FillColor = Colors.WhiteSmoke;
                canvas.FillRectangle(x, y, cellSize, cellSize);

                if (_view.OwnedCells.TryGetValue(i, out var ownerColor))
                {
                    canvas.FillColor = ownerColor.WithAlpha(0.3f); // Прозрачный цвет игрока
                    canvas.FillRectangle(x, y, cellSize, cellSize);
                }
                // Рамка клетки
                canvas.StrokeColor = Colors.Gray;
                canvas.StrokeSize = 1;
                canvas.DrawRectangle(x, y, cellSize, cellSize);

                // Подпись номера
                canvas.FontSize = 12;
                canvas.FontColor = Colors.Black;
                canvas.DrawString(i.ToString(), x + 40, y + 15, HorizontalAlignment.Left);
            }

            // Отображение фишек игроков
            foreach (var token in _view.PlayerTokens)
            {
                int index = token.CellIndex;
                var (row, col) = GetCellCoordinates(index);

                float x = col * cellSize + cellSize / 2;
                float y = row * cellSize + cellSize / 2;
                float radius = cellSize * 0.2f;

                canvas.FillColor = token.Color;
                canvas.FillCircle(x, y, radius);
            }
        }

        // Перевод позиции в координаты на поле 11x11
        private (int row, int col) GetCellCoordinates(int index)
        {
            index %= 40;

            if (index <= 10)                 // нижняя сторона (справа налево)
                return (10, 10 - index);
            else if (index <= 20)            // левая сторона (снизу вверх)
                return (10 - (index - 10), 0);
            else if (index <= 30)            // верхняя сторона (слева направо)
                return (0, index - 20);
            else                             // правая сторона (сверху вниз)
                return (index - 30, 10);
        }
    }
}
