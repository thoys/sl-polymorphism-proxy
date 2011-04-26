using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;
using OpenMetaverse.Packets;
using Nwc.XmlRpc;
using GridProxy;

namespace imaginary_friends
{
    public class AvatarTracking
    {
        public AvatarTracking(string name,UUID id, uint localid, Vector3 position, uint parentid)
        {
            Name = name;
            ID = id;
            LocalID = localid;
            Position = position;
            ParentID = parentid;
            
        }
        public string Name;
        public UUID ID;
        public uint LocalID;
        public uint ParentID;
        public Vector3 Position;
    }

    public class ImaginaryFriend : Avatar
    {
        public bool is_male = false;
        public uint SittingOn = 0;
        public UUID CurrentAnimID = UUID.Zero;

        AvatarAppearancePacket appearance_packet = null;

        string namevaluesstr = null;

        public override string ToString()
        {
            return ID.ToString();
        }

        public ImaginaryFriend(string firstname, string lastname, Vector3 location, bool male,ulong region_handle,uint localid)
        {
            Position = location;
            Rotation = Quaternion.Identity;
            is_male = male;

            namevaluesstr = string.Format("DisplayName STRING RW DS {0}\nFirstName STRING RW DS {1}\nLastName STRING RW DS {2}\nTitle STRING RW DS {3}\0", firstname + " " + lastname, firstname, lastname, "Imaginairy Friend");

            string[] lines = namevaluesstr.Split('\n');
            NameValues = new NameValue[lines.Length];

            for (int j = 0; j < lines.Length; j++)
            {
                if (!String.IsNullOrEmpty(lines[j]))
                {
                    NameValue nv = new NameValue(lines[j]);
                    NameValues[j] = nv;
                }
            }

            ID = UUID.Random();
            LocalID = localid;
            RegionHandle = region_handle;
        }

        ObjectUpdatePacket DefaultFriendUpdate()
        {
            ObjectUpdatePacket p = new ObjectUpdatePacket();
            p.ObjectData = new ObjectUpdatePacket.ObjectDataBlock[1];
            p.ObjectData[0] = new ObjectUpdatePacket.ObjectDataBlock();
            p.ObjectData[0].ID = LocalID;
            p.ObjectData[0].FullID = ID;
            p.ObjectData[0].ParentID = SittingOn;
            p.ObjectData[0].OwnerID = UUID.Zero;
            p.ObjectData[0].PCode = (byte)PCode.Avatar;
            p.ObjectData[0].PSBlock = new byte[0];
            p.ObjectData[0].ExtraParams = new byte[1];
            p.ObjectData[0].MediaURL = new byte[0];
            p.ObjectData[0].NameValue = new byte[0];
            p.ObjectData[0].Text = new byte[0];
            p.ObjectData[0].TextColor = new byte[4];
            p.ObjectData[0].JointAxisOrAnchor = new Vector3(0, 0, 0);
            p.ObjectData[0].JointPivot = new Vector3(0, 0, 0);
            p.ObjectData[0].Material = 4;
            p.ObjectData[0].TextureAnim = new byte[0];
            p.ObjectData[0].Sound = UUID.Zero;
            TextureEntry ntex = new TextureEntry(new UUID("00000000-0000-0000-5005-000000000005"));
            p.ObjectData[0].TextureEntry = ntex.GetBytes();
            p.ObjectData[0].State = 0;
            p.ObjectData[0].Data = new byte[0];
            p.ObjectData[0].UpdateFlags = 61 + (9 << 8) + (130 << 16) + (16 << 24);
            p.ObjectData[0].PathCurve = 16;
            p.ObjectData[0].ProfileCurve = 1;
            p.ObjectData[0].PathScaleX = 100;
            p.ObjectData[0].PathScaleY = 100;
            p.ObjectData[0].Scale = new Vector3(1, 1, 1);

            p.ObjectData[0].NameValue = Encoding.ASCII.GetBytes(namevaluesstr);



            byte[] objectdata = new byte[76];
            //Collision
            Buffer.BlockCopy(Vector4.Zero.GetBytes(), 0, objectdata, 0, 16);
            if (SittingOn == 0)
            {
                //Position
                Buffer.BlockCopy(Position.GetBytes(), 0, objectdata, 16, 12);

                //Velocity
                Buffer.BlockCopy(Velocity.GetBytes(), 0, objectdata, 28, 12);
                //Acceleration
                Buffer.BlockCopy(Acceleration.GetBytes(), 0, objectdata, 40, 12);
                //Rotation (theta)
                Buffer.BlockCopy(Rotation.GetBytes(), 0, objectdata, 52, 12);
                //Angular Velocity (omega)
                Buffer.BlockCopy(AngularVelocity.GetBytes(), 0, objectdata, 64, 12);
            }
            else
            {
                //Position
                Buffer.BlockCopy(Vector3.Zero.GetBytes(), 0, objectdata, 16, 12);

                //Velocity
                Buffer.BlockCopy(Vector3.Zero.GetBytes(), 0, objectdata, 28, 12);
                //Acceleration
                Buffer.BlockCopy(Vector3.Zero.GetBytes(), 0, objectdata, 40, 12);
                //Rotation (theta)
                Buffer.BlockCopy(Quaternion.Identity.GetBytes(), 0, objectdata, 52, 12);
                //Angular Velocity (omega)
                Buffer.BlockCopy(Vector3.Zero.GetBytes(), 0, objectdata, 64, 12);
            }
            p.ObjectData[0].ObjectData = new byte[76];

            Array.Copy(objectdata, 0, p.ObjectData[0].ObjectData, 0, objectdata.Length);

            p.RegionData.RegionHandle = RegionHandle;
            p.RegionData.TimeDilation = 64096;
            return p;
        }

