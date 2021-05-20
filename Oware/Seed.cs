/* This is an almost-complete translation of https://github.com/haarismemon/oware/ from Java to C#
*/
namespace Oware {
    public class Seed {
        private bool isCaptured = false;

        public bool GetIsCaptured() {
            return isCaptured;
        }

        public void SetIsCaptured(bool v) {
            isCaptured = v;
        }
    }
}