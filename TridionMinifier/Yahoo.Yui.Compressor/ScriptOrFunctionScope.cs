using System;
using System.Collections;
using System.Collections.Generic;

namespace Yahoo.Yui.Compressor
{
    public class ScriptOrFunctionScope
    {
		// Copyright (c) 2008 - 2011, Pure Krome
		// All rights reserved.
		// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
		// * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
		// * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
		// * Neither the name of World Domination Technologies nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
		// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

        #region Fields

        public int BraceNesting { get; private set; }
        public ScriptOrFunctionScope ParentScope { get; private set; }
        private ArrayList SubScopes { get; set; }
        private readonly IDictionary<string, JavaScriptIdentifier> _identifiers = new SortedDictionary<string, JavaScriptIdentifier>();
        private readonly IDictionary<string,string> _hints = new SortedDictionary<string,string>();
        private bool _markedForMunging = true;
        public int VarCount { get; set; }

        #endregion
        
        #region Constructors

        public ScriptOrFunctionScope(int braceNesting, 
            ScriptOrFunctionScope parentScope)
        {
            this.BraceNesting = braceNesting;
            this.ParentScope = parentScope;
            this.SubScopes = new ArrayList();
            if (parentScope != null)
            {
                parentScope.SubScopes.Add(this);
            }
        }

        #endregion Constructors

        #region Methods

        #region Private Methods

        private ArrayList GetUsedSymbols()
        {
            ArrayList result = new ArrayList();
            foreach (JavaScriptIdentifier identifier in this._identifiers.Values)
            {
                string mungedValue = identifier.MungedValue;
                if (string.IsNullOrEmpty(mungedValue))
                {
                    mungedValue = identifier.Value;
                }

                result.Add(mungedValue);
            }

            return result;
        }

        private ArrayList GetAllUsedSymbols()
        {
            ArrayList result = new ArrayList();
            ScriptOrFunctionScope scope = this;
            while (scope != null)
            {
                result.AddRange(scope.GetUsedSymbols());
                scope = scope.ParentScope;
            }

            return result;
        }

        #endregion

        #region Public Methods

        public JavaScriptIdentifier DeclareIdentifier(string symbol)
        {
            JavaScriptIdentifier identifier = _identifiers.ContainsKey(symbol) ? _identifiers[symbol] : null;
            //JavaScriptIdentifier identifier = (JavaScriptIdentifier)this._identifiers[symbol];

            if (identifier == null)
            {
                identifier = new JavaScriptIdentifier(symbol, this);
                this._identifiers.Add(symbol, identifier);
            }

            return identifier;
        }

        public void Munge()
        {
            if (!this._markedForMunging)
            {
                // Stop right here if this scope was flagged as unsafe for munging.
                return;
            }

            int pickFromSet = 1;

            // Do not munge symbols in the global scope!
            if (this.ParentScope != null)
            {
                ArrayList freeSymbols = new ArrayList();

                freeSymbols.AddRange(JavaScriptCompressor.Ones);
                foreach (string symbol in this.GetAllUsedSymbols())
                {
                    freeSymbols.Remove(symbol);
                }

                if (freeSymbols.Count == 0)
                {
                    pickFromSet = 2;
                    freeSymbols.AddRange(JavaScriptCompressor.Twos);
                    foreach (string symbol in this.GetAllUsedSymbols())
                    {
                        freeSymbols.Remove(symbol);
                    }
                }

                if (freeSymbols.Count == 0)
                {
                    pickFromSet = 3;
                    freeSymbols.AddRange(JavaScriptCompressor.Threes);
                    foreach (string symbol in this.GetAllUsedSymbols())
                    {
                        freeSymbols.Remove(symbol);
                    }
                }

                if (freeSymbols.Count == 0)
                {
                    throw new InvalidOperationException("The YUI Compressor ran out of symbols. Aborting...");
                }

                foreach (JavaScriptIdentifier identifier in this._identifiers.Values)
                {
                    if (freeSymbols.Count == 0)
                    {
                        pickFromSet++;
                        if (pickFromSet == 2)
                        {
                            freeSymbols.AddRange(JavaScriptCompressor.Twos);
                        }
                        else if (pickFromSet == 3)
                        {
                            freeSymbols.AddRange(JavaScriptCompressor.Threes);
                        }
                        else
                        {
                            throw new InvalidOperationException("The YUI Compressor ran out of symbols. Aborting...");
                        }
                        // It is essential to remove the symbols already used in
                        // the containing scopes, or some of the variables declared
                        // in the containing scopes will be redeclared, which can
                        // lead to errors.
                        foreach (string symbol in this.GetAllUsedSymbols())
                        {
                            freeSymbols.Remove(symbol);
                        }
                    }

                    string mungedValue;
                    if (identifier.MarkedForMunging)
                    {
                        mungedValue = (string)freeSymbols[0];
                        freeSymbols.RemoveAt(0);
                    }
                    else
                    {
                        mungedValue = identifier.Value;
                    }

                    identifier.MungedValue = mungedValue;
                }
            }

            for (int i = 0; i < this.SubScopes.Count; i++)
            {
                ScriptOrFunctionScope scope = (ScriptOrFunctionScope)this.SubScopes[i];
                scope.Munge();
            }
        }

        public void PreventMunging()
        {
            if (this.ParentScope != null)
            {
                // The symbols in the global scope don't get munged,
                // but the sub-scopes it contains do get munged.
                this._markedForMunging = false;
            }
        }

        public JavaScriptIdentifier GetIdentifier(string symbol)
        {
            return _identifiers.ContainsKey(symbol) ? _identifiers[symbol] : null;
            //return (JavaScriptIdentifier)this._identifiers[symbol];
        }

        public void AddHint(string variableName,
            string variableType)
        {
            this._hints.Add(variableName, variableType);
        }

        #endregion

        #endregion
    }
}
