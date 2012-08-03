using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using HuntTheWumpus.Utilities;
namespace HuntTheWumpus.UICore
{

    public abstract class GUIElement
    {
        private MouseState _prevMouseState;
        private KeyboardState _prevKeyState;

        private Control activeControl;
        public Control ActiveControl
        {
            get
            {
                return activeControl;
            }

            set
            {
                if (activeControl != null)
                    activeControl.Deselect();
                activeControl = value;
                if (activeControl != null)
                    activeControl.Select();
            }
        }
        public List<Control> Controls { get; protected set; }

        private Vector2 location;
        public Vector2 Location 
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
                //foreach (Control control in Controls)
                //{
                //    control.Location = new Vector2(
                //        control.Location.X + location.X,
                //        control.Location.Y + location.Y);
                //}
            }
        }

        /// <summary>
        /// Move the entire control, including the child controls
        /// </summary>
        public void MoveAll(Vector2 location)
        {
            Vector2 delta = location - Location;
            Location = location;
            foreach (Control control in Controls)
            {
                control.MoveAll(control.Location + delta);
            }
        }

        /// <summary>
        /// Makes the Top Left corner of the element the center
        /// </summary>
        public void CenterAboutLocation(bool moveChildren)
        {
            if (moveChildren)
                MoveAll(this.Location - (Vector2)this.ElementSize / 2);
            else this.Location -= (Vector2)this.ElementSize / 2;
        }

        public Size ElementSize { get; protected set; }

        public UIEngine UIManager { get; private set; }
        public void BindToGUIEngine(UIEngine engine)
        {
            if (UIManager != null)
                throw new Exception("Already bound to a GUIEngine");
            UIManager = engine;
        }

        public void AddControl(Control control)
        {
            Controls.Add(control);
            control.setParent(this);
            //control.Location = new Vector2(
            //            control.Location.X + location.X,
            //            control.Location.Y + location.Y);
        }
        public void RemoveControl(Control control)
        {
            control.Destroy();
            if (ActiveControl == control)
                ActiveControl = null;
            Controls.Remove(control);
        }

        public event MouseEventHandler MouseOver;
        public event MouseEventHandler MouseEnter;
        public event MouseEventHandler MouseExit;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseClick;
        public event MouseEventHandler MouseScroll;
        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;
        // Should not need to override
        protected virtual void OnMouseOver(MouseEventArgs e)
        {
            if (MouseOver != null)
                MouseOver(this, e);
        }
        protected virtual void OnMouseEnter(MouseEventArgs e)
        {
            if (MouseEnter != null)
                MouseEnter(this, e);
        }
        protected virtual void OnMouseExit(MouseEventArgs e)
        {
            if (MouseExit != null)
                MouseExit(this, e);
        }
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            if (MouseDown != null)
                MouseDown(this, e);
        }
        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            if (MouseClick != null)
                MouseClick(this, e);
        }
        protected virtual void OnMouseScroll(MouseEventArgs e)
        {
            if (MouseScroll != null)
                MouseScroll(this, e);
        }
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            if (KeyDown != null)
                KeyDown(this, e);
        }
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            if (KeyUp != null)
                KeyUp(this, e);
        }
        
        // Helper Functions
        private void handleComputerEvents()
        {

            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();

            // Mouse Handling
            if (ContainsPoint(mouseState.X, mouseState.Y))
            {
                if (!ContainsPoint(_prevMouseState.X, _prevMouseState.Y))
                    OnMouseEnter(new MouseEventArgs(_prevMouseState, mouseState));
                OnMouseOver(new MouseEventArgs(_prevMouseState, mouseState));

                if (_prevMouseState.ScrollWheelValue != mouseState.ScrollWheelValue)
                    OnMouseScroll(new MouseEventArgs(_prevMouseState, mouseState));

                if (mouseState.LeftButton == ButtonState.Pressed ||
                    mouseState.RightButton == ButtonState.Pressed ||
                    mouseState.MiddleButton == ButtonState.Pressed)
                    OnMouseDown(new MouseEventArgs(_prevMouseState, mouseState));
                else if (isMouseClicked(mouseState))
                    OnMouseClick(new MouseEventArgs(_prevMouseState, mouseState));
            }
            else
            {
                if (this is Control &&
                    ((Control)this).Parent.ActiveControl == this &&
                    isMouseClicked(mouseState))
                    ((Control)this).Parent.ActiveControl = null;
                if (ContainsPoint(_prevMouseState.X, _prevMouseState.Y))
                    OnMouseExit(new MouseEventArgs(_prevMouseState, mouseState));
            }

            // Keyboard Handling
            if (!(this is Control) || ((Control)this).Parent.ActiveControl == this)
            {
                var prevPressedKeys = _prevKeyState.GetPressedKeys();
                var pressedKeys = keyState.GetPressedKeys();

                if (pressedKeys.Length > 0)
                    OnKeyDown(new KeyEventArgs(pressedKeys));

                var releasedKeys = prevPressedKeys
                    .Where<Keys>(k => !pressedKeys.Contains<Keys>(k))
                    .ToArray<Keys>();

                if (releasedKeys.Length > 0)
                    OnKeyUp(new KeyEventArgs(releasedKeys));
            }

            _prevMouseState = mouseState;
            _prevKeyState = keyState;
        }

        private bool isMouseClicked(MouseState mouseState)
        {
            return (_prevMouseState.LeftButton == ButtonState.Pressed &&
                            mouseState.LeftButton == ButtonState.Released) ||
                    (_prevMouseState.MiddleButton == ButtonState.Pressed &&
                            mouseState.MiddleButton == ButtonState.Released) ||
                    (_prevMouseState.RightButton == ButtonState.Pressed &&
                            mouseState.RightButton == ButtonState.Released);
        }
        
        /// <summary>
        /// For future(?) XBox support
        /// </summary>
        private void handleGamepadEvents()
        {
            throw new NotImplementedException();
        }
        
        public bool ContainsPoint(int x, int y)
        {
            var Dx = x - Location.X;
            var Dy = y - Location.Y;

            return
                Dx > 0 &&
                Dy > 0 &&
                Dx < ElementSize.X &&
                Dy < ElementSize.Y;
        }

        // May need to override //
        public virtual void Update(GameTime time)
        {
            handleComputerEvents();
            Controls.ForEach(c => c.Update(time));
        }

        public virtual void Draw(GameTime time, SpriteBatch spriteBatch)
        {
            Controls.ForEach(c => c.Draw(time, spriteBatch));
        }
        public virtual void Destroy() { }
    }
}
