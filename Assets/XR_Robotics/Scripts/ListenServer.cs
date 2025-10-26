using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Collections.Generic;
// [Serializable]
// public class Pair
// {
//     public string cube_label;
//     public float distance_m;
// }
// [Serializable]
// public class FramePayload
// {
//     public double ts;
//     public Pair[] pairs;
// }
public class ListenServer : MonoBehaviour
{
    // [Header("Listen settings")]
    // public int listenPort = 10002;   // MUST match TCP_TARGET_PORT in Python
    // [Header("Debug")]
    // public bool logIncoming = true;
    // // most recent decoded frame you can read from another script
    // private FramePayload _latest;
    // public FramePayload Latest => _latest;
    // // internals
    // private TcpListener _listener;
    // private Thread _thread;
    // private volatile bool _running;
    // // buffer for partial TCP reads
    // private StringBuilder _accum = new StringBuilder();
    // void Start()
    // {
    //     Application.runInBackground = true; // don't pause in VR if window not focused
    //     try
    //     {
    //         _listener = new TcpListener(IPAddress.Any, listenPort);
    //         _listener.Start();
    //         Debug.Log($"[TCP] Listening on 0.0.0.0:{listenPort}");
    //         _running = true;
    //         _thread = new Thread(ServerLoop) { IsBackground = true };
    //         _thread.Start();
    //     }
    //     catch (Exception e)
    //     {
    //         Debug.LogError("[TCP] Failed to start listener: " + e);
    //         _running = false;
    //     }
    // }
    // void OnDestroy()
    // {
    //     _running = false;
    //     try { _listener?.Stop(); } catch { }
    // }
    // private void ServerLoop()
    // {
    //     while (_running)
    //     {
    //         TcpClient client = null;
    //         try
    //         {
    //             Debug.Log("[TCP] Waiting for client...");
    //             client = _listener.AcceptTcpClient(); // blocking
    //             Debug.Log("[TCP] AcceptTcpClient returned");
    //         }
    //         catch (SocketException se)
    //         {
    //             Debug.LogError("[TCP] Accept failed: " + se);
    //             break;
    //         }
    //         catch (ObjectDisposedException ode)
    //         {
    //             Debug.LogError("[TCP] Listener disposed: " + ode);
    //             break;
    //         }
    //         Debug.Log("[TCP] Client connected");
    //         HandleClient(client);
    //         Debug.Log("[TCP] Client disconnected");
    //     }
    // }
    // private void HandleClient(TcpClient client)
    // {
    //     using (client)
    //     using (NetworkStream stream = client.GetStream())
    //     {
    //         byte[] buf = new byte[4096];
    //         while (_running && client.Connected)
    //         {
    //             int count;
    //             try
    //             {
    //                 count = stream.Read(buf, 0, buf.Length);
    //                 if (count == 0)
    //                 {
    //                     // remote closed connection
    //                     break;
    //                 }
    //             }
    //             catch
    //             {
    //                 break;
    //             }
    //             // append this chunk to the rolling buffer
    //             string chunk = Encoding.UTF8.GetString(buf, 0, count);
    //             _accum.Append(chunk);
    //             // We are using newline '\n' as the message delimiter,
    //             // same as your Python code.
    //             while (true)
    //             {
    //                 string current = _accum.ToString();
    //                 int newlineIndex = current.IndexOf('\n');
    //                 if (newlineIndex < 0)
    //                 {
    //                     // still don't have a full line yet
    //                     break;
    //                 }
    //                 // full line available
    //                 string oneLine = current.Substring(0, newlineIndex).Trim();
    //                 string remainder = current.Substring(newlineIndex + 1);
    //                 _accum.Length = 0;
    //                 _accum.Append(remainder);
    //                 if (oneLine.Length == 0)
    //                     continue;
    //                 try
    //                 {
    //                     var frame = JsonUtility.FromJson<FramePayload>(oneLine);
    //                     if (frame != null)
    //                     {
    //                         // publish the most recent frame
    //                         _latest = frame;
    //                         if (logIncoming && frame.pairs != null)
    //                         {
    //                             var sb = new StringBuilder();
    //                             sb.Append($"[TCP] ts={frame.ts:F3} -> ");
    //                             for (int i = 0; i < frame.pairs.Length; i++)
    //                             {
    //                                 var p = frame.pairs[i];
    //                                 sb.Append(p.cube_label)
    //                                   .Append(":")
    //                                   .Append(p.distance_m.ToString("F3"));
    //                                 if (i < frame.pairs.Length - 1)
    //                                     sb.Append(" | ");
    //                             }
    //                             Debug.Log(sb.ToString());
    //                         }
    //                     }
    //                 }
    //                 catch (Exception ex)
    //                 {
    //                     Debug.LogWarning("[TCP] bad JSON line: " + oneLine + "\n" + ex);
    //                 }
    //             }
    //         }
    //     }
    // }
    // public IReadOnlyList<Pair> GetLatestPairs()
    // {
    //     return Latest?.pairs ?? Array.Empty<Pair>();
    // }
}