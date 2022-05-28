using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TileTransforms
{
    public class TilePosition : IEquatable<TilePosition>
    {
        public Vector3Int value { get; }
        private Tilemap tilemap { get; }

        public TilePosition(Vector3 value, Tilemap tilemap)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (tilemap == null) throw new ArgumentNullException(nameof(tilemap));

            this.tilemap = tilemap;
            this.value = this.tilemap.WorldToCell(value);

            if (this.tilemap.GetTile(this.value) == null) throw new Exception("タイルから外れています");
        }
        public TilePosition GetAroundTile(TileDirection tileDirection)
        {
            Vector3 value = this.value;
            value += (Vector3)tileDirection.GetVector2();
            if (tilemap.GetTile(new Vector3Int((int)value.x, (int)value.y, 0)) == null) return null;
            else return new TilePosition(value, tilemap);
        }
        public Vector2 GetWorldPosition()
        {
            return tilemap.CellToWorld(value) + tilemap.cellSize / 2;
        }

        public bool Equals(TilePosition other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(other, this)) return true;
            return Equals(value, other.value);
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
            return value.GetHashCode();
        }
    }
}
