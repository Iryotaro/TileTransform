using UnityEngine;

namespace Ryocatusn.TileTransforms
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
            throw new TileTransformException("許可されていない数値です");
        }
        public TileDirection(float angle)
        {
            angle = Mathf.Abs(angle % 360);

            if (45 <= angle && angle < 135) value = Direction.Left;
            else if (135 <= angle && angle < 225) value = Direction.Down;
            else if (225 <= angle && angle < 315) value = Direction.Right;
            else value = Direction.Up;
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
        public Quaternion GetRotation()
        {
            return value switch
            {
                Direction.Up => Quaternion.Euler(0, 0, 0),
                Direction.Down => Quaternion.Euler(0, 0, 180),
                Direction.Left => Quaternion.Euler(0, 0, 90),
                Direction.Right => Quaternion.Euler(0, 0, -90),
                _ => Quaternion.Euler(0, 0, 0)
            };
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
