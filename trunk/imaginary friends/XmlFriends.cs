using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.IO;
using OpenMetaverse;

namespace imaginary_friends.XmlFriends
{
    [XmlRoot("XmlFriends"),Serializable()]
    public class ImaginaryConfig
    {
        [Serializable()]
        public class FriendConfig
        {
            public FriendConfig()
            {
            }
            public FriendConfig(ImaginaryFriend iFriend)
            {
                Firstname = iFriend.FirstName;
                Lastname = iFriend.LastName;
                ID = iFriend.ID.ToString();
                localID = iFriend.LocalID;
                PosX = iFriend.Position.X;
                PosY = iFriend.Position.Y;
                PosZ = iFriend.Position.Z;
                RotX = iFriend.Rotation.X;
                RotY = iFriend.Rotation.Y;
                RotZ = iFriend.Rotation.Z;
                RotW = iFriend.Rotation.W;
                male = iFriend.is_male;
                RegionHandle = iFriend.RegionHandle;
            }
            /// <summary>
            /// Converts the FriendConfig to an ImaginaryFriend object
            /// </summary>
            /// <returns>List of friends</returns>
            public ImaginaryFriend GetFriend()
            {
                ImaginaryFriend iTemp = new ImaginaryFriend(Firstname, Lastname, new Vector3(PosX,PosY,PosZ), new Quaternion(RotX,RotY,RotZ,RotW), male, RegionHandle, localID);
                iTemp.ID = new UUID(ID);
                return iTemp;
            }
            [XmlAttribute("ID")]
            public string ID;
            [XmlAttribute("localID")]
            public uint localID;
            [XmlAttribute("Firstname")]
            public string Firstname;
            [XmlAttribute("Lastname")]
            public string Lastname;
            [XmlAttribute("PosX")]
            public float PosX;
            [XmlAttribute("PosY")]
            public float PosY;
            [XmlAttribute("PosZ")]
            public float PosZ;
            [XmlAttribute("RotX")]
            public float RotX;
            [XmlAttribute("RotY")]
            public float RotY;
            [XmlAttribute("RotZ")]
            public float RotZ;
            [XmlAttribute("RotW")]
            public float RotW;
            [XmlAttribute("male")]
            public bool male;
            [XmlAttribute("RegionHandle")]
            public ulong RegionHandle;

        }
        public ImaginaryConfig()
        {
        }
        private List<FriendConfig> imaginaryfriends = new List<FriendConfig>();
        
        /// <summary>
        /// Loads the friends from the XML Config to a List object
        /// </summary>
        /// <returns>List with ImaginaryFriends</returns>
        public List<ImaginaryFriend> Load()
        {
            imaginaryfriends.Clear();
            FileStream stream = null;
            ImaginaryConfig config = null;
            List<ImaginaryFriend> friends = null;
            try
            {
                stream = File.OpenRead("friends.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(ImaginaryConfig));
                config = (ImaginaryConfig)serializer.Deserialize(stream);
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            if (config != null)
            {
                if (config.imaginaryfriends.Count > 0)
                {
                    friends = new List<ImaginaryFriend>();
                    foreach (FriendConfig FC in config.imaginaryfriends)
                    {
                        friends.Add(FC.GetFriend());
                    }
                }
            }
            return friends;
        }
        /// <summary>
        /// Saves The Friends to an XML File
        /// </summary>
        /// <param name="Friends"></param>
        /// <returns>true if save worked</returns>
        public bool Save(Dictionary<UUID,ImaginaryFriend> Friends)
        {
            bool result = true;
            imaginaryfriends.Clear();
            foreach(ImaginaryFriend friend in Friends.Values)
            {                
                imaginaryfriends.Add(new FriendConfig(friend));
            }
            StreamWriter stream = null;
            try
            {
                stream = new StreamWriter("friends.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(ImaginaryConfig));
                serializer.Serialize(stream, this);
            }
            catch(Exception)
            {
                result = false;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }
            return result;
        }
        [XmlElement("Friend")]
        public FriendConfig[] Friends
        {
            get
            {
                FriendConfig[] friends = new FriendConfig[imaginaryfriends.Count];
                imaginaryfriends.CopyTo(friends);
                return friends;
            }
            set
            {
                if (value == null) return;
                imaginaryfriends.Clear();
                FriendConfig[] friends = (FriendConfig[])value;
                foreach (FriendConfig f in friends)
                {
                    imaginaryfriends.Add(f);
                }
                
            }
        }
    }

}
