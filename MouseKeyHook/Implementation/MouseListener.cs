// This code is distributed under MIT license.
// Copyright (c) 2015 George Mamaladze
// See license.txt or https://mit-license.org/

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Gma.System.MouseKeyHook.WinApi;

namespace Gma.System.MouseKeyHook.Implementation
{
    // Because it is a P/Invoke method, 'GetSystemMetrics(int)'
    // should be defined in a class named NativeMethods, SafeNativeMethods,
    // or UnsafeNativeMethods.
    // https://msdn.microsoft.com/en-us/library/windows/desktop/ms724385(v=vs.85).aspx
    internal static class NativeMethods
    {
        private const int WM_SM_CXDRAG = 68;
        private const int WM_SM_CYDRAG = 69;
        private const int WM_SM_CXDOUBLECLK = 36;
        private const int WM_SM_CYDOUBLECLK = 37;

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int index);

        public static int GetXDragThreshold()
        {
            return GetSystemMetrics(WM_SM_CXDRAG);
        }

        public static int GetYDragThreshold()
        {
            return GetSystemMetrics(WM_SM_CYDRAG);
        }

        public static int GetXDoubleClickThreshold()
        {
            return GetSystemMetrics(WM_SM_CXDOUBLECLK) / 2 + 1;
        }

