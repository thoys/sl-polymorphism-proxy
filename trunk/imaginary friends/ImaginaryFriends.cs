using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;
using OpenMetaverse.Packets;
using Nwc.XmlRpc;
using GridProxy;
using OpenMetaverse.Messages.Linden;
using OpenMetaverse.StructuredData;

namespace imaginary_friends
{
    public class ImaginaryFriends
    {
        Proxy proxy;
        UUID agentID;
        UUID sessionID;
        Vector3 Position;
        Quaternion Rotation;
        ulong RegionHandle=0;

        float MIN_FOLLOW_DISTANCE = 1.5F;

        Dictionary<uint, AvatarTracking> AvatarTracker = new Dictionary<uint, AvatarTracking>();

        Dictionary<UUID, AvatarAppearancePacket> appearances = new Dictionary<UUID, AvatarAppearancePacket>();

        bool FourDimensions = false;
        UUID lastselectedobj = UUID.Zero;
        uint LocalID = 0;
        uint localidcount = 13371337;

        Dictionary<ulong, Dictionary<uint,UUID>> GlobalLocalIDs = new Dictionary<ulong, Dictionary<uint,UUID>>();
        XmlFriends.ImaginaryConfig Config = new imaginary_friends.XmlFriends.ImaginaryConfig();
        Dictionary<UUID, ImaginaryFriend> IFriends = new Dictionary<UUID, ImaginaryFriend>();
        
        public ImaginaryFriends()
        {
            proxy = new Proxy(new ProxyConfig("imaginaryfriends", "fw0rdiscrazy-crew"));
            proxy.AddCapsDelegate("GetDisplayNames", new CapsDelegate(GetDisplayNamesCapsHandler));
            proxy.AddDelegate(PacketType.ChatFromViewer, Direction.Outgoing, new PacketDelegate(OnOutgoingChat));
            proxy.AddDelegate(PacketType.ObjectUpdate, Direction.Incoming, new PacketDelegate(OnIncommingObjectUpdate));
            proxy.AddDelegate(PacketType.ImprovedTerseObjectUpdate, Direction.Incoming, new PacketDelegate(OnImprovedTerseObjectUpdate));
            proxy.AddDelegate(PacketType.ImprovedInstantMessage, Direction.Outgoing, new PacketDelegate(OnOutgoingImprovedInstantMessage));
            proxy.AddDelegate(PacketType.KillObject, Direction.Incoming, new PacketDelegate(OnIncommingKillObject));
            proxy.AddDelegate(PacketType.AgentMovementComplete, Direction.Incoming, new PacketDelegate(IncommingMovementComplete));
            proxy.AddDelegate(PacketType.ViewerEffect, Direction.Outgoing, new PacketDelegate(OutgoingViewerEffect));
            proxy.AddDelegate(PacketType.AvatarAppearance, Direction.Incoming, new PacketDelegate(OnIncommingApearance));
            proxy.AddDelegate(PacketType.AgentSetAppearance, Direction.Outgoing, new PacketDelegate(OnOutgoingAgentSetAppearance));
            proxy.AddDelegate(PacketType.AvatarAnimation, Direction.Incoming, new PacketDelegate(OnIncommingAvatarAnimation));
            proxy.AddLoginResponseDelegate(new XmlRpcResponseDelegate(LoginResponse));
            proxy.Start();
        }
        bool GetDisplayNamesCapsHandler(CapsRequest request, CapsStage stage)
        {
            if (stage == CapsStage.Request)
            {
                string split = "/?ids=";
                if (request.FullUri.Contains(split))
                {
                    string possible_ids = request.FullUri.Substring(request.FullUri.IndexOf(split) + split.Length);
                    string[] possible_id_arr = possible_ids.Split(new string[]{"?ids="}, StringSplitOptions.None);
                    foreach (string possible_id in possible_id_arr)
                    {
                        UUID parsed_id = UUID.Zero;
                        if (UUID.TryParse(possible_id, out parsed_id))
                        {
                            if (IFriends.ContainsKey(parsed_id))
                            {
                                //proxy
                                return false;
                            }
                        }
                    }
                }
            }
            if (stage == CapsStage.Response)
            {
                GetDisplayNamesMessage gdnm = new GetDisplayNamesMessage();
                gdnm.Deserialize((OSDMap)request.Response);
                
                List<AgentDisplayName> adnNewNames = new List<AgentDisplayName>();

                foreach (UUID baduuid in gdnm.BadIDs)
                {
                    if (IFriends.ContainsKey(baduuid))
                    {
                        AgentDisplayName adnTempName = new AgentDisplayName();
                        adnTempName.DisplayName = IFriends[baduuid].Name;
                        adnTempName.ID = IFriends[baduuid].ID;
                        adnTempName.IsDefaultDisplayName = true;
                        adnTempName.LegacyFirstName = IFriends[baduuid].FirstName;
                        adnTempName.LegacyLastName = IFriends[baduuid].LastName;
                        adnTempName.NextUpdate = DateTime.Now.AddDays(7);
                        adnTempName.UserName = (IFriends[baduuid].FirstName + "." + IFriends[baduuid].LastName).ToLower();
                        adnNewNames.Add(adnTempName);
                    }
                }

                AgentDisplayName[] newNames = new AgentDisplayName[adnNewNames.Count + gdnm.Agents.Length];

                for (int i = 0; i < gdnm.Agents.Length; i++)
                {
                    newNames[i] = gdnm.Agents[i];
                }

                for (int j = 0; j < adnNewNames.Count; j++)
                {
                    newNames[j + gdnm.Agents.Length] = adnNewNames[j];
                }

                gdnm.Agents = newNames;
                gdnm.BadIDs = new UUID[0];
                request.Response = gdnm.Serialize();

            }
            return false;
        }

