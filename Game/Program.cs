using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClassLibrary1;

namespace Game
{
    class Program
    {

        static void Main(string[] args)
        {
            Player player1      = new Player3();
            Player player2      = new Player1();
            int numberOfGames   = 10;
            int boardSize       = 10;  // must be even
            int gameLevel       = 1;  // between 1 to 4
            bool toPrint        = true;
            GameController controller =
                new GameController(numberOfGames, boardSize, gameLevel, toPrint, player1, player2);
            Tuple<int,int> overallScore = controller.Run();
            printOverallScore(overallScore);
            Console.ReadLine();
        }

        static void printOverallScore
        (
            Tuple<int, int> scores
        )
        {
            Console.WriteLine("***   OverallScore   ***");
            Console.WriteLine("Player 1: " + scores.Item1);
            Console.WriteLine("Player 2: " + scores.Item2);
        }
    }
}
