using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gangs.Grid {
    
    public enum CardinalDirection {
        North = 0,
        NorthEast = 1,
        East = 2,
        SouthEast = 3,
        South = 4,
        SouthWest = 5,
        West = 6,
        NorthWest = 7
    }

    public struct Direction2D {
        public readonly int X;
        public readonly int Z;

        public Direction2D(int x, int z) {
            X = x;
            Z = z;
        }

        public Direction2D(GridPosition gridPosition) {
            X = gridPosition.X;
            Z = gridPosition.Z;
        }
    }

    public static class CardinalDirectionExtensions {
        public static Direction2D ToDirection2D(this CardinalDirection direction) {
            return direction switch {
                CardinalDirection.North => new Direction2D(0, 1),
                CardinalDirection.East => new Direction2D(1, 0),
                CardinalDirection.South => new Direction2D(0, -1),
                CardinalDirection.West => new Direction2D(-1, 0),
                CardinalDirection.NorthEast => new Direction2D(1, 1),
                CardinalDirection.SouthEast => new Direction2D(1, -1),
                CardinalDirection.SouthWest => new Direction2D(-1, -1),
                CardinalDirection.NorthWest => new Direction2D(-1, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(direction))
            };
        }
        
        public static CardinalDirection ToCardinalDirection(this Direction2D direction) {
            if (direction is {X: 0, Z: 0}) throw new ArgumentException("Direction cannot be zero");
            var angle = Math.Atan2(direction.X, direction.Z);
            angle = angle * 180.0 / Math.PI;
            if (angle < 0) angle += 360;

            return angle switch {
                < 22.5 => CardinalDirection.North,
                >= 337.5 => CardinalDirection.North,
                < 67.5 => CardinalDirection.NorthEast,
                < 112.5 => CardinalDirection.East,
                < 157.5 => CardinalDirection.SouthEast,
                < 202.5 => CardinalDirection.South,
                < 247.5 => CardinalDirection.SouthWest,
                < 292.5 => CardinalDirection.West,
                _ => CardinalDirection.NorthWest
            };
        }
        
        public static CardinalDirection GetOpposite(this CardinalDirection direction) {
            var cd = direction.ToDirection2D();
            return new Direction2D(-cd.X, -cd.Z).ToCardinalDirection();
        }

        public static IEnumerable<CardinalDirection> GetFourAdjacent(this CardinalDirection direction) {
            return direction switch {
                CardinalDirection.North => new[] {CardinalDirection.East, CardinalDirection.West},
                CardinalDirection.East => new[] {CardinalDirection.North, CardinalDirection.South},
                CardinalDirection.South => new[] {CardinalDirection.East, CardinalDirection.West},
                CardinalDirection.West => new[] {CardinalDirection.North, CardinalDirection.South},
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Not a 4-cardinal direction")
            };
        }

        public static IEnumerable<CardinalDirection> GetCardinals(bool includeDiagonals) {
            if (includeDiagonals) return Enum.GetValues(typeof(CardinalDirection)) as CardinalDirection[];
            return new[] {
                CardinalDirection.North,
                CardinalDirection.East,
                CardinalDirection.South,
                CardinalDirection.West
            };
        }
    }
}