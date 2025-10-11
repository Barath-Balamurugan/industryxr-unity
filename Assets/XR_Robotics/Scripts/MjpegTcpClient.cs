using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class MjpegTcpClient : MonoBehaviour
{
    [Header("Connection")]
    public string host = "127.0.0.1"; // <-- set to your Mac's IP if remote
    public int port = 8081;
    public string boundary = "spionisto"; // must match multipartmux boundary

    [Header("Targets (assign one)")]
    public Renderer targetRenderer;
    public RawImage targetRawImage;

    [Header("Texture")]
    public TextureFormat textureFormat = TextureFormat.RGB24;
    public bool mipChain = false;
    public bool linear = false;

    TcpClient _client;
    NetworkStream _stream;
    Thread _thread;
    volatile bool _running;

    Texture2D _tex;
    readonly object _frameLock = new object();
    byte[] _latestJpeg;

    byte[] _boundaryBytes;
    byte[] _headerSep = Encoding.ASCII.GetBytes("\r\n\r\n");
    int _frames;
    float _t;

    void Start()
    {
        if (host == "0.0.0.0")
            Debug.LogWarning("[MJPEG] Host is 0.0.0.0 (invalid for connect). Use 127.0.0.1 or your Mac's LAN IP.");

        _boundaryBytes = Encoding.ASCII.GetBytes("--" + boundary);
        _tex = new Texture2D(2, 2, textureFormat, mipChain, linear);

        if (targetRenderer) targetRenderer.material.mainTexture = _tex;
        if (targetRawImage) targetRawImage.texture = _tex;

        _running = true;
        _thread = new Thread(ReadLoop) { IsBackground = true, Name = "MJPEG Reader" };
        _thread.Start();
    }

    void OnDestroy()
    {
        _running = false;
        try { _stream?.Close(); } catch { }
        try { _client?.Close(); } catch { }
        try { _thread?.Join(200); } catch { }
    }

    void Update()
    {
        byte[] jpg = null;
        lock (_frameLock)
        {
            if (_latestJpeg != null)
            {
                jpg = _latestJpeg;
                _latestJpeg = null;
            }
        }
        if (jpg != null)
        {
            if (_tex.LoadImage(jpg, false))
            {
                _tex.Apply(false, false);
                _frames++;
            }
        }

        _t += Time.deltaTime;
        if (_t >= 1f)
        {
            Debug.Log($"[MJPEG] FPS={_frames}");
            _frames = 0;
            _t = 0f;
        }
    }

    void ReadLoop()
    {
        try
        {
            _client = new TcpClient();
#if !UNITY_WEBGL
            _client.NoDelay = true;
#endif
            var ar = _client.BeginConnect(host, port, null, null);
            if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(5)))
                throw new Exception("Connect timeout");
            _client.EndConnect(ar);

            _stream = _client.GetStream();
            _stream.ReadTimeout = 10000; // 10s

            Debug.Log("[MJPEG] Connected. Waiting for boundary: " + boundary);

            while (_running)
            {
                if (!ReadUntilBoundary(_stream))
                    throw new IOException("Boundary not found (check 'boundary=' in server).");

                var headers = ReadHeaders(_stream);
                if (headers == null)
                    throw new IOException("Failed reading MIME headers.");

                int contentLen = -1;
                if (headers.TryGetValue("content-length", out string cl))
                    int.TryParse(cl.Trim(), out contentLen);

                byte[] jpg = (contentLen > 0)
                    ? ReadExact(_stream, contentLen)
                    : ReadUntilNextBoundary(_stream, out _);

                if (jpg == null || jpg.Length == 0)
                    throw new IOException("Empty JPEG payload.");

                lock (_frameLock) _latestJpeg = jpg;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[MJPEG] Reader exception: " + e);
        }
    }

    bool ReadUntilBoundary(Stream s)
    {
        int match = 0;
        int b;
        while (_running && (b = ReadByteSafe(s)) != -1)
        {
            byte bb = (byte)b;
            if (bb == _boundaryBytes[match])
            {
                match++;
                if (match == _boundaryBytes.Length) return true;
            }
            else
            {
                match = (bb == _boundaryBytes[0]) ? 1 : 0;
            }
        }
        return false;
    }

    Dictionary<string, string> ReadHeaders(Stream s)
    {
        using (var ms = new MemoryStream())
        {
            if (!ReadUntilSequence(s, _headerSep, ms)) return null;
            var headerText = Encoding.ASCII.GetString(ms.ToArray());
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var lines = headerText.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                int colon = line.IndexOf(':');
                if (colon > 0)
                {
                    dict[line.Substring(0, colon).Trim()] = line.Substring(colon + 1).Trim();
                }
            }
            return dict;
        }
    }

    byte[] ReadExact(Stream s, int len)
    {
        byte[] buf = new byte[len];
        int off = 0, n;
        while (off < len && (n = s.Read(buf, off, len - off)) > 0) off += n;
        return (off == len) ? buf : null;
    }

    byte[] ReadUntilNextBoundary(Stream s, out bool okPart)
    {
        okPart = false;
        using (var ms = new MemoryStream())
        {
            var needle = Encoding.ASCII.GetBytes("\r\n--" + boundary);
            if (!ReadUntilSequence(s, needle, ms)) return null;
            var data = ms.ToArray();
            if (data.Length >= 2 && data[^2] == (byte)'\r' && data[^1] == (byte)'\n')
                Array.Resize(ref data, data.Length - 2);
            okPart = true;
            return data;
        }
    }

    bool ReadUntilSequence(Stream s, byte[] seq, MemoryStream outBuf)
    {
        int match = 0;
        int b;
        while (_running && (b = ReadByteSafe(s)) != -1)
        {
            byte bb = (byte)b;
            outBuf.WriteByte(bb);
            if (bb == seq[match]) { match++; if (match == seq.Length) break; }
            else match = (bb == seq[0]) ? 1 : 0;
            if (outBuf.Length > 32 * 1024 * 1024) return false;
        }
        if (outBuf.Length >= seq.Length)
        {
            outBuf.SetLength(outBuf.Length - seq.Length);
            return true;
        }
        return false;
    }

    int ReadByteSafe(Stream s)
    {
        try { return s.ReadByte(); }
        catch (IOException) { return -1; }
    }
}
