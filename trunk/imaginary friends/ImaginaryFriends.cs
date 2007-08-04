using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using libsecondlife;
using libsecondlife.Packets;
using Nwc.XmlRpc;

namespace imaginary_friends
{
    public class ImaginaryFriend : Avatar
    {
        ulong regionhandle = 0;
        //uint LocalID = 0;
        bool is_male = false;
        string fname, lname;
        public ImaginaryFriend(string firstname, string lastname, LLVector3 location, bool male,ulong region_handle,uint localid)
        {
            Position = location;
            Rotation = LLQuaternion.Identity;
            is_male = male;
            fname = firstname;
            lname = lastname;
            ID = LLUUID.Random();
            LocalID = localid;
            regionhandle = region_handle;
        }
        public void ImaginaryLogin(Proxy proxy)
        {
            ObjectUpdatePacket p = new ObjectUpdatePacket();
            p.ObjectData = new ObjectUpdatePacket.ObjectDataBlock[1];
            p.ObjectData[0] = new ObjectUpdatePacket.ObjectDataBlock();
            p.ObjectData[0].ID = LocalID;
            p.ObjectData[0].FullID = ID;
            p.ObjectData[0].OwnerID = LLUUID.Zero;
            p.ObjectData[0].PCode = (byte)ObjectManager.PCode.Avatar;
            p.ObjectData[0].PSBlock = new byte[0];
            p.ObjectData[0].ExtraParams = new byte[1];
            p.ObjectData[0].MediaURL = new byte[0];
            p.ObjectData[0].NameValue = new byte[0];
            p.ObjectData[0].Text = new byte[0];
            p.ObjectData[0].TextColor = new byte[4];
            p.ObjectData[0].JointAxisOrAnchor = new LLVector3(0, 0, 0);
            p.ObjectData[0].JointPivot = new LLVector3(0, 0, 0);
            p.ObjectData[0].Material = 4;
            p.ObjectData[0].TextureAnim = new byte[0];
            p.ObjectData[0].Sound = LLUUID.Zero;
            LLObject.TextureEntry ntex = new LLObject.TextureEntry(new LLUUID("00000000-0000-0000-5005-000000000005"));
            p.ObjectData[0].TextureEntry = ntex.ToBytes();
            p.ObjectData[0].State = 0;
            p.ObjectData[0].Data = new byte[0];
            p.ObjectData[0].UpdateFlags = 61 + (9 << 8) + (130 << 16) + (16 << 24);
            p.ObjectData[0].PathCurve = 16;
            p.ObjectData[0].ProfileCurve = 1;
            p.ObjectData[0].PathScaleX = 100;
            p.ObjectData[0].PathScaleY = 100;
            p.ObjectData[0].Scale = new LLVector3(1, 1, 1);
            p.ObjectData[0].NameValue = Encoding.ASCII.GetBytes("FirstName STRING RW SV " + fname + " \nLastName STRING RW SV " + lname + " \0");
            byte[] objectdata = new byte[76];
            //Collision
            Buffer.BlockCopy(LLVector4.Zero.GetBytes(), 0, objectdata, 0, 16);
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

            p.ObjectData[0].ObjectData = new byte[76];

            Array.Copy(objectdata, 0, p.ObjectData[0].ObjectData, 0, objectdata.Length);
           
            p.RegionData.RegionHandle = regionhandle;
            p.RegionData.TimeDilation = 64096;

            proxy.InjectPacket(p, Direction.Incoming);

            
            
            
        }
        public void SendAppearance(Proxy proxy)
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
            p.VisualParam[38 - 1].ParamValue = 25; // Deep_Chin
            p.VisualParam[39 - 1].ParamValue = 22; // Double_Chin
            p.VisualParam[40 - 1].ParamValue = 0; // Ears_Out
            p.VisualParam[41 - 1].ParamValue = 0; // Egg_Head
            p.VisualParam[42 - 1].ParamValue = 127; // Eye_Color
            p.VisualParam[43 - 1].ParamValue = 0; // Eye_Depth
            p.VisualParam[44 - 1].ParamValue = 99; // Eye_Lightness
            p.VisualParam[45 - 1].ParamValue = 0; // Eye_Size
            p.VisualParam[46 - 1].ParamValue = 0; // Eye_Spacing
            p.VisualParam[47 - 1].ParamValue = 114; // Eyebrow_Density
            p.VisualParam[48 - 1].ParamValue = 127; // Eyebrow_Size
            p.VisualParam[49 - 1].ParamValue = 99; // Eyelashes_Long
            p.VisualParam[50 - 1].ParamValue = 63; // Eyelid_Corner_Up
            p.VisualParam[51 - 1].ParamValue = 127; // Eyelid_Inner_Corner_Up
            p.VisualParam[52 - 1].ParamValue = 140; // Eyeliner
            p.VisualParam[53 - 1].ParamValue = 127; // Eyeliner_Color
            p.VisualParam[54 - 1].ParamValue = 127; // Face_Shear
            p.VisualParam[55 - 1].ParamValue = 0; // Facial_Definition
            p.VisualParam[56 - 1].ParamValue = 0; // Foot_Size
            p.VisualParam[57 - 1].ParamValue = 0; // Forehead_Angle
            p.VisualParam[58 - 1].ParamValue = 191; // Freckles
            p.VisualParam[59 - 1].ParamValue = 38; // Front_Fringe
            p.VisualParam[60 - 1].ParamValue = 74; // Glove_Fingers
            p.VisualParam[61 - 1].ParamValue = 17; // Glove_Length
            p.VisualParam[62 - 1].ParamValue = 22; // gloves_blue
            p.VisualParam[63 - 1].ParamValue = 0; // gloves_green
            p.VisualParam[64 - 1].ParamValue = 0; // gloves_red
            p.VisualParam[65 - 1].ParamValue = 0; // Hair_Back
            p.VisualParam[66 - 1].ParamValue = 0; // Hair_Front
            p.VisualParam[67 - 1].ParamValue = 0; // Hair_Sides
            p.VisualParam[68 - 1].ParamValue = 0; // Hair_Sweep
            p.VisualParam[69 - 1].ParamValue = 0; // Hair_Thickness
            p.VisualParam[70 - 1].ParamValue = 145; // Hair_Tilt
            p.VisualParam[71 - 1].ParamValue = 216; // Hair_Volume
            p.VisualParam[72 - 1].ParamValue = 133; // Hair_Big_Back
            p.VisualParam[73 - 1].ParamValue = 0; // Hair_Big_Front
            p.VisualParam[74 - 1].ParamValue = 89; // Hair_Big_Top
            p.VisualParam[75 - 1].ParamValue = 0; // Hair_Part_Left
            p.VisualParam[76 - 1].ParamValue = 124; // Hair_Part_Middle
            p.VisualParam[77 - 1].ParamValue = 137; // Hair_Part_Right
            p.VisualParam[78 - 1].ParamValue = 0; // Hair_Rumpled
            p.VisualParam[79 - 1].ParamValue = 0; // Hair_Shear_Back
            p.VisualParam[80 - 1].ParamValue = 137; // Hair_Shear_Front
            p.VisualParam[81 - 1].ParamValue = 127; // Hair_Sides_Full
            p.VisualParam[82 - 1].ParamValue = 153; // Hair_Spiked
            p.VisualParam[83 - 1].ParamValue = 85; // Hair_Taper_Back
            p.VisualParam[84 - 1].ParamValue = 127; // Hair_Taper_Front
            p.VisualParam[85 - 1].ParamValue = 127; // Hand_Size
            p.VisualParam[86 - 1].ParamValue = 7; // Head_Length
            p.VisualParam[87 - 1].ParamValue = 68; // Head_Shape
            p.VisualParam[88 - 1].ParamValue = 255; // Head_Size
            p.VisualParam[89 - 1].ParamValue = 255; // Heel_Height
            p.VisualParam[90 - 1].ParamValue = 255; // Heel_Shape
            p.VisualParam[91 - 1].ParamValue = 214; // Height
            p.VisualParam[92 - 1].ParamValue = 204; // High_Cheek_Bones
            p.VisualParam[93 - 1].ParamValue = 204; // Hip_Length
            p.VisualParam[94 - 1].ParamValue = 204; // Hip_Width
            p.VisualParam[95 - 1].ParamValue = 51; // In_Shdw_Color
            p.VisualParam[96 - 1].ParamValue = 25; // In_Shdw_Opacity
            p.VisualParam[97 - 1].ParamValue = 160; // Inner_Shadow
            p.VisualParam[98 - 1].ParamValue = 76; // Jacket_Wrinkles
            p.VisualParam[99 - 1].ParamValue = 204; // jacket_blue
            p.VisualParam[100 - 1].ParamValue = 0; // jacket_green
            p.VisualParam[101 - 1].ParamValue = 124; // jacket_red
            p.VisualParam[102 - 1].ParamValue = 17; // Jaw_Angle
            p.VisualParam[103 - 1].ParamValue = 0; // Jaw_Jut
            p.VisualParam[104 - 1].ParamValue = 168; // Jowls
            p.VisualParam[105 - 1].ParamValue = 85; // Leg_Length
            p.VisualParam[106 - 1].ParamValue = 102; // Leg_Muscles
            p.VisualParam[107 - 1].ParamValue = 176; // Leg_Pantflair
            p.VisualParam[108 - 1].ParamValue = 216; // Lip_Pinkness
            p.VisualParam[109 - 1].ParamValue = 79; // Lip_Ratio
            p.VisualParam[110 - 1].ParamValue = 0; // Lip_Thickness
            p.VisualParam[111 - 1].ParamValue = 127; // Lip_Width
            p.VisualParam[112 - 1].ParamValue = 173; // Lip_Cleft_Deep
            p.VisualParam[113 - 1].ParamValue = 127; // Lipgloss
            p.VisualParam[114 - 1].ParamValue = 127; // Lipstick
            p.VisualParam[115 - 1].ParamValue = 127; // Lipstick_Color
            p.VisualParam[116 - 1].ParamValue = 112; // Loose_Lower_Clothing
            p.VisualParam[117 - 1].ParamValue = 59; // Loose_Upper_Clothing
            p.VisualParam[118 - 1].ParamValue = 68; // Love_Handles
            p.VisualParam[119 - 1].ParamValue = 48; // Low_Crotch
            p.VisualParam[120 - 1].ParamValue = 127; // Low_Septum_Nose
            p.VisualParam[121 - 1].ParamValue = 150; // Lower_Bridge_Nose
            p.VisualParam[122 - 1].ParamValue = 97; // Lower_Eyebrows
            p.VisualParam[123 - 1].ParamValue = 33; // male
            p.VisualParam[124 - 1].ParamValue = 79; // Male_Package
            p.VisualParam[125 - 1].ParamValue = 107; // Moustache
            p.VisualParam[126 - 1].ParamValue = 229; // Mouth_Corner
            p.VisualParam[127 - 1].ParamValue = 145; // Mouth_Height
            p.VisualParam[128 - 1].ParamValue = 186; // Nail_Polish
            p.VisualParam[129 - 1].ParamValue = 153; // Nail_Polish_Color
            p.VisualParam[130 - 1].ParamValue = 61; // Neck_Length
            p.VisualParam[131 - 1].ParamValue = 43; // Neck_Thickness
            p.VisualParam[132 - 1].ParamValue = 45; // Noble_Nose_Bridge
            p.VisualParam[133 - 1].ParamValue = 127; // Nose_Big_Out
            p.VisualParam[134 - 1].ParamValue = 127; // open_jacket
            p.VisualParam[135 - 1].ParamValue = 0; // Out_Shdw_Color
            p.VisualParam[136 - 1].ParamValue = 0; // Out_Shdw_Opacity
            p.VisualParam[137 - 1].ParamValue = 89; // Outer_Shadow
            p.VisualParam[138 - 1].ParamValue = 0; // Pants_Length
            p.VisualParam[139 - 1].ParamValue = 127; // Pants_Length
            p.VisualParam[140 - 1].ParamValue = 10; // Pants_Waist
            p.VisualParam[141 - 1].ParamValue = 121; // Pants_Wrinkles
            p.VisualParam[142 - 1].ParamValue = 255; // pants_blue
            p.VisualParam[143 - 1].ParamValue = 0; // pants_green
            p.VisualParam[144 - 1].ParamValue = 0; // pants_red
            p.VisualParam[145 - 1].ParamValue = 127; // Pigment
            p.VisualParam[146 - 1].ParamValue = 61; // Pigtails
            p.VisualParam[147 - 1].ParamValue = 85; // Platform_Height
            p.VisualParam[148 - 1].ParamValue = 131; // Pointy_Ears
            p.VisualParam[149 - 1].ParamValue = 119; // Pointy_Eyebrows
            p.VisualParam[150 - 1].ParamValue = 0; // Ponytail
            p.VisualParam[151 - 1].ParamValue = 145; // Pop_Eye
            p.VisualParam[152 - 1].ParamValue = 122; // Puffy_Lower_Lids
            p.VisualParam[153 - 1].ParamValue = 160; // Puffy_Upper_Cheeks
            p.VisualParam[154 - 1].ParamValue = 0; // Rainbow_Color
            p.VisualParam[155 - 1].ParamValue = 0; // Rainbow_Color
            p.VisualParam[156 - 1].ParamValue = 160; // Red_Hair
            p.VisualParam[157 - 1].ParamValue = 0; // Red_Skin
            p.VisualParam[158 - 1].ParamValue = 112; // Rosy_Complexion
            p.VisualParam[159 - 1].ParamValue = 127; // Saddlebags
            p.VisualParam[160 - 1].ParamValue = 0; // Shift_Mouth
            p.VisualParam[161 - 1].ParamValue = 214; // Shirt_Bottom
            p.VisualParam[162 - 1].ParamValue = 204; // Shirt_Wrinkles
            p.VisualParam[163 - 1].ParamValue = 255; // shirt_blue
            p.VisualParam[164 - 1].ParamValue = 0; // shirt_green
            p.VisualParam[165 - 1].ParamValue = 0; // shirt_red
            p.VisualParam[166 - 1].ParamValue = 127; // Shirtsleeve_flair
            p.VisualParam[167 - 1].ParamValue = 33; // Shoe_Height
            p.VisualParam[168 - 1].ParamValue = 127; // Shoe_Platform_Width
            p.VisualParam[169 - 1].ParamValue = 150; // Shoe_Toe_Thick
            p.VisualParam[170 - 1].ParamValue = 255; // shoes_blue
            p.VisualParam[171 - 1].ParamValue = 255; // shoes_green
            p.VisualParam[172 - 1].ParamValue = 255; // shoes_red
            p.VisualParam[173 - 1].ParamValue = 255; // Shoulders
            p.VisualParam[174 - 1].ParamValue = 255; // Side_Fringe
            p.VisualParam[175 - 1].ParamValue = 255; // Sideburns
            p.VisualParam[176 - 1].ParamValue = 255; // Skirt_Length
            p.VisualParam[177 - 1].ParamValue = 255; // skirt_blue
            p.VisualParam[178 - 1].ParamValue = 0; // skirt_bustle
            p.VisualParam[179 - 1].ParamValue = 0; // skirt_green
            p.VisualParam[180 - 1].ParamValue = 255; // skirt_looseness
            p.VisualParam[181 - 1].ParamValue = 137; // skirt_red
            p.VisualParam[182 - 1].ParamValue = 0; // Sleeve_Length
            p.VisualParam[183 - 1].ParamValue = 0; // Sleeve_Length
            p.VisualParam[184 - 1].ParamValue = 0; // Sleeve_Length
            p.VisualParam[185 - 1].ParamValue = 0; // Slit_Back
            p.VisualParam[186 - 1].ParamValue = 0; // Slit_Front
            p.VisualParam[187 - 1].ParamValue = 255; // Slit_Left
            p.VisualParam[188 - 1].ParamValue = 255; // Slit_Right
            p.VisualParam[189 - 1].ParamValue = 255; // Socks_Length
            p.VisualParam[190 - 1].ParamValue = 255; // socks_blue
            p.VisualParam[191 - 1].ParamValue = 255; // socks_green
            p.VisualParam[192 - 1].ParamValue = 255; // socks_red
            p.VisualParam[193 - 1].ParamValue = 0; // Soulpatch
            p.VisualParam[194 - 1].ParamValue = 0; // Square_Jaw
            p.VisualParam[195 - 1].ParamValue = 0; // Squash_Stretch_Head
            p.VisualParam[196 - 1].ParamValue = 0; // Sunken_Cheeks
            p.VisualParam[197 - 1].ParamValue = 255; // Tall_Lips
            p.VisualParam[198 - 1].ParamValue = 255; // Thickness
            p.VisualParam[199 - 1].ParamValue = 255; // Toe_Shape
            p.VisualParam[200 - 1].ParamValue = 0; // Torso_Length
            p.VisualParam[201 - 1].ParamValue = 132; // Torso_Muscles
            p.VisualParam[202 - 1].ParamValue = 17; // Torso_Muscles
            p.VisualParam[203 - 1].ParamValue = 255; // underpants_blue
            p.VisualParam[204 - 1].ParamValue = 25; // underpants_green
            p.VisualParam[205 - 1].ParamValue = 100; // underpants_red
            p.VisualParam[206 - 1].ParamValue = 255; // undershirt_blue
            p.VisualParam[207 - 1].ParamValue = 255; // undershirt_green
            p.VisualParam[208 - 1].ParamValue = 255; // undershirt_red
            p.VisualParam[209 - 1].ParamValue = 255; // Upper_Eyelid_Fold
            p.VisualParam[210 - 1].ParamValue = 84; // Upturned_Nose_Tip
            p.VisualParam[211 - 1].ParamValue = 0; // Waist_Height
            p.VisualParam[212 - 1].ParamValue = 0; // Weak_Chin
            p.VisualParam[213 - 1].ParamValue = 0; // White_Hair
            p.VisualParam[214 - 1].ParamValue = 51; // Wide_Eyes
            p.VisualParam[215 - 1].ParamValue = 107; // Wide_Lip_Cleft
            p.VisualParam[216 - 1].ParamValue = 255; // Wide_Nose
            p.VisualParam[217 - 1].ParamValue = 255; // Wide_Nose_Bridge
            p.VisualParam[218 - 1].ParamValue = 255; // wrinkles

