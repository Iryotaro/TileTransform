using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ryocatusn.TileTransforms.AStars
{
    public class Node : IEquatable<Node>
    {
        public TilePosition tilePosition { get; }
        public Node parentNode { get; private set; }

        public float realCost { get; private set; }
        public float heuristicCost { get; private set; }
        public float score { get; private set; }

        public Node(TilePosition tilePosition)
        {
            if (tilePosition == null) throw new TileTransformException("node��tilePosition��null�ł�");
            this.tilePosition = tilePosition;
        }

        public void SetScore(Node goalNode)
        {
            realCost = GetRealCost(goalNode);
            heuristicCost = GetHeuristicCost(goalNode);
            score = realCost + heuristicCost;
        }

        private float GetRealCost(Node goalNode)
        {
            return goalNode.realCost + 1;
        }
        private float GetHeuristicCost(Node goalNode)
        {
            float x = Mathf.Abs(GetWorldPosition().x - goalNode.GetWorldPosition().x);
            float y = Mathf.Abs(GetWorldPosition().y - goalNode.GetWorldPosition().y);
            return x + y;
        }
        private Vector2 GetWorldPosition()
        {
            return tilePosition.GetWorldPosition();
        }

        public TilePosition[] GetAroundPosition()
        {
            List<TilePosition> aroundPositions = new List<TilePosition>();

            foreach (TileDirection.Direction direction in Enum.GetValues(typeof(TileDirection.Direction)))
            {
                TilePosition aroundPosition = tilePosition.GetAroundTilePosition(new TileDirection(direction));
                if (aroundPosition == null) continue;
                aroundPositions.Add(aroundPosition);
            }

            return aroundPositions.ToArray();
        }

        public void SetParentNode(Node parentNode)
        {
            this.parentNode = parentNode;
        }

        public bool Equals(Node other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Equals(tilePosition.cellPosition, other.tilePosition.cellPosition);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(obj, this)) return true;
            if (GetType() != obj.GetType()) return false;
            return Equals((Node)obj);
        }
        public override int GetHashCode()
        {
            return tilePosition.cellPosition.GetHashCode();
        }
    }
}
