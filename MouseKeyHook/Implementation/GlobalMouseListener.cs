// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or https://mit-license.org/

using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook.WinApi;

namespace Gma.System.MouseKeyHook.Implementation
{
    internal class GlobalMouseListener : MouseListener
    {
        private readonly int _mSystemDoubleClickTime;
        private readonly int _mXDoubleClickThreshold;
        private readonly int _mYDoubleClickThreshold;
        private MouseButtons _mPreviousClicked;
        private Point _mPreviousClickedPosition;
        private int _mPreviousClickedTime;

        public GlobalMouseListener()
            : base(HookHelper.HookGlobalMouse)
        {
            _mSystemDoubleClickTime = MouseNativeMethods.GetDoubleClickTime();
            _mXDoubleClickThreshold = NativeMethods.GetXDoubleClickThreshold();
            _mYDoubleClickThreshold = NativeMethods.GetYDoubleClickThreshold();
        }

        protected override void ProcessDown(ref MouseEventExtArgs e)
        {
            if (IsDoubleClick(e))
                e = e.ToDoubleClickEventArgs();
            else
                StartDoubleClickWaiting(e);
            base.ProcessDown(ref e);
        }

        protected override void ProcessUp(ref MouseEventExtArgs e)
        {
            base.ProcessUp(ref e);
            if (e.Clicks == 2)
                StopDoubleClickWaiting();
        }

        private void StartDoubleClickWaiting(MouseEventExtArgs e)
        {
            _mPreviousClicked = e.Button;
            _mPreviousClickedTime = e.Timestamp;
            _mPreviousClickedPosition = e.Point;
        }

        private void StopDoubleClickWaiting()
        {
            _mPreviousClicked = MouseButtons.None;
            _mPreviousClickedTime = 0;
            _mPreviousClickedPosition = MUninitialisedPoint;
        }

        private bool IsDoubleClick(MouseEventExtArgs e)
        {
            var isXMoving = Math.Abs(e.Point.X - _mPreviousClickedPosition.X) > _mXDoubleClickThreshold;
            var isYMoving = Math.Abs(e.Point.Y - _mPreviousClickedPosition.Y) > _mYDoubleClickThreshold;

            return
                e.Button == _mPreviousClicked &&
                !isXMoving &&
                !isYMoving &&
                e.Timestamp - _mPreviousClickedTime <= _mSystemDoubleClickTime;
        }

        protected override MouseEventExtArgs GetEventArgs(CallbackData data)
        {
            return MouseEventExtArgs.FromRawDataGlobal(data);
        }
    }
}
