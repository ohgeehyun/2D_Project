using Google.Protobuf;
using Protocol;
using ServerCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketManager
{
    //Ŭ���̾�Ʈ���� ���� �� ��Ŷ�� �Ŵ��� 
    #region Singleton
    static PacketManager _instance = new PacketManager();
    public static PacketManager Instance { get { return _instance; } }
    #endregion

    PacketManager()
    {
        // TODO : ��Ŷ �Ŵ����� ��������鼭 ����� ��Ŷ ���� ���� �Լ� 
        Register();
    }

    Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>> _onRecv = new Dictionary<ushort, Action<PacketSession, ArraySegment<byte>, ushort>>();
    Dictionary<ushort, Action<PacketSession, IMessage>> _handler = new Dictionary<ushort, Action<PacketSession, IMessage>>();

    public void  Register()
    {
        _onRecv.Add((ushort)MsgId.SChat,MakePacket<S_CHAT>);
        _handler.Add((ushort)MsgId.SChat,PacketHandler.S_ChatHandler);
        _onRecv.Add((ushort)MsgId.SEnterGame, MakePacket<S_ENTER_GAME>);
        _handler.Add((ushort)MsgId.SEnterGame, PacketHandler.S_EnterGameHandler);
    }

    public void OnRecvPacket(PacketSession session, ArraySegment<byte> buffer)
    {
        ushort count = 0;

        ushort id = BitConverter.ToUInt16(buffer.Array, buffer.Offset);
        count += 2;
        ushort size = BitConverter.ToUInt16(buffer.Array, buffer.Offset+count);
        count += 2;

        Action<PacketSession, ArraySegment<byte>, ushort> action = null;
        if (_onRecv.TryGetValue(id, out action))
            action.Invoke(session, buffer, id);

    }


    void MakePacket<T>(PacketSession session, ArraySegment<byte> buffer,ushort id) where T : IMessage, new()
    {
        T pkt = new T();
        pkt.MergeFrom(buffer.Array, buffer.Offset + 4, buffer.Count -4);
        Action<PacketSession, IMessage> action = null;
        if (_handler.TryGetValue(id, out action))
            action.Invoke(session, pkt);
    }

    public Action<PacketSession, IMessage> GetPacketHandler(ushort id)
    {
        Action<PacketSession, IMessage> action = null;
        if (_handler.TryGetValue(id,out action))
            return action;

        return null;
    }

}
