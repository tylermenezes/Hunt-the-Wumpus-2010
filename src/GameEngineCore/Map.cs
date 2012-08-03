using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Xml;

using HuntTheWumpus.Utilities;
using HuntTheWumpus.PhysicsCore;
using HuntTheWumpus.GameEngineCore.GameStates;
namespace HuntTheWumpus.GameEngineCore
{
    delegate void MapEventHandler(Map sender);
    class Map
    {
        /// <summary>
        /// All actors INCLUDING MainPlayer
        /// </summary>
        public List<Actor> GameObjects { get; private set; }
        
        /// <summary>
        /// Actors that do stuff themselves
        /// </summary>
        public List<Actor> DynamicObjects { get; private set; }

        
        /// <summary>
        /// Objects that need to be addressed by index : ex. portals, switches, gates etc.
        /// </summary>
        public Dictionary<String, Actor> LinkedObjects { get; private set; }

        public List<BinaryForceComponent> binaryForceComps = new List<BinaryForceComponent>();

        /// <summary>
        /// Player that recieves user interaction
        /// </summary>
        public Actors.Player MainPlayer { get; private set; }

        public event MapEventHandler Loaded;
        public event MapEventHandler UnLoaded;

        public float PixelsPerMeter { get; private set; }
        public string ID { get; private set; }
        public string StartUpPortalID { get; private set; }
        public void AddGameObject(Actor actor)
        {
            GameObjects.Add(actor);
            if (actor is Actors.IDynamic)
                DynamicObjects.Add(actor);
            if (actor is Actors.ILinked)
                LinkedObjects.Add(((Actors.ILinked)actor).ID, actor);
        }

        public void AddActorGroup(ActorGroup ag)
        {
            foreach (var actor in ag.actors)
            {
                GameObjects.Add(actor);
                if (actor is Actors.IDynamic)
                    DynamicObjects.Add(actor);
                if (actor is Actors.ILinked)
                    LinkedObjects.Add(((Actors.ILinked)actor).ID, actor);
            }
            this.binaryForceComps.AddRange(ag.bfcs);
            
        }

        public void RemoveGameObject(Actor actor)
        {
            GameObjects.Remove(actor);
            if (actor is Actors.IDynamic)
                DynamicObjects.Remove(actor);
            if (actor is Actors.ILinked)
                LinkedObjects.Remove(((Actors.ILinked)actor).ID);
        }

        /// <summary>
        /// Generate Map from XML
        /// </summary>
        public Map(string mapID, XmlNode mapCode)
        {
            GameObjects = new List<Actor>();
            DynamicObjects = new List<Actor>();
            LinkedObjects = new Dictionary<string, Actor>();
            PixelsPerMeter = float.Parse(mapCode.Attributes.GetNamedItem("pixelspermeter").Value);
            ID = mapID;
            
            var actorNamespace = "HuntTheWumpus.GameEngineCore.Actors.";
            foreach (XmlNode node in mapCode.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                   // position="{X:10.66667 Y:0.25 Z:0}" velocity="{X:0 Y:0 Z:0}" time="0" Friction="0.15" Gravity="{X:0 Y:-9.8}
                    object[] args = ExtractParameters(node);
                    Actor actor = (Actor)
                        System.Reflection.Assembly.GetExecutingAssembly()
                        .CreateInstance(actorNamespace + node.Name, true,
                        System.Reflection.BindingFlags.CreateInstance, null,
                        args,
                        null, new object[0]);
                    Vector3 position = ParseVector(node.Attributes.GetNamedItem("position").Value);
                    Vector3 velocity = ParseVector(node.Attributes.GetNamedItem("velocity").Value);
                    Vector3 gravity = ParseVector(node.Attributes.GetNamedItem("gravity").Value);

                    float time = float.Parse(node.Attributes.GetNamedItem("time").Value);
                    float friction = float.Parse(node.Attributes.GetNamedItem("friction").Value);

                    actor.PolyBody.SetMasterPosition(position.ToVector2());
                    //actor.PolyBody.Rotation = position.Z;
                    actor.PolyBody.SetMasterRotation(position.Z);
                    
                    actor.PolyBody.Velocity = velocity.ToVector2();
                    actor.PolyBody.AngularVelocity = velocity.Z;

                    actor.PolyBody.Gravity = gravity.ToVector2();
                    actor.PolyBody.Time = time;
                    actor.PolyBody.Friction = friction;

                    AddGameObject(actor);
                }
            }
        }