        public void ImaginaryLogin(Proxy proxy)
        {
            proxy.InjectPacket(DefaultFriendUpdate(), Direction.Incoming);
            ///TODO: FRIEND ONLINE CODE 
        }

        public void UpdateImaginaryObject(Proxy proxy)
        {
            proxy.InjectPacket(DefaultFriendUpdate(), Direction.Incoming);
        }

        public void UpdateImaginaryMovement(Proxy proxy)
        {
            //TODO: MOVEMENT UPDATE CODE
        }

        public void SendAppearance(Proxy proxy)
        {
            if (appearance_packet != null)
            {
                appearance_packet.Sender.ID = ID;
                proxy.InjectPacket(appearance_packet, Direction.Incoming);
            }
            else
            {

                AvatarAppearancePacket p = new AvatarAppearancePacket();
                string stextures = "wijRz0tdS6iE9ImaB5aql4GAAIdWXRFk4sNIWem+q4PVhYPAALEF9g8V3JWZzIYrP5MXdD6gAJABsl1I8A7APhv2NzNqWfuQAFSu8lK1rdMhAXqhKXiOs9aIAJlvtJTwgqyn+4kWOxiqq+GEADI026I0CCd+lDCPeMrD3jOCAIk4by3Sli7/3DaXxODY5dGBADT3SI8gLG8uOHPGuvZmYThAUTyu7udje3xPPeCYvByqzCCJDLlIAYNfYqd15l2n8fYFEBnDc3vKnnH66NtTu4MaRPkIZSLnTRZgTn+2AW9IwWWadwKsCj23E3nJuwNoouECDri6AWyiKtR80sJVYxotyp/tR0kAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                p.ObjectData.TextureEntry = encoding.GetBytes(stextures);
                p.Sender.ID = ID;
                p.Sender.IsTrial = true;
                p.VisualParam = new AvatarAppearancePacket.VisualParamBlock[218];
                for (int i = 0; i < 218; i++)
                {
                    p.VisualParam[i] = new AvatarAppearancePacket.VisualParamBlock();
                }
                p.VisualParam[0].ParamValue = 48; // Arced_Eyebrows
                p.VisualParam[1].ParamValue = 51; // Arm_Length
                p.VisualParam[2].ParamValue = 68; // Attached_Earlobes
                p.VisualParam[3].ParamValue = 23; // Back_Fringe
                p.VisualParam[4].ParamValue = 74; // Baggy_Eyes
                p.VisualParam[5].ParamValue = 86; // Bangs_Part_Middle
                p.VisualParam[6].ParamValue = 63; // Belly_Size
                p.VisualParam[7].ParamValue = 83; // Big_Brow
                p.VisualParam[8].ParamValue = 48; // Big_Ears
                p.VisualParam[9].ParamValue = 42; // Blonde_Hair
                p.VisualParam[10].ParamValue = 0; // Blush
                p.VisualParam[11].ParamValue = 164; // Blush_Color
                p.VisualParam[12].ParamValue = 38; // Blush_Opacity
                p.VisualParam[13].ParamValue = 0; // Body_Definition
                p.VisualParam[14].ParamValue = 83; // Body_Fat
                p.VisualParam[15].ParamValue = 114; // Body_Freckles
                p.VisualParam[16].ParamValue = 158; // Bottom
                p.VisualParam[17].ParamValue = 71; // bottom_length_lower
                p.VisualParam[18].ParamValue = 85; // Bowed_Legs
                p.VisualParam[19].ParamValue = 61; // Breast_Size
                p.VisualParam[20].ParamValue = 5; // Breast_Female_Cleavage
                p.VisualParam[21].ParamValue = 127; // Breast_Gravity
                p.VisualParam[22].ParamValue = 88; // Broad_Nostrils
                p.VisualParam[23].ParamValue = 142; // Bulbous_Nose
                p.VisualParam[24].ParamValue = 0; // Bulbous_Nose_Tip
                p.VisualParam[25].ParamValue = 255; // Butt_Size
                p.VisualParam[26].ParamValue = 63; // Chest_Male_No_Pecs
                p.VisualParam[27].ParamValue = 43; // Chin_Curtains
                p.VisualParam[28].ParamValue = 106; // Cleft_Chin
                p.VisualParam[29].ParamValue = 170; // Cleft_Chin_Upper
                p.VisualParam[30].ParamValue = 145; // Collar_Back
                p.VisualParam[31].ParamValue = 0; // Collar_Back
                p.VisualParam[32].ParamValue = 203; // Collar_Back
                p.VisualParam[33].ParamValue = 255; // Collar_Front
                p.VisualParam[34].ParamValue = 0; // Collar_Front
                p.VisualParam[35].ParamValue = 229; // Collar_Front
                p.VisualParam[36].ParamValue = 0; // Crooked_Nose
                p.VisualParam[37].ParamValue = 25; // Deep_Chin
                p.VisualParam[38].ParamValue = 22; // Double_Chin
                p.VisualParam[39].ParamValue = 0; // Ears_Out
                p.VisualParam[40].ParamValue = 0; // Egg_Head
                p.VisualParam[41].ParamValue = 127; // Eye_Color
                p.VisualParam[42].ParamValue = 0; // Eye_Depth
                p.VisualParam[43].ParamValue = 99; // Eye_Lightness
                p.VisualParam[44].ParamValue = 0; // Eye_Size
                p.VisualParam[45].ParamValue = 0; // Eye_Spacing
                p.VisualParam[46].ParamValue = 114; // Eyebrow_Density
                p.VisualParam[47].ParamValue = 127; // Eyebrow_Size
                p.VisualParam[48].ParamValue = 99; // Eyelashes_Long
                p.VisualParam[49].ParamValue = 63; // Eyelid_Corner_Up
                p.VisualParam[50].ParamValue = 127; // Eyelid_Inner_Corner_Up
                p.VisualParam[51].ParamValue = 140; // Eyeliner
                p.VisualParam[52].ParamValue = 127; // Eyeliner_Color
                p.VisualParam[53].ParamValue = 127; // Face_Shear
                p.VisualParam[54].ParamValue = 0; // Facial_Definition
                p.VisualParam[55].ParamValue = 0; // Foot_Size
                p.VisualParam[56].ParamValue = 0; // Forehead_Angle
                p.VisualParam[57].ParamValue = 191; // Freckles
                p.VisualParam[58].ParamValue = 38; // Front_Fringe
                p.VisualParam[59].ParamValue = 74; // Glove_Fingers
                p.VisualParam[60].ParamValue = 17; // Glove_Length
                p.VisualParam[61].ParamValue = 22; // gloves_blue
                p.VisualParam[62].ParamValue = 0; // gloves_green
                p.VisualParam[63].ParamValue = 0; // gloves_red
                p.VisualParam[64].ParamValue = 0; // Hair_Back
                p.VisualParam[65].ParamValue = 0; // Hair_Front
                p.VisualParam[66].ParamValue = 0; // Hair_Sides
                p.VisualParam[67].ParamValue = 0; // Hair_Sweep
                p.VisualParam[68].ParamValue = 0; // Hair_Thickness
                p.VisualParam[69].ParamValue = 145; // Hair_Tilt
                p.VisualParam[70].ParamValue = 216; // Hair_Volume
                p.VisualParam[71].ParamValue = 133; // Hair_Big_Back
                p.VisualParam[72].ParamValue = 0; // Hair_Big_Front
                p.VisualParam[73].ParamValue = 89; // Hair_Big_Top
                p.VisualParam[74].ParamValue = 0; // Hair_Part_Left
                p.VisualParam[75].ParamValue = 124; // Hair_Part_Middle
                p.VisualParam[76].ParamValue = 137; // Hair_Part_Right
                p.VisualParam[77].ParamValue = 0; // Hair_Rumpled
                p.VisualParam[78].ParamValue = 0; // Hair_Shear_Back
                p.VisualParam[79].ParamValue = 137; // Hair_Shear_Front
                p.VisualParam[80].ParamValue = 127; // Hair_Sides_Full
                p.VisualParam[81].ParamValue = 153; // Hair_Spiked
                p.VisualParam[82].ParamValue = 85; // Hair_Taper_Back
                p.VisualParam[83].ParamValue = 127; // Hair_Taper_Front
                p.VisualParam[84].ParamValue = 127; // Hand_Size
                p.VisualParam[85].ParamValue = 7; // Head_Length
                p.VisualParam[86].ParamValue = 68; // Head_Shape
                p.VisualParam[87].ParamValue = 255; // Head_Size
                p.VisualParam[88].ParamValue = 255; // Heel_Height
                p.VisualParam[89].ParamValue = 255; // Heel_Shape
                p.VisualParam[90].ParamValue = 214; // Height
                p.VisualParam[91].ParamValue = 204; // High_Cheek_Bones
                p.VisualParam[92].ParamValue = 204; // Hip_Length
                p.VisualParam[93].ParamValue = 204; // Hip_Width
                p.VisualParam[94].ParamValue = 51; // In_Shdw_Color
                p.VisualParam[95].ParamValue = 25; // In_Shdw_Opacity
                p.VisualParam[96].ParamValue = 160; // Inner_Shadow
                p.VisualParam[97].ParamValue = 76; // Jacket_Wrinkles
                p.VisualParam[98].ParamValue = 204; // jacket_blue
                p.VisualParam[99].ParamValue = 0; // jacket_green
                p.VisualParam[100].ParamValue = 124; // jacket_red
                p.VisualParam[101].ParamValue = 17; // Jaw_Angle
                p.VisualParam[102].ParamValue = 0; // Jaw_Jut
                p.VisualParam[103].ParamValue = 168; // Jowls
                p.VisualParam[104].ParamValue = 85; // Leg_Length
                p.VisualParam[105].ParamValue = 102; // Leg_Muscles
                p.VisualParam[106].ParamValue = 176; // Leg_Pantflair
                p.VisualParam[107].ParamValue = 216; // Lip_Pinkness
                p.VisualParam[108].ParamValue = 79; // Lip_Ratio
                p.VisualParam[109].ParamValue = 0; // Lip_Thickness
                p.VisualParam[110].ParamValue = 127; // Lip_Width
                p.VisualParam[111].ParamValue = 173; // Lip_Cleft_Deep
                p.VisualParam[112].ParamValue = 127; // Lipgloss
                p.VisualParam[113].ParamValue = 127; // Lipstick
                p.VisualParam[114].ParamValue = 127; // Lipstick_Color
                p.VisualParam[115].ParamValue = 112; // Loose_Lower_Clothing
                p.VisualParam[116].ParamValue = 59; // Loose_Upper_Clothing
                p.VisualParam[117].ParamValue = 68; // Love_Handles
                p.VisualParam[118].ParamValue = 48; // Low_Crotch
                p.VisualParam[119].ParamValue = 127; // Low_Septum_Nose
                p.VisualParam[120].ParamValue = 150; // Lower_Bridge_Nose
                p.VisualParam[121].ParamValue = 97; // Lower_Eyebrows
                p.VisualParam[122].ParamValue = 33; // male
                p.VisualParam[123].ParamValue = 79; // Male_Package
                p.VisualParam[124].ParamValue = 107; // Moustache
                p.VisualParam[125].ParamValue = 229; // Mouth_Corner
                p.VisualParam[126].ParamValue = 145; // Mouth_Height
                p.VisualParam[127].ParamValue = 186; // Nail_Polish
                p.VisualParam[128].ParamValue = 153; // Nail_Polish_Color
                p.VisualParam[129].ParamValue = 61; // Neck_Length
                p.VisualParam[130].ParamValue = 43; // Neck_Thickness
                p.VisualParam[131].ParamValue = 45; // Noble_Nose_Bridge
                p.VisualParam[132].ParamValue = 127; // Nose_Big_Out
                p.VisualParam[133].ParamValue = 127; // open_jacket
                p.VisualParam[134].ParamValue = 0; // Out_Shdw_Color
                p.VisualParam[135].ParamValue = 0; // Out_Shdw_Opacity
                p.VisualParam[136].ParamValue = 89; // Outer_Shadow
                p.VisualParam[137].ParamValue = 0; // Pants_Length
                p.VisualParam[138].ParamValue = 127; // Pants_Length
                p.VisualParam[139].ParamValue = 10; // Pants_Waist
                p.VisualParam[140].ParamValue = 121; // Pants_Wrinkles
                p.VisualParam[141].ParamValue = 255; // pants_blue
                p.VisualParam[142].ParamValue = 0; // pants_green
                p.VisualParam[143].ParamValue = 0; // pants_red
                p.VisualParam[144].ParamValue = 127; // Pigment
                p.VisualParam[145].ParamValue = 61; // Pigtails
                p.VisualParam[146].ParamValue = 85; // Platform_Height
                p.VisualParam[147].ParamValue = 131; // Pointy_Ears
                p.VisualParam[148].ParamValue = 119; // Pointy_Eyebrows
                p.VisualParam[149].ParamValue = 0; // Ponytail
                p.VisualParam[150].ParamValue = 145; // Pop_Eye
                p.VisualParam[151].ParamValue = 122; // Puffy_Lower_Lids
                p.VisualParam[152].ParamValue = 160; // Puffy_Upper_Cheeks
                p.VisualParam[153].ParamValue = 0; // Rainbow_Color
                p.VisualParam[154].ParamValue = 0; // Rainbow_Color
                p.VisualParam[155].ParamValue = 160; // Red_Hair
                p.VisualParam[156].ParamValue = 0; // Red_Skin
                p.VisualParam[157].ParamValue = 112; // Rosy_Complexion
                p.VisualParam[158].ParamValue = 127; // Saddlebags
                p.VisualParam[159].ParamValue = 0; // Shift_Mouth
                p.VisualParam[160].ParamValue = 214; // Shirt_Bottom
                p.VisualParam[161].ParamValue = 204; // Shirt_Wrinkles
                p.VisualParam[162].ParamValue = 255; // shirt_blue
                p.VisualParam[163].ParamValue = 0; // shirt_green
                p.VisualParam[164].ParamValue = 0; // shirt_red
                p.VisualParam[165].ParamValue = 127; // Shirtsleeve_flair
                p.VisualParam[166].ParamValue = 33; // Shoe_Height
                p.VisualParam[167].ParamValue = 127; // Shoe_Platform_Width
                p.VisualParam[168].ParamValue = 150; // Shoe_Toe_Thick
                p.VisualParam[169].ParamValue = 255; // shoes_blue
                p.VisualParam[170].ParamValue = 255; // shoes_green
                p.VisualParam[171].ParamValue = 255; // shoes_red
                p.VisualParam[172].ParamValue = 255; // Shoulders
                p.VisualParam[173].ParamValue = 255; // Side_Fringe
                p.VisualParam[174].ParamValue = 255; // Sideburns
                p.VisualParam[175].ParamValue = 255; // Skirt_Length
                p.VisualParam[176].ParamValue = 255; // skirt_blue
                p.VisualParam[177].ParamValue = 0; // skirt_bustle
                p.VisualParam[178].ParamValue = 0; // skirt_green
                p.VisualParam[179].ParamValue = 255; // skirt_looseness
                p.VisualParam[180].ParamValue = 137; // skirt_red
                p.VisualParam[181].ParamValue = 0; // Sleeve_Length
                p.VisualParam[182].ParamValue = 0; // Sleeve_Length
                p.VisualParam[183].ParamValue = 0; // Sleeve_Length
                p.VisualParam[184].ParamValue = 0; // Slit_Back
                p.VisualParam[185].ParamValue = 0; // Slit_Front
                p.VisualParam[186].ParamValue = 255; // Slit_Left
                p.VisualParam[187].ParamValue = 255; // Slit_Right
                p.VisualParam[188].ParamValue = 255; // Socks_Length
                p.VisualParam[189].ParamValue = 255; // socks_blue
                p.VisualParam[190].ParamValue = 255; // socks_green
                p.VisualParam[191].ParamValue = 255; // socks_red
                p.VisualParam[192].ParamValue = 0; // Soulpatch
                p.VisualParam[193].ParamValue = 0; // Square_Jaw
                p.VisualParam[194].ParamValue = 0; // Squash_Stretch_Head
                p.VisualParam[195].ParamValue = 0; // Sunken_Cheeks
                p.VisualParam[196].ParamValue = 255; // Tall_Lips
                p.VisualParam[197].ParamValue = 255; // Thickness
                p.VisualParam[198].ParamValue = 255; // Toe_Shape
                p.VisualParam[199].ParamValue = 0; // Torso_Length
                p.VisualParam[200].ParamValue = 132; // Torso_Muscles
                p.VisualParam[201].ParamValue = 17; // Torso_Muscles
                p.VisualParam[202].ParamValue = 255; // underpants_blue
                p.VisualParam[203].ParamValue = 25; // underpants_green
                p.VisualParam[204].ParamValue = 100; // underpants_red
                p.VisualParam[205].ParamValue = 255; // undershirt_blue
                p.VisualParam[206].ParamValue = 255; // undershirt_green
                p.VisualParam[207].ParamValue = 255; // undershirt_red
                p.VisualParam[208].ParamValue = 255; // Upper_Eyelid_Fold
                p.VisualParam[209].ParamValue = 84; // Upturned_Nose_Tip
                p.VisualParam[210].ParamValue = 0; // Waist_Height
                p.VisualParam[211].ParamValue = 0; // Weak_Chin
                p.VisualParam[212].ParamValue = 0; // White_Hair
                p.VisualParam[213].ParamValue = 51; // Wide_Eyes
                p.VisualParam[214].ParamValue = 107; // Wide_Lip_Cleft
                p.VisualParam[215].ParamValue = 255; // Wide_Nose
                p.VisualParam[216].ParamValue = 255; // Wide_Nose_Bridge
                p.VisualParam[217].ParamValue = 255; // wrinkles

                proxy.InjectPacket(p, Direction.Incoming);
            }
        }

