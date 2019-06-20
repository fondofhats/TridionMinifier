using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yahoo.Yui.Compressor
{
	public static class CssCompressor
	{
		// Copyright (c) 2008 - 2011, Pure Krome
		// All rights reserved.
		// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
		// * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
		// * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
		// * Neither the name of World Domination Technologies nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
		// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

		public static string Compress(string css)
		{
			return Compress(css, 0, CssCompressionType.StockYuiCompressor);
		}

		public static string Compress(string css, int columnWidth, CssCompressionType cssCompressionType)
		{
			string compressedCss = null;

			switch (cssCompressionType)
			{
				case CssCompressionType.StockYuiCompressor:
					compressedCss = YUICompressor.Compress(css, columnWidth);
					break;
				case CssCompressionType.MichaelAshRegexEnhancements:
					compressedCss = MichaelAshRegexCompressor.Compress(css, columnWidth);
					break;
				case CssCompressionType.Hybrid:
					string yuiCompressedCss = YUICompressor.Compress(css, columnWidth);
					string michaelAshsRegexEnhancementsCompressedCss = MichaelAshRegexCompressor.Compress(css, columnWidth);
					compressedCss = yuiCompressedCss.Length < michaelAshsRegexEnhancementsCompressedCss.Length ? yuiCompressedCss : michaelAshsRegexEnhancementsCompressedCss;
					break;
				default:
					throw new InvalidOperationException("Unhandled CssCompressionType found when trying to determine which compression method to use.");
			}

			return compressedCss;
		}
	}
}