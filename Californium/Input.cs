﻿using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Californium
{
    public class Input
    {
        public delegate bool KeyEvent(KeyInputArgs args);
        public delegate bool TextEvent(TextInputArgs args);
        public delegate bool MouseButtonEvent(MouseButtonInputArgs args);
        public delegate bool MouseWheelEvent(MouseWheelInputArgs args);
        public delegate bool MouseMoveEvent(MouseMoveInputArgs args);

        /// <summary>
        /// Keyboard button event handlers
        /// </summary>
        public readonly Dictionary<Keyboard.Key, KeyEvent> Key;

        /// <summary>
        /// Mouse button event handlers
        /// </summary>
        public readonly Dictionary<Mouse.Button, MouseButtonEvent> MouseButton;

        /// <summary>
        /// Text event handler. This event should always used for text input.
        /// </summary>
        public TextEvent Text;

        /// <summary>
        /// Mouse wheel handler. Will not occur when outside of the window.
        /// </summary>
        public MouseWheelEvent MouseWheel;
        
        /// <summary>
        /// Mouse move handler. Will not occur when outside of the window.
        /// </summary>
        public MouseMoveEvent MouseMove;

        public Input()
        {
            Key = new Dictionary<Keyboard.Key, KeyEvent>();
            MouseButton = new Dictionary<Mouse.Button, MouseButtonEvent>();
            Text = null;
            MouseWheel = null;
            MouseMove = null;
        }

        internal bool ProcessInput(InputArgs args)
        {
            if (MouseMove != null && args is MouseMoveInputArgs)
            {
                var eArgs = (MouseMoveInputArgs)args;
                return MouseMove(eArgs);
            }
            
            if (args is KeyInputArgs)
            {
                var eArgs = (KeyInputArgs)args;
                KeyEvent e;

                if (Key.TryGetValue(eArgs.Key, out e))
                    return e(eArgs);
            }
            else if (Text != null && args is TextInputArgs)
            {
                var eArgs = (TextInputArgs)args;
                return Text(eArgs);
            }
            else if (args is MouseButtonInputArgs)
            {
                var eArgs = (MouseButtonInputArgs)args;
                MouseButtonEvent e;

                if (MouseButton.TryGetValue(eArgs.Button, out e))
                    return e(eArgs);
            }
            else if (MouseWheel != null && args is MouseWheelInputArgs)
            {
                var eArgs = (MouseWheelInputArgs)args;
                return MouseWheel(eArgs);
            }

            return false;
        }
    }

    public abstract class InputArgs
    {
        internal View View;
    }

    public class KeyInputArgs : InputArgs
    {
        public Keyboard.Key Key { get; protected set; }
        public bool Pressed { get; protected set; }
        public bool Control { get; private set; }
        public bool Shift { get; private set; }

        public KeyInputArgs(Keyboard.Key key, bool pressed, bool control, bool shift)
        {
            Key = key;
            Pressed = pressed;
            Control = control;
            Shift = shift;
        }
    }

    public class TextInputArgs : InputArgs
    {
        public string Text { get; protected set; }

        public TextInputArgs(string text)
        {
            Text = text;
        }
    }

    public class MouseButtonInputArgs : InputArgs
    {
        public Mouse.Button Button { get; protected set; }
        public bool Pressed { get; protected set; }
        public Vector2f Position { get { return Game.Window.MapPixelToCoords(screenPosition, View); } }
        private readonly Vector2i screenPosition;

        public MouseButtonInputArgs(Mouse.Button button, bool pressed, int x, int y)
        {
            Button = button;
            Pressed = pressed;
            screenPosition = new Vector2i(x, y);
        }
    }

    public class MouseWheelInputArgs : InputArgs
    {
        public int Delta { get; protected set; }
        public Vector2f Position { get { return Game.Window.MapPixelToCoords(screenPosition, View); } }
        private readonly Vector2i screenPosition;

        public MouseWheelInputArgs(int delta, int x, int y)
        {
            Delta = delta;
            screenPosition = new Vector2i(x, y);
        }
    }

    public class MouseMoveInputArgs : InputArgs
    {
        public Vector2f Position { get { return Game.Window.MapPixelToCoords(screenPosition, View); } }
        private readonly Vector2i screenPosition;

        public MouseMoveInputArgs(int x, int y)
        {
            screenPosition = new Vector2i(x, y);
        }
    }
}