        public void SendAnimations(Proxy proxy,List<UUID> Animations)
        {
            if (Animations.Count == 0)
            {
                CurrentAnimID = UUID.Zero;
            }
            else
            {
                CurrentAnimID = Animations[0];
            }
            AvatarAnimationPacket p = new AvatarAnimationPacket();

            p.AnimationList = new AvatarAnimationPacket.AnimationListBlock[Animations.Count];
            p.AnimationSourceList = new AvatarAnimationPacket.AnimationSourceListBlock[Animations.Count];

            int i = 0;
            foreach (UUID id in Animations)
            {
                p.AnimationList[i] = new AvatarAnimationPacket.AnimationListBlock();
                p.AnimationList[i].AnimID = id;
                p.AnimationList[i].AnimSequenceID = 1;
                p.AnimationSourceList[i] = new AvatarAnimationPacket.AnimationSourceListBlock();
                p.AnimationSourceList[i].ObjectID = ID;
                i++;
            }
            p.Sender = new AvatarAnimationPacket.SenderBlock();
            p.Sender.ID = ID;
            proxy.InjectPacket(p, Direction.Incoming);
        }

        public void ImaginaryKill(Proxy proxy)
        {
            KillObjectPacket p = new KillObjectPacket();
            p.ObjectData = new KillObjectPacket.ObjectDataBlock[1];
            p.ObjectData[0] = new KillObjectPacket.ObjectDataBlock();
            p.ObjectData[0].ID = LocalID;
            proxy.InjectPacket(p, Direction.Incoming);
        }
    }

