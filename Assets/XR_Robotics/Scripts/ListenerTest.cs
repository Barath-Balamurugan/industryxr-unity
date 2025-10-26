using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TcpServer : MonoBehaviour
{
    private TcpListener server;
    private Thread serverThread;
    public int port = 10002;

    void Start()
    {
        // Start server on a separate thread to avoid blocking Unity
        serverThread = new Thread(StartServer);
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    void StartServer()
    {
        try
        {
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, port);
            server.Start();
            Debug.Log($"[TCP] Localhost server listening on {localAddr}:{port}");

            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Debug.Log("[TCP] Connected by client: " + client.Client.RemoteEndPoint);

                // Handle client in separate thread
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.IsBackground = true;
                clientThread.Start();
            }
        }
        catch (SocketException e)
        {
            Debug.LogError("[TCP] SocketException: " + e);
        }
    }

    void HandleClient(TcpClient client)
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        try
        {
            int bytesRead;
            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string received = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Debug.Log("[TCP] Received: " + received);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[TCP] Client disconnected: " + e.Message);
        }
        finally
        {
            client.Close();
            Debug.Log("[TCP] Client closed connection");
        }
    }

    private void OnApplicationQuit()
    {
        if (server != null)
            server.Stop();
        if (serverThread != null && serverThread.IsAlive)
            serverThread.Abort();
    }
}