        Dictionary<UUID, AvatarAnimationPacket> AvatarAnimation = new Dictionary<UUID, AvatarAnimationPacket>();

        Packet OnIncommingAvatarAnimation(Packet packet, System.Net.IPEndPoint endp)
        {
            AvatarAnimationPacket set = (AvatarAnimationPacket)packet;
            AvatarAnimation[set.Sender.ID] = set;
            return packet;
        }

        Packet OnOutgoingAgentSetAppearance(Packet packet, System.Net.IPEndPoint endp)
        {
            AgentSetAppearancePacket set = (AgentSetAppearancePacket)packet;
            AvatarAppearancePacket own_appearance = new AvatarAppearancePacket();
            own_appearance.Sender = new AvatarAppearancePacket.SenderBlock();
            own_appearance.Sender.ID = set.AgentData.AgentID;
            own_appearance.ObjectData = new AvatarAppearancePacket.ObjectDataBlock();
            own_appearance.ObjectData.TextureEntry = set.ObjectData.TextureEntry;
            own_appearance.VisualParam = new AvatarAppearancePacket.VisualParamBlock[set.VisualParam.Length];
            for (int i = 0; i < set.VisualParam.Length; i++)
            {
                own_appearance.VisualParam[i] = new AvatarAppearancePacket.VisualParamBlock();
                own_appearance.VisualParam[i].ParamValue = set.VisualParam[i].ParamValue;
            }
            //set.
            appearances[set.AgentData.AgentID] = own_appearance;
            return packet;
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
            foreach(ImaginaryFriend f in IFriends.Values)
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
                    // State
                    byte state = terse.ObjectData[i].Data[4];
                    // Avatar boolean
                    bool avatar = (terse.ObjectData[i].Data[5] != 0);
                    if (!avatar) return null;
                    Vector3 tpos = new Vector3(terse.ObjectData[i].Data, 22);
                    Quaternion trot = //new Quaternion(terse.ObjectData[i].Data, 46);
                    new Quaternion(
                        Utils.UInt16ToFloat(terse.ObjectData[i].Data, 46, -1.0f, 1.0f),
                        Utils.UInt16ToFloat(terse.ObjectData[i].Data, 48, -1.0f, 1.0f),
                        Utils.UInt16ToFloat(terse.ObjectData[i].Data, 50, -1.0f, 1.0f),
                        Utils.UInt16ToFloat(terse.ObjectData[i].Data, 52, -1.0f, 1.0f));

                    if (tid == LocalID)
                    {
                        Position = tpos;
                        Rotation = trot;
                    }
                    if (AvatarTracker.ContainsKey(tid))
                    {
                        if (AvatarTracker[tid].Position != tpos)
                        {
                            if (FourDimensions && Vector3.Distance(AvatarTracker[tid].LastPlacedPos, tpos) >= MIN_FOLLOW_DISTANCE)
                            {
                                //new ImaginaryFriend(
                                //lets create a new friendpoint
                                ImaginaryFriend temp = new ImaginaryFriend(AvatarTracker[tid].Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], AvatarTracker[tid].Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] + " time = " + System.DateTime.Now.ToString(), AvatarTracker[tid].Position, AvatarTracker[tid].Rotation, true, RegionHandle, localidcount++);
                                IFriends.Add(temp.ID,temp);
                                temp.ImaginaryLogin(proxy);
                                if(appearances.ContainsKey(AvatarTracker[tid].ID))
                                {
                                    temp.Appearance = appearances[AvatarTracker[tid].ID];
                                }
                                temp.SendAppearance(proxy);
                                if (AvatarAnimation.ContainsKey(AvatarTracker[tid].ID))
                                {
                                    List<UUID> anims = new List<UUID>();
                                    foreach (AvatarAnimationPacket.AnimationListBlock alb in AvatarAnimation[AvatarTracker[tid].ID].AnimationList)
                                    {
                                        anims.Add(alb.AnimID);
                                    }
                                    temp.SendAnimations(proxy, anims);
                                }
                                AvatarTracker[tid].LastPlacedPos = AvatarTracker[tid].Position;
                            }
                            AvatarTracker[tid].Position = tpos;
                            AvatarTracker[tid].Rotation = trot;
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
                Quaternion lrot = Quaternion.Identity;
                int pos = 0;
                switch (b.ObjectData.Length)
                {
                    case 76:
                        pos += 16;
                        goto case 60;
                    case 60:
                        lpos = new Vector3(b.ObjectData, pos);
                        pos += 36;
                        lrot = new Quaternion(b.ObjectData, pos, true);
                        break;
                    case 48:
                        pos += 16;
                        goto case 32;
                    case 32:
                        lpos = new Vector3(
                            Utils.UInt16ToFloat(b.ObjectData, pos + 0, -0.5f * 256.0f, 1.5f * 256.0f),
                            Utils.UInt16ToFloat(b.ObjectData, pos + 2, -0.5f * 256.0f, 1.5f * 256.0f),
                            Utils.UInt16ToFloat(b.ObjectData, pos + 4, -256.0f, 3.0f * 256.0f));
                        pos += 18;
                        lrot = new Quaternion(
                            Utils.UInt16ToFloat(b.ObjectData, pos, -1.0f, 1.0f),
                            Utils.UInt16ToFloat(b.ObjectData, pos + 2, -1.0f, 1.0f),
                            Utils.UInt16ToFloat(b.ObjectData, pos + 4, -1.0f, 1.0f),
                            Utils.UInt16ToFloat(b.ObjectData, pos + 6, -1.0f, 1.0f));
                        break;
                    case 16:
                        lpos = new Vector3(
                            Utils.ByteToFloat(b.ObjectData, pos + 0, -256.0f, 256.0f),
                            Utils.ByteToFloat(b.ObjectData, pos + 1, -256.0f, 256.0f),
                            Utils.ByteToFloat(b.ObjectData, pos + 2, -256.0f, 256.0f));
                        pos += 9;
                        lrot = new Quaternion(
                            Utils.ByteToFloat(b.ObjectData, pos, -1.0f, 1.0f),
                            Utils.ByteToFloat(b.ObjectData, pos + 1, -1.0f, 1.0f),
                            Utils.ByteToFloat(b.ObjectData, pos + 2, -1.0f, 1.0f),
                            Utils.ByteToFloat(b.ObjectData, pos + 3, -1.0f, 1.0f));
                        break;
                }
                if (b.PCode == 47) // 47 == AVATAR
                {
                    if (AvatarTracker.ContainsKey(b.ID))
                    {
                        //check for movement
                        if (AvatarTracker[b.ID].Position != lpos && FourDimensions)
                        {
                            ImaginaryFriend temp = new ImaginaryFriend(AvatarTracker[b.ID].Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[0], AvatarTracker[b.ID].Name.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)[1] + " " + System.DateTime.Now.ToString(), AvatarTracker[b.ID].Position, AvatarTracker[b.ID].Rotation, true, RegionHandle, localidcount++);

                            IFriends.Add(temp.ID,temp);
                            temp.SittingOn = b.ParentID; //set sitting on
                            temp.ImaginaryLogin(proxy);

                           // Console.WriteLine("Create new av here");
                        }

                        AvatarTracker[b.ID].Position = lpos;
                        AvatarTracker[b.ID].Rotation = lrot;
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
                        AvatarTracker[b.ID] = new AvatarTracking(tfirstname+" "+tlastname,b.FullID, b.ID, lpos,lrot, b.ParentID);
                    }
                }
                if (b.FullID == agentID)
                {
                    RegionHandle = ou.RegionData.RegionHandle;
                    LocalID = b.ID;
                    Position = lpos;
                    Rotation = lrot;
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
                    ImaginaryFriend IF = new ImaginaryFriend(split[2], combineArguments(split, 3), Position,Rotation , male, RegionHandle,localidcount++);
                    IF.ImaginaryLogin(proxy);

                    IFriends.Add(IF.ID,IF);
                }
                return null;
            }
            else if (set.ToLower().StartsWith("/killall"))
            {
                foreach (ImaginaryFriend f in IFriends.Values)
                {
                    f.ImaginaryKill(proxy);
                }
                IFriends.Clear();
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
                string output = set.ToLower().Remove(0, 11).Trim();
                UUID animation = UUID.Zero;
                if(UUID.TryParse(output,out animation))
                {
                    
                    List<UUID> animations = new List<UUID>();
                    animations.Add(animation);
                    foreach (ImaginaryFriend f in IFriends.Values)
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
                        if (IFriends.ContainsKey(tempfriend.ID))
                        {
                            IFriends[tempfriend.ID].ImaginaryKill(proxy);
                            IFriends.Remove(tempfriend.ID);
                        }

                        IFriends.Add(tempfriend.ID,tempfriend);
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
