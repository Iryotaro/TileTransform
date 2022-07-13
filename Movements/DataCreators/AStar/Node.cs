using UnityEngine;

namespace Ryocatusn.TileTransforms.AStars
{
    public class Node
    {
        private TilePosition tilePosition { get; }
        private TilePosition parentTilePosition { get; }
        private int cost;

        public Node(TilePosition tilePosition, TilePosition parentTilePosition)
        {
            this.tilePosition = tilePosition;
            this.parentTilePosition = parentTilePosition;
        }
    }
}
