using System.Collections.Generic;

namespace GameOfLife
{
    public class GridHistory
    {
        private List<Cell[,]> _history = new List<Cell[,]>();

        public int Count
        {
            get { return _history.Count; }
        }

        public void AddHistory(Cell[,] currentGrid)
        {
            /* we have to deep copy cell objects
             * since it shouldn't be a reference to the current cell object!
             */
            var gridCopy = new Cell[currentGrid.GetLength(0), currentGrid.GetLength(1)];
            for (var i = 0; i < currentGrid.GetLength(0); i++)
            {
                for (var j = 0; j < currentGrid.GetLength(1); j++)
                {
                    gridCopy[i, j] = new Cell(currentGrid[i, j]);
                }
            }
            _history.Add(gridCopy);
        }

        public Cell[,] GetHistory(int position)
        {
            if (position < 0 || position >= _history.Count)
                return null;

            return _history[position];
        }
    }
}