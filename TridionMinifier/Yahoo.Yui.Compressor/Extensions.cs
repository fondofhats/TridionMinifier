using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Yahoo.Yui.Compressor
{
	public static class Extensions
	{
		// Copyright (c) 2008 - 2011, Pure Krome
		// All rights reserved.
		// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
		// * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
		// * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
		// * Neither the name of World Domination Technologies nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
		// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

		public static int AppendReplacement(this Capture capture, StringBuilder value, string input, string replacement, int index)
		{
			string preceding = input.Substring(index, capture.Index - index);

			value.Append(preceding);
			value.Append(replacement);

			return capture.Index + capture.Length;
		}

		public static void AppendTail(this StringBuilder value, string input, int index)
		{
			value.Append(input.Substring(index));
		}

		public static string RegexReplace(this string input, string pattern, string replacement)
		{
			return Regex.Replace(input, pattern, replacement);
		}

		public static string RegexReplace(this string input, string pattern, string replacement, RegexOptions options)
		{
			return Regex.Replace(input, pattern, replacement, options);
		}

		public static string Fill(this string format, params object[] args)
		{
			return String.Format(CultureInfo.InvariantCulture, format, args);
		}

		public static string RemoveRange(this string input, int startIndex, int endIndex)
		{
			return input.Remove(startIndex, endIndex - startIndex);
		}

		public static bool EqualsIgnoreCase(this string left, string right)
		{
			return String.Compare(left, right, StringComparison.OrdinalIgnoreCase) == 0;
		}

		// NOTE: To check out some decimal -> Hex converstions, goto http://www.openstrike.co.uk/cgi-bin/decimalhex.cgi
		public static string ToHexString(this int value)
		{
			return value.ToString("X");
		}

		public static string ToPluralString(this int value)
		{
			return value == 1 ? string.Empty : "s";
		}

		public static bool IsNullOrEmpty<T>(this IEnumerable<T> value)
		{
			return value == null || value.Count() <= 0 ? true : false;
		}

		public static IList<T> ToListIfNotNullOrEmpty<T>(this IList<T> value)
		{
			return value.IsNullOrEmpty() ? null : value;
		}

		public static string Replace(this string value, int startIndex, int endIndex, string newContent)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}

			// Chop the string into two parts, the before and then the after.
			string before = value.Substring(0, startIndex);
			string after = value.Substring(endIndex);
			return before + newContent + after;
		}
	}
}