        public static int GetYDoubleClickThreshold()
        {
            return GetSystemMetrics(WM_SM_CYDOUBLECLK) / 2 + 1;
        }
    }

    internal abstract class MouseListener : BaseListener, IMouseEvents
    {
        private readonly ButtonSet _mDoubleDown;
        private readonly ButtonSet _mSingleDown;
        protected readonly Point MUninitialisedPoint = new Point(-99999, -99999);
        private readonly int _mXDragThreshold;
        private readonly int _mYDragThreshold;
        private Point _mDragStartPosition;

        private bool _mIsDragging;

        private Point _mPreviousPosition;

        protected MouseListener(Subscribe subscribe)
            : base(subscribe)
        {
            _mXDragThreshold = NativeMethods.GetXDragThreshold();
            _mYDragThreshold = NativeMethods.GetYDragThreshold();
            _mIsDragging = false;

            _mPreviousPosition = MUninitialisedPoint;
            _mDragStartPosition = MUninitialisedPoint;

            _mDoubleDown = new ButtonSet();
            _mSingleDown = new ButtonSet();
        }

        public event MouseEventHandler MouseMove;
        public event EventHandler<MouseEventExtArgs> MouseMoveExt;
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseDown;
        public event EventHandler<MouseEventExtArgs> MouseDownExt;
        public event MouseEventHandler MouseUp;
        public event EventHandler<MouseEventExtArgs> MouseUpExt;
        public event MouseEventHandler MouseWheel;
        public event EventHandler<MouseEventExtArgs> MouseWheelExt;
        public event MouseEventHandler MouseHWheel;
        public event EventHandler<MouseEventExtArgs> MouseHWheelExt;
        public event MouseEventHandler MouseDoubleClick;
        public event MouseEventHandler MouseDragStarted;
        public event EventHandler<MouseEventExtArgs> MouseDragStartedExt;
        public event MouseEventHandler MouseDragFinished;
        public event EventHandler<MouseEventExtArgs> MouseDragFinishedExt;

        protected override bool Callback(CallbackData data)
        {
            var e = GetEventArgs(data);

            if (e.IsMouseButtonDown)
                ProcessDown(ref e);

            if (e.IsMouseButtonUp)
                ProcessUp(ref e);

            if (e.WheelScrolled)
            {
                if (e.IsHorizontalWheel)
                    ProcessHWheel(ref e);
                else
                    ProcessWheel(ref e);
            }

            if (HasMoved(e.Point))
                ProcessMove(ref e);

            ProcessDrag(ref e);

            return !e.Handled;
        }

        protected abstract MouseEventExtArgs GetEventArgs(CallbackData data);

        protected virtual void ProcessWheel(ref MouseEventExtArgs e)
        {
            OnWheel(e);
            OnWheelExt(e);
        }

        protected virtual void ProcessHWheel(ref MouseEventExtArgs e)
        {
            OnHWheel(e);
            OnHWheelExt(e);
        }

        protected virtual void ProcessDown(ref MouseEventExtArgs e)
        {
            OnDown(e);
            OnDownExt(e);
            if (e.Handled)
                return;

            if (e.Clicks == 2)
                _mDoubleDown.Add(e.Button);

            if (e.Clicks == 1)
                _mSingleDown.Add(e.Button);
        }

        protected virtual void ProcessUp(ref MouseEventExtArgs e)
        {
            OnUp(e);
            OnUpExt(e);
            if (e.Handled)
                return;

            if (_mSingleDown.Contains(e.Button))
            {
                OnClick(e);
                _mSingleDown.Remove(e.Button);
            }

            if (_mDoubleDown.Contains(e.Button))
            {
                e = e.ToDoubleClickEventArgs();
                OnDoubleClick(e);
                _mDoubleDown.Remove(e.Button);
            }
        }

        private void ProcessMove(ref MouseEventExtArgs e)
        {
            _mPreviousPosition = e.Point;

            OnMove(e);
            OnMoveExt(e);
        }

        private void ProcessDrag(ref MouseEventExtArgs e)
        {
            if (_mSingleDown.Contains(MouseButtons.Left))
            {
                if (_mDragStartPosition.Equals(MUninitialisedPoint))
                    _mDragStartPosition = e.Point;

                ProcessDragStarted(ref e);
            }
            else
            {
                _mDragStartPosition = MUninitialisedPoint;
                ProcessDragFinished(ref e);
            }
        }

        private void ProcessDragStarted(ref MouseEventExtArgs e)
        {
            if (!_mIsDragging)
            {
                var isXDragging = Math.Abs(e.Point.X - _mDragStartPosition.X) > _mXDragThreshold;
                var isYDragging = Math.Abs(e.Point.Y - _mDragStartPosition.Y) > _mYDragThreshold;
                _mIsDragging = isXDragging || isYDragging;

                if (_mIsDragging)
                {
                    OnDragStarted(e);
                    OnDragStartedExt(e);
                }
            }
        }

        private void ProcessDragFinished(ref MouseEventExtArgs e)
        {
            if (_mIsDragging)
            {
                OnDragFinished(e);
                OnDragFinishedExt(e);
                _mIsDragging = false;
            }
        }

        private bool HasMoved(Point actualPoint)
        {
            return _mPreviousPosition != actualPoint;
        }

        protected virtual void OnMove(MouseEventArgs e)
        {
            var handler = MouseMove;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnMoveExt(MouseEventExtArgs e)
        {
            var handler = MouseMoveExt;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnClick(MouseEventArgs e)
        {
            var handler = MouseClick;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDown(MouseEventArgs e)
        {
            var handler = MouseDown;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDownExt(MouseEventExtArgs e)
        {
            var handler = MouseDownExt;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnUp(MouseEventArgs e)
        {
            var handler = MouseUp;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnUpExt(MouseEventExtArgs e)
        {
            var handler = MouseUpExt;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnWheel(MouseEventArgs e)
        {
            var handler = MouseWheel;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnWheelExt(MouseEventExtArgs e)
        {
            var handler = MouseWheelExt;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnHWheel(MouseEventArgs e)
        {
            var handler = MouseHWheel;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnHWheelExt(MouseEventExtArgs e)
        {
            var handler = MouseHWheelExt;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDoubleClick(MouseEventArgs e)
        {
            var handler = MouseDoubleClick;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDragStarted(MouseEventArgs e)
        {
            var handler = MouseDragStarted;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDragStartedExt(MouseEventExtArgs e)
        {
            var handler = MouseDragStartedExt;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDragFinished(MouseEventArgs e)
        {
            var handler = MouseDragFinished;
            if (handler != null) handler(this, e);
        }

        protected virtual void OnDragFinishedExt(MouseEventExtArgs e)
        {
            var handler = MouseDragFinishedExt;
            if (handler != null) handler(this, e);
        }
    }
}
