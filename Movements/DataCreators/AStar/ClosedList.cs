using System.Collections;
using System.Collections.Generic;

namespace Ryocatusn.TileTransforms.AStars
{
    public class ClosedList
    {
        private List<Node> nodes = new List<Node>();

        public void Add(Node node)
        {
            if (nodes.Contains(node)) return;

            nodes.Add(node);
        }
        public bool IsClosed(Node node)
        {
            return nodes.Find(x => x.Equals(node)) != null;
        }
        public void Remove(Node node)
        {
            nodes.Remove(node);
        }
    }
}
