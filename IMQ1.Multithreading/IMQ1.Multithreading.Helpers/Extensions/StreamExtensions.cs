using System.IO;
using System.IO.Pipes;
using System.Text;

namespace IMQ1.Multithreading.Helpers.Extensions
{
    public static class StreamExtensions
    {
        private static readonly UnicodeEncoding StreamEncoding = new UnicodeEncoding();

        public static string ReadString(this PipeStream stream)
        {
            var buffer = new byte[1024];
            using (var memoryStream = new MemoryStream())
            {
                do
                {
                    var readBytes = stream.Read(buffer, 0, buffer.Length);
                    memoryStream.Write(buffer, 0, readBytes);
                }
                while (!stream.IsMessageComplete);

                return StreamEncoding.GetString(memoryStream.ToArray());
            }
        }

        public static int WriteString(this PipeStream stream, string outString)
        {
            var outBuffer = StreamEncoding.GetBytes(outString);
            var len = outBuffer.Length;

            stream.Write(outBuffer, 0, len);
            stream.Flush();

            return outBuffer.Length;
        }
    }
}
