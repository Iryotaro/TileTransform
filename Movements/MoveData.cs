using System;
using System.Collections.Generic;

namespace Ryocatusn.TileTransforms
{
    public class MoveData
    {
        public TilePosition this[int index] 
        {
            get
            {
                TilePosition tilePosition = data[index];

                if (tilePosition == null) throw new Exception("move data に null が含まれてます");

                return tilePosition;
            }
        }

        public List<TilePosition> data { get; }

        public MoveData(List<TilePosition> data)
        {
            if (data.Count < 2) throw new ArgumentException("2つ以上設定する必要があります", nameof(data));
            this.data = data;
        }
        public int GetCount()
        {
            return data.Count;
        }
    }
}
