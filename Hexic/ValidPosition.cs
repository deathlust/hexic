using System;
using System.Collections.Generic;

namespace Hexic {
    class ValidPosition: Position {
        ValidPosition(int[,] shortColumns, int[,] longColumns, ulong score)
            : base(shortColumns, longColumns, score) {
        }
        public static ValidPosition Init() {
            var shortColumns = new int[COLUMN_COUNT, SHORT_COLUMN_LENGTH];
            var longColumns = new int[COLUMN_COUNT, SHORT_COLUMN_LENGTH + 1];
            RandomLayout(shortColumns, longColumns);
            var position = new ValidPosition(shortColumns, longColumns, 0);
            if (position.ClusterCenterCount != 0)
                throw new InvalidOperationException("Initial position not valid");
            return position;
        }
        static void RandomLayout(int[,] shortColumns, int[,] longColumns) {
            for (int i = 0; i < COLUMN_COUNT; ++i) {
                for (int j = 0; j < SHORT_COLUMN_LENGTH + 1; ++j) {
                    longColumns[i, j] = random.Next(MAX_NUMBER);
                }
            }
            for (int i = 0; i < COLUMN_COUNT; ++i) {
                for (int j = 0; j < SHORT_COLUMN_LENGTH; ++j) {
                    int value;
                    int deniedNumber1 = -1;
                    int deniedNumber2 = -1;
                    int deniedNumber3 = -1;
                    if (longColumns[i, j] == longColumns[i, j + 1]) {
                        deniedNumber1 = longColumns[i, j];
                    }
                    if (i > 0 && longColumns[i - 1, j] == longColumns[i - 1, j + 1]) {
                        deniedNumber2 = longColumns[i - 1, j];
                    }
                    if (j > 0) {
                        if (shortColumns[i, j - 1] == longColumns[i, j]) {
                            deniedNumber3 = shortColumns[i, j - 1];
                        }
                        if (i > 0 && shortColumns[i, j - 1] == longColumns[i - 1, j]) {
                            deniedNumber3 = shortColumns[i, j - 1];
                        }
                    }
                    // prevent clusters in initial layout
                    do {
                        value = random.Next(MAX_NUMBER);
                    } while (value == deniedNumber1 || value == deniedNumber2 || value == deniedNumber3);
                    shortColumns[i, j] = value;
                }
            }
        }
        class PositionBuilder: Position {
            bool ready = false;
            ulong scoreIncrement = 0;
            PositionBuilder(int[,] shortColumns, int[,] longColumns, ulong score)
                : base(shortColumns, longColumns, score) {
            }

