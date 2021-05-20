/* This is an almost-complete translation of https://github.com/haarismemon/oware/ from Java to C#
*/
namespace Oware {
    public class Player {
        // sets or returns the name of the player
        private string name;
        private ScoreHouse scoreHouse; // player's scorehouse
        private bool isPlayersTurn; // is true when it is player's turn

        // create Player with a name and initialized (empty) score house
        public Player(string name) {
            this.name = name;
            isPlayersTurn = false;
            scoreHouse = new ScoreHouse();
        }

        public string GetName() {
            return name;
        }

        public void SetName(string name) {
            this.name = name;
        }

        public ScoreHouse GetScoreHouse() {
            return scoreHouse;
        }

        public int GetScore() {
            return scoreHouse.GetCount();
        }

        public bool IsPlayersTurn() {
            return isPlayersTurn;
        }

        public void SetIsPlayersTurn(bool value) {
            isPlayersTurn = value;
        }

        // adds a seed to the Player's Score House
        public void AddSeedToScoreHouse(Seed seed) {
            scoreHouse.AddSeed(seed);
        }

        // Resets the player's scorehouse
        public void ClearScoreHouse() {
            scoreHouse.Reset();
        }

        public override string ToString()
        {
            return name;
        }
    }
}