    public class ImaginaryFriends
    {
        Proxy proxy;
        UUID agentID;
        UUID sessionID;
        Vector3 Position;
        ulong RegionHandle=0;

        Dictionary<uint, AvatarTracking> AvatarTracker = new Dictionary<uint, AvatarTracking>();

        Dictionary<UUID, AvatarAppearancePacket> appearances = new Dictionary<UUID, AvatarAppearancePacket>();

        bool FourDimensions = false;
        UUID lastselectedobj = UUID.Zero;
        uint LocalID = 0;
        uint localidcount = 13371337;
        Dictionary<ulong, Dictionary<uint,UUID>> GlobalLocalIDs = new Dictionary<ulong, Dictionary<uint,UUID>>();
        XmlFriends.ImaginaryConfig Config = new imaginary_friends.XmlFriends.ImaginaryConfig();
        List<ImaginaryFriend> IFriends = new List<ImaginaryFriend>();
        
        public ImaginaryFriends()
        {
            proxy = new Proxy(new ProxyConfig("imaginaryfriends", "fw0rdiscrazy-crew"));
            proxy.AddDelegate(PacketType.ChatFromViewer, Direction.Outgoing, new PacketDelegate(OnOutgoingChat));
            proxy.AddDelegate(PacketType.ObjectUpdate, Direction.Incoming, new PacketDelegate(OnIncommingObjectUpdate));
            proxy.AddDelegate(PacketType.ImprovedTerseObjectUpdate, Direction.Incoming, new PacketDelegate(OnImprovedTerseObjectUpdate));
            proxy.AddDelegate(PacketType.ImprovedInstantMessage, Direction.Outgoing, new PacketDelegate(OnOutgoingImprovedInstantMessage));
            proxy.AddDelegate(PacketType.KillObject, Direction.Incoming, new PacketDelegate(OnIncommingKillObject));
            proxy.AddDelegate(PacketType.AgentMovementComplete, Direction.Incoming, new PacketDelegate(IncommingMovementComplete));
            proxy.AddDelegate(PacketType.ViewerEffect, Direction.Outgoing, new PacketDelegate(OutgoingViewerEffect));
            proxy.AddDelegate(PacketType.AvatarAppearance, Direction.Incoming, new PacketDelegate(OnIncommingApearance));
            proxy.AddLoginResponseDelegate(new XmlRpcResponseDelegate(LoginResponse));
            proxy.Start();
        }

