using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Serializable]
public class Pair
{
    public string cube_label;
    public float distance_m;
}

[Serializable]
public class FramePayload
{
    public double ts;
    public Pair[] pairs;
}

public class TCPClient : MonoBehaviour
{
    [Header("Python Server Address")]
    // Set this in Inspector:
    // - If Python is running on the SAME Mac as the Unity editor: 127.0.0.1
    // - If Python is on your Mac and you're running on Quest: your Mac's Wi-Fi IP, e.g. 192.168.0.42
    public string host = "127.0.0.1";
    public int port = 10002;

    private TcpClient _client;
    private NetworkStream _stream;
    private Thread _netThread;
    private volatile bool _running;

    // Thread -> main thread handoff for logs
    private readonly object _logLock = new object();
    private string _pendingLog;

    // Thread -> main thread handoff for detection data
    private readonly object _dataLock = new object();
    private List<Pair> _latestPairs = new List<Pair>(); // safe snapshot for spawner

    void Start()
    {
        _running = true;
        _netThread = new Thread(NetworkLoop);
        _netThread.IsBackground = true;
        _netThread.Start();
    }

    private void NetworkLoop()
    {
        try
        {
            // 1. connect
            _client = new TcpClient();
            _client.Connect(host, port);
            SafeLog($"[TCP] Connected to {host}:{port}");

            _stream = _client.GetStream();

            // optional hello (server will ignore or just print it)
            string hello = "Hello, Server!";
            byte[] helloBytes = Encoding.UTF8.GetBytes(hello);
            _stream.Write(helloBytes, 0, helloBytes.Length);
            SafeLog("[TCP] Sent hello");

            // 2. read loop
            byte[] recvBuf = new byte[4096];
            StringBuilder sb = new StringBuilder();

            while (_running)
            {
                int bytesRead = _stream.Read(recvBuf, 0, recvBuf.Length);
                if (bytesRead <= 0)
                {
                    SafeLog("[TCP] Server closed connection.");
                    break;
                }

                string chunk = Encoding.UTF8.GetString(recvBuf, 0, bytesRead);
                sb.Append(chunk);

                // Process all complete lines (newline-delimited JSON)
                while (true)
                {
                    string full = sb.ToString();
                    int newlineIndex = full.IndexOf('\n');
                    if (newlineIndex < 0)
                        break; // no full JSON line yet

                    string line = full.Substring(0, newlineIndex).Trim();
                    sb.Remove(0, newlineIndex + 1);

                    if (!string.IsNullOrEmpty(line))
                    {
                        HandleFrameJson(line);
                    }
                }
            }
        }
        catch (Exception e)
        {
            SafeLog("[TCP] Error: " + e.Message);
        }
        finally
        {
            CleanupSocket();
        }
    }

    // parse JSON line -> FramePayload -> store pairs
    private void HandleFrameJson(string jsonLine)
    {
        try
        {
            var frame = JsonUtility.FromJson<FramePayload>(jsonLine);
            if (frame != null && frame.pairs != null)
            {
                lock (_dataLock)
                {
                    // overwrite snapshot
                    _latestPairs = frame.pairs.ToList();
                }

                // Debug preview for sanity
                SafeLog("[TCP] Frame OK. Pairs: " + frame.pairs.Length);
            }
            else
            {
                SafeLog("[TCP] Frame parse but no pairs");
            }
        }
        catch (Exception e)
        {
            SafeLog("[TCP] JSON parse fail: " + e.Message + " line=" + jsonLine);
        }
    }

    // called from spawner
    public List<Pair> GetLatestPairs()
    {
        lock (_dataLock)
        {
            // return a copy so caller can't mutate internal list
            return new List<Pair>(_latestPairs);
        }
    }

    private void SafeLog(string msg)
    {
        lock (_logLock)
        {
            _pendingLog = msg;
        }
    }

    void Update()
    {
        // flush logs onto Unity main thread so we can see them in Console
        string toLog = null;
        lock (_logLock)
        {
            if (!string.IsNullOrEmpty(_pendingLog))
            {
                toLog = _pendingLog;
                _pendingLog = null;
            }
        }

        if (!string.IsNullOrEmpty(toLog))
        {
            Debug.Log(toLog);
        }
    }

    void OnDestroy()
    {
        _running = false;

        if (_netThread != null && _netThread.IsAlive)
        {
            try
            {
                _netThread.Abort(); // fine for now
            }
            catch { }
            _netThread = null;
        }

        CleanupSocket();
    }

    private void CleanupSocket()
    {
        try { _stream?.Close(); } catch { }
        _stream = null;

        try { _client?.Close(); } catch { }
        _client = null;
    }
}
