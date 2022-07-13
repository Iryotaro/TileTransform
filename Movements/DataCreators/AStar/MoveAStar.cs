using UnityEngine;

namespace Ryocatusn.TileTransforms.AStars
{
    public class MoveAStar : IMoveDataCreater
    {
        private Node startNode;
        private Node goalNode;
        private OpenList openList;
        private CloseList closeList;

        private bool isSucess = true;

        public MoveAStar(TileTransform target, Vector2 goalWorldPosition)
        {
            if (target.tilePosition.Get() == null)
            {
                isSucess = false;
                return;
            }

            TilePosition goalTilePosition = new TilePosition(goalWorldPosition, target.tilemaps);

            if (goalTilePosition.outSideRoad)
            {
                isSucess = false;
                return;
            }

            startNode = new Node(target.tilePosition.Get(), null);
            goalNode = new Node();
            openList = new OpenList();
            closeList = new CloseList();

            //オープン
            openList.Add(startNode);

            //親を設定

            //コストを設定

        }
        public MoveData GetData()
        {

        }
        public bool IsSuccess()
        {
            
        }
    }
}