        private Vector3 ParseVector(string value)
        {
            // 10.66667,0.25,0
            string[] values = value.Split(',');

            return new Vector3(
                float.Parse(values[0]),
                float.Parse(values[1]),
                float.Parse(values[2]));
        }

        private object[] ExtractParameters(XmlNode obj)
        {
            List<object> args = new List<object>();
            foreach (XmlNode node in obj.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    string paramType = node.Name;
                    string paramValue = node.InnerText;

                    // Someone replace this with reflection. I don't want to do the research
                    switch (paramType)
                    {
                        case "float":
                            args.Add(float.Parse(paramValue));
                            break;
                        case "int":
                            args.Add(Int32.Parse(paramValue));
                            break;
                        case "string":
                            args.Add(paramValue);
                            break;
                        case "Color":
                            args.Add(
                                new Color(ParseVector(paramValue)));
                            break;
                        default:
                            throw new Exception("Unrecognized Param Type");
                    }
                }
            }
            return args.ToArray();
        }

        public Map(string mapID, float pixelsPerMeter, Actor[] gameObjects)
        {
            GameObjects = gameObjects.ToList<Actor>();
            DynamicObjects = new List<Actor>();
            LinkedObjects = new Dictionary<string, Actor>();
            PixelsPerMeter = pixelsPerMeter;
            ID = mapID;
            
            foreach (Actor actor in GameObjects)
            {
                if (actor is Actors.IDynamic)
                    DynamicObjects.Add(actor);
                if (actor is Actors.ILinked)
                    LinkedObjects.Add(
                        ((Actors.ILinked)actor).ID,
                        actor);
            }
        }

        public Map(string mapID, Actor[] gameObjects)
        {
            GameObjects = gameObjects.ToList<Actor>();
            DynamicObjects = new List<Actor>();
            LinkedObjects = new Dictionary<string, Actor>();
            PixelsPerMeter = (float)Utilities.DefaultSettings.Settings["PixelsPerMeter"];
            ID = mapID;
            foreach (Actor actor in GameObjects)
            {
                if (actor is Actors.IDynamic)
                    DynamicObjects.Add(actor);
                if (actor is Actors.ILinked)
                    LinkedObjects.Add(
                        ((Actors.ILinked)actor).ID,
                        actor);
            }
        }

        /// <summary>
        /// Set this Map as Active
        /// </summary>
        public void Load(string startupPortalID)
        {
            var game = GameEngine.Singleton.GetPlayState();
            Actors.Player mainPlayer;
            if (game.ActiveMap == null)
                mainPlayer = new HuntTheWumpus.GameEngineCore.Actors.Player(Vector2.Zero);
            else
                mainPlayer = game.ActiveMap.MainPlayer.Clone();
            GameEngine.Singleton.AddAndLoad(new FlashBackState(ID));
            mainPlayer.PolyBody.Position = LinkedObjects[startupPortalID].PolyBody.Position;
            
            StartUpPortalID = startupPortalID;
            this.Load(mainPlayer);
        }


        public void Load(Actors.Player mainPlayer)
        {
            var game = GameEngine.Singleton.GetPlayState();

            if (game.ActiveMap != null &&
                game.ActiveMap.UnLoaded != null)
                game.ActiveMap.UnLoaded(game.ActiveMap);

            MainPlayer = mainPlayer;
            this.AddGameObject(MainPlayer);
            game.ActiveMap = this;
            game.Reload();

            if (Loaded != null)
                Loaded(this);
        }

        public override string ToString()
        {
            var content = new StringBuilder();
            content.AppendFormat("<map name=\"{0}\" pixelspermeter=\"{1}\">\n", 
                this.ID, this.PixelsPerMeter);
            foreach (Actor actor in GameObjects)
            {
                if (!(actor is Actors.Player))
                    content.AppendLine(actor.ToString());
            }
            content.AppendLine("</map>");
            return content.ToString();
        }
    }
}
