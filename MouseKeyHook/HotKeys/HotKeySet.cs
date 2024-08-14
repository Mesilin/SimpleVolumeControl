// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or https://mit-license.org/

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Gma.System.MouseKeyHook.Implementation;

namespace Gma.System.MouseKeyHook.HotKeys
{
    /// <summary>
    ///     An immutable set of Hot Keys that provides an event for when the set is activated.
    /// </summary>
    public class HotKeySet
    {
        /// <summary>
        ///     A delegate representing the signature for the OnHotKeysDownHold event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void HotKeyHandler(object sender, HotKeyArgs e);

        private readonly Dictionary<Keys, bool> _mHotkeystate; //Keeps track of the status of the set of Keys

        /*
         * Example of m_remapping:
         * a single key from the set of Keys requested is chosen to be the reference key (aka primary key)
         * 
         * m_remapping[ Keys.LShiftKey ] = Keys.LShiftKey
         * m_remapping[ Keys.RShiftKey ] = Keys.LShiftKey
         * 
         * This allows the m_hotkeystate to use a single key (primary key) from the set that will act on behalf of all the keys in the set, 
         * which in turn reduces to this:
         * 
         * Keys k = Keys.RShiftKey
         * Keys primaryKey = PrimaryKeyOf( k ) = Keys.LShiftKey
         * m_hotkeystate[ primaryKey ] = true/false
         */
        private readonly Dictionary<Keys, Keys> _mRemapping; //Used for mapping multiple keys to a single key

        private bool _mEnabled = true; //enabled by default

        //These provide the actual status of whether a set is truly activated or not.
        private int _mHotkeydowncount; //number of hot keys down

        private int _mRemappingCount;
        //the number of remappings, i.e., a set of mappings, not the individual count in m_remapping

        /// <summary>
        ///     Creates an instance of the HotKeySet class.  Once created, the keys cannot be changed.
        /// </summary>
        /// <param name="hotkeys">Set of Hot Keys</param>
        public HotKeySet(IEnumerable<Keys> hotkeys)
        {
            _mHotkeystate = new Dictionary<Keys, bool>();
            _mRemapping = new Dictionary<Keys, Keys>();
            HotKeys = hotkeys;
            InitializeKeys();
        }

        /// <summary>
        ///     Enables the ability to name the set
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Enables the ability to describe what the set is used for or supposed to do
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Gets the set of hotkeys that this class handles.
        /// </summary>
        public IEnumerable<Keys> HotKeys { get; }

        /// <summary>
        ///     Returns whether the set of Keys is activated
        /// </summary>
        public bool HotKeysActivated
        {
            get { return _mHotkeydowncount == _mHotkeystate.Count - _mRemappingCount; }
        }

        /// <summary>
        ///     Gets or sets the enabled state of the HotKey set.
        /// </summary>
        public bool Enabled
        {
            get { return _mEnabled; }
            set
            {
                if (value)
                    InitializeKeys(); //must get the actual current state of each key to update

                _mEnabled = value;
            }
        }

        /// <summary>
        ///     Called as the user holds down the keys in the set.  It is NOT triggered the first time the keys are set.
        ///     <see cref="OnHotKeysDownOnce" />
        /// </summary>
        public event HotKeyHandler OnHotKeysDownHold;

        /// <summary>
        ///     Called whenever the hot key set is no longer active.  This is essentially a KeyPress event, indicating that a full
        ///     key cycle has occurred, only for HotKeys because a single key removed from the set constitutes an incomplete set.
        /// </summary>
        public event HotKeyHandler OnHotKeysUp;

        /// <summary>
        ///     Called the first time the down keys are set.  It does not get called throughout the duration the user holds it but
        ///     only the
        ///     first time it's activated.
        /// </summary>
        public event HotKeyHandler OnHotKeysDownOnce;

        /// <summary>
        ///     General invocation handler
        /// </summary>
        /// <param name="hotKeyDelegate"></param>
        private void InvokeHotKeyHandler(HotKeyHandler hotKeyDelegate)
        {
            if (hotKeyDelegate != null)
                hotKeyDelegate(this, new HotKeyArgs(DateTime.Now));
        }

        /// <summary>
        ///     Adds the keys into the dictionary tracking the keys and gets the real-time status of the Keys
        ///     from the OS
        /// </summary>
        private void InitializeKeys()
        {
            foreach (var k in HotKeys)
            {
                if (_mHotkeystate.ContainsKey(k))
                    _mHotkeystate.Add(k, false);

                //assign using the current state of the keyboard
                _mHotkeystate[k] = KeyboardState.GetCurrent().IsDown(k);
            }
        }

