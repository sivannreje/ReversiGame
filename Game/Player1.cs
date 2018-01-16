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
        private long TotalTime { get; set; } //Time given in the playYourTurn method
        private int n { get; set; }            // size of the board as given in the playYourTurn method
        public Stopwatch currTime { get; set; } // to keep track on time
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
            currTime = new Stopwatch(); // initial the time watch to track the time
            currTime.Start();
            TotalTime = timesup.Ticks;  //save the maximum time the turn should take in the field
            n = board._n;               //save the size of the board for convenient
            double maxScore = -1;
            //get all legal moves for the player, iterate all the moves and send to the Alpha Beta
            List<Tuple<int, int>> legalMoves = board.getLegalMoves(playerChar); //
            Tuple<int, int> toReturn = legalMoves[0];  //initial the toReturn value. this variable is the returned variable of this method.
            foreach (Tuple<int, int> move in legalMoves)
            {
                Board state = getState(move, board, playerChar);
                double score = AlphaBeta(state, 6, Double.NegativeInfinity, Double.PositiveInfinity, playerChar, move);
                if (score > maxScore)
                {
                    maxScore = score; // get the max scored (maximum root)
                    toReturn = move;
                }
                if (!isTime()) //keep track the time
                    break;
            }
            return toReturn;
        }
        /// <summary>
        /// 
        /// Alpha Beta Pruning algorithm. return the maximun score from the heuristic function
        /// </summary>
        /// <param name="board"></param>
        /// <param name="depth"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="playerChar"></param>
        /// <param name="move"></param>
        /// <returns></returns>
        private double AlphaBeta(Board board, int depth, double a, double b, char playerChar, Tuple<int, int> move)
        {
            double v = 0;
            if (!isTime() || depth == 0 || board.isTheGameEnded()) //stop condition for the recursive
            {
                return TotalHeuristic(move, n, board, playerChar);
            }
            if (playerChar == '1') //maximum player
            {
                v = Double.NegativeInfinity;

                foreach (KeyValuePair<Tuple<int, int>, Board> child in getChildrenOfState(board, playerChar)) // iterate all possible moves
                {
                    if (!isTime()) //keep tracking the time
                        return TotalHeuristic(move, n, board, playerChar);
                    v = Math.Max(v, AlphaBeta(child.Value, depth - 1, a, b, '2', child.Key));
                    a = Math.Max(a, v);
                    if (b <= a) //cutoff condition
                    {
                        break;
                    }
                }
                return v;
            }
            else //minimum player
            {
                v = Double.PositiveInfinity;
                foreach (KeyValuePair<Tuple<int, int>, Board> child in getChildrenOfState(board, playerChar)) // iterate all possible moves
                {
                    if (!isTime()) //keep tracking the time
                        return TotalHeuristic(move, n, board, playerChar);
                    v = Math.Min(v, AlphaBeta(child.Value, depth - 1, a, b, '1', child.Key));
                    b = Math.Min(b, v);
                    if (b <= a) //cutoff condition
                    {
                        break;
                    }
                }
                return v;
            }
        }

        /// <summary>
        /// return dictionary with the move of a given player as a key and the board it creates as a value
        /// </summary>
        /// <param name="board"></param>
        /// <param name="playerChar"></param>
        /// <returns></returns>
        private Dictionary<Tuple<int, int>, Board> getChildrenOfState(Board board, char playerChar)
        {
            Dictionary<Tuple<int, int>, Board> children = new Dictionary<Tuple<int, int>, Board>();                                       //create list of all children states
            List<Tuple<int, int>> legalMoves = board.getLegalMoves(playerChar);            //get all the legal move for the given player

            //iterate all the moves, for each move - create the state and add it to the list
            foreach (Tuple<int, int> move in legalMoves)
            {
                Board child = getState(move, board, playerChar);
                children.Add(move, child);
            }
            //return the list of all states.
            return children;
        }
        /// <summary>
        /// return the board after playing a given move of a given player.
        /// </summary>
        /// <param name="move"></param>
        /// <param name="board"></param>
        /// <param name="playerChar"></param>
        /// <returns></returns>
        private Board getState(Tuple<int, int> move, Board board, char playerChar)
        {
            Board state = new Board(board);
            state.fillPlayerMove(playerChar, move.Item1, move.Item2);
            return state;
        }
        /// <summary>
        /// calculate the board score of a given player
        /// </summary>
        /// <param name="board"></param>
        /// <param name="playerChar"></param>
        /// <returns></returns>
        private int scoreValue(Board board, char playerChar)
        {
            int score = 0;
            if (playerChar == '1')
            {
                score = board.gameScore().Item1;
            }
            else
            {
                score = board.gameScore().Item2;
            }
            return score;
        }
        /// <summary>
        /// return a heuristic value in the range of 1 to 5 according to the position the move is
        /// </summary>
        /// <param name="move"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private int AreaCapturedHeuristic(Tuple<int, int> move, int n)
        {
            int row = move.Item1;
            int col = move.Item2;

            //the position is in the corners
            if ((row == 0 && col == 0) || (row == n && col == 0) || (row == 0 && col == n) || (row == n && col == n))
                return 5;
            //the position is in the 2X2 squares in the corners
            else if ((row >= 0 && row <= 1) && (col <= 1 && col >= 0) || (row >= n - 1 && row <= n) && (col >= n - 1 && col <= n))
                return 1;
            //the position is in the edge rows or columns
            else if ((row == 0) || (col == 0) || (row == n) || (col == n))
                return 4;
            //the position is in the adjacent of the edge rows or columns
            else if ((row == 1) || (col == 1) || (row == n - 1) || (col == n - 1))
                return 2;
            //the position is in the middle
            else return 3;
        }
        /// <summary>
        /// return a heuristic value - a negative value of the number of legal moves that the other player has
        /// </summary>
        /// <param name="board"></param>
        /// <param name="playerChar"></param>
        /// <returns></returns>
        private double MobilityHeuristic(Board board, char playerChar)
        {
            double NegOtherPlaverMoves = -(board.getLegalMoves(Board.otherPlayer(playerChar)).Count * 0.2);
            return NegOtherPlaverMoves;
        }
        /// <summary>
        /// total heuristic that normalize all the heuristics values and return the sum of them
        /// </summary>
        /// <param name="move"></param>
        /// <param name="n"></param>
        /// <param name="board"></param>
        /// <param name="playerChar"></param>
        /// <returns></returns>
        private double TotalHeuristic(Tuple<int, int> move, int n, Board board, char playerChar)
        {
            return MobilityHeuristic(board, playerChar) + AreaCapturedHeuristic(move, n);
        }

        private double scoreValueWithHeuristic(Board board, char playerChar, Tuple<int, int> move)
        {
            double totalScore = scoreValue(board, playerChar) / (n*n) + AreaCapturedHeuristic(move, board._n) / 5 + MobilityHeuristic(board, playerChar) / (n * n);
            return totalScore;
        }
        /// <summary>
        /// check if there is time left for the turn to happen. Helps keep tracking the time
        /// </summary>
        /// <returns></returns>
        public bool isTime()
        {
            if (currTime.ElapsedMilliseconds < (TotalTime * 0.7))
                return true;
            return false;
        }

    }
}
