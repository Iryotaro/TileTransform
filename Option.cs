using System;

namespace Ryocatusn.TileTransforms.Util
{
    public class Option<T> : IEquatable<Option<T>>
    {
        private T value;

        public Option(T value)
        {
            this.value = value;
        }

        public void Match(Action<T> Some = null, Action None = null)
        {
            if (value != null)
            {
                if (Some != null) Some(value);
            }
            else
            {
                if (None != null) None();
            }
        }

        public void Set(T value)
        {
            this.value = value;
        }
        public T Get()
        {
            return value;
        }

        public void SetInArrayIndex(T[] values, int index)
        {
            int maxIndex = values.Length - 1;

            if (index > maxIndex) Set(default);
            else Set(values[index]);
        }

        public bool Equals(Option<T> other)
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
            return Equals((Option<T>)obj);
        }
        public override int GetHashCode()
        {
            return value != null ? value.GetHashCode() : 0;
        }
    }
}
