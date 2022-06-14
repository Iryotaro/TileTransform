using System.Collections.Generic;

namespace Ryocatusn.TileTransforms.Movements
{
    public class MoveTranslate : IMoveDataCreater
    {
        private MoveData moveData;

        public MoveTranslate(TilePosition position, TileDirection tileDirection)
        {
            if (position == null)
            {
                moveData = null;
                return;
            }

            TilePosition nextPosition = position.GetAroundTile(tileDirection);

            if (nextPosition == null) moveData = null;
            else moveData = new MoveData(new List<TilePosition>() { position, nextPosition });
        }
        public MoveData GetData()
        {
            return moveData;
        }
        public bool IsSuccess()
        {
            return moveData != null;
        }
    }
}
