using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Player2 : Player
    {
        public void getPlayers              // players ids
        (
            ref string player1_1,
            ref string player1_2
        )
        {
            player1_1 = "123456789";        // id1
            player1_2 = "123456789";        // id2
        }
        public Tuple<int, int> playYourTurn
        (
            Board       board,
            TimeSpan    timesup,
            char        playerChar          // 1 or 2
        )
        {
            Tuple<int, int> toReturn = null;
            //Random Algorithm - Start
            int randomMove;
            List<Tuple<int, int>> legalMoves    = board.getLegalMoves(playerChar);
            Random random                       = new Random();
            randomMove                          = random.Next(0, legalMoves.Count());
            toReturn                            = legalMoves[randomMove];
            //Random Algorithm - End
            return toReturn;
        }
    }
}