        Packet OnIncommingApearance(Packet packet, System.Net.IPEndPoint endp)
        {
            AvatarAppearancePacket set = (AvatarAppearancePacket)packet;
            appearances[set.Sender.ID] = set;
            return packet;
        }

        private void LoginResponse(XmlRpcResponse response)
        {
            Hashtable values = (Hashtable)response.Value;
            if (values.Contains("agent_id"))
                agentID = new UUID((string)values["agent_id"]);
            if (values.Contains("session_id"))
                sessionID = new UUID((string)values["session_id"]);
        }

        Packet OnOutgoingImprovedInstantMessage(Packet packet, System.Net.IPEndPoint endp)
        {
            ImprovedInstantMessagePacket p = (ImprovedInstantMessagePacket)packet;
            foreach(ImaginaryFriend f in IFriends)
            {
                if (p.MessageBlock.ToAgentID == f.ID)
                {
                    //do stuff here
                    switch (Utils.BytesToString(p.MessageBlock.Message).ToLower())
                    {
                        case "kill":
                            f.ImaginaryKill(proxy);
                            break;
                        case "sithere":
                            lock (GlobalLocalIDs)
                            {
                                if (GlobalLocalIDs.ContainsKey(RegionHandle))
                                {
                                    if (GlobalLocalIDs[RegionHandle].ContainsValue(lastselectedobj))
                                    {
                                        uint local = 0;
                                        foreach (uint id in GlobalLocalIDs[RegionHandle].Keys)
                                        {
                                            if (GlobalLocalIDs[RegionHandle][id] == lastselectedobj)
                                            {
                                                local = id;
                                                break;
                                            }
                                        }
                                        f.SittingOn = local;
                                        f.UpdateImaginaryObject(proxy);
                                    }
                                }
                            }
                            break;
                        case "come":
                            f.SittingOn = 0; //get rid of the parent
                            f.Position = Position;
                            if (RegionHandle != f.RegionHandle)
                            {
                                f.ImaginaryKill(proxy);
                                f.RegionHandle = RegionHandle;
                                f.ImaginaryLogin(proxy);
                            }
                            else
                            {
                                f.UpdateImaginaryObject(proxy);
                            }
                            break;

                    }
                    return null;
                }
            }
            return packet;
        }

