using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Ryocatusn.TileTransforms.AStars;

namespace Ryocatusn.TileTransforms
{
    public class MoveAStar : IMoveDataCreater
    {
        private Node startNode;
        private Node goalNode;
        private OpenList openList;
        private ClosedList closedList;

        private List<TilePosition> data = new List<TilePosition>();

        private bool isSucess = true;

        public MoveAStar(Vector2 startWorldPosition, Vector2 goalWorldPosition, List<Tilemap> tilemaps)
        {
            TilePosition startTilePosition = new TilePosition(startWorldPosition, tilemaps);
            TilePosition goalTilePosition = new TilePosition(goalWorldPosition, tilemaps);

            if (startTilePosition.IsOutsideRoad() || goalTilePosition.IsOutsideRoad())
            {
                isSucess = false;
                return;
            }

            startNode = new Node(startTilePosition);
            goalNode = new Node(goalTilePosition);
            openList = new OpenList();
            closedList = new ClosedList();

            CreateData(Search());

            Node Search()
            {
                Open(startNode);

                while (true)
                {
                    Node baseNode = GetBaseNode();
                    Node[] nodes = OpenAroundTile(baseNode);

                    foreach (Node node in nodes)
                        if (node.Equals(goalNode)) return node;

                    Close(baseNode);
                }
            }
            void CreateData(Node goalNode)
            {
                Node node = goalNode;

                while (true)
                {
                    if (node.parentNode == null) break;

                    data.Add(node.parentNode.tilePosition);
                    node = node.parentNode;
                }

                data.Reverse();
                data.Add(goalNode.tilePosition);
            }
        }

        private Node GetBaseNode()
        {
            Node node = openList.GetLeastScoreNode();
            return node;
        }
        private Node[] OpenAroundTile(Node baseNode)
        {
            List<Node> nodes = new List<Node>();
            TilePosition[] aroundPositions = baseNode.GetAroundPosition();

            if (aroundPositions.Length == 0) throw new TileTransformException("Goalが見つかりません");

            foreach (TilePosition aroundPosition in aroundPositions)
            {
                Node aroundNode = new Node(aroundPosition);
                if (closedList.IsClosed(aroundNode)) continue;

                aroundNode.SetParentNode(baseNode);

                nodes.Add(aroundNode);

                Open(aroundNode);
            }

            return nodes.ToArray();
        }
        private void Open(Node node)
        {
            openList.Add(node);

            node.SetScore(goalNode);
        }
        private void Close(Node node)
        {
            openList.Remove(node);
            closedList.Add(node);
        }

        public MoveData GetData()
        {
            return new MoveData(data);
        }
        public bool IsSuccess()
        {
            return isSucess;
        }
    }
}
