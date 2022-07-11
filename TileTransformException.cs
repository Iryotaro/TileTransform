using System;

namespace Ryocatusn.TileTransforms
{
    public class TileTransformException : Exception
    {
        public TileTransformException(string message) : base($"TileTransformException : {message}") { }
    }
}
