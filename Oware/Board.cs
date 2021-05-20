/* This is an almost-complete translation of https://github.com/haarismemon/oware/ from Java to C#
*/
using System;
using System.Collections.Generic;

namespace Oware
{

    public class Board
    {
        private Player player1;
        private Player player2;
        private bool gameStarted;
        private bool gameOverNoMovesPossible;
        private House[][] board;
        public Board(Player player1, Player player2) {
            // initialize the grid of houses
            board = new House[2][];
            board[0] = new House[6];
            board[1] = new House[6];
            this.player1 = player1;
            this.player2 = player2;
            // randomly chooses who plays in the first turn
            SetFirstTurn();
            // initialises the houses on the board
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 6; j++) {
                    board[i][j] = new House(i, j);
                }
            }
        }

        public void SetFirstTurn() {
            Random rand = new Random();
            // random number generated either 0 or 1
            int whichPlayer = rand.Next(2);
            // sets the boolean variables for if its a players turn
            if (whichPlayer == 0) {
                player1.SetIsPlayersTurn(true);
                player2.SetIsPlayersTurn(false);
            } else if (whichPlayer == 1) {
                player1.SetIsPlayersTurn(false);
                player2.SetIsPlayersTurn(true);
            }
        }

        // Checks that the opponent will have some seeds at the end.
        // Returs true if the opponent has seeds on their side.
        public bool WillGiveOpponentSeeds(int i, int j) {
            int opponentRow = 0;
            if (i == 0) {
                opponentRow = 1;
            }
            // if there are seeds on the opponents row then return true;
            if (GetNumSeedsOnRow (opponentRow) > 0) {
                return true;
            } else {
                // there are no seeds on the opponents row
                int numberToDistribute = board[i][j].GetCount();
                House targetHouse = board[i][j];
                // moves the house to the target house
                for (int index = 0; index < numberToDistribute; index++) {
                    targetHouse = GetNextHouse(targetHouse);
                }
                // if the target house is on the opponents side then return true
                if (targetHouse.GetXPos() != i) {
                    return true;
                }
                // if the target house is on the players side you need to check to make
                // sure the move gives the opponent seeds
                CheckValidMoves(i);
                return false;
            }
        }

        // checks if the move is valid
        private bool CheckValidMoves(int i) {
            for (int j = 0; j < 6; j++) {
                // stores the number of seeds in the house
                int numberToDistribute = board[i][j].GetCount();
                House targetHouse = board[i][j];
                // keep moving to next house as there are seeds
                for (int index = 0; index < numberToDistribute; index++) {
                    targetHouse = GetNextHouse(targetHouse);
                }
                // checks to see if the target house is on the opponents side
                if (targetHouse.GetXPos() != i) {
                    // if it is, it means the move will give the opponend seeds
                    return true;
                }
            }
            // otherwise the game is over, so the players capture their own seeds.
            // the method returns false because there is no valid move.
            CaptureOwnSeeds();
            SetGameIsOver(true);
            return false;
        }

        // performs the sowing action
        public void Sow(int i, int j) {
            // only allows the move if it is valid
            if (board[i][j].GetCount() != 0 && WillGiveOpponentSeeds(i, j)) {
                gameStarted = true;

                // get list of seeds and empty house
                IList<Seed> toSow = board[i][j].GetSeedsAndEmptyHouse();

                House currentHouse = board[i][j]; // get the current house
                for (int index = 0; index < toSow.Count; index++) {
                    // get the next one
                    currentHouse = GetNextHouse(currentHouse);
                    /* 12-seed rule: if sowing more than 12 seeds, we don't want
                       to replant in the starting house, so it is skipped
                    */
                    if (currentHouse == board[i][j]) {
                        currentHouse = GetNextHouse(currentHouse);
                    }
                    currentHouse.AddSeedInPot(toSow[index]);
                }

                // start capture from the last house
                Capture(currentHouse.GetXPos(), currentHouse.GetYPos(), GetPlayerTurn());

                // switches the players' turns
                player1.SetIsPlayersTurn(!player1.IsPlayersTurn());
                player2.SetIsPlayersTurn(!player2.IsPlayersTurn());
            }
        }

        // return the number of seeds currently on a particular row
        public int GetNumSeedsOnRow(int row) {
            int totalSeeds = 0;
            for (int col = 0; col < 6; col++) {
                totalSeeds += board[row][col].GetCount();
            }
            return totalSeeds;
        }

        // get the House at coordinate (i, j)
        public House getHouseOnBoard(int i, int j) {
            return board[i][j];
        }

        // returns if the game has been started
        public bool IsGameStarted() {
            return gameStarted;
        }

        private void Capture(int x, int y, int playerTurn) {
            House currentHouse = board[x][y];

            if (playerTurn == 1) { // player 2 made the last move
                CaptureHelper(player2, currentHouse, 1);
            } else { // player 1 made the last move
                CaptureHelper(player1, currentHouse, 0);
            }
        }

        // Method to actually capture the seeds.  The house is only captured
        // if it contains 2 or 3 seeds.
        private void CaptureHelper(Player lastPlayer, House lastHouse, int playerNumber) {
            List<House> toCapture = new List<House>();
            // checks to make sure the house is on the opponents side and it has 2 or 3 seeds
            if (lastHouse.GetXPos() != playerNumber && (lastHouse.GetCount() == 2 || lastHouse.GetCount() == 3)) {
                // add house to list of houses
                toCapture.Add(lastHouse);

                // get previous house
                House previousHouse = GetPreviousHouse(lastHouse);
                for (int j = 0; j < 6; j++) {
                    // if still on the opponents row and has size 2 or 3
                    if (previousHouse.GetXPos() == lastHouse.GetXPos() && (previousHouse.GetCount() == 2 || previousHouse.GetCount() == 3)) {
                        // add house to list to capture
                        toCapture.Add(previousHouse);
                        // moves to the next previous house;
                        previousHouse = GetPreviousHouse(previousHouse);
                    } else { // quit the loop
                        break;
                    }
                }
            }

            // only go through if we have something to capture (list not empty)
            if (toCapture.Count > 0) {
                // keeps track of how many seeds captured
                int capturedSeedTotal = 0;
                // for each house that is captured, add the seed count to the counter
                foreach (House capturedHouse in toCapture) {
                    capturedSeedTotal += capturedHouse.GetCount();
                }
                // keep track of how many seeds on the row
                int totalOnRow = 0;
                // for each house on the row, add the seed count to the counter
                // (to check if opponent still has seeds)
                for (int j = 0; j < 6; j++) {
                    totalOnRow += board[lastHouse.GetXPos()][j].GetCount();
                }

                // if the opponent now has no more seeds, then forfeit capture
                if (capturedSeedTotal != totalOnRow) {
                    foreach (House house in toCapture) {
                        // for each house get and empty the seeds
                        List<Seed> toAddToScoreHouse = house.GetSeedsAndEmptyHouse();
                        // add each seed to the players score house
                        foreach (Seed seed in toAddToScoreHouse) {
                            seed.SetIsCaptured(true);
                            lastPlayer.AddSeedToScoreHouse(seed);
                        }
                    }
                }
            }
        }

        private int GetPlayerTurn() {
            if (player1.IsPlayersTurn()) {
                return 0;
            }
            return 1;
        }

        // Get next house in anticlockwise rotation by checking which row.
        // If on first row, we go backwards, if on second row we go forwards.
        public House GetNextHouse(House house) {
            int currentX = house.GetXPos();
            int currentY = house.GetYPos();

            // on the first row
            if (currentX == 0) {
                // on the first column
                if (currentY == 0) {
                    // move up.
                    return board[currentX + 1][currentY];
                } else {
                    // else move to the left
                    return board[currentX][currentY - 1];
                }
            } else {
                // on second row
                // on last column
                if (currentY == 5) {
                    return board[currentX - 1][currentY];
                } else {
                    // else move to the right
                    return board[currentX][currentY + 1];
                }
            }
        }

        // Get previous house in clockwise rotation by checking which row.
        // If on first row, we go forwards, if on second row we go backwards.
        public House GetPreviousHouse(House house) {
            int currentX = house.GetXPos();
            int currentY = house.GetYPos();

            // on the first row
            if (currentX == 0) {
                // on the first column
                if (currentY == 5) {
                    // move up.
                    return board[currentX + 1][currentY];
                } else {
                    // else move to the right
                    return board[currentX][currentY + 1];
                }
            } else {
                // on second row
                // on last column
                if (currentY == 0) {
                    // move down
                    return board[currentX - 1][currentY];
                } else {
                    // move left
                    return board[currentX][currentY - 1];
                }
            }
        }

        // check to see if the game has been won
        public bool GameWonCheck() {
            // if a player has more than 24 seeds they have won
            if (player1.GetScore() > 24 || player2.GetScore() > 24) {
                return true;
            }
            return false;
        }

        // check to see if the game ended in a draw
        public bool GameDrawCheck() {
            // the game is a draw if both players have 24 seeds
            if (player1.GetScore() == 24 && player2.GetScore() == 24) {
                return true;
            }
            return false;
        }

        // Each player captures all the seeds on their own side
        public void CaptureOwnSeeds() {
            for (int j = 0; j < 6; j++) {
                // list of seeds for the house (that is emptied) on player 1 side
                List<Seed> toAddToPlayer1 = new List<Seed>(board[0][j].GetSeedsAndEmptyHouse());
                foreach (Seed seed in toAddToPlayer1) {
                    // each seed is captured
                    seed.SetIsCaptured(true);
                    // each seed is added to the player 1 score house
                    player1.AddSeedToScoreHouse(seed);
                }

                // list of seeds for the house (that is emptied) on player 2 side
                List<Seed> toAddToPlayer2 = new List<Seed>(board[1][j].GetSeedsAndEmptyHouse());
                foreach (Seed seed in toAddToPlayer2) {
                    // each seed is captured
                    seed.SetIsCaptured(true);
                    // each seed is added to the player 2 score house
                    player2.AddSeedToScoreHouse(seed);
                }
            }
        }

        // get the number of seeds that are stored in a particular house
        public int GetHouseCount(int i, int j) {
            return board[i][j].GetCount();
        }

        // get the Player 1's name
        public string GetPlayer1Name() {
            return player1.GetName();
        }

        // get the Player 2's name
        public string GetPlayer2Name() {
            return player2.GetName();
        }

        public void SetPlayer1Name(string name) {
            player1.SetName(name);
        }

        public void SetPlayer2Name(string name) {
            player2.SetName(name);
        }

        public int GetPlayer1Score() {
            return player1.GetScore();
        }

        // get the Player 2's name
        public int GetPlayer2Score() {
            return player2.GetScore();
        }

        public bool IsGameOverNoMovesPossible() {
            return gameOverNoMovesPossible;
        }

        public void SetGameIsOver(bool gameOver) {
            gameOverNoMovesPossible = gameOver;
        }

        public void resetBoard() {
            for (int i = 0; i < 2; i++) {
                for (int j = 0; j < 6; j++) {
                    board[i][j].ResetHouse();
                }
            }
            player1.ClearScoreHouse();
            player2.ClearScoreHouse();
            gameStarted = false;
            gameOverNoMovesPossible = false;
            SetFirstTurn();
        }
    }
}