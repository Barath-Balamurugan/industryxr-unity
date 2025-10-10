using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Linq;

[Serializable]
public class Pair {
    public string cube_label;
    public float distance_m; 
}

[Serializable]
public class FramePayload {
    public double ts;
    public Pair[] pairs;
}

public class Listener : MonoBehaviour
{
    public int listenPort = 10000;      // you're already using this
    public bool logParsedPairs = true;  // toggle console spam

    public event Action<FramePayload> OnFrame;  // subscribe from other scripts

    UdpClient client;
    Thread t;
    volatile bool running;

    FramePayload _latest;
    public FramePayload Latest => _latest;  // getter for polling

    void Start()
    {
        Application.runInBackground = true;
        client = new UdpClient(listenPort);
        running = true;
        t = new Thread(Loop) { IsBackground = true };
        t.Start();
        Debug.Log($"[UDP] Listening on 0.0.0.0:{listenPort} (UDP, any sender)");
    }

    void Loop()
    {
        var any = new IPEndPoint(IPAddress.Any, 0);
        while (running)
        {
            try
            {
                var data = client.Receive(ref any);               // one full datagram
                var txt = Encoding.UTF8.GetString(data);

                // Parse JSON into our model
                var payload = JsonUtility.FromJson<FramePayload>(txt);

                // JsonUtility returns null if the JSON doesn't match; guard it
                if (payload != null && payload.pairs != null && payload.pairs.Length > 0)
                {
                    _latest = payload;

                    if (logParsedPairs)
                    {
                        if (payload?.pairs != null && payload.pairs.Length > 0)
{
                            var line = string.Join(" | ",
                                payload.pairs.Select(p =>
                                {
                                    // var mStr = p.distance_m > 0 ? $"{p.distance_m:F3} m" : $"{p.distance_px:F1} px";
                                    return $"{p.cube_label}: {p.distance_m}";
                                }));

                            Debug.Log(line);
                        }
                    }

                    // fire event for subscribers
                    OnFrame?.Invoke(payload);
                }
                else
                {
                    // Optional: ignore heartbeats like {"heartbeat":true}
                    // Debug.Log("[UDP] JSON parsed but payload/pairs empty");
                }
            }
            catch (SocketException) { /* closing */ }
            catch (Exception ex)
            {
                Debug.LogWarning($"[UDP] Error: {ex.Message}");
            }
        }
    }

    void OnDisable() => Shutdown();
    void OnApplicationQuit() => Shutdown();
    void Shutdown()
    {
        if (!running) return;
        running = false;
        try { client?.Close(); } catch { }
        try { t?.Join(50); } catch { }
    }
}
