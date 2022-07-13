using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Tilemaps;
using Ryocatusn.TileTransforms.Util;
using Ryocatusn.TileTransforms.Movements;

namespace Ryocatusn.TileTransforms
{
    public class TileTransform : MonoBehaviour, IEquatable<TileTransform>
    {
        public TileTransformId id;
        public Option<TilePosition> tilePosition = new Option<TilePosition>(null);
        public TileDirection tileDirection { get; private set; }
        public Option<Movement> movement { get; } = new Option<Movement>(null);
        public List<Tilemap> tilemaps { get; private set; }
        private bool enable = true;

        [SerializeField]
        private List<Tilemap> m_tilemaps;

        private void Awake()
        {
            tilemaps = m_tilemaps ?? new List<Tilemap>();

            id = new TileTransformId(Guid.NewGuid().ToString());

            ChangePosition(transform.position);
            ChangeDirection(tileDirection = new TileDirection(TileDirection.Direction.Down));

            GetManager().Save(this);
        }
        private void OnDestroy()
        {
            GetManager().Delete(this);
        }

        private void Update()
        {
            tilePosition.Match(Some: x => { if (enable && x.outSideRoad) SetDisable(); });
            if (!IsEnableMovement() && enable) SetPositionWhenDisableMovement();
        }

        public TileTransformManager GetManager()
        {
            return TileTransformManager.Instance;
        }

        public void SetEnable()
        {
            ChangePosition(transform.position);
            enable = true;
        }
        public void SetDisable()
        {
            KillMovement();
            enable = false;
        }

        public void ChangePosition(Vector2 worldPosition, bool killMovement = true)
        {
            if (killMovement) KillMovement();

            tilePosition.Set(new TilePosition(worldPosition, tilemaps));
        }
        public void ChangeDirection(TileDirection tileDirection)
        {
            this.tileDirection = tileDirection;
        }
        public void StopMovement()
        {
            movement.Match(x => x.Stop());
        }
        private void KillMovement()
        {
            if (IsEnableMovement()) movement.Match(x => x.Kill());
        }

        public void SetMovement(IMoveDataCreater moveDataCreater, MoveRate moveRate)
        {
            if (!IsAllowedSetMovement(moveDataCreater)) return;

            MoveData moveData = moveDataCreater.GetData();
            movement.Set(new Movement(moveData, moveRate));

            movement.Match(Some: x =>
            {
                x.ChangeTilePositionEvent.Subscribe(x => ChangePosition(x.GetWorldPosition(), false)).AddTo(this);
                x.ChangeWorldPositionEvent.Subscribe(x => transform.position = x).AddTo(this);
                x.CompleteEvent.Subscribe(_ => movement.Set(null));
            });
        }
        private bool IsAllowedSetMovement(IMoveDataCreater moveDataCreater)
        {
            if (!enable) return false;
            if (!moveDataCreater.IsSuccess()) return false;
            if (IsEnableMovement()) return false;
            return true;
        }
        private bool IsEnableMovement()
        {
            if (movement.Get() == null) return false;
            if (movement.Get().isCompleted) return false;
            return true;
        }

        private void SetPositionWhenDisableMovement()
        {
            tilePosition.Match(Some: x => transform.position = x.GetWorldPosition());
        }

        public void ChangeTilemap(Tilemap[] tilemaps, Vector2 startWorldPosition)
        {
            this.tilemaps = tilemaps.ToList();

            SetDisable();
            transform.position = startWorldPosition;
            ChangePosition(startWorldPosition);
            SetEnable();
        }
        public void AddTilemap(Tilemap addTilemap)
        {
            tilemaps.Add(addTilemap);

            tilePosition.Match(Some: x => ChangePosition(x.GetWorldPosition(), false));
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
