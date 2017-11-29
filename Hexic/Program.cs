using System;
using System.Linq;

namespace Hexic {
    class Program {
        static void Main(string[] args) {
            var position = ValidPosition.Init();
            Console.WriteLine(position);
            Console.ReadLine();
            var nextPosition = position.MakeTurn();
            while (nextPosition.Score > position.Score) {
                position = nextPosition;
                Console.WriteLine(position);
                Console.ReadLine();
                nextPosition = position.MakeTurn();
            }
            Console.WriteLine("The End");
            Console.ReadLine();
        }
    }
}
