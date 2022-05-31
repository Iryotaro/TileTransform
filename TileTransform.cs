using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileTransforms.Movements;

namespace TileTransforms
{
    public class TileTransform : MonoBehaviour, IEquatable<TileTransform>
    {
        public TileTransformId id;
        public TilePosition tilePosition { get; private set; }
        public TileDirection tileDirection { get; private set; }
        private Movement movement;

        [SerializeField]
        private Tilemap tilemap;

        private void Awake()
        {
            ChangePosition(new TilePosition(transform.position, tilemap));
            ChangeDirection(tileDirection = new TileDirection(TileDirection.Direction.Down));
        }
        private void Update()
        {
            transform.position = tilePosition.position;

            if (movement == null) return;

            tilePosition = movement.GetTilePosition();
            transform.position = movement.GetWorldPosition();
        }

        public void ChangePosition(TilePosition tilePosition)
        {
            this.tilePosition = tilePosition;
            transform.position = tilePosition.GetWorldPosition();
        }
        public void ChangeDirection(TileDirection tileDirection)
        {
            this.tileDirection = tileDirection;
        }

        public void Translate(TileDirection direction, MoveRate moveRate)
        {
            if (direction == null) throw new ArgumentNullException(nameof(direction));

            MoveTranslate moveData = new MoveTranslate(tilePosition, direction);
            ChangeMovement(moveData, moveRate);
        }
        public void Dijkstra(TilePosition destinationPosition, MoveRate moveRate)
        {
            if (destinationPosition == null) throw new ArgumentNullException(nameof(destinationPosition));

            TileMoveDijkstra tileMoveDijkstra = new TileMoveDijkstra(tilePosition, destinationPosition);
            ChangeMovement(tileMoveDijkstra, moveRate);
        }

        public void ChangeMovement(IMoveDataCreater moveDataCreater, MoveRate moveRate)
        {
            if (!IsAllowedChangeMovement(moveDataCreater)) return;

            MoveData moveData = moveDataCreater.GetData();
            Movement movement = new Movement(moveData, moveRate);

            this.movement = movement;
        }

        private bool IsAllowedChangeMovement(IMoveDataCreater moveDataCreater)
        {
            if (!moveDataCreater.IsSuccess()) return false;
            if (movement == null) return true;
            if (movement.IsCompleted()) return true;
            return false;
        }

        public bool Equals(TileTransform other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Equals(id, other.id);
        }
        public override bool Equals(object other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            if (GetType() != other.GetType()) return false;
            return Equals((TileTransform)other);
        }
        public override int GetHashCode()
        {
            return id != null ? id.GetHashCode() : 0;
        }
    }
}
