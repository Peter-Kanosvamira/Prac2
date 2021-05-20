/* This is an almost-complete translation of https://github.com/haarismemon/oware/ from Java to C#
*/
using System;
using System.Collections.Generic;

namespace Oware {
    public class House : IEquatable<House> {
        private List<Seed> seedsInHouse;
        public int xPos, yPos;
        
        public House(int x, int y) {
            seedsInHouse = new List<Seed>();
            InitializeHouse();
        }

        void InitializeHouse() {
            for (int i = 0; i < 4; i++) {
                seedsInHouse.Add(new Seed());
            }
        }

        public void AddSeedInPot(Seed seed) {
            seedsInHouse.Add(seed);
        }

        public int GetCount() {
            return seedsInHouse.Count;
        }

        public void ResetHouse() {
            seedsInHouse.Clear();
            InitializeHouse();
        }

        public List<Seed> GetSeedsAndEmptyHouse() {
            List<Seed> temp = new List<Seed>(seedsInHouse);
            seedsInHouse.Clear();
            return temp;
        }

        public IReadOnlyList<Seed> GetSeeds() {
            return seedsInHouse;
        }

        public int GetXPos() {
            return xPos;
        }

        public int GetYPos() {
            return yPos;
        }

        public override string ToString()
        {
            return $"{xPos}, {yPos}";
        }

        public override bool Equals(object obj)
        {
            return this.Equals((House)obj);
        }

        public bool Equals(House otherHouse)
        {
            return otherHouse.GetXPos() == GetXPos() && otherHouse.GetYPos() == GetYPos();
        }

        public override int GetHashCode()
        {
            return xPos ^ yPos; // XOR of xpos and ypos
        }
    }
}