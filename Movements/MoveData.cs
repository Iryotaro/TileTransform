using System;
using System.Collections.Generic;
using System.Linq;

namespace Ryocatusn.TileTransforms.Movements
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

        private List<TilePosition> data;

        public MoveData(List<TilePosition> data)
        {
            if (data.Count < 2) throw new ArgumentException("2つ以上設定する必要があります", nameof(data));
            this.data = data;
        }
        public int GetCount()
        {
            return data.Count;
        }
        public bool IsActiveTilemaps()
        {
            return data.Where(x => x.position.tilemap == null).Count() == 0;
        }
    }
}
