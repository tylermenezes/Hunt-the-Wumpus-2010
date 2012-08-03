using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HuntTheWumpus.UICore
{
    public class UIEngine
    {
        List<Frame> _frames;
        public Frame ActiveFrame
        {
            get
            {
                return (_frames.Count > 0) ? _frames[_frames.Count - 1] : null;
            }
        }

        public UIEngine()
        {
            _frames = new List<Frame>();
        }

        public void AddFrame(Frame frame)
        {
            if (ActiveFrame != null)
                ActiveFrame.OnLoseControl(ActiveFrame);
            _frames.Add(frame);
            frame.BindToGUIEngine(this);
            ActiveFrame.OnGainControl(ActiveFrame);
        }
        public void AddAndLoad(Frame frame)
        {
            if (ActiveFrame != null)
               ActiveFrame.OnLoseControl(ActiveFrame);
            _frames.Add(frame);
            frame.BindToGUIEngine(this);
            frame.Load();
            ActiveFrame.OnGainControl(ActiveFrame);
        }

        private void RemoveFrame(int n)
        {
            _frames[n].PleaseDestroy = true;
        }

        private void DestroyAndRemove(int n)
        {
            RemoveFrame(n);
        }

        public void RemoveFrame()
        {
            ActiveFrame.OnLoseControl(ActiveFrame);
            ActiveFrame.PleaseDestroy = true;
        }
        public void DestroyAndRemove()
        {
            ActiveFrame.OnLoseControl(ActiveFrame);
            ActiveFrame.PleaseDestroy = true;

        }

        public void RemoveFrames(int n)
        {
            for (int i = 0; i < n; i++)
            {
                RemoveFrame(_frames.Count - i);
            }
        }
        public void RemoveFramesAndDestroy(int n)
        {
            for (int i = 0; i < n; i++)
            {
                DestroyAndRemove(_frames.Count - i);

            }
        }
        public bool Update(GameTime time)
        {
            if (ActiveFrame.PleaseDestroy)
            {
                ActiveFrame.Destroy();
                _frames.RemoveAt(_frames.Count - 1);
            }
            if (_frames.Count > 0)
                ActiveFrame.Update(time);
            else
                return false;
            return true;
        }
        public bool Draw(GameTime time, SpriteBatch spriteBatch)
        {
            if (_frames.Count > 0)
            {
                var startDrawingAt = _frames.Count - 1;
                for (int i = startDrawingAt; i >= 0; i--)
                {
                    if (!_frames[i].Transparent)
                    {
                        startDrawingAt = i;
                        break;
                    }
                }
                for (int i = startDrawingAt; i < _frames.Count; i++)
                    _frames[i].Draw(time, spriteBatch);
            }
            else
                return false;
            return true;
        }

        public void Destroy()
        {
            _frames.ForEach(x => x.Destroy());
        }
    }

}
