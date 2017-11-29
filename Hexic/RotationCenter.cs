namespace Hexic {
    class RotationCenter {
        readonly int Column;
        readonly int Row;
        readonly bool HasTwoLeftCells;
        readonly bool HasFirstShortColumn;
        readonly int LongColumnIndex;
        readonly int ShortColumnIndex;
        readonly bool OneCellInShortColumn;
        readonly int LowCellIndex;
        readonly int UpperCellIndex;
        readonly int OneCellIndex;
        public RotationCenter(int i, int j) {
            Column = i;
            Row = j;
            HasTwoLeftCells = (i + j) % 2 != 0;
            HasFirstShortColumn = i % 2 == 0;
            LongColumnIndex = i / 2;
            ShortColumnIndex = HasFirstShortColumn ? LongColumnIndex : LongColumnIndex + 1;
            OneCellInShortColumn = HasFirstShortColumn ^ HasTwoLeftCells;
            LowCellIndex = j / 2;
            UpperCellIndex = LowCellIndex + 1;
            OneCellIndex = OneCellInShortColumn ? LowCellIndex : LowCellIndex + 1;
        }
        public void CellValues(out int one, out int low, out int upper, int[,] shortColumns, int[,] longColumns) {
            one = OneCellInShortColumn ? shortColumns[ShortColumnIndex, OneCellIndex] : longColumns[LongColumnIndex, OneCellIndex];
            low = OneCellInShortColumn ? longColumns[LongColumnIndex, LowCellIndex] : shortColumns[ShortColumnIndex, LowCellIndex];
            upper = OneCellInShortColumn ? longColumns[LongColumnIndex, UpperCellIndex] : shortColumns[ShortColumnIndex, UpperCellIndex];
        }
        public void Rotate(bool clockwise, int[,] shortColumns, int[,] longColumns) {
            int cell1, cell2, cell3;
            CellValues(out cell1, out cell2, out cell3, shortColumns, longColumns);
            if (clockwise) {
                if (HasTwoLeftCells) {
                    SetCellData(cell3, cell1, cell2, shortColumns, longColumns);
                    //if (OneCellInShortColumn) {
                    //    longColumns[LongColumnIndex, LowCellIndex] = cell1;
                    //    longColumns[LongColumnIndex, UpperCellIndex] = cell2;
                    //    shortColumns[ShortColumnIndex, OneCellIndex] = cell3;
                    //} else {
                    //    shortColumns[ShortColumnIndex, LowCellIndex] = cell1;
                    //    shortColumns[ShortColumnIndex, UpperCellIndex] = cell2;
                    //    longColumns[LongColumnIndex, OneCellIndex] = cell3;
                    //}
                } else {
                    SetCellData(cell2, cell3, cell1, shortColumns, longColumns);
                    //if (OneCellInShortColumn) {
                    //    longColumns[LongColumnIndex, LowCellIndex] = cell3;
                    //    longColumns[LongColumnIndex, UpperCellIndex] = cell1;
                    //    shortColumns[ShortColumnIndex, OneCellIndex] = cell2;
                    //} else {
                    //    shortColumns[ShortColumnIndex, LowCellIndex] = cell3;
                    //    shortColumns[ShortColumnIndex, UpperCellIndex] = cell1;
                    //    longColumns[LongColumnIndex, OneCellIndex] = cell2;
                    //}
                }
            } else {
                if (HasTwoLeftCells) {
                    SetCellData(cell2, cell3, cell1, shortColumns, longColumns);
                    //if (OneCellInShortColumn) {
                    //    longColumns[LongColumnIndex, LowCellIndex] = cell3;
                    //    longColumns[LongColumnIndex, UpperCellIndex] = cell1;
                    //    shortColumns[ShortColumnIndex, OneCellIndex] = cell2;
                    //} else {
                    //    shortColumns[ShortColumnIndex, LowCellIndex] = cell3;
                    //    shortColumns[ShortColumnIndex, UpperCellIndex] = cell1;
                    //    longColumns[LongColumnIndex, OneCellIndex] = cell2;
                    //}
                } else {
                    SetCellData(cell3, cell1, cell2, shortColumns, longColumns);
                    //if (OneCellInShortColumn) {
                    //    longColumns[LongColumnIndex, LowCellIndex] = cell1;
                    //    longColumns[LongColumnIndex, UpperCellIndex] = cell2;
                    //    shortColumns[ShortColumnIndex, OneCellIndex] = cell3;
                    //} else {
                    //    shortColumns[ShortColumnIndex, LowCellIndex] = cell1;
                    //    shortColumns[ShortColumnIndex, UpperCellIndex] = cell2;
                    //    longColumns[LongColumnIndex, OneCellIndex] = cell3;
                    //}
                }
            }
        }
        public bool CellsAreEqual(out int value, int[,] shortColumns, int[,] longColumns) {
            int one, low, upper;
            CellValues(out one, out low, out upper, shortColumns, longColumns);
            value = one;
            if (one == low && one == upper)
                return true;
            return false;
        }
        public void SetAllCells(int value, int[,] shortColumns, int[,] longColumns) {
            SetCellData(value, value, value, shortColumns, longColumns);
        }
        void SetCellData(int one, int low, int upper, int[,] shortColumns, int[,] longColumns) {
            if (OneCellInShortColumn) {
                shortColumns[ShortColumnIndex, OneCellIndex] = one;
                longColumns[LongColumnIndex, LowCellIndex] = low;
                longColumns[LongColumnIndex, UpperCellIndex] = upper;
            } else {
                longColumns[LongColumnIndex, OneCellIndex] = one;
                shortColumns[ShortColumnIndex, LowCellIndex] = low;
                shortColumns[ShortColumnIndex, UpperCellIndex] = upper;
            }
        }
    }
}
