using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _8PuzzelGame
{
    class Game
    {
        // static
        public static int NumberOfTiles => Rows * Columns;
        public static int Rows = 3;
        public static int Columns => Rows;

        public static int EmptyValue = 30 * 30;
        public static int StartState = 1000;
        public static int StopState = 5000;
        public static int MaxMinutes = 3;

        public static int UpDirection = 0;
        public static int RightDirection = 1;
        public static int DownDirection = 2;
        public static int LeftDirection = 3;

        // attribute
        private int state = StopState;
        private int[] keys/* = new int[3, 3] { { 0, 1, 2}, { 3, 4, 5 }, { 6, 7, EmptyValue } }*/;
        private Board board;
        private Stopwatch stopwatch = new Stopwatch();
        private string resultState = "You losed";

        // constructor
        public Game()
        {
            Reset();
        }

        // method
        public void Reset()
        {
            state = StopState;

            resultState = "You losed";

            board = new Board(Rows, Columns);

            for (int i = 0; i < NumberOfTiles; i++)
            {
                board.SetAt(i, i);

                if (i == NumberOfTiles - 1)
                {
                    board.SetAt(i, EmptyValue);
                }
            }

            GenerateKeys();
        }

        public void GenerateKeys()
        {
            keys = new int[NumberOfTiles];
            for (int i = 0; i < NumberOfTiles; i++)
            {
                keys[i] = i;
                if (i == NumberOfTiles - 1)
                {
                    keys[i] = EmptyValue;
                }
            }
        }

        public void Start()
        {
            if (state == StopState)
            {
                state = StartState;

                resultState = "You losed";
            }
        }

        public void Stop()
        {
            if (state == StartState)
            {
                state = StopState;
            }
        }

        public bool CheckGameOver()
        {
            bool isOver = true;

            // check board
            for (int i = 0; i < NumberOfTiles; i++)
            {
                if (board.GetAt(i) != keys[i])
                {
                    isOver = false;
                    break;
                }
            }

            if (isOver == true)
            {
                state = StopState;
                resultState = "You win";
            }

            return isOver;
        }

        public void Shuffle()
        {
            List<int> validValues = new List<int>()/* { 0, 1, 2, 3, 4, 5, 6, 7, EmptyValue }*/;
            for (int i = 0; i < NumberOfTiles; i++)
            {
                validValues.Add(keys[i]);
            }

            var random = new Random();

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    int index = random.Next(validValues.Count);
                    board.Matrix[i, j] = validValues[index];
                    validValues.RemoveAt(index);
                }
            }
        }

        public void GoAt(int from, int to)
        {
            if (from < 0 || from > NumberOfTiles - 1 || to < 0 || to > NumberOfTiles - 1)
                return;

            if (state == StopState)
                return;


            int row = from / Rows;
            int column = from % Columns;
            int row2 = to / Rows;
            int column2 = to % Columns;

            if (board.Matrix[row2, column2] == EmptyValue)
            {
                if (row2 == row - 1 && column2 == column
                    || row2 == row && column2 == column + 1
                    || row2 == row + 1 && column2 == column
                    || row2 == row && column2 == column - 1)
                {
                    // update board
                    int value = board.Matrix[row, column];
                    board.Matrix[row, column] = EmptyValue;
                    board.Matrix[row2, column2] = value;

                }
                
            }


        }

        // direction: 0 up, 1 right, 2 bottom, 3 left
        public int ShowWhereCanMoveTo(int direction, out int lastIndex, out int newIndex)
        {
            int result = -1;
            lastIndex = -1;
            newIndex = -1;
            if (direction < UpDirection || direction > LeftDirection)
                return result;

            int[] rowDif = new int[] {-1,0,1,0 };
            int[] columnDif = new int[] {0,1,0,-1 };

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    int row = i + rowDif[direction];
                    int column = j + columnDif[direction];
                    if (row >= 0 && row <= Rows - 1 && column >= 0 && column <= Columns - 1)
                    {
                        if (board.Matrix[row, column] == Game.EmptyValue)
                        {
                            lastIndex = Rows * i + j;
                            newIndex = Rows * row + column;
                            result = 0;
                            break;
                            
                        }
                    }
                }

                if (result > -1)
                    break;
            }

            return result;

        }

        public Board GetCurrentBoard()
        {

            Board copyBoard = new Board(this.board);

            return copyBoard;

        }

        public string GetResult() { return resultState; }

        public string GetSavedState()
        {
            string state = "";

            // rows
            state += Rows.ToString() + " ";

            // board
            for (int i = 0; i < NumberOfTiles; i++)
            {
                state += board.GetAt(i).ToString() + " ";
            }

            return state;
        }

        public void SetSavedState(string state)
        {
            string[] tokens = state.Split(new string[] { " "}, StringSplitOptions.RemoveEmptyEntries);
            if (tokens != null)
            {
                // rows 
                Rows = Int32.Parse(tokens[0]);

                // board
                board = new Board(Rows, Columns);
                for (int i = 0; i < NumberOfTiles; i++)
                {
                    int value = Int32.Parse(tokens[i + 1]);
                    board.SetAt(i, value);
                }

                GenerateKeys();
            }
        }
    }
}