        /// <summary>
        ///     Unregisters a previously set exclusive or based on the primary key.
        /// </summary>
        /// <param name="anyKeyInTheExclusiveOrSet">Any key used in the Registration method used to create an exclusive or set</param>
        /// <returns>
        ///     True if successful.  False doesn't indicate a failure to unregister, it indicates that the Key is not
        ///     registered as an Exclusive Or key or it's not the Primary Key.
        /// </returns>
        public bool UnregisterExclusiveOrKey(Keys anyKeyInTheExclusiveOrSet)
        {
            var primaryKey = GetExclusiveOrPrimaryKey(anyKeyInTheExclusiveOrSet);

            if (primaryKey == Keys.None || !_mRemapping.ContainsValue(primaryKey))
                return false;

            var keystoremove = new List<Keys>();

            foreach (var pair in _mRemapping)
                if (pair.Value == primaryKey)
                    keystoremove.Add(pair.Key);

            foreach (var k in keystoremove)
                _mRemapping.Remove(k);

            --_mRemappingCount;

            return true;
        }

        /// <summary>
        ///     Registers a group of Keys that are already part of the HotKeySet in order to provide better flexibility among keys.
        ///     <example>
        ///         <code>
        ///  HotKeySet hks = new HotKeySet( new [] { Keys.T, Keys.LShiftKey, Keys.RShiftKey } );
        ///  RegisterExclusiveOrKey( new [] { Keys.LShiftKey, Keys.RShiftKey } );
        ///  </code>
        ///         allows either Keys.LShiftKey or Keys.RShiftKey to be combined with Keys.T.
        ///     </example>
        /// </summary>
        /// <param name="orKeySet"></param>
        /// <returns>Primary key used for mapping or Keys.None on error</returns>
        public Keys RegisterExclusiveOrKey(IEnumerable<Keys> orKeySet)
        {
            //Verification first, so as to not leave the m_remapping with a partial set.
            foreach (var k in orKeySet)
                if (!_mHotkeystate.ContainsKey(k))
                    return Keys.None;

            var i = 0;
            var primaryKey = Keys.None;

            //Commit after verification
            foreach (var k in orKeySet)
            {
                if (i == 0)
                    primaryKey = k;

                _mRemapping[k] = primaryKey;

                ++i;
            }

            //Must increase to keep a true count of how many keys are necessary for the activation to be true
            ++_mRemappingCount;

            return primaryKey;
        }

        /// <summary>
        ///     Gets the primary key
        /// </summary>
        /// <param name="k"></param>
        /// <returns>The primary key if it exists, otherwise Keys.None</returns>
        private Keys GetExclusiveOrPrimaryKey(Keys k)
        {
            return _mRemapping.ContainsKey(k) ? _mRemapping[k] : Keys.None;
        }

        /// <summary>
        ///     Resolves obtaining the key used for state checking.
        /// </summary>
        /// <param name="k"></param>
        /// <returns>The primary key if it exists, otherwise the key entered</returns>
        private Keys GetPrimaryKey(Keys k)
        {
            //If the key is remapped then get the primary keys
            return _mRemapping.ContainsKey(k) ? _mRemapping[k] : k;
        }

        /// <summary>
        /// </summary>
        /// <param name="kex"></param>
        internal void OnKey(KeyEventArgsExt kex)
        {
            if (!Enabled)
                return;

            //Gets the primary key if mapped to a single key or gets the key itself
            var primaryKey = GetPrimaryKey(kex.KeyCode);

            if (kex.IsKeyDown)
                OnKeyDown(primaryKey);
            else //reset
                OnKeyUp(primaryKey);
        }

        private void OnKeyDown(Keys k)
        {
            //If the keys are activated still then keep invoking the event
            if (HotKeysActivated)
            {
                InvokeHotKeyHandler(OnHotKeysDownHold); //Call the duration event
            }

            //indicates the key's state is current false but the key is now down
            else if (_mHotkeystate.ContainsKey(k) && !_mHotkeystate[k])
            {
                _mHotkeystate[k] = true; //key's state is down
                ++_mHotkeydowncount; //increase the number of keys down in this set

                if (HotKeysActivated) //because of the increase, check whether the set is activated
                    InvokeHotKeyHandler(OnHotKeysDownOnce); //Call the initial event
            }
        }

        private void OnKeyUp(Keys k)
        {
            if (_mHotkeystate.ContainsKey(k) && _mHotkeystate[k]) //indicates the key's state was down but now it's up
            {
                var wasActive = HotKeysActivated;

                _mHotkeystate[k] = false; //key's state is up
                --_mHotkeydowncount; //this set is no longer ready

                if (wasActive)
                    InvokeHotKeyHandler(OnHotKeysUp); //call the KeyUp event because the set is no longer active
            }
        }
    }
}