using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace GameOfLife
{
    /// <summary>
    /// A single cell
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// The current state of a cell
        /// </summary>
        public enum CellState
        {
            Unknown = 0,
            Dead,
            Alive,
        }

        /// <summary>
        /// Relative positions of the neighbours, not including ourself.
        /// </summary>
        private static readonly Point[] NeighbourPositions = {
            /*    -1 0  1
             * -1  X X  X
             *  0  X X  X
             *  1  X X  X
             */
            new Point(-1, -1), // Upper Left
            new Point(0, -1), // Upper Middle
            new Point(1, -1), // Upper Right

            new Point(-1, 0), // Middle Left 
            // Middle skipped (Self)
            new Point(1, 0), // Middle Right

            new Point(-1, 1), // Lower Left 
            new Point(0, 1), // Lower Middle 
            new Point(1, 1), // Lower Right
        };

        /// <summary>
        /// The state of the cell
        /// 0 = Dead
        /// 1 = Alive
        /// </summary>
        public CellState State;

        /// <summary>
        /// How long has the cell been in this state?
        /// (Specified in generations)
        /// </summary>
        public int StateTime;

        /// <summary>
        /// The next state of the cell after calling EndUpdate
        /// </summary>
        private CellState _nextState;

        /// <summary>
        /// The grid position of the cell
        /// </summary>
        private Point _gridPos;

        /// <summary>
        /// Copy constructor, used in history
        /// </summary>
        /// <param name="other">Cell object to copy from</param>
        public Cell(Cell other)
        {
            State = other.State;
            _gridPos = other._gridPos;
            StateTime = other.StateTime;
        }
        
        /// <summary>
        /// Initializes the cell at grid pos 0,0 with state unknown
        /// </summary>
        public Cell() : this(Point.Zero)
        {
        }

        /// <summary>
        /// Initializes the cell at the specified grid pos with state dead
        /// </summary>
        /// <param name="position">The desired grid position</param>
        public Cell(Point position) : this(CellState.Dead, position)
        {
        }

        /// <summary>
        /// Initializes the cell at the specified gird pos with the specified state
        /// </summary>
        /// <param name="state">The current state of the cell</param>
        /// <param name="gridPos">The grid position of the cell</param>
        public Cell(CellState state, Point gridPos)
        {
            State = state;
            _gridPos = gridPos;
        }

        /// <summary>
        /// Updates the cell a single step
        /// Call <see cref="EndUpdate"/> to actually update the state of the cell
        /// </summary>
        public void UpdateStep()
        {
            // Get alive neighbours
            var numAliveNeighbours = 0;
            foreach (var neighbourPosition in NeighbourPositions)
            {
                var checkPos = _gridPos + neighbourPosition;
                if(Board.Instance.WrapAround)
                    checkPos = new Point((_gridPos.X + neighbourPosition.X + Board.Instance.Cols) % Board.Instance.Cols,
                        (_gridPos.Y + neighbourPosition.Y + Board.Instance.Rows) % Board.Instance.Rows);
                var neighbourCell = Board.Instance.GetCell(checkPos);
                if (neighbourCell?.State == CellState.Alive) numAliveNeighbours++;
            }
            
            _nextState = State;
            if (State == CellState.Alive)
            {
                /*
                 * Any live cell with fewer than two live neighbours dies, as if caused by underpopulation.
                 * Any live cell with more than three live neighbours dies, as if by overpopulation.
                 */
                if (numAliveNeighbours > 3 || numAliveNeighbours < 2)
                    _nextState = CellState.Dead;
                // Any live cell with two or three live neighbours lives on to the next generation.
                else if (numAliveNeighbours == 3 || numAliveNeighbours == 2)
                    _nextState = CellState.Alive;
            }
            else
            {
                /*
                 * Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
                 */
                if (numAliveNeighbours == 3)
                    _nextState = CellState.Alive;
            }

            if (_nextState == State)
                StateTime += 1;
            else
                StateTime = 0;
        }

        public void Update()
        {
        }

        /// <summary>
        /// Gets a color for the current state the cell is in
        /// </summary>
        /// <returns>Color</returns>
        private Color GetStateColor()
        {
            if (State == CellState.Alive)
            {
                if (StateTime >= 100)
                    return Color.Red;
                if (StateTime >= 50)
                    return Color.Orange;
                if (StateTime >= 25)
                    return Color.Yellow;
                
                return Color.White;
            }
            
            return Color.Black;
        }

        /// <summary>
        /// Draws the cells to the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (State == CellState.Unknown)
                return;

            spriteBatch.FillRectangle(
                GetScreenPosition(),
                new Size2(Board.GridSize - 1, Board.GridSize - 1),
                GetStateColor());

            spriteBatch.DrawRectangle(
                GetScreenPosition(),
                new Size2(Board.GridSize, Board.GridSize), Color.Red);
        }

        /// <summary>
        /// Actually updates the cell state
        /// </summary>
        public void EndUpdate()
        {
            State = _nextState;
            _nextState = CellState.Unknown;
        }

        /// <summary>
        /// Converts col,row to screen coordinates
        /// </summary>
        /// <returns>Screen coordinates</returns>
        public Vector2 GetScreenPosition()
        {
            var board = Board.Instance;
            return new Vector2(board.Position.X + (_gridPos.X * Board.GridSize),
                board.Position.Y + (_gridPos.Y * Board.GridSize));
        }
    }
}