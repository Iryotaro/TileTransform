using System.Collections.Generic;

namespace Ryocatusn.TileTransforms
{
    public class TileTransformManager
    {
        public static TileTransformManager Instance = new TileTransformManager();
        private TileTransformManager() { }


        private List<TileTransform> Ryocatusn.TileTransforms = new List<TileTransform>();

        public void Save(TileTransform tileTransform)
        {
            Ryocatusn.TileTransforms.Add(tileTransform);
        }
        public TileTransform[] FindByPosition(TilePosition tilePosition)
        {
            return Ryocatusn.TileTransforms.FindAll(x => x.tilePosition.Value.Equals(tilePosition)).ToArray();
        }
        public void Delete(TileTransform tileTransform)
        {
            Ryocatusn.TileTransforms.Remove(tileTransform);
        }
    }
}
