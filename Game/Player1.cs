using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Game
{
    public class Player1 : Player
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
            Board       board,
            TimeSpan    timesup,
            char        playerChar          // 1 or 2
        )
        {
            
            double maxScore = -1;
            List<Tuple<int, int>> legalMoves = board.getLegalMoves(playerChar);
            Tuple<int, int> toRerurn = legalMoves[0];
            foreach (Tuple<int, int> move in legalMoves)
            {
                Board state = getState(move, board, playerChar);
                double score = AlphaBeta(state, 2, Double.NegativeInfinity, Double.PositiveInfinity, playerChar);
                if(score > maxScore)
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
            if(depth == 0 || board.isTheGameEnded())
            {
                return scoreValue(board, playerChar);
            }
            if(playerChar == '1')
            {
                v = Double.NegativeInfinity;
                foreach (Board child in getChildrenOfState(board, playerChar))
                {
                    v = Math.Max(v, AlphaBeta(child, depth - 1, a, b, '2'));
                    a = Math.Max(a, v);
                    if(b <= a)
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
                    v = Math.Min(v, AlphaBeta(child, depth - 1, a, b, '1'));
                    b = Math.Min(b, v);
                    if (b <= a)
                    {
                        break;
                    }
                }
                return v;
            }
        }
        private int scoreValue(Board board, char playerChar)
        {
            int score = 0;
            if(playerChar == '1')
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
            
            //sort the moves by the heuristic function
            Dictionary<Tuple<int, int>, int> DictionaryMoves = new Dictionary<Tuple<int, int>, int>();
            foreach (Tuple<int, int> move in legalMoves)
            {
                DictionaryMoves.Add(move, AreaCapturedHeuristic(move, board._n));

            }
            List<KeyValuePair<Tuple<int, int>, int>> sortedMoves = DictionaryMoves.ToList();

            sortedMoves.Sort(
                delegate (KeyValuePair<Tuple<int, int>, int> pair1,
                KeyValuePair<Tuple<int, int>, int> pair2)
                {
                    return pair2.Value.CompareTo(pair1.Value);
                }
            );
            //iterate all the moves, for each move - create the state and add it to the list
            foreach (KeyValuePair<Tuple<int, int>, int> move in sortedMoves)
            {
                Board child = getState(move.Key, board, playerChar);
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
        private int AreaCapturedHeuristic(Tuple<int, int> move, int n)
        {
            int row = move.Item1;
            int col = move.Item2;

            //the position is in the corners
            if ((row == 0 && col == 0) || (row == n && col == 0) || (row == 0 && col == n) || (row == n && col == n))
                return 5;
            else if ((row >= 0 && row <= 1) && (col <= 1 && col >= 0) || (row >= n - 1 && row <= n) && (col >= n - 1 && col <= n))
                return 1;
            else if ((row == 0) || (col == 0) || (row == n) || (col == n))
                return 4;
            else if ((row == 1) || (col == 1) || (row == n - 1) || (col == n - 1))
                return 2;
            else return 3;
        }
        private int MobilityHeuristic(Board board, char playerChar)
        {
            return 0; // not implemented yet
        }

    }
}
