using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEngine.tvOS;
using Google.Protobuf;
using Protocol;
using System.IO;
using UnityEditor.Sprites;
using System.Drawing;

namespace Assets.Scripts.Network
{
    internal class SocketUtils
    {
        public static Socket CreateSocket()
        {
            return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public static IPEndPoint setIPendPoint(IPAddress ipAddress, int port)
        {

            return new IPEndPoint(ipAddress, port);
        }

        public static bool socketConnect(Socket clientSocket, IPEndPoint remoteEp)
        {
            try
            {
                clientSocket.Connect(remoteEp);
                Debug.Log("Socket Connenect Succese");
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket Connect Error : " + ex);
                return false;
            }

            return true;
        }

        public static bool DisconnectSocket(ref Socket socket, bool reused)
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Disconnect(reuseSocket: reused);
                    switch (reused)
                    {
                        case true:
                            Debug.Log("연결이 종료되었습니다.재사용 가능");
                            break;
                        case false:
                            Debug.Log("연결이 종료되었습니다.재사용 불가능");
                            break;
                    }
                }
            }
            catch (SocketException ex)
            {
                Debug.Log("Socket Connect Error : " + ex);
                return false;
            }
            return true;
        }

        public async static Task SendMessageAsync(Socket socket, IMessage  message)
        {
            //패킷 id 
            PacketId packetIdValue = (PacketId)Enum.Parse(typeof(PacketId), message.Descriptor.Name);
            ushort packetId = (ushort)packetIdValue;

            //직렬화
            byte[] messageData;
            using (var memoryStream = new MemoryStream())
            {
                message.WriteTo(memoryStream);
                messageData = memoryStream.ToArray();
            }

            //proto buffer에 담긴 데이터의 사이즈
            ushort messageSize = (ushort)messageData.Length;

            
            byte[] packet = new byte[sizeof(ushort) + sizeof(ushort) + messageSize];
            Array.Copy(BitConverter.GetBytes(packetId), 0, packet, 0, sizeof(ushort));
            Array.Copy(BitConverter.GetBytes(packet.Length), 0, packet, 2, sizeof(ushort));
            Array.Copy(messageData, 0, packet, 4, messageSize);

            await socket.SendAsync(new ArraySegment<byte>(packet), SocketFlags.None);

        }

        public async static Task ReceiveMessageAsync(Socket socket)
        {
            while (true)
            {
                byte[] _receiveBuffer = new byte[2048];  // 임시 사용 수신 버퍼 크기 설정
                int received = await socket.ReceiveAsync(new ArraySegment<byte>(_receiveBuffer), SocketFlags.None);

                //현재 패킷의 헤더는 4로 고정이기때문에 임시로 사용
                if (received <= 4)
                {
                    Debug.Log("서버 연결이 종료되었거나 수신된 데이터가 충분하지 않습니다.");
                    break;
                }

                //현재 패킷의 헤더는 4로 고정이기때문에 임시로 사용
                if (received >= 2048)
                {
                    Debug.Log("받은 메세지의 크기가 recv_buffer를 초과 합니다.");
                    break;
                }

                ushort packetId = BitConverter.ToUInt16(_receiveBuffer, 0);
                ushort packetSize = BitConverter.ToUInt16(_receiveBuffer, 2);


                Debug.Log($"패킷 ID: {packetId}, 메시지 크기: {packetSize}바이트");

            }
        }



    }
}
