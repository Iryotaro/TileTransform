using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileTransforms
{
    public class TilePosition : IEquatable<TilePosition>
    {
        public Vector3Int position { get; }
        private Tilemap tilemap { get; }

        public TilePosition(Vector3 worldPosition, Tilemap tilemap)
        {
            if (worldPosition == null) throw new ArgumentNullException(nameof(worldPosition));
            if (tilemap == null) throw new ArgumentNullException(nameof(tilemap));

            this.tilemap = tilemap;
            position = this.tilemap.WorldToCell(worldPosition);

            if (this.tilemap.GetTile(position) == null) throw new Exception("タイルから外れています");
        }
        public TilePosition GetAroundTile(TileDirection tileDirection)
        {
            Vector3 nowPosition = tilemap.CellToWorld(position);
            Vector3 direction = tileDirection.GetVector2();
            Vector3 distanceEachTile = Vector3.Scale(tilemap.cellSize + tilemap.cellGap, tilemap.transform.lossyScale);

            Vector3 aroundTilePosition = nowPosition + Vector3.Scale(direction, distanceEachTile);

            if (tilemap.GetTile(tilemap.WorldToCell(aroundTilePosition)) == null) return null;
            else return new TilePosition(aroundTilePosition, tilemap);
        }
        public Vector2 GetWorldPosition()
        {
            return tilemap.CellToWorld(position) + Vector3.Scale(tilemap.cellSize, tilemap.transform.lossyScale) / 2;
        }

        public bool Equals(TilePosition other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Equals(position, other.position);
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
            return position.GetHashCode();
        }
    }
}
