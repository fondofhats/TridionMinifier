﻿using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace Yahoo.Yui.Compressor
{
	public static class YUICompressor
	{
		// Copyright (c) 2008 - 2011, Pure Krome
		// All rights reserved.
		// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
		// * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
		// * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
		// * Neither the name of World Domination Technologies nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
		// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

		public static string Compress(string css, int columnWidth)
		{
			if (string.IsNullOrEmpty(css))
			{
				throw new ArgumentNullException("css");
			}

			int totalLen = css.Length;
			int startIndex = 0;
			ArrayList comments = new ArrayList();
			ArrayList preservedTokens = new ArrayList();
			int max;

			while ((startIndex = css.IndexOf(@"/*", startIndex, StringComparison.OrdinalIgnoreCase)) >= 0)
			{
				var endIndex = css.IndexOf(@"*/", startIndex + 2, StringComparison.OrdinalIgnoreCase);
				if (endIndex < 0)
				{
					endIndex = totalLen;
				}

				// Note: java substring-length param = end index - 2 (which is end index - (startindex + 2))
				var token = css.Substring(startIndex + 2, endIndex - (startIndex + 2));

				comments.Add(token);

				var newResult = css.Replace(startIndex + 2, endIndex, "___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_" + (comments.Count - 1) + "___");

				//var newResult = css.Substring(startIndex + 2, endIndex - (startIndex + 2)) + "___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_" +
				//                (comments.Count - 1) + "___" + css.Substring(endIndex + 1);

				startIndex += 2;
				css = newResult;
			}

			// Preserve strings so their content doesn't get accidently minified
			StringBuilder stringBuilder = new StringBuilder();
			Regex pattern = new Regex("(\"([^\\\\\"]|\\\\.|\\\\)*\")|(\'([^\\\\\']|\\\\.|\\\\)*\')");
			Match match = pattern.Match(css);
			int index = 0;
			while (match.Success)
			{
				string text = match.Groups[0].Value;
				if (!string.IsNullOrEmpty(text))
				{
					var token = match.Value;
					char quote = token[0];

					// Java code: token.substring(1, token.length() -1) .. but that's ...
					//            token.substring(start index, end index) .. while .NET it's length for the 2nd arg.
					token = token.Substring(1, token.Length - 2);

					// Maybe the string contains a comment-like substring?
					// one, maybe more? put'em back then.
					if (token.IndexOf("___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_") >= 0)
					{
						max = comments.Count;
						for (int i = 0; i < max; i += 1)
						{
							token = token.Replace("___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_" + i + "___", comments[i].ToString());
						}
					}

					// Minify alpha opacity in filter strings.
					token = token.RegexReplace("(?i)progid:DXImageTransform.Microsoft.Alpha\\(Opacity=", "alpha(opacity=");

					preservedTokens.Add(token);
					var preserver = quote + "___YUICSSMIN_PRESERVED_TOKEN_" + (preservedTokens.Count - 1) + "___" + quote;

					index = match.AppendReplacement(stringBuilder, css, preserver, index);
					match = match.NextMatch();
				}
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();

			// Strings are safe, now wrestle the comments.
			max = comments.Count;
			for (int i = 0; i < max; i += 1)
			{
				string token = comments[i].ToString();
				var placeholder = "___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_" + i + "___";

				// ! in the first position of the comment means preserve
				// so push to the preserved tokens while stripping the !
				if (token.StartsWith("!"))
				{
					preservedTokens.Add(token);
					css = css.Replace(placeholder, "___YUICSSMIN_PRESERVED_TOKEN_" + (preservedTokens.Count - 1) + "___");
					continue;
				}

				// \ in the last position looks like hack for Mac/IE5
				// shorten that to /*\*/ and the next one to /**/
				if (token.EndsWith("\\"))
				{
					preservedTokens.Add("\\");
					css = css.Replace(placeholder, "___YUICSSMIN_PRESERVED_TOKEN_" + (preservedTokens.Count - 1) + "___");
					i = i + 1; // attn: advancing the loop.
					preservedTokens.Add(string.Empty);
					css = css.Replace("___YUICSSMIN_PRESERVE_CANDIDATE_COMMENT_" + i + "___", "___YUICSSMIN_PRESERVED_TOKEN_" + (preservedTokens.Count - 1) + "___");
					continue;
				}

				// keep empty comments after child selectors (IE7 hack)
				// e.g. html >/**/ body
				if (token.Length == 0)
				{
					startIndex = css.IndexOf(placeholder);
					if (startIndex > 2)
					{
						if (css[startIndex - 3] == '>')
						{
							preservedTokens.Add(string.Empty);
							css = css.Replace(placeholder, "___YUICSSMIN_PRESERVED_TOKEN_" + (preservedTokens.Count - 1) + "___");
						}
					}
				}

				// In all other cases kill the comment.
				css = css.Replace("/*" + placeholder + "*/", string.Empty);
			}

			// Normalize all whitespace strings to single spaces. Easier to work with that way.
			css = css.RegexReplace("\\s+", " ");

			// Remove the spaces before the things that should not have spaces before them.
			// But, be careful not to turn "p :link {...}" into "p:link{...}"
			// Swap out any pseudo-class colons with the token, and then swap back.
			stringBuilder = new StringBuilder();
			pattern = new Regex("(^|\\})(([^\\{:])+:)+([^\\{]*\\{)");
			match = pattern.Match(css);
			index = 0;
			while (match.Success)
			{
				string text = match.Value;
				text = text.Replace(":", "___YUICSSMIN_PSEUDOCLASSCOLON___");
				text = text.Replace("\\\\", "\\\\\\\\");
				text = text.Replace("\\$", "\\\\\\$");
				index = match.AppendReplacement(stringBuilder, css, text, index);
				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();

			// Remove spaces before the things that should not have spaces before them.
			css = css.RegexReplace("\\s+([!{};:>+\\(\\)\\],])", "$1");

			// Bring back the colon.
			css = css.RegexReplace("___YUICSSMIN_PSEUDOCLASSCOLON___", ":");

			// Retain space for special IE6 cases.
			css = css.RegexReplace(":first\\-(line|letter)(\\{|,)", ":first-$1 $2");

			// no space after the end of a preserved comment.
			css = css.RegexReplace("\\*/ ", "*/");

			// If there is a @charset, then only allow one, and push to the top of the file.
			css = css.RegexReplace("^(.*)(@charset \"[^\"]*\";)", "$2$1");
			css = css.RegexReplace("^(\\s*@charset [^;]+;\\s*)+", "$1");

			// Put the space back in some cases, to support stuff like
			// @media screen and (-webkit-min-device-pixel-ratio:0){
			css = css.RegexReplace("\\band\\(", "and (");

			// Remove the spaces after the things that should not have spaces after them.
			css = css.RegexReplace("([!{}:;>+\\(\\[,])\\s+", "$1");

			// remove unnecessary semicolons.
			css = css.RegexReplace(";+}", "}");

			// Replace 0(px,em,%) with 0.
			css = css.RegexReplace("([\\s:])(0)(px|em|%|in|cm|mm|pc|pt|ex)", "$1$2");

			// Replace 0 0 0 0; with 0.
			css = css.RegexReplace(":0 0 0 0(;|})", ":0$1");
			css = css.RegexReplace(":0 0 0(;|})", ":0$1");
			css = css.RegexReplace(":0 0(;|})", ":0$1");

			// Replace background-position:0; with background-position:0 0;
			// same for transform-origin
			stringBuilder = new StringBuilder();
			pattern = new Regex("(?i)(background-position|transform-origin|webkit-transform-origin|moz-transform-origin|o-transform-origin|ms-transform-origin):0(;|})");
			match = pattern.Match(css);
			index = 0;
			while (match.Success)
			{
				index = match.AppendReplacement(stringBuilder, css, match.Groups[1].Value.ToLowerInvariant() + ":0 0" + match.Groups[2], index);
				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();

			// Replace 0.6 to .6, but only when preceded by : or a white-space.
			css = css.RegexReplace("(:|\\s)0+\\.(\\d+)", "$1.$2");

			// Shorten colors from rgb(51,102,153) to #336699
			// This makes it more likely that it'll get further compressed in the next step.
			stringBuilder = new StringBuilder();
			pattern = new Regex("rgb\\s*\\(\\s*([0-9,\\s]+)\\s*\\)");
			match = pattern.Match(css);
			index = 0;
			int value;
			while (match.Success)
			{
				string[] rgbcolors = match.Groups[1].Value.Split(',');
				StringBuilder hexcolor = new StringBuilder("#");
				foreach (string rgbColour in rgbcolors)
				{
					if (!Int32.TryParse(rgbColour, out value))
					{
						value = 0;
					}

					if (value < 16)
					{
						hexcolor.Append("0");
					}
					hexcolor.Append(value.ToHexString().ToLowerInvariant());
				}

				index = match.AppendReplacement(stringBuilder, css, hexcolor.ToString(), index);
				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();


			// Shorten colors from #AABBCC to #ABC. Note that we want to make sure
			// the color is not preceded by either ", " or =. Indeed, the property
			//     filter: chroma(color="#FFFFFF");
			// would become
			//     filter: chroma(color="#FFF");
			// which makes the filter break in IE.
			stringBuilder = new StringBuilder();
			pattern = new Regex("([^\"'=\\s])(\\s*)#([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])([0-9a-fA-F])");
			match = pattern.Match(css);
			index = 0;
			while (match.Success)
			{
				// Test for AABBCC pattern.
				if (match.Groups[3].Value.EqualsIgnoreCase(match.Groups[4].Value) &&
					match.Groups[5].Value.EqualsIgnoreCase(match.Groups[6].Value) &&
					match.Groups[7].Value.EqualsIgnoreCase(match.Groups[8].Value))
				{
					var replacement = String.Concat(match.Groups[1].Value, match.Groups[2].Value, "#", match.Groups[3].Value, match.Groups[5].Value, match.Groups[7].Value);
					index = match.AppendReplacement(stringBuilder, css, replacement, index);
				}
				else
				{
					index = match.AppendReplacement(stringBuilder, css, match.Value, index);
				}

				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();

			// border: none -> border:0
			stringBuilder = new StringBuilder();
			pattern = new Regex("(?i)(border|border-top|border-right|border-bottom|border-right|outline|background):none(;|})");
			match = pattern.Match(css);
			index = 0;
			while (match.Success)
			{
				var replacement = match.Groups[1].Value.ToLowerInvariant() + ":0" + match.Groups[2].Value;
				index = match.AppendReplacement(stringBuilder, css, replacement, index);
				match = match.NextMatch();
			}
			stringBuilder.AppendTail(css, index);
			css = stringBuilder.ToString();

			// Shorter opacity IE filter.
			css = css.RegexReplace("(?i)progid:DXImageTransform.Microsoft.Alpha\\(Opacity=", "alpha(opacity=");

			// Remove empty rules.
			css = css.RegexReplace("[^\\}\\{/;]+\\{\\}", string.Empty);

			if (columnWidth >= 0)
			{
				// Some source control tools don't like it when files containing lines longer
				// than, say 8000 characters, are checked in. The linebreak option is used in
				// that case to split long lines after a specific column.
				int i = 0;
				int linestartpos = 0;
				stringBuilder = new StringBuilder(css);
				while (i < stringBuilder.Length)
				{
					char c = stringBuilder[i++];
					if (c == '}' && i - linestartpos > columnWidth)
					{
						stringBuilder.Insert(i, '\n');
						linestartpos = i;
					}
				}

				css = stringBuilder.ToString();
			}

			// Replace multiple semi-colons in a row by a single one.
			// See SF bug #1980989.
			css = css.RegexReplace(";;+", ";");

			// Restore preserved comments and strings.
			max = preservedTokens.Count;
			for (int i = 0; i < max; i++)
			{
				css = css.Replace("___YUICSSMIN_PRESERVED_TOKEN_" + i + "___", preservedTokens[i].ToString());
			}

			// Trim the final string (for any leading or trailing white spaces).
			css = css.Trim();

			// Write the output...
			return css;
		}
	}
}