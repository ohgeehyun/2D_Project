using Google.Protobuf;
using Protocol;
using ServerCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketHandler 
{
  
    public static void S_ChatHandler(PacketSession session, IMessage packet)
    {
        S_CHAT chatPacket = packet as S_CHAT;
        ServerSession serverSession = session as ServerSession;

        Debug.Log(chatPacket.Context);
    }

    public static void S_EnterGameHandler(PacketSession session, IMessage packet)
    {
        S_ENTER_GAME enterGamePacket = packet as S_ENTER_GAME;
        ServerSession serverSession = session as ServerSession;
    }

}
