using System.Collections;
using System.Collections.Generic;
using Unity.Robotics.ROSTCPConnector;
using RosMessageTypes.Sensor;
using UnityEngine;

public class CameraSubscriber : MonoBehaviour
{
    ROSConnection ros;
    // Start is called before the first frame update
    private Texture2D colorTexture;

    [SerializeField]
    private string cameraTopic;

    private bool newFrameAvailable = false;
    private ImageMsg latestImage; // Store only the latest frame

    void Start()
    {
        ROSConnection.GetOrCreateInstance().Subscribe<ImageMsg>(cameraTopic, ReceiveImage);
        
    }

    void ReceiveImage(ImageMsg imageMessage)
{
    // Verify if imageMessage.data matches the expected format of RGB.
    if (imageMessage.encoding != "rgb8")
    {
        Debug.LogError("Expected image encoding to be 'rgb8'. Received: " + imageMessage.encoding);
        return;
    }

    latestImage = imageMessage;
    newFrameAvailable = true;

    // if(colorTexture == null)
    //     {
    //         colorTexture = new Texture2D((int)imageMessage.width, (int)imageMessage.height, TextureFormat.RGB24, false);
    //         GetComponent<Renderer>().material.mainTexture = colorTexture;
    //     }

    //     colorTexture.LoadRawTextureData(imageMessage.data);
    //     colorTexture.Apply();

// olddddd
    // Create a new texture with the width and height from the message.
    // Texture2D texture = new Texture2D((int)imageMessage.width, (int)imageMessage.height, TextureFormat.RGB24, false);

    // Load the image data directly if it's already in byte format and compatible with RGB.
    // texture.LoadRawTextureData(imageMessage.data);
    // texture.Apply();

    // Apply the texture to the renderer.
    // GetComponent<Renderer>().material.mainTexture = texture;
}

void Update()
    {
        if (newFrameAvailable) // Process only the latest image
        {
            newFrameAvailable = false;
            ProcessImage(latestImage);
        }
    }

    void ProcessImage(ImageMsg imageMessage)
    {
        if (colorTexture == null)
        {
            colorTexture = new Texture2D((int)imageMessage.width, (int)imageMessage.height, TextureFormat.RGB24, false);
            GetComponent<Renderer>().material.mainTexture = colorTexture;
        }

        colorTexture.LoadRawTextureData(imageMessage.data);
        colorTexture.Apply();
    }


}
