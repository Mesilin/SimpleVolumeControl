// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or https://mit-license.org/

using System.Collections.Generic;

namespace Gma.System.MouseKeyHook.HotKeys
{
    /// <summary>
    ///     A collection of HotKeySets
    /// </summary>
    public sealed class HotKeySetCollection : List<HotKeySet>
    {
        private KeyChainHandler _mKeyChain;

        /// <summary>
        ///     Adds a HotKeySet to the collection.
        /// </summary>
        /// <param name="hks"></param>
        public new void Add(HotKeySet hks)
        {
            _mKeyChain += hks.OnKey;
            base.Add(hks);
        }

        /// <summary>
        ///     Removes the HotKeySet from the collection.
        /// </summary>
        /// <param name="hks"></param>
        public new void Remove(HotKeySet hks)
        {
            _mKeyChain -= hks.OnKey;
            base.Remove(hks);
        }

        /// <summary>
        ///     Uses a multi-case delegate to invoke individual HotKeySets if the Key is in use by any HotKeySets.
        /// </summary>
        /// <param name="e"></param>
        internal void OnKey(KeyEventArgsExt e)
        {
            if (_mKeyChain != null)
                _mKeyChain(e);
        }

        private delegate void KeyChainHandler(KeyEventArgsExt kex);
    }
}