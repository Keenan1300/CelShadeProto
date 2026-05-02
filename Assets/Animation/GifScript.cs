using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GifScript : MonoBehaviour
{
    private Texture2D[] frames;
    private float fps = 10.0f;
    private Material mat;

    public Renderer renderer;

    private RawImage IMG;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
   
        mat = GetComponent<Renderer>().material;
        IMG = GetComponent<RawImage>();
    }

    // Update is called once per frame
    void Update()
    {
        int index = (int)(Time.time * fps);
        index = index % frames.Length;
        mat.mainTexture = frames[index]; // usar en planeObjects
        IMG.texture = frames [index];
    }
}
