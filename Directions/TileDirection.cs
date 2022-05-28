using System;
using UnityEngine;

namespace TileTransforms
{
    public class TileDirection
    {
        public Direction value { get; }
        private (Direction, Vector2)[] directionVectors = new (Direction, Vector2)[]
        {
            (Direction.Up, Vector2.up),
            (Direction.Down, Vector2.down),
            (Direction.Left, Vector2.left),
            (Direction.Right, Vector2.right),
        };

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right,
        }

        public TileDirection(Direction value)
        {
            this.value = value;
        }
        public TileDirection(Vector2 vector)
        {
            foreach ((Direction, Vector2) directionVector in directionVectors)
            {
                if (directionVector.Item2 == vector)
                {
                    value = directionVector.Item1;
                    return;
                }
            }
            throw new ArgumentException("許可されていない数値です", nameof(vector));
        }
        public Vector2 GetVector2()
        {
            Vector2 vector = Vector2.zero;

            foreach ((Direction, Vector2) directionVector in directionVectors)
            {
                if (directionVector.Item1 == value)
                {
                    vector = directionVector.Item2;
                }
            }

            return vector;
        }

        public bool Equals(TileDirection other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Equals(value, other.value);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(obj, this)) return true;
            if (GetType() != obj.GetType()) return false;
            return Equals((TileDirection)obj);
        }
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}
