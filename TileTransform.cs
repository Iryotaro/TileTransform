using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;
using Ryocatusn.TileTransforms.Util;
using Ryocatusn.TileTransforms.Movements;

namespace Ryocatusn.TileTransforms
{
    public class TileTransform : MonoBehaviour, IEquatable<TileTransform>
    {
        public TileTransformId id;
        public Option<TilePosition> tilePosition { get; private set; } = new Option<TilePosition>(null);
        public TileDirection tileDirection { get; private set; }
        public Option<Movement> movement = new Option<Movement>(null);
        public List<Tilemap> tilemaps { get; private set; }
        private bool enable = true;

        [SerializeField]
        private List<Tilemap> m_tilemaps;

        private void Awake()
        {
            tilemaps = m_tilemaps;

            ChangeTilemap(tilemaps != null ? tilemaps.ToArray() : new Tilemap[0], transform.position);
            ChangeDirection(tileDirection = new TileDirection(TileDirection.Direction.Down));

            this.UpdateAsObservable()
                .Where(_ => enable)
                .Where(_ => !IsEnableMovement())
                .Subscribe(_ => SetPositionWhenDisableMovement());

            GetManager().Save(this);
        }
        private void OnDestroy()
        {
            GetManager().Delete(this);
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
                ChangePosition(new TilePosition(transform.position, tilemaps.ToArray()));
            }
            catch
            {
                SetDisable();
            }
        }
        public void SetDisable()
        {
            if (IsEnableMovement()) movement.Get().Kill();

            tilePosition.Set(null);
            enable = false;
        }

        public void ChangePosition(TilePosition tilePosition)
        {
            StopMovement();

            this.tilePosition.Set(new TilePosition(tilePosition.position.value, tilemaps.ToArray()));
        }
        public void ChangePosition(Vector2 worldPosition)
        {
            StopMovement();

            tilePosition.Set(new TilePosition(worldPosition, tilemaps.ToArray()));
        }
        public void ChangeDirection(TileDirection tileDirection)
        {
            this.tileDirection = tileDirection;
        }

        public void ChangeTilemap(Tilemap[] tilemaps, TilePosition startTilePosition)
        {
            SetDisable();

            this.tilemaps = tilemaps.ToList();
            foreach (Tilemap tilemap in this.tilemaps)
            {
                tilemap.OnDestroyAsObservable()
                    .FirstOrDefault()
                    .Subscribe(_ => TilemapOnDestroy());
            }
            transform.position = startTilePosition.GetWorldPosition();

            SetEnable();
        }
        public void ChangeTilemap(Tilemap[] tilemaps, Vector2 startWorldPosition)
        {
            SetDisable();

            this.tilemaps = tilemaps.ToList();
            foreach (Tilemap tilemap in this.tilemaps)
            {
                tilemap.OnDestroyAsObservable()
                    .FirstOrDefault()
                    .Subscribe(_ => TilemapOnDestroy());
            }
            transform.position = startWorldPosition;

            SetEnable();
        }
        public void AddTilemap(Tilemap addTilemap)
        {
            tilemaps.Add(addTilemap);

            addTilemap.OnDestroyAsObservable()
                .FirstOrDefault()
                .Subscribe(_ => TilemapOnDestroy());

            if (IsEnableMovement())
            {
                movement.Get().CompleteEvent.Subscribe(_ =>
                {
                    ChangePosition(transform.position);
                });
            }
            else
            {
                ChangePosition(transform.position);
            }
        }
        public void TilemapOnDestroy()
        {
            Debug.Log("tilemap on destroy");
            tilemaps.RemoveAll(x => x == null);

            TilePosition pos = tilePosition.Get();

            if (pos == null) return;

            if (!OnTheRoad(pos))
            {
                Debug.Log("tile position isn't on the road");
                SetDisable();
                return;
            }

            if (IsEnableMovement())
            {
                if (!movement.Get().IsActiveTilemaps()) SetDisable();
                if (!movement.Get().IsActiveTilemaps()) Debug.Log("movement has disable tilemaps");
                else Debug.Log("movement doesn't have disable tilemaps");
            }

            bool OnTheRoad(TilePosition tilePosition)
            {
                return tilePosition.position.tilemap != null;
            }
        }

        public void StopMovement()
        {
            movement.Match(Some: x => x.Stop());
        }

        public void SetMovement(IMoveDataCreater moveDataCreater, MoveRate moveRate)
        {
            if (!IsAllowedSetMovement(moveDataCreater)) return;

            MoveData moveData = moveDataCreater.GetData();
            movement.Set(new Movement(moveData, moveRate));

            movement.Match(Some: x =>
            {
                x.ChangeTilePositionEvent.Subscribe(x => tilePosition.Set(x)).AddTo(this);
                x.ChangeWorldPositionEvent.Subscribe(x => transform.position = x).AddTo(this);
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
            Movement movement = this.movement.Get();

            if (movement == null) return false;
            if (movement.isCompleted) return false;

            return true;
        }

        private void SetPositionWhenDisableMovement()
        {
            tilePosition.Match(Some: x => transform.position = x.GetWorldPosition());
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
