using System.Collections.Generic;

namespace Ryocatusn.TileTransforms
{
    public class TileTransformManager
    {
        public static TileTransformManager Instance = new TileTransformManager();
        private TileTransformManager() { }

        private List<TileTransform> tileTransforms = new List<TileTransform>();

        public void Save(TileTransform tileTransform)
        {
            tileTransforms.Add(tileTransform);
        }
        public TileTransform[] FindByPosition(TilePosition tilePosition)
        {
            //Œã‚Å•ÏX
            return tileTransforms.FindAll(x => x.tilePosition.Equals(tilePosition)).ToArray();
        }
        public void Delete(TileTransform tileTransform)
        {
            tileTransforms.Remove(tileTransform);
        }
    }
}
