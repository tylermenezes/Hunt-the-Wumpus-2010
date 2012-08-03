using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using HuntTheWumpus.GameEngineCore.Actors;

namespace HuntTheWumpus.GameEngineCore.Editor
{
    class FileIO
    {
        private static char[] splitters = {'#', '^', ',', '|', '!', '@', '?'};

        public static String SaveToString(List<Actor> actors)
        {
            StringBuilder saveString = new StringBuilder();

            StandardObjects objectCreator = new StandardObjects();
            objectCreator.Load();

            foreach (Actor actor in actors)
            {
                if (actor == null)
                    saveString.Append(splitters[3]);
                else if ((actor.objectType == objectCreator.objectCodes.IndexOf("InterPortal")) || (actor.objectType == objectCreator.objectCodes.IndexOf("IntraPortal")))
                    saveString.Append(splitters[0].ToString() + splitters[4].ToString() + ((ILinked)actor).ID + splitters[5].ToString() + ((ILinked)actor).PartnerAddress + splitters[6].ToString() + actor.objectType + splitters[1].ToString() + actor.PolyBody.Position.X + splitters[2].ToString() + actor.PolyBody.Position.Y);
                else
                    saveString.Append(splitters[0].ToString() + actor.objectType + splitters[1].ToString() + actor.PolyBody.Position.X + splitters[2].ToString() + actor.PolyBody.Position.Y);
            }
            return saveString.ToString();

        }
        
        public static void SaveToFile(List<Actor> actors)
        {
            StreamWriter writer = new StreamWriter(Environment.CurrentDirectory + "\\level.rptr", false);
            writer.Write(SaveToString(actors));
            writer.Close();
        }

        public static List<Actor> LoadFromString(String loadString)
        {
            char[] splitter = new char[1];

            StandardObjects objectCreator = new StandardObjects();
            objectCreator.Load();

            List<Actor> actors = new List<Actor>();

            splitter[0] = splitters[3];
            String[] splitData = loadString.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < splitData.Length; i++)
            {
                splitter[0] = splitters[0];
                String[] parsedData = splitData[i].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                foreach (String rawActor in parsedData)
                {
                    if (rawActor[0] == splitters[4])
                    {
                        String rawLinked = rawActor.Remove(0, 1);
                        splitter[0] = splitters[5];
                        String[] parsedPortalID = rawLinked.Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        String linkID = parsedPortalID[0];
                        splitter[0] = splitters[6];
                        String[] parsedAddress = parsedPortalID[1].Split(splitter, StringSplitOptions.RemoveEmptyEntries);
                        String address = parsedAddress[0];

                        splitter[0] = splitters[1];
                        String[] parsedActor = parsedAddress[1].Split(splitter);
                        splitter[0] = splitters[2];
                        String[] parsedPosition = parsedActor[1].Split(splitter);
                        Actor actor = objectCreator.create(int.Parse(parsedActor[0]), linkID, address);
                        float x;
                        float y;
                        float.TryParse(parsedPosition[0], out x);
                        float.TryParse(parsedPosition[1], out y);
                        actor.PolyBody.SetMasterPosition(new Vector2(x, y));
                        actors.Add(actor);
                    }
                    else
                    {
                        splitter[0] = splitters[1];
                        String[] parsedActor = rawActor.Split(splitter);
                        splitter[0] = splitters[2];
                        String[] parsedPosition = parsedActor[1].Split(splitter);
                        Actor actor = objectCreator.create(int.Parse(parsedActor[0]));
                        float x;
                        float y;
                        float.TryParse(parsedPosition[0], out x);
                        float.TryParse(parsedPosition[1], out y);
                        if (i == 0)
                            actor.PolyBody.SetMasterPosition(new Vector2(x, y));
                        else if (i == 1)
                            actor.PolyBody.Position = new Vector2(x, y);
                        actors.Add(actor);
                    }
                }
                actors.Add(null);
            }
            return actors;
        }

        public static List<Actor> LoadFromFile()
        {
            StreamReader reader = new StreamReader(Environment.CurrentDirectory + "\\level.rptr");
            String loadString = reader.ReadToEnd();
            reader.Close();
            return LoadFromString(loadString);
        }
    }
}
