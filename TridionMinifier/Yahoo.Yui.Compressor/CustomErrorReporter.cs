using System;
using System.Collections.Specialized;
using EcmaScript.NET;

namespace Yahoo.Yui.Compressor
{
    public class CustomErrorReporter : ErrorReporter
    {
		// Copyright (c) 2008 - 2011, Pure Krome
		// All rights reserved.
		// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
		// * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
		// * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
		// * Neither the name of World Domination Technologies nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
		// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

        private readonly bool _isVerboseLogging;
        public StringCollection ErrorMessages { get; private set; }

        public CustomErrorReporter(bool isVerboseLogging)
        {
            _isVerboseLogging = isVerboseLogging;
            ErrorMessages = new StringCollection();
        }

        public virtual void Warning(string message, 
            string sourceName, 
            int line, 
            string lineSource, 
            int lineOffset)
        {
            if (_isVerboseLogging)
            {
                string text = "[WARNING] " + message;
                Console.WriteLine(text);
                //ErrorMessages.Add(
                //    string.Format("[WARNING] {0} ******** Source: {1}. Line: {2}. LineSource: {3}. LineOffset: {4}",
                //                  message,
                //                  sourceName,
                //                  line,
                //                  lineSource,
                //                  lineOffset));
                ErrorMessages.Add(text);
            }
        }

        public virtual void Error (string message, 
            string sourceName, 
            int line,
            string lineSource, 
            int lineOffset)
        {
            //throw new InvalidOperationException(
            //    string.Format("[ERROR] {0} ******** Source: {1}. Line: {2}. LineSource: {3}. LineOffset: {4}",
            //                  message,
            //                  sourceName,
            //                  line,
            //                  lineSource,
            //                  lineOffset));
            throw new InvalidOperationException("[ERROR] " + message);
        }

        public virtual EcmaScriptRuntimeException RuntimeError(string message,
            string sourceName, 
            int line,
            string lineSource, 
            int lineOffset)
        {
            //throw new InvalidOperationException(
            //    string.Format("[ERROR] {0} ******** Source: {1}. Line: {2}. LineSource: {3}. LineOffset: {4}",
            //                  message,
            //                  sourceName,
            //                  line,
            //                  lineSource,
            //                  lineOffset));
            throw new InvalidOperationException("[ERROR] EcmaScriptRuntimeException :: " + message);
        }
    }
}