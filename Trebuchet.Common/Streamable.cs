using System.IO;

namespace Trebuchet.Common
{
    public abstract class Streamable
    {
        public abstract void Read(Stream stream);
        public abstract void Write(Stream stream);
    }
}