using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace HuntTheWumpus.GameEngineCore
{
    public static class Debug
    {

        public static void Trace(string msg)
        {
            StackTrace strace = new StackTrace();
            var caller = strace.GetFrame(1).GetMethod();
            Console.Write("[{0}|{1}]>> ", caller.Module, caller.Name);
            Console.WriteLine(msg);
        }
    }

    public class FPSManager
    {
        int fps;
        int frames = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;

        public FPSManager()
        {
        }
        public void Update(GameTime time)
        {
            frames++;

            elapsedTime += time.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                fps = frames;
                frames = 0;

                //Debug.Trace(String.Format("fps: {0}", GetFPS().ToString()));
            }
        }
        public int GetFPS() //this is only a method because it feels like there should be some calculation involved. (there really isn't...)
        {
            return fps;
        }
    }
}
