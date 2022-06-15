using System;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Ryocatusn.TileTransforms
{
    public class TilePosition : IEquatable<TilePosition>
    {
        public (Vector3Int value, Tilemap tilemap) position { get; }
        private Tilemap[] tilemaps { get; }

        public TilePosition(Vector3 worldPosition, Tilemap[] tilemaps)
        {
            if (worldPosition == null) throw new ArgumentNullException(nameof(worldPosition));
            if (tilemaps == null) throw new ArgumentNullException(nameof(tilemaps));

            this.tilemaps = tilemaps;

            foreach (Tilemap tilemap in tilemaps)
            {
                position = (tilemap.WorldToCell(worldPosition), tilemap);
                if (tilemap.GetTile(position.value) != null) return;
            }

            throw new Exception("タイルから外れています");
        }
        public TilePosition(Vector3Int value, Tilemap[] tilemaps)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if (tilemaps == null) throw new ArgumentNullException(nameof(tilemaps));

            this.tilemaps = tilemaps;

            foreach (Tilemap tilemap in tilemaps)
            {
                position = (value, tilemap);
                if (tilemap.GetTile(position.value) != null) return;
            }

            throw new Exception("タイルから外れています");
        }

        public TilePosition GetAroundTile(TileDirection tileDirection)
        {
            foreach (Tilemap tilemap in tilemaps)
            {
                Vector3 nowPosition = tilemap.CellToWorld(tilemap.WorldToCell(GetWorldPosition()));
                Vector3 direction = tileDirection.GetVector2();
                Vector3 distanceEachTile = Vector3.Scale(tilemap.cellSize + tilemap.cellGap, tilemap.transform.lossyScale);

                Vector3 aroundTilePosition = nowPosition + Vector3.Scale(direction, distanceEachTile);

                if (tilemap.GetTile(tilemap.WorldToCell(aroundTilePosition)) != null) return new TilePosition(aroundTilePosition, tilemaps);
            }

            return null;
        }
        public Vector2 GetWorldPosition()
        {
            Tilemap tilemap = position.tilemap;
            return tilemap.CellToWorld(position.value) + Vector3.Scale(tilemap.cellSize, tilemap.transform.lossyScale) / 2;
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
