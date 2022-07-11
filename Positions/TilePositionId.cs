using System;

namespace Ryocatusn.TileTransforms
{
    public class TilePositionId : IEquatable<TilePositionId>
    {
        public string value { get; }

        public TilePositionId(string value)
        {
            this.value = value;
        }

        public bool Equals(TilePositionId other)
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
            return Equals((TilePositionId)obj);
        }
        public override int GetHashCode()
        {
            return value != null ? value.GetHashCode() : 0;
        }
    }
}
