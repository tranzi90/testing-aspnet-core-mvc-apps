using System.IO;

namespace AtmSimulator.UnitTests.Extensions
{
    public static class StreamExtensions
    {
        public static string Read(this Stream stream)
        {
            stream.Position = 0;

            using StreamReader reader = new StreamReader(stream);

            var text = reader.ReadToEnd();

            return text;
        }
    }
}
