using System.Globalization;
using System.Linq;
using System.Text;

namespace Mentoz.AspNet.Api
{
    public static class ByteExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {
            var builder = new StringBuilder();
            bytes.ToList().ForEach(x => builder.Append(x.ToString("X2", CultureInfo.InvariantCulture)));
            return builder.ToString();
        }
    }
}