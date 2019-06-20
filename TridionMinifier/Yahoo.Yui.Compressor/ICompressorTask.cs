namespace Yahoo.Yui.Compressor
{
    public interface ICompressorTask
    {
		// Copyright (c) 2008 - 2011, Pure Krome
		// All rights reserved.
		// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
		// * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
		// * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
		// * Neither the name of World Domination Technologies nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
		// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

        string CssCompressionType { get; set; }
        string DeleteCssFiles { get; set; }
        string CssOutputFile { get; set; }
        string ObfuscateJavaScript { get; set; }
        string PreserveAllSemicolons { get; set; }
        string DisableOptimizations { get; set; }
        string LineBreakPosition { get; set; }
        string EncodingType { get; set; }
        string DeleteJavaScriptFiles { get; set; }
        string JavaScriptOutputFile { get; set; }
        string LoggingType { get; set; }
        string ThreadCulture { get; set; }
        string IsEvalIgnored { get; set; }
    }
}