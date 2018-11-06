using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class Pixelate : MonoBehaviour
{
    [Range(64f, 512f)] public float Blockiness = 128;
    [Range(1, 3)] public int FrameDivider = 2;
    private Shader shader;
    private Material _material;
    private new Camera camera;
    private int frameCount;
    private RenderTexture lastTexture;

    private Material material
    {
        get
        {
            if(!_material)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
                Vector2 count = new Vector2(Blockiness, Blockiness / camera.aspect);
                _material.SetVector("count", count);
                _material.SetVector("size", new Vector2(1.0f / count.x, 1.0f / count.y));
            }
            return _material;
        }
    }

    void Start()
    {
        camera = GetComponent<Camera>();
        shader = Shader.Find("Hidden/Pixelate");
        frameCount = FrameDivider;
    }

    void OnDisable()
    {
        if (material)
        {
            DestroyImmediate(material);
        }
        if(lastTexture)
        {
            DestroyImmediate(lastTexture);
        }
        frameCount = FrameDivider;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!lastTexture) lastTexture = new RenderTexture(destination.width, destination.height, destination.depth);
        if (frameCount >= FrameDivider)
        {
            Graphics.Blit(source, destination, material);
            Graphics.Blit(source, lastTexture, material);
            frameCount = 0;
        }
        else
        {
            Graphics.Blit(lastTexture, destination, material);
            frameCount++;
        }
    }
}
