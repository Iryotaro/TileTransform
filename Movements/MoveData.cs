using System;
using System.Collections.Generic;

namespace TileTransforms.Movements
{
    public class MoveData
    {
        private List<TilePosition> data;
        public bool finish { get; private set; }

        public MoveData(List<TilePosition> data)
        {
            if (data.Count < 2) throw new ArgumentException("2つ以上設定する必要があります", nameof(data));
            this.data = data;
        }
        public TilePosition GetNextPosition()
        {
            if (finish) return null;

            TilePosition tilePosition = data[0];
            data.RemoveAt(0);

            if (data.Count == 0) finish = true;

            if (tilePosition == null) throw new Exception("move data に null が含まれてます");

            return tilePosition;
        }
    }
}
