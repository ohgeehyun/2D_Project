using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using Google.Protobuf;
using Protocol;
using System.Net.Sockets;
using Assets.Scripts.Network;
using UnityEditor.Experimental.GraphView;
using System.Text;
using System;
using ServerCore;

public class NetworkManager 
{
    ServerSession _session = new ServerSession();

    public void Send(ArraySegment<byte> sendBuff)
    {
        _session.Send(sendBuff);
    }

    public  void Init()
    {

        IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 5252);
        Connector connector = new Connector();

   
        connector.Connect(endPoint, () => { return _session; }, 1);

    }

    public void Update()
    {
        List<PacketMessage> list = PacketQueue.Instance.PopAll();
        foreach(PacketMessage packet in list)
        {
            Action<PacketSession, IMessage> handler = PacketManager.Instance.GetPacketHandler(packet.Id);
            if (handler != null)
                handler.Invoke(_session, packet.Message);
        }
    }
}
