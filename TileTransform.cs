using System;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Ryocatusn.TileTransforms.Movements;

namespace Ryocatusn.TileTransforms
{
    public class TileTransform : MonoBehaviour, IEquatable<TileTransform>
    {
        public TileTransformId id;
        public ReactiveProperty<TilePosition> tilePosition { get; private set; } = new ReactiveProperty<TilePosition>();
        public TileDirection tileDirection { get; private set; }
        private Movement movement;

        private bool enable = true;

        [SerializeField]
        private Tilemap[] tilemaps;

        private void Awake()
        {
            try
            {
                ChangePosition(new TilePosition(transform.position, tilemaps));
            }
            catch
            {
                SetDisable();
            }
            
            ChangeDirection(tileDirection = new TileDirection(TileDirection.Direction.Down));

            GetManager().Save(this);
        }
        private void OnDestroy()
        {
            GetManager().Delete(this);
        }

        private void Update()
        {
            if (!enable) return;

            if (IsActiveMovement())
            {
                TilePosition tilePosition = movement.GetTilePosition();
                this.tilePosition.Value = tilePosition;
                transform.position = movement.GetWorldPosition();
            }
            else
            {
                transform.position = tilePosition.Value.GetWorldPosition();
            }
        }

        public TileTransformManager GetManager()
        {
            return TileTransformManager.Instance;
        }

        public void SetEnable()
        {
            enable = true;

            try
            {
                ChangePosition(new TilePosition(transform.position, tilemaps));
            }
            catch
            {
                enable = false;
            }
        }
        public void SetDisable()
        {
            enable = false;
            tilePosition.Value = null;
            movement = null;
        }

        public void ChangeTilemap(Tilemap[] tilemaps)
        {
            if (movement != null) movement.Cancel();

            if (tilePosition.Value != null) tilePosition.Value = tilePosition.Value.ChangeTilemap(tilemaps);
            this.tilemaps = tilemaps;
        }
        
        public void ChangePosition(TilePosition tilePosition)
        {
            if (!enable) return;
            if (IsActiveMovement()) CancelMovement();
            this.tilePosition.Value = tilePosition;
        }
        public void ChangeDirection(TileDirection tileDirection)
        {
            if (!enable) return;
            this.tileDirection = tileDirection;
        }

        public void Translate(TileDirection direction, MoveRate moveRate)
        {
            if (direction == null) throw new ArgumentNullException(nameof(direction));

            MoveTranslate moveData = new MoveTranslate(tilePosition.Value, direction);
            ChangeMovement(moveData, moveRate);
        }
        public void Dijkstra(TilePosition destinationPosition, MoveRate moveRate)
        {
            if (destinationPosition == null) throw new ArgumentNullException(nameof(destinationPosition));

            MoveDijkstra tileMoveDijkstra = new MoveDijkstra(tilePosition.Value, destinationPosition);
            ChangeMovement(tileMoveDijkstra, moveRate);
        }

        public void ChangeMovement(IMoveDataCreater moveDataCreater, MoveRate moveRate)
        {
            if (!enable) return;
            if (!IsAllowedChangeMovement(moveDataCreater)) return;

            MoveData moveData = moveDataCreater.GetData();
            Movement movement = new Movement(moveData, moveRate);

            this.movement = movement;
        }

        public void CancelMovement()
        {
            if (!IsActiveMovement()) return;
            movement.Cancel();
        }

        private bool IsAllowedChangeMovement(IMoveDataCreater moveDataCreater)
        {
            if (!moveDataCreater.IsSuccess()) return false;
            if (IsActiveMovement()) return false;
            return true;
        }
        private bool IsActiveMovement()
        {
            if (movement == null || movement.IsCompleted()) return false;
            return true;
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
