using System;

namespace Hexic {
    class Program {
        static void Main(string[] args) {
            var position = ValidPosition.Init();
            Console.WriteLine(position);
            var nextPosition = position.MakeTurn();
            while (nextPosition.Score > position.Score) {
                position = nextPosition;
                Console.ReadLine();
                Console.WriteLine(position);
                nextPosition = position.MakeTurn();
            }
            Console.WriteLine("The End");
            Console.ReadLine();
        }
    }
}
