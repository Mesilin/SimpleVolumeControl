// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or https://mit-license.org/

using System.Windows.Forms;

namespace Gma.System.MouseKeyHook.Implementation
{
    internal class ButtonSet
    {
        private MouseButtons _mSet;

        public ButtonSet()
        {
            _mSet = MouseButtons.None;
        }

        public void Add(MouseButtons element)
        {
            _mSet |= element;
        }

        public void Remove(MouseButtons element)
        {
            _mSet &= ~element;
        }

        public bool Contains(MouseButtons element)
        {
            return (_mSet & element) != MouseButtons.None;
        }
    }
}