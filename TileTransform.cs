﻿using System;
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
        private Option<Movement> movement = new Option<Movement>(null);
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

            foreach (Tilemap tilemap in tilemaps)
            {
                tilemap.OnDestroyAsObservable()
                    .FirstOrDefault()
                    .Subscribe(_ => SetDisable());
            }

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
            CancelMovement();

            tilePosition.Set(null);
            enable = false;
        }

        public void ChangePosition(TilePosition tilePosition)
        {
            CancelMovement();

            this.tilePosition.Set(tilePosition);
        }
        public void ChangeDirection(TileDirection tileDirection)
        {
            this.tileDirection = tileDirection;
        }

        public void ChangeTilemap(Tilemap[] tilemaps)
        {
            SetDisable();
            this.tilemaps = tilemaps;
            SetEnable();
        }

        public void CancelMovement()
        {
            movement.Match(Some: x => x.Cancel());
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
