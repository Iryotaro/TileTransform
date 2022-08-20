using System;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ryocatusn.TileTransforms
{
    public class TilePosition : IEquatable<TilePosition>
    {
        private TilePositionId id { get; }
        private Vector3 worldPosition { get; }
        public Vector3Int cellPosition { get; private set; }
        private Tilemap tilemap;
        private List<Tilemap> tilemaps;

        public TilePosition(Vector3 worldPosition, List<Tilemap> tilemaps)
        {
            id = new TilePositionId(Guid.NewGuid().ToString());

            tilemaps.RemoveAll(x => x == null);
            this.tilemaps = tilemaps;
            this.worldPosition = worldPosition;

            foreach (Tilemap tilemap in this.tilemaps)
            {
                this.tilemap = tilemap;
                cellPosition = this.tilemap.WorldToCell(worldPosition);

                this.tilemap.OnDestroyAsObservable()
                    .Subscribe(_ => TilemapOnDestroy(tilemap));

                if (this.tilemap.GetTile(cellPosition) != null) return;
            }
        }

        private void TilemapOnDestroy(Tilemap destroyTilemap)
        {
            tilemaps.Remove(destroyTilemap);
            tilemaps.RemoveAll(x => x.Equals(null));

            if (tilemap.Equals(destroyTilemap))
            {
                foreach (Tilemap tilemap in tilemaps)
                {
                    this.tilemap = tilemap;
                    cellPosition = this.tilemap.WorldToCell(worldPosition);

                    if (this.tilemap.GetTile(tilemap.WorldToCell(worldPosition)) != null) return;
                }
            }
        }
        public TilePosition GetAroundTilePosition(TileDirection tileDirection)
        {
            if (IsOutsideRoad()) return null;

            foreach (Tilemap tilemap in tilemaps)
            {
                if (tilemap == null) continue;

                Vector3 nowPosition = tilemap.CellToWorld(tilemap.WorldToCell(GetWorldPosition()));
                Vector3 direction = tileDirection.GetVector2();
                Vector3 distanceEachTile = Vector3.Scale(tilemap.cellSize + tilemap.cellGap, tilemap.transform.lossyScale);

                Vector3 aroundTilePosition = nowPosition + Vector3.Scale(direction, distanceEachTile);

                if (tilemap.GetTile(tilemap.WorldToCell(aroundTilePosition)) != null) return new TilePosition(aroundTilePosition, tilemaps);
            }

            return null;
        }
        public Vector3 GetWorldPosition()
        {
            return tilemap.CellToWorld(cellPosition) + Vector3.Scale(tilemap.cellSize, tilemap.transform.lossyScale) / 2;
        }

        public bool IsOutsideRoad()
        {
            foreach (Tilemap tilemap in tilemaps)
            {
                this.tilemap = tilemap;
                cellPosition = this.tilemap.WorldToCell(worldPosition);

                if (this.tilemap.GetTile(tilemap.WorldToCell(worldPosition)) != null) return false;
            }

            return true;
        }

        public bool IsSamePlace(TilePosition other, float allowedRange = 0.3f)
        {
            if ((GetWorldPosition() - other.GetWorldPosition()).magnitude < allowedRange) return true;
            return false;
        }
        public bool IsSamePlace(Vector3 globalPosition, float allowedRange = 0.3f)
        {
            if ((GetWorldPosition() - globalPosition).magnitude < allowedRange) return true;
            return false;
        }

        public bool Equals(TilePosition other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Equals(id, other.id);
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(obj, null)) return false;
            if (ReferenceEquals(obj, this)) return true;
            if (GetType() != obj.GetType()) return false;
            return Equals((TilePosition)obj);
        }
        public override int GetHashCode()
        {
            return id != null ? id.GetHashCode() : 0;
        }
    }
}
