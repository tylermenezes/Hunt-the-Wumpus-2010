using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml;
using HuntTheWumpus.GameEngineCore.Editor;

namespace HuntTheWumpus.GameEngineCore
{
    /// <summary>
    /// Handles loading, saving, and managing games and maps
    /// </summary>
    class PlayerProfile
    {
        private XmlDocument savedProfile;
        public bool IsSavedProfileNull() { return savedProfile == null; }
        /// <summary>
        /// Represents the player name and indicates the filename where the game will be saved
        /// </summary>
        public string ID { get; private set; }

        /// <summary>
        /// Gets the active map's name
        /// </summary>
        public string GetCurrentMapID()      
        {
            return (GameEngine.Singleton.GetPlayState()).ActiveMap.ID;
        }

        /// <summary>
        /// Saves the previous map and loads Map from file
        /// </summary>
        public void SetCurrentMap(string mapName, string startupPortal, bool saveBeforeClosing)
        {
            var game = GameEngine.Singleton.GetPlayState();
            if (game.ActiveMap != null && saveBeforeClosing)
                this.SaveCurrentMap();

            XmlNode mapNode = (savedProfile == null) ? null :
                savedProfile.DocumentElement
                .SelectSingleNode(
                String.Format("./map[@name='{0}']", mapName));

            if (mapNode != null)
            {
                new Map(mapName, mapNode).Load(startupPortal);
            }
            else
            {
                bool CanRemoveHack // When Linked Objects Work 
                    // No need to implement linking yourself,
                    // just load linked objects with 
                    // strings ID and PartnerAddress 
                 = false;

                if (CanRemoveHack)
                {
                    Map map = new Map(mapName, FileIO.LoadFromFile().ToArray());
                    map.Load(startupPortal);
                }
                else // Proceed With Super Hack
                {
                    Map map = new Map(mapName, ((GameStates.TheGameState)game).HardCodedMaps[mapName]);
                    map.Load(startupPortal);
                }
            }
        }

        /// <summary>
        ///  Saves the current map
        /// </summary>
        public void SaveCurrentMap()
        {
            // Get Points
            int points = 0;
            foreach (PhysicsWeaponDeployer deployer in PhysicsWeaponInventory.WeaponDeployers)
                points += (int)deployer.EnergyAvailable;
            HighScoreManager.Singleton.RankScore(ID, 
                points);
            //
            XmlNode playerNode;
            XmlNode inventoryNode;
            if (savedProfile == null)
            {
                savedProfile = new XmlDocument();
                savedProfile.LoadXml(
                    String.Format(
                    "<PlayerProfile name=\"{0}\" points=\"{1}\" map=\"{2}\" portal=\"{3}\"></PlayerProfile>",
                    ID, Points, GetCurrentMapID(), GetCurrentPortalID()));

                playerNode = savedProfile.DocumentElement;
                inventoryNode = savedProfile.CreateElement("WeaponsInventory");
            }
            else
            {
                playerNode = savedProfile.DocumentElement;
                playerNode.Attributes.GetNamedItem("points").Value = Points.ToString();

                inventoryNode = playerNode.SelectSingleNode("WeaponsInventory");
                inventoryNode.RemoveAll();
            }


            foreach (PhysicsWeaponDeployer weapon in PhysicsWeaponInventory.WeaponDeployers)
            {
                inventoryNode.AppendChild(
                    savedProfile.CreateElement(weapon.Name));
            }
            playerNode.AppendChild(inventoryNode);

            var game = GameEngine.Singleton.GetPlayState();

            using (var mapReader = XmlTextReader.Create(
                new System.IO.StringReader(game.ActiveMap.ToString())))
            {

                XmlNode mapNode = savedProfile.ReadNode(mapReader);

                var mapName = mapNode.Attributes.GetNamedItem("name");
                foreach (XmlNode node in playerNode.ChildNodes)
                    if (node.NodeType == XmlNodeType.Element &&
                        node.Name == "map" &&
                        node.Attributes.GetNamedItem("name").Value == mapName.Value)
                    {
                        playerNode.ReplaceChild(mapNode, node);
                        break;
                    }
                savedProfile.DocumentElement.AppendChild(mapNode);
                mapReader.Close();
            }
        }

        private object GetCurrentPortalID()
        {
            return
                (GameEngine.Singleton.GetPlayState()).ActiveMap.StartUpPortalID;
        }

        public int Points { get; set;}
        public PhysicsWeaponManager PhysicsWeaponInventory { get; private set; }

        public PlayerProfile(string profileName)
        {
            ID = profileName;
            PhysicsWeaponInventory = new PhysicsWeaponManager();
            Points = 0;
            savedProfile = null;
        }

        public void Load()
        {
            XmlNode playerNode = savedProfile.DocumentElement;
            XmlNode inventoryNode = playerNode.SelectSingleNode("WeaponsInventory");

            PhysicsWeaponInventory = new PhysicsWeaponManager();
            foreach (XmlNode node in inventoryNode.ChildNodes)
                PhysicsWeaponInventory.AddWeaponDeployer(
                    PhysicsWeaponDeployer.CreateFromName(node.Name));

            Points = Int32.Parse(playerNode.Attributes.GetNamedItem("points").Value);
            SetCurrentMap(playerNode.Attributes.GetNamedItem("map").Value,
                playerNode.Attributes.GetNamedItem("portal").Value, true);

        }

        /// <summary>
        /// Loads a saved profile (saved game) from file
        /// </summary>
        public static PlayerProfile LoadFromFile(string profileName)
        {
            var SaveGamesDirectory = (string)Utilities.Globals.Variables["SaveGamesDirectory"];

            var savedGame = new XmlDocument();
            savedGame.LoadXml(
                System.IO.File.ReadAllText(SaveGamesDirectory + profileName + ".wgm"));

            XmlNode playerNode = savedGame.DocumentElement;

            PlayerProfile profile = new PlayerProfile(playerNode.Attributes.GetNamedItem("name").Value);
            profile.savedProfile = savedGame;

            return profile;

        }
        /// <summary>
        /// Saves a profile (saved game) to file
        /// </summary>
        public static void SaveToFile(PlayerProfile profile)
        {
            var SaveGamesDirectory = (string)Utilities.Globals.Variables["SaveGamesDirectory"];
            if (profile.savedProfile != null)
            {
                using (var writer = new XmlTextWriter(SaveGamesDirectory + profile.ID + ".wgm", null))
                {
                    writer.Formatting = Formatting.Indented;
                    profile.savedProfile.WriteTo(writer);
                    writer.Close();
                }
            }
        }
    }
}