            proxy.InjectPacket(p, Direction.Incoming);
        }
        public void SendAnimations(Proxy proxy,List<LLUUID> Animations)
        {
            if (Animations.Count > 0)
            {
                AvatarAnimationPacket p = new AvatarAnimationPacket();

                p.AnimationList = new AvatarAnimationPacket.AnimationListBlock[Animations.Count];
                p.AnimationSourceList = new AvatarAnimationPacket.AnimationSourceListBlock[Animations.Count];
                p.PhysicalAvatarEventList = new AvatarAnimationPacket.PhysicalAvatarEventListBlock[Animations.Count];
                int i = 0;
                foreach (LLUUID id in Animations)
                {
                    p.AnimationList[i] = new AvatarAnimationPacket.AnimationListBlock();
                    p.AnimationList[i].AnimID = id;
                    p.AnimationList[i].AnimSequenceID = 0;
                    p.AnimationSourceList[i] = new AvatarAnimationPacket.AnimationSourceListBlock();
                    p.AnimationSourceList[i].ObjectID = ID;
                    p.PhysicalAvatarEventList[i] = new AvatarAnimationPacket.PhysicalAvatarEventListBlock();
                    p.PhysicalAvatarEventList[i].TypeData = new byte[0];
                    i++;
                }
                proxy.InjectPacket(p, Direction.Incoming);
            }
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
        LLUUID agentID;
        LLUUID sessionID;
        LLVector3 Position;
        ulong RegionHandle=0;
        uint LocalID = 0;
        uint localidcount = 13371337;
        List<ImaginaryFriend> IFriends = new List<ImaginaryFriend>();
        public ImaginaryFriends()
        {
            proxy = new Proxy(new ProxyConfig("imaginaryfriends", "fw0rdiscrazy-crew"));
            proxy.AddDelegate(libsecondlife.Packets.PacketType.ChatFromViewer, Direction.Outgoing, new PacketDelegate(OnOutgoingChat));
            proxy.AddDelegate(libsecondlife.Packets.PacketType.ObjectUpdate, Direction.Incoming, new PacketDelegate(OnIncommingObjectUpdate));
            proxy.AddDelegate(libsecondlife.Packets.PacketType.ImprovedTerseObjectUpdate, Direction.Incoming, new PacketDelegate(OnImprovedTerseObjectUpdate));
            proxy.AddDelegate(libsecondlife.Packets.PacketType.ImprovedInstantMessage, Direction.Outgoing, new PacketDelegate(OnOutgoingImprovedInstantMessage));
            proxy.SetLoginResponseDelegate(new XmlRpcResponseDelegate(LoginResponse));
            proxy.Start();
        }
        private void LoginResponse(XmlRpcResponse response)
        {
            Hashtable values = (Hashtable)response.Value;
            if (values.Contains("agent_id"))
                agentID = new LLUUID((string)values["agent_id"]);
            if (values.Contains("session_id"))
                sessionID = new LLUUID((string)values["session_id"]);
        }
        Packet OnOutgoingImprovedInstantMessage(Packet packet, System.Net.IPEndPoint endp)
        {
            ImprovedInstantMessagePacket p = (ImprovedInstantMessagePacket)packet;
            foreach(ImaginaryFriend f in IFriends)
            {
                if (p.MessageBlock.ToAgentID == f.ID)
                {
                    //do stuff here
                    switch (Helpers.FieldToUTF8String(p.MessageBlock.Message).ToLower())
                    {
                        case "kill":
                            f.ImaginaryKill(proxy);
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
                    if (Helpers.BytesToUIntBig(terse.ObjectData[i].Data, 0) == LocalID)
                    {
                        Position = new LLVector3(terse.ObjectData[i].Data, 22);
                    }
                }
            }
            return packet;
        }
        Packet OnIncommingObjectUpdate(Packet packet, System.Net.IPEndPoint endp)
        {
            ObjectUpdatePacket ou = (ObjectUpdatePacket)packet;
            foreach (ObjectUpdatePacket.ObjectDataBlock b in ou.ObjectData)
            {
                if (b.FullID == agentID)
                {
                    RegionHandle = ou.RegionData.RegionHandle;
                    LocalID = b.ID;
                    switch(b.ObjectData.Length)
                    {
                        case 76:
                            Position = new LLVector3(b.ObjectData, 16);
                            break;
                        case 60:
                            Position = new LLVector3(b.ObjectData, 0);
                            break;
                        case 48:
                            Position = new LLVector3(
                                Helpers.UInt16ToFloat(b.ObjectData, 16, -0.5f * 256.0f, 1.5f * 256.0f),
                                Helpers.UInt16ToFloat(b.ObjectData, 18, -0.5f * 256.0f, 1.5f * 256.0f),
                                Helpers.UInt16ToFloat(b.ObjectData, 20, -256.0f, 3.0f * 256.0f));
                            break;
                        case 32:
                            Position = new LLVector3(
                                Helpers.UInt16ToFloat(b.ObjectData, 0, -0.5f * 256.0f, 1.5f * 256.0f),
                                Helpers.UInt16ToFloat(b.ObjectData, 2, -0.5f * 256.0f, 1.5f * 256.0f),
                                Helpers.UInt16ToFloat(b.ObjectData, 4, -256.0f, 3.0f * 256.0f));
                            break;
                        case 16:
                            Position = new LLVector3(
                                Helpers.ByteToFloat(b.ObjectData, 0, -256.0f, 256.0f),
                                Helpers.ByteToFloat(b.ObjectData, 1, -256.0f, 256.0f),
                                Helpers.ByteToFloat(b.ObjectData, 2, -256.0f, 256.0f));
                            break;
                        default: break;
                    }
                    
                }
            }
            
            return packet;
        }
        Packet OnOutgoingChat(Packet packet, System.Net.IPEndPoint endp)
        {
            ChatFromViewerPacket p = (ChatFromViewerPacket)packet;
            string set = Helpers.FieldToUTF8String(p.ChatData.Message);
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
            else if (set.ToLower().StartsWith("/activate"))
            {
                List<LLUUID> animations = new List<LLUUID>();
                animations.Add(Animations.STAND);
                foreach (ImaginaryFriend f in IFriends)
                {
                    f.SendAnimations(proxy, animations);
                    f.SendAppearance(proxy);
                }
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
        Dictionary<LLUUID, AvatarAppearancePacket> appearances = new Dictionary<LLUUID, AvatarAppearancePacket>();
    }
}
