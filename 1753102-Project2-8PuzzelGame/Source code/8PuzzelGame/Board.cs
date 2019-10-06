using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8PuzzelGame
{
    class Board
    {
        public Board(int rows, int columns)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.Matrix = new int[Rows, Columns];
        }

        public Board(Board copy)
        {
            Rows = copy.Rows;
            Columns = copy.Columns;
            Matrix = new int[Rows, Columns];
            for (int i = 0; i < Rows * Columns; i++) 
            {
                int value = copy.GetAt(i);
                SetAt(i, value);
            }
        }



        public int[,] Matrix;
        public int Rows;
        public int Columns;

        // return value at index
        public int GetAt(int index)
        {
            int result = 0;

            if (index >= 0 && index <= Rows * Columns - 1)
            {
                int row = index / Rows;
                int column = index % Columns;
                result = Matrix[row, column];
            }

            return result;
        }

        // set value at index
        public void SetAt(int index, int value)
        {
            if (index >= 0 && index <= Rows * Columns - 1)
            {
                int row = index / Rows;
                int column = index % Columns;
                Matrix[row, column] = value;
            }
        }
    }
}
