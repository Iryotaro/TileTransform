using System.Collections.Generic;
using System.Linq;

namespace Ryocatusn.TileTransforms.AStars
{
    public class OpenList
    {
        private List<Node> nodes = new List<Node>();

        public void Add(Node node)
        {
            if (nodes.Contains(node)) return;

            nodes.Add(node);
        }
        public Node GetLeastScoreNode()
        {
            return nodes.OrderBy(x => x.score).FirstOrDefault();
        }
        public void Remove(Node node)
        {
            nodes.Remove(node);
        }
    }
}
