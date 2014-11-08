//
// This file is part of the Kerbal Space Program Community API.
// 
// This code is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This code is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this code.  If not, see <http://www.gnu.org/licenses/>. 
//

namespace KerbalSpaceProgram.Api
{
    internal static class StringExtensions
    {
        [PublicAPI, StringFormatMethod("format")]
        public static string With([NotNull] this string format, object arg1)
        {
            return string.Format(format, arg1);
        }

        [PublicAPI, StringFormatMethod("format")]
        public static string With([NotNull] this string format, object arg1, object arg2)
        {
            return string.Format(format, arg1, arg2);
        }

        [PublicAPI, StringFormatMethod("format")]
        public static string With([NotNull] this string format, object arg1, object arg2, object arg3)
        {
            return string.Format(format, arg1, arg2, arg3);
        }

        [PublicAPI, StringFormatMethod("format")]
        public static string With([NotNull] this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
