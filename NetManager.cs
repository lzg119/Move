using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UnityEngine.UI;
using System;

public static class NetManager{
	static Socket socket;
	static byte[] readBuff = new byte[1024]; 
    // 委托类型
    public delegate void MsgListener(string msg);
    private static Dictionary<string, MsgListener> listenDict = new Dictionary<string, MsgListener>();
    private static List<string> msgList = new List<string>();

    public static string debug_info = "";

    public static void AddListener(string msgName, MsgListener listener){
        listenDict[msgName] = listener;
    }
    public static string GetDesc(){
        if(socket==null) {
            Debug.Log("Socket is null fail ");
            return "";
        }
        
        if(!socket.Connected) {
            Debug.Log("Socket not Connected");
            return "";
        }
        
        return socket.LocalEndPoint.ToString();
    }
    public static void Connect(string ip, int port){
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(ip, port);
        socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCbk, socket);
    }
    public static void ReceiveCbk(IAsyncResult ar){
        try{
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndReceive(ar);
            string recvStr = System.Text.Encoding.Default.GetString(readBuff, 0, count);
            msgList.Add(recvStr);
            socket.BeginReceive(readBuff, 0, 1024, 0, ReceiveCbk, socket);
        }catch(SocketException ex){
            Debug.Log("Socket Recevie fail " + ex.ToString());
        }
    }
    public static void Send(string sendStr){
        if(socket == null) return;
        if(!socket.Connected) return;

        byte[] sendBytes = System.Text.Encoding.Default.GetBytes(sendStr);
        socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, SendCbk, socket);
    }
    public static void SendCbk(IAsyncResult ar){
        try{
            Socket socket = (Socket) ar.AsyncState;
            int count = socket.EndSend(ar);
            Debug.Log("Socket Send succ " + count);
        }catch(SocketException ex){
            Debug.Log("Socket Send fail " + ex.ToString());
        }
    }
    public static void Update(){
        if(msgList.Count>0){
            string msgStr = msgList[0];
            msgList.RemoveAt(0);
            string[] split = msgStr.Split('|');
            string msgName = split[0];
            string msgArgs = split[1];
            if(listenDict.ContainsKey(msgName)){
                listenDict[msgName](msgArgs);
            }
        }
    }
}
