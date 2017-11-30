using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hexic {
    abstract class Position {
        public const int COLUMN_COUNT = 5;
        public const int SHORT_COLUMN_LENGTH = 8;
        public const int MAX_NUMBER = 9;
        public int[,] ShortColumns {
            get {
                return shortColumns.Clone() as int[,];
            }
        }
        protected readonly int[,] shortColumns;
        public int[,] LongColumns {
            get {
                return longColumns.Clone() as int[,];
            }
        }
        protected readonly int[,] longColumns;
        public readonly ulong Score;
        protected static readonly Random random = new Random();

        protected Position(int[,] shortColumns, int[,] longColumns, ulong score) {
            this.shortColumns = shortColumns;
            this.longColumns = longColumns;
            this.Score = score;
        }

        protected int ClusterCenterCount {
            get {
                return GetClusterCenters().Sum(list => list.Count);
            }
        }
        protected List<RotationCenter>[] GetClusterCenters() {
            var clusterCenters = new List<RotationCenter>[MAX_NUMBER + 1];
            for (int c = 0; c < MAX_NUMBER + 1; ++c) {
                clusterCenters[c] = new List<RotationCenter>();
            }
            for (int i = 0; i < 2 * COLUMN_COUNT - 1; ++i) {
                for (int j = 0; j < 2 * SHORT_COLUMN_LENGTH - 1; ++j) {
                    var point = new RotationCenter(i, j);
                    int value;
                    if (point.HasEqualCells(out value, shortColumns, longColumns))
                        clusterCenters[value].Add(point);
                }
            }
            return clusterCenters;
        }
        public override string ToString() {
            var sb = new StringBuilder("   _   _   _   _   _ \n");
            for (int j = SHORT_COLUMN_LENGTH - 1; j >= 0; --j) {
                sb.Append(j == SHORT_COLUMN_LENGTH - 1 ? ' ' : '\\');
                for (int i = 0; i < COLUMN_COUNT; ++i) {
                    sb.Append("_/")
                        .Append(Symbol(longColumns[i, j + 1]))
                        .Append('\\');
                }
                sb.Append('\n');
                for (int i = 0; i < COLUMN_COUNT; ++i) {
                    sb.Append('/')
                        .Append(Symbol(shortColumns[i, j]))
                        .Append(@"\_");
                }
                sb.Append("/\n");
            }
            for (int i = 0; i < COLUMN_COUNT; ++i) {
                sb.Append(@"\_/")
                    .Append(Symbol(longColumns[i, 0]));
            }
            sb.Append("\\\n")
                .Append("  \\_/ \\_/ \\_/ \\_/ \\_/\n")
                .Append("Score: " + Score)
                .Append('\n');
            return sb.ToString();
        }
        protected virtual string Symbol(int cell) {
            if (cell < 0 || cell > MAX_NUMBER)
                throw new InvalidOperationException("Wrong cell value");
            return cell.ToString();
        }
    }
}
