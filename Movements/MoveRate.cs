using System;

namespace TileTransforms.Movements
{
    public class MoveRate
    {
        public float value;

        public MoveRate(float value)
        {
            if (value < 0) throw new ArgumentException("0未満は許可されていません", nameof(value));

            this.value = value;
        }
        public MoveRate SpeedUp(MoveRate speed)
        {
            return new MoveRate(value + speed.value);
        }
        public MoveRate SlowDown(MoveRate speed)
        {
            if (value - speed.value < 0) return new MoveRate(0);
            return new MoveRate(value - speed.value);
        }
    }
}