            public ValidPosition Build() {
                if (!ready)
                    throw new InvalidOperationException("Position is not ready to be built");
                return new ValidPosition(shortColumns, longColumns, Score + scoreIncrement);
            }
            public PositionBuilder MakeTurn() {
                while (ClusterCenterCount > 0) {
                    MarkHexesForClusterCount();
                    CountIterationScore();
                    RefreshPosition();
                }
                ready = true;
                return this;
            }
            void MarkHexesForClusterCount() {
                var clusterCenters = GetClusterCenters();
                for (int value = 0; value < clusterCenters.Length; ++value) {
                    var list = clusterCenters[value];
                    list.ForEach(point => {
                        point.SetAllCells(value - MAX_NUMBER - 1, shortColumns, longColumns);
                    });
                }
            }
            void CountIterationScore() {
                PartialCountIterationScore(longColumns, SHORT_COLUMN_LENGTH + 1);
                PartialCountIterationScore(shortColumns, SHORT_COLUMN_LENGTH);
            }
            void PartialCountIterationScore(int[,] columns, int columnLength) {
                for (int i = 0; i < COLUMN_COUNT; ++i) {
                    for (int j = 0; j < columnLength; ++j) {
                        int value = columns[i, j];
                        if (value < 0) {
                            scoreIncrement += ClusterScore(CountClusterCells(columns, i, j, value), value);
                        }
                    }
                }
            }
            int CountClusterCells(int[,] columns, int i, int j, int value) {
                Predicate<int> valueEquals = v => v == value;
                if (!valueEquals(columns[i,j]))
                    throw new InvalidOperationException("Starting cell is not forming cluster");
                columns[i, j] += 2 + 2 * MAX_NUMBER; 
                return 1 + CountNeighborCells(columns, i, j, valueEquals);
            }
            struct Cell {
                public int I { get; set; }
                public int J { get; set; }
                public int[,] Columns { get; set; }
            }
            int CountNeighborCells(int[,] columns, int i, int j, Predicate<int> filter) {
                int count = 0;
                var neighbors = CellNeighbors(columns, i, j, filter);
                neighbors.ForEach(cell => {
                    cell.Columns[cell.I, cell.J] += 2 + 2 * MAX_NUMBER;
                    ++count;
                });
                neighbors.ForEach(cell => {
                    count += CountNeighborCells(cell.Columns, cell.I, cell.J, filter);
                });
                return count;
            }
            List<Cell> CellNeighbors(int[,] columns, int i, int j, Predicate<int> filter) {
                var list = new List<Cell>(6);
                int[,] otherColumns;
                int length, otherLength;
                int otherColumnIndex, otherRowIndex;
                if (columns == shortColumns) {
                    otherColumns = longColumns;
                    length = SHORT_COLUMN_LENGTH;
                    otherLength = SHORT_COLUMN_LENGTH + 1;
                    otherColumnIndex = i - 1;
                    otherRowIndex = j + 1;
                } else {
                    otherColumns = shortColumns;
                    length = SHORT_COLUMN_LENGTH + 1;
                    otherLength = SHORT_COLUMN_LENGTH;
                    otherColumnIndex = i + 1;
                    otherRowIndex = j - 1;
                }
                // this column neighbors
                if (j > 0)
                    list.Add(new Cell { Columns = columns, I = i, J = j - 1});
                if (j < length - 1)
                    list.Add(new Cell { Columns = columns, I = i, J = j + 1 });
                // same index other column neighbors
                if (j < otherLength)
                    list.Add(new Cell { Columns = otherColumns, I = i, J = j });
                if (otherRowIndex >= 0 && otherRowIndex < otherLength)
                    list.Add(new Cell { Columns = otherColumns, I = i, J = otherRowIndex });
                // another index column neighbors
                if (otherColumnIndex >= 0 && otherColumnIndex < COLUMN_COUNT) {
                    if (j < otherLength)
                        list.Add(new Cell { Columns = otherColumns, I = otherColumnIndex, J = j });
                    if (otherRowIndex >= 0 && otherRowIndex < otherLength)
                        list.Add(new Cell { Columns = otherColumns, I = otherColumnIndex, J = otherRowIndex });
                }

                return list.FindAll(cell => filter(cell.Columns[cell.I, cell.J]));
            }
            ulong ClusterScore(int cells, int value) {
                if (cells < 3)
                    throw new InvalidOperationException("Too few cells in a cluster");
                ulong res = 3;
                for (int i = cells - 3; i > 0; --i)
                    res *= 3;
                return res;
            }

            void RefreshPosition() {
                PartialRefreshPosition(longColumns, SHORT_COLUMN_LENGTH + 1);
                PartialRefreshPosition(shortColumns, SHORT_COLUMN_LENGTH);
            }
            void PartialRefreshPosition(int[,] columns, int columnLength) {
                for (int i = 0; i < COLUMN_COUNT; ++i) {
                    var values = new List<int>(columnLength);
                    // store non-removed cells in a column
                    for (int j = 0; j < columnLength; ++j) {
                        int value = columns[i, j];
                        if (value <= MAX_NUMBER) {
                            values.Add(value);
                        }
                    }
                    // add random cells above
                    for (int j = 0; j < columnLength; ++j) {
                        columns[i, j] = j < values.Count ? values[j] : random.Next(MAX_NUMBER);
                    }
                }
            }

            public static PositionBuilder Generate(ValidPosition position, RotationCenter point, bool clockwise) {
                var shortColumns = position.ShortColumns;
                var longColumns = position.LongColumns;
                point.Rotate(clockwise, shortColumns, longColumns);
                return new PositionBuilder(shortColumns, longColumns, position.Score);
            }
            protected override string Symbol(int cell) {
                if (cell < 0) {
                    char c = (char)('a' + cell + MAX_NUMBER + 1);
                    return c.ToString();
                }
                if (cell > MAX_NUMBER) {
                    char c = (char)('A' + cell - MAX_NUMBER - 1);
                    return c.ToString();
                }
                return cell.ToString();
            }
        }
        public ValidPosition MakeTurn() {
            ValidPosition bestPosition = this;
            Predicate<Position> betterScore =
                p => p.Score > bestPosition.Score;
            for (int i = 0; i < 2 * COLUMN_COUNT - 1; ++i) {
                for (int j = 0; j < 2 * SHORT_COLUMN_LENGTH - 1; ++j) {
                    var point = new RotationCenter(i, j);
                    var position = PositionBuilder.Generate(this, point, true).MakeTurn().Build();
                    if (betterScore(position))
                        bestPosition = position;
                    position = PositionBuilder.Generate(this, point, false).MakeTurn().Build();
                    if (betterScore(position))
                        bestPosition = position;
                }
            }
            return bestPosition;
        }
    }
}
