using System;
using UnityEngine;

namespace Gangs.Grid {
    public readonly struct GridPosition {
        public int X { get; }
        public int Y { get; }
        public int Z { get; }
        
        public GridPosition(int x, int y, int z) { X = x; Y = y; Z = z; }
        public GridPosition(Vector3 position) : this((int)Math.Round(position.x), (int)Math.Round(position.y), (int)Math.Round(position.z)) { }
        public GridPosition(float x, float y, float z) {
            X = (int) Math.Round(x);
            Y = (int) Math.Round(y);
            Z = (int) Math.Round(z);
        }
        
        public static GridPosition GridPositionFromDirection(GridPosition gridPosition, CardinalDirection direction) {
            var x = gridPosition.X + direction.ToDirection2D().X;
            var z = gridPosition.Z + direction.ToDirection2D().Z;
            return new GridPosition(x, gridPosition.Y, z);
        }
        
        public static bool CheckDirection(GridPosition a, GridPosition b, CardinalDirection direction) {
            return direction switch {
                CardinalDirection.North => a.Z < b.Z,
                CardinalDirection.South => a.Z > b.Z,
                CardinalDirection.East => a.X < b.X,
                CardinalDirection.West => a.X > b.X,
                _ => false
            };
        }
        
        public static CardinalDirection? GetCardinalDirection(GridPosition a, GridPosition b) {
            var dx = Math.Abs(a.X - b.X);
            var dz = Math.Abs(a.Z - b.Z);
            if (dx == 0 && dz == 0) return null;
            if (dx > dz) {
                return a.X > b.X ? CardinalDirection.West : CardinalDirection.East;
            }
            
            return a.Z > b.Z ? CardinalDirection.South : CardinalDirection.North;
        }
        
        public static int Distance(GridPosition a, GridPosition b) {
            var dx = Math.Abs(a.X - b.X);
            var dy = Math.Abs(a.Y - b.Y);
            var dz = Math.Abs(a.Z - b.Z);
            return Math.Max(dx, Math.Max(dy, dz));
        }
        
        public Vector3 ToVector3() => new(X, Y, Z);
        
        public override string ToString() => $"({X}, {Y}, {Z})";

        #region Operators

        public static GridPosition operator +(GridPosition a, GridPosition b) {
            return new GridPosition(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        
        public static GridPosition operator -(GridPosition a, GridPosition b) {
            return new GridPosition(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }
        
        public static bool operator ==(GridPosition a, GridPosition b) {
            return a.X == b.X && a.Y == b.Y && a.Z == b.Z;
        }
        
        public static bool operator !=(GridPosition a, GridPosition b) {
            return !(a == b);
        }
        
        public override bool Equals(object obj) {
            if (obj is GridPosition other) {
                return this == other;
            }
            return false;
        }
        
        public override int GetHashCode() {
            return X ^ Y ^ Z;
        }

        #endregion
    }
}