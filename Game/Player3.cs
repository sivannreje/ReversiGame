using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Player3 : Player
    {
        public void getPlayers              // players ids
        (
            ref string player1_1,
            ref string player1_2
        )
        {
            player1_1 = "308465954";        // id1
            player1_2 = "308089671";        // id2
        }
        public Tuple<int, int> playYourTurn
        (
            Board board,
            TimeSpan timesup,
            char playerChar          // 1 or 2
        )
        {

            double maxScore = -1;
            List<Tuple<int, int>> legalMoves = board.getLegalMoves(playerChar);
            Tuple<int, int> toRerurn = legalMoves[0];
            foreach (Tuple<int, int> move in legalMoves)
            {
                Board state = getState(move, board, playerChar);
                double score = AlphaBeta(state, 2, Double.NegativeInfinity, Double.PositiveInfinity, playerChar);
                if (score > maxScore)
                {
                    maxScore = score;
                    toRerurn = move;
                }
            }
            return toRerurn;
        }

        private double AlphaBeta(Board board, int depth, double a, double b, char playerChar)
        {
            double v = 0;
            if (depth == 0 || board.isTheGameEnded())
            {
                return HeuristicValue(board, playerChar);
            }
            if (playerChar == '2')
            {
                v = Double.NegativeInfinity;
                foreach (Board child in getChildrenOfState(board, playerChar))
                {
                    v = Math.Max(v, AlphaBeta(child, depth - 1, a, b, '1'));
                    a = Math.Max(a, v);
                    if (b <= a)
                    {
                        break;
                    }
                }
                return v;
            }
            else
            {
                v = Double.PositiveInfinity;
                foreach (Board child in getChildrenOfState(board, playerChar))
                {
                    v = Math.Min(v, AlphaBeta(child, depth - 1, a, b, '2'));
                    b = Math.Min(b, v);
                    if (b <= a)
                    {
                        break;
                    }
                }
                return v;
            }
        }
        private int HeuristicValue(Board board, char playerChar)
        {
            int score = 0;
            if (playerChar == '2')
            {
                score = board.gameScore().Item1;
            }
            else
            {
                score = board.gameScore().Item2;
            }
            return score;
        }
        private List<Board> getChildrenOfState(Board board, char playerChar)
        {
            List<Board> children = new List<Board>();            //create list of all children states
            List<Tuple<int, int>> legalMoves = board.getLegalMoves(playerChar);            //get all the legal move for the given player

            //iterate all the moves, for each move - create the state and add it to the list
            foreach (Tuple<int, int> move in legalMoves)
            {
                Board child = getState(move, board, playerChar);
                children.Add(child);
            }
            //return the list of all states.
            return children;
        }
        private Board getState(Tuple<int, int> move, Board board, char playerChar)
        {
            Board state = new Board(board);
            state.fillPlayerMove(playerChar, move.Item1, move.Item2);
            return state;
        }

    }
}