        Packet OnImprovedTerseObjectUpdate(Packet packet, System.Net.IPEndPoint endp)
        {
            ImprovedTerseObjectUpdatePacket terse = (ImprovedTerseObjectUpdatePacket)packet;

            if(terse.RegionData.RegionHandle == RegionHandle)
            {
                for (int i = 0; i < terse.ObjectData.Length; i++ )
                {
                    uint tid = Utils.BytesToUInt(terse.ObjectData[i].Data, 0);// == LocalID)
                    Vector3 tpos = new Vector3(terse.ObjectData[i].Data, 22);
                    if (tid == LocalID)
                    {
                        Position = tpos;
                    }
                    if (AvatarTracker.ContainsKey(tid))
                    {
                        if (AvatarTracker[tid].Position != tpos)
                        {
                            if (FourDimensions)
                            {
                                //new ImaginaryFriend(
                                //lets create a new friendpoint
                                ImaginaryFriend temp = new ImaginaryFriend(AvatarTracker[tid].Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], AvatarTracker[tid].Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] + " time = " + System.DateTime.Now.ToString(), AvatarTracker[tid].Position, true, RegionHandle, localidcount++);
                                IFriends.Add(temp);
                                temp.ImaginaryLogin(proxy);
                                temp.SendAppearance(proxy);
                            }
                            AvatarTracker[tid].Position = tpos;
                        }
                    }
                }
            }
            return packet;
        }

        Packet OnIncommingKillObject(Packet packet, System.Net.IPEndPoint endp)
        {
            KillObjectPacket ko = (KillObjectPacket)packet;
            
            foreach (KillObjectPacket.ObjectDataBlock b in ko.ObjectData)
            {
                if(GlobalLocalIDs.ContainsKey(RegionHandle))
                {
                    if(GlobalLocalIDs[RegionHandle].ContainsKey(b.ID))
                    {
                        GlobalLocalIDs.Remove(b.ID);
                    }
                }
                if (AvatarTracker.ContainsKey(b.ID))
                {
                    AvatarTracker.Remove(b.ID);
                }
            }
            return packet;
        }

        Packet OnIncommingObjectUpdate(Packet packet, System.Net.IPEndPoint endp)
        {
            ObjectUpdatePacket ou = (ObjectUpdatePacket)packet;
            foreach (ObjectUpdatePacket.ObjectDataBlock b in ou.ObjectData)
            {
                Vector3 lpos = Vector3.Zero;
                switch (b.ObjectData.Length)
                {
                    case 76:
                        lpos = new Vector3(b.ObjectData, 16);
                        break;
                    case 60:
                        lpos = new Vector3(b.ObjectData, 0);
                        break;
                    case 48:
                        lpos = new Vector3(
                            Utils.UInt16ToFloat(b.ObjectData, 16, -0.5f * 256.0f, 1.5f * 256.0f),
                            Utils.UInt16ToFloat(b.ObjectData, 18, -0.5f * 256.0f, 1.5f * 256.0f),
                            Utils.UInt16ToFloat(b.ObjectData, 20, -256.0f, 3.0f * 256.0f));
                        break;
                    case 32:
                        lpos = new Vector3(
                            Utils.UInt16ToFloat(b.ObjectData, 0, -0.5f * 256.0f, 1.5f * 256.0f),
                            Utils.UInt16ToFloat(b.ObjectData, 2, -0.5f * 256.0f, 1.5f * 256.0f),
                            Utils.UInt16ToFloat(b.ObjectData, 4, -256.0f, 3.0f * 256.0f));
                        break;
                    case 16:
                        lpos = new Vector3(
                            Utils.ByteToFloat(b.ObjectData, 0, -256.0f, 256.0f),
                            Utils.ByteToFloat(b.ObjectData, 1, -256.0f, 256.0f),
                            Utils.ByteToFloat(b.ObjectData, 2, -256.0f, 256.0f));
                        break;
                    // OR not :)))
                    //default: break; // Position is weird, GIVE UP! lets break! :)
                }
                if (b.PCode == 47) // 47 == AVATAR
                {
                    if (AvatarTracker.ContainsKey(b.ID))
                    {
                        //check for movement
                        if (AvatarTracker[b.ID].Position != lpos && FourDimensions)
                        {
                            ImaginaryFriend temp = new ImaginaryFriend(AvatarTracker[b.ID].Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], AvatarTracker[b.ID].Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] + " " + System.DateTime.Now.ToString(), AvatarTracker[b.ID].Position, true, RegionHandle, localidcount++);

                            IFriends.Add(temp);
                            temp.SittingOn = b.ParentID; //set sitting on
                            temp.ImaginaryLogin(proxy);

                           // Console.WriteLine("Create new av here");
                        }

                        AvatarTracker[b.ID].Position = lpos;
                        AvatarTracker[b.ID].ParentID = b.ParentID; // sitting?
                    }
                    else
                    {
                        string[] lines = Utils.BytesToString(b.NameValue).Split(new char[] { '\n' });
                        string tfirstname = "no";
                        string tlastname = "name";
                        for (int i = 0; i < lines.Length; i++)
                        {
                            if (lines[i].Length > 0)
                            {
                                string[] splitlines =  lines[i].Split(new char[] { ' ', '\n', '\t', '\r' });
                                if (splitlines.Length > 3)
                                {
                                    if(splitlines[0].ToLower() == "firstname")
                                    {
                                        tfirstname = lines[i].Remove(0, lines[i].IndexOf(' ', lines[i].IndexOf("DS")));
                                    }
                                    if (splitlines[0].ToLower() == "lastname")
                                    {
                                        tlastname = lines[i].Remove(0, lines[i].IndexOf(' ', lines[i].IndexOf("DS")));
                                    }
                                }
                            }
                        }
                        AvatarTracker[b.ID] = new AvatarTracking(tfirstname+" "+tlastname,b.FullID, b.ID, lpos, b.ParentID);
                    }
                }
                if (b.FullID == agentID)
                {
                    RegionHandle = ou.RegionData.RegionHandle;
                    LocalID = b.ID;
                    Position = lpos;
                }
                if (b.PCode == (byte)PCode.Prim)
                {
                    if(!GlobalLocalIDs.ContainsKey(RegionHandle))
                        GlobalLocalIDs[RegionHandle] = new Dictionary<uint,UUID>();

                    GlobalLocalIDs[RegionHandle][b.ID] = b.FullID;
                }

            }
            
            return packet;
        }

        Packet IncommingMovementComplete(Packet packet, System.Net.IPEndPoint ip)
        {
            AgentMovementCompletePacket p = (AgentMovementCompletePacket)packet;
            RegionHandle = p.Data.RegionHandle;
            Position = p.Data.Position;
            return packet;
        }
        Packet OutgoingViewerEffect(Packet packet, System.Net.IPEndPoint ip)
        {
            ViewerEffectPacket vep = (ViewerEffectPacket)packet;
            foreach (ViewerEffectPacket.EffectBlock eb in vep.Effect)
            {
                if (eb.TypeData.Length == 57)
                {
                    if (eb.Type == (byte)EffectType.PointAt)
                    {
                        UUID temp = new UUID(eb.TypeData, 16);//eb.ID;

                        if (temp != UUID.Zero)
                        {
                            lastselectedobj = temp;
                        }
                    }
                }
            }
            return packet;
        }

        Packet OnOutgoingChat(Packet packet, System.Net.IPEndPoint endp)
        {
            ChatFromViewerPacket p = (ChatFromViewerPacket)packet;
            string set = Utils.BytesToString(p.ChatData.Message);
            if (set.ToLower().StartsWith("/makefriend"))
            {
                string [] split = set.Split(new char[]{' '});
                if(split.Length > 3)
                {
                    bool male = false;
                    if (split[1].ToLower() == "male")
                    {
                        male = true;
                    }
                    else if(split[1].ToLower() != "female")
                    {
                        return null;
                    }
                    ImaginaryFriend IF = new ImaginaryFriend(split[2], combineArguments(split, 3), Position , male, RegionHandle,localidcount++);
                    IF.ImaginaryLogin(proxy);

                    IFriends.Add(IF);
                }
                return null;
            }
            else if (set.ToLower().StartsWith("/killall"))
            {
                foreach (ImaginaryFriend f in IFriends)
                {
                    f.ImaginaryKill(proxy);
                }
                return null;
            }
            else if (p.ChatData.Channel == 4 && set.ToLower().StartsWith("d"))
            {
                string [] fdsplit = set.ToLower().Split(new char[]{' '});
                if (fdsplit.Length == 2)
                {
                    FourDimensions = (fdsplit[1] == "on");
                    SayToUser("4d mode is now " + FourDimensions.ToString());
                }
                else
                {
                    SayToUser("usage: /4d [on | off]");
                }
                return null;
            }
            else if (set.ToLower().StartsWith("/animateall"))
            {

                /*temporairy rubbish down here
                 * 
                 * Type type = Type.GetType("libsecondlife.Animations");
                System.Reflection.MemberInfo[] info = type.GetMembers();
                string result = "";
                foreach (System.Reflection.MemberInfo i in info)
                {
                    result += i.Name + " " + i.ToString() + "\n";
                }
                SayToUser(result);
                 */
                string output = set.ToLower().Remove(0, 11).Trim();
                UUID animation = UUID.Zero;
                if(UUID.TryParse(output,out animation))
                {
                    
                    List<UUID> animations = new List<UUID>();
                    animations.Add(animation);
                    foreach (ImaginaryFriend f in IFriends)
                    {
                        f.SendAnimations(proxy, animations);
                        f.SendAppearance(proxy);
                    }
                }
                return null;
            }
            else if (set.ToLower().StartsWith("/savefriends"))
            {
                if (Config.Save(IFriends))
                {
                    SayToUser("Saving successful");
                }
                else
                {
                    SayToUser("Saving Failed");
                }
                return null;
            }
            else if (set.ToLower().StartsWith("/loadfriends"))
            {
                List<ImaginaryFriend> IFTempLoad = Config.Load();
                if (IFTempLoad != null)
                {
                    foreach (ImaginaryFriend tempfriend in IFTempLoad)
                    {
                        for (int i = IFriends.Count-1; i >= 0 ; i--)
                        {
                            if (IFriends[i].ID == tempfriend.ID)
                            {
                                IFriends[i].ImaginaryKill(proxy);
                                IFriends.Remove(IFriends[i]);
                                break;
                            }
                            
                        }
                        IFriends.Add(tempfriend);
                        tempfriend.ImaginaryLogin(proxy);
                    }
                    SayToUser("Loaded successful");
                }
                else
                {
                    SayToUser("Loading Failed");
                }
                return null;
            }
            return packet;
        }

        string combineArguments(string[] arguments, int index)
        {
            string output = "";
            for (int i = index; i < arguments.Length; i++)
            {
                output += arguments[i] + " ";
            }
            return output.Trim();
        }

        public void SayToUser(string message)
        {
            ChatFromSimulatorPacket packet = new ChatFromSimulatorPacket();
            packet.ChatData.FromName = Utils.StringToBytes("ImaginaryBrain");
            packet.ChatData.SourceID = UUID.Random();
            packet.ChatData.OwnerID = agentID;
            packet.ChatData.SourceType = (byte)2;
            packet.ChatData.ChatType = (byte)1;
            packet.ChatData.Audible = (byte)1;
            packet.ChatData.Position = new Vector3(0, 0, 0);
            packet.ChatData.Message = Utils.StringToBytes(message);
            proxy.InjectPacket(packet, Direction.Incoming);
        }
    }
}
