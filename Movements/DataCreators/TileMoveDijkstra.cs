using System;
using System.Collections.Generic;

namespace TileTransforms.Movements
{
    public class TileMoveDijkstra : IMoveDataCreater
    {
        private MoveData moveData;

        private List<Tile> searchTiles = new List<Tile>();
        private List<Tile> searchedTiles = new List<Tile>();
        private bool success;

        public TileMoveDijkstra(TilePosition currentPosition, TilePosition destinationPosition)
        {
            CreateData(currentPosition, destinationPosition);
        }
        private void CreateData(TilePosition currentPosition, TilePosition destinationPosition)
        {
            Tile destinationTile = GetDestinationTile();

            if (destinationTile == null)
            {
                success = false;
            }
            else
            {
                success = true;

                List<TilePosition> data = new List<TilePosition>();
                Tile prevTile = destinationTile;
                while (true)
                {
                    data.Add(prevTile.position);

                    prevTile = prevTile.prevTile;
                    if (prevTile == null) break;
                }

                data.Reverse();
                moveData = new MoveData(data);
            }

            Tile GetDestinationTile()
            {
                Tile currentTile = new Tile(currentPosition, null);
                Tile destinationTile = new Tile(destinationPosition, null);
                searchTiles.Add(currentTile);

                while (true)
                {
                    List<Tile> newSearchTiles = new List<Tile>();
                    List<Tile> newSearchedTiles = new List<Tile>();
                    foreach (Tile searchTile in searchTiles)
                    {
                        if (searchTile.Equals(destinationTile)) return searchTile;

                        List<Tile> result = searchTile.Search(newSearchTiles, searchedTiles);

                        newSearchTiles.AddRange(result);
                        newSearchedTiles.Add(searchTile);
                    }
                    searchTiles = newSearchTiles;
                    searchedTiles.AddRange(newSearchedTiles);

                    if (searchTiles.Count == 0) break;
                }

                return null;
            }
        }
        public MoveData GetData()
        {
            return moveData;
        }
        public bool IsSuccess()
        {
            return success;
        }

        public class Tile : IEquatable<Tile>
        {
            public TilePosition position { get; }
            public Tile prevTile { get; }
            
            public Tile(TilePosition position, Tile prevTile)
            {
                this.position = position;
                this.prevTile = prevTile;
            }
            public List<Tile> Search(List<Tile> searchTiles, List<Tile> searchedTiles)
            {
                List<Tile> tiles = new List<Tile>();

                foreach (TileDirection.Direction direction in Enum.GetValues(typeof(TileDirection.Direction)))
                {
                    TilePosition position = this.position.GetAroundTile(new TileDirection(direction));
                    if (position == null) continue;

                    Tile tile = new Tile(position, this);

                    if (searchedTiles.Contains(tile)) continue;
                    if (searchTiles.Contains(tile)) continue;

                    tiles.Add(tile);
                }

                return tiles;
            }

            public bool Equals(Tile other)
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
                return Equals((Tile)obj);
            }
            public override int GetHashCode()
            {
                return position != null ? position.GetHashCode() : 0;
            }
        }
    }
}
