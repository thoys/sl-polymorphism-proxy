using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;

namespace imaginary_friends
{
    public class AvatarTracking
    {
        public AvatarTracking(string name, UUID id, uint localid, Vector3 position, Quaternion rotation, uint parentid)
        {
            Name = name;
            ID = id;
            LocalID = localid;
            Position = position;
            Rotation = rotation;
            ParentID = parentid;
            LastPlacedPos = Vector3.Zero;
        }

        public string Name;
        public UUID ID;
        public uint LocalID;
        public uint ParentID;
        public Vector3 Position;
        public Vector3 LastPlacedPos;
        public Quaternion Rotation;
    }
}
