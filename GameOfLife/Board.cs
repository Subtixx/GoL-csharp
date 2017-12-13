using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameOfLife
{
    /// <summary>
    /// A board containing cells
    /// </summary>
    public class Board
    {
        /// <summary>
        /// Random provider
        /// </summary>
        private static Random _rand = new Random();

        /// <summary>
        /// Class instance
        /// </summary>
        public static Board Instance;

        /// <summary>
        /// Position of the board
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// Cell Grid
        /// </summary>
        private Cell[,] _grid;

        /// <summary>
        /// Current Generation we're on
        /// If EnableHistory is true, it can be a generation in the past
        /// </summary>
        public int Generation;

        /// <summary>
        /// How big the cells are in pixels
        /// </summary>
        public const int GridSize = 32;

        /// <summary>
        /// The history provider
        /// </summary>
        private GridHistory _history = new GridHistory();

        /// <summary>
        /// Can we jump back into the past
        /// </summary>
        public bool EnableHistory;

        /// <summary>
        /// Are we in the past?
        /// </summary>
        public bool InPast
        {
            get
            {
                if (!EnableHistory)
                    return false;

                return _history.Count - 1 > Generation;
            }
        }

        /// <summary>
        /// How many columns the board has
        /// </summary>
        public int Cols => _grid.GetLength(0) - 1;

        /// <summary>
        /// How many rows the board has
        /// </summary>
        public int Rows => _grid.GetLength(1) - 1;

        /// <summary>
        /// Should we wrap positions around?
        /// </summary>
        public bool WrapAround;

        /// <summary>
        /// Constructor
        /// </summary>
        public Board()
        {
            Instance = this;
            WrapAround = true;
            EnableHistory = true;
        }

        /// <summary>
        /// Resets the board to the initial state
        /// </summary>
        public void Reset()
        {
            if (EnableHistory)
                _history = new GridHistory();

            Generation = 0;
            Initialize(_grid.GetLength(0), _grid.GetLength(1));
            _rand = new Random();
        }

        /// <summary>
        /// Initializes the board and grid
        /// </summary>
        /// <param name="cols">How many columns does the board have</param>
        /// <param name="rows">How many rows does the board have</param>
        public void Initialize(int cols, int rows)
        {
            _grid = new Cell[cols, rows];
            for (var i = 0; i < _grid.GetLength(0); i++)
            {
                for (var j = 0; j < _grid.GetLength(1); j++)
                {
                    _grid[i, j] = new Cell(_rand.Next(0, 2) == 1 ? Cell.CellState.Alive : Cell.CellState.Dead,
                        new Point(i, j));
                }
            }
        }

        /// <summary>
        /// Updates one step (single tick)
        /// </summary>
        public void UpdateStep()
        {
            if (EnableHistory)
                _history.AddHistory(_grid);

            for (var i = 0; i < _grid.GetLength(0); i++)
            {
                for (var j = 0; j < _grid.GetLength(1); j++)
                {
                    _grid[i, j].UpdateStep();
                }
            }

            for (var i = 0; i < _grid.GetLength(0); i++)
            {
                for (var j = 0; j < _grid.GetLength(1); j++)
                {
                    _grid[i, j].EndUpdate();
                }
            }

            Generation++;
        }

        /// <summary>
        /// General update function
        /// </summary>
        public void Update()
        {
            if(Game1.Instance.IsSimulating)
                UpdateStep();
            
        }

        /// <summary>
        /// Go back in time
        /// </summary>
        public void Undo()
        {
            if (!EnableHistory)
                return;
            if (Generation <= 0)
                return;

            // Add current gen to history
            if (Generation >= _history.Count)
                _history.AddHistory(_grid);

            // Go back one generation
            Generation--;
            _grid = _history.GetHistory(Generation);
        }

        /// <summary>
        /// Redo actions
        /// </summary>
        public void Redo()
        {
            if (!EnableHistory)
                return;
            if (_history.Count <= Generation + 1)
                return;

            Generation++;
            _grid = _history.GetHistory(Generation);
        }

        /// <summary>
        /// Draws the board
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < _grid.GetLength(0); i++)
            {
                for (var j = 0; j < _grid.GetLength(1); j++)
                {
                    _grid[i, j].Draw(spriteBatch);
                }
            }
        }

        /// <summary>
        /// Returns the cell in the specified position
        /// </summary>
        /// <param name="position">The cell position (cols, rows)</param>
        /// <returns>Cell if found, null otherwise</returns>
        public Cell GetCell(Point position)
        {
            if (_grid.GetLength(0) <= position.X || position.X < 0)
                return null;
            if (_grid.GetLength(1) <= position.Y || position.Y < 0)
                return null;

            return _grid[position.X, position.Y];
        }

        /// <summary>
        /// Returns the cell in the specified col,row coordinates
        /// </summary>
        /// <param name="x">Col</param>
        /// <param name="y">Row</param>
        /// <returns>Cell if found, null otherwise</returns>
        public Cell GetCell(int x, int y)
        {
            return GetCell(new Point(x, y));
        }
    }
}