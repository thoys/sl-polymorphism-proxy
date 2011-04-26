using System;
using System.Collections.Generic;
using System.Text;
using OpenMetaverse;
using OpenMetaverse.Packets;
using GridProxy;

namespace polymorphism_proxy
{
    public class morphProxy
    {
        int NUM = 19;
        System.Timers.Timer timerTit;
        int thisstep;
        bool reverseplayback;
        bool jiggletitactive;

        byte bc; // cleavage
        byte bg; // gravity
        byte bs; // size
        Proxy proxy;

        public morphProxy()
        {
            proxy = new Proxy(new ProxyConfig("polymophism", "Thoys"));
            proxy.AddDelegate(OpenMetaverse.Packets.PacketType.AvatarAppearance, Direction.Incoming, new PacketDelegate(OnIncommingApearance));
            proxy.AddDelegate(OpenMetaverse.Packets.PacketType.ChatFromViewer, Direction.Outgoing, new PacketDelegate(OnOutgoingChat));
            proxy.Start();
        }

        Packet OnIncommingApearance(Packet packet, System.Net.IPEndPoint endp)
        {
            jiggletit();
            AvatarAppearancePacket set = (AvatarAppearancePacket)packet;
            appearances[set.Sender.ID] = set;
            set.VisualParam[NUM].ParamValue = 255; //  61; // Breast_Size
            set.VisualParam[NUM+1].ParamValue = 255; // 5; // Breast_Female_Cleavage
            set.VisualParam[NUM+2].ParamValue = 255; // 127; // Breast_Gravity
            return set;
        }

        void mod()
        {
            foreach(AvatarAppearancePacket ap in appearances.Values)
            {
                AvatarAppearancePacket newp = new AvatarAppearancePacket();
                newp.ObjectData = ap.ObjectData;
                newp.Sender = ap.Sender;
                newp.VisualParam = new AvatarAppearancePacket.VisualParamBlock[ap.VisualParam.Length];
                for(int i=0;i<ap.VisualParam.Length;i++)
                {
                    newp.VisualParam[i] = new AvatarAppearancePacket.VisualParamBlock();
                    newp.VisualParam[i].ParamValue = ap.VisualParam[i].ParamValue;
                }
                if(NUM<(newp.VisualParam.Length-2) && NUM >= 0)
                {
                    newp.VisualParam[NUM].ParamValue = 255; //  61; // Breast_Size
                    newp.VisualParam[NUM + 1].ParamValue = 255; // 5; // Breast_Female_Cleavage
                    newp.VisualParam[NUM + 2].ParamValue = 255; // 127; // Breast_Gravity
                }
                proxy.InjectPacket(newp, Direction.Incoming);
            }
        }

        Packet OnOutgoingChat(Packet packet, System.Net.IPEndPoint endp)
        {
            ChatFromViewerPacket p = (ChatFromViewerPacket)packet;
            string set = Utils.BytesToString(p.ChatData.Message);
            if (set.ToLower().StartsWith("set"))
            {
                string [] split = set.Split(new char[]{' '});
                if(split.Length == 2)
                {
                    int num = 0;
                    if (int.TryParse(split[1], out num))
                    {
                        NUM = num;
                        mod();
                    }

                }
            }
            return packet;
        }

        Dictionary<UUID, AvatarAppearancePacket> appearances = new Dictionary<UUID, AvatarAppearancePacket>();


        void InitializeTitTimer()
        {
            timerTit = new System.Timers.Timer();
            timerTit.Elapsed += new System.Timers.ElapsedEventHandler(timerTit_Elapsed);
            timerTit.Interval = 60000;
            timerTit.Enabled = true;
        }

        void timerTit_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            jiggletit();
        }

        //timertit_elapsed

        // call jiggletit
        void toggletit()
        {
            jiggletitactive = !jiggletitactive;
            if (jiggletitactive)
            {
                timerTit.Start();
            }
            else
            {
                timerTit.Stop();
            }
        }

        void jiggletit()
        {
            // jiggletit: thanks to root66, thoys, and libsecondlife
            // mutate cleavage up and down on a bell curve

            // fixme: these numbers need to be graded from max to min
            // it should be a bell curve to smooth the mutation

            switch (thisstep)
            {
                case 1: bs = 10; bc = 10; bg = 127; break;
                case 2: bs = 10; bc = 10; bg = 127; break;
                case 3: bs = 10; bc = 10; bg = 127; break;
                case 4: bs = 10; bc = 10; bg = 127; break;
                case 5: bs = 10; bc = 10; bg = 127; break;
                case 6: bs = 10; bc = 10; bg = 127; break;
                case 7: bs = 10; bc = 10; bg = 127; break;
                case 8: bs = 10; bc = 10; bg = 127; break;
                case 9: bs = 10; bc = 10; bg = 127; break;
                default: break;
            }

            if (thisstep > 10)
            {
                reverseplayback = true;
            }

            if (reverseplayback)
            {
                thisstep--;
            }
            else
            {
                thisstep++;
            }
        }
    }
}
