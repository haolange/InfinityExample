using System.IO;
using UnityEngine;

[ExecuteAlways]
public class BestFit : MonoBehaviour
{
    void OnEnable()
    {
        Texture2D texBestFit = new Texture2D(1024, 1024, TextureFormat.RGBA32, false, true);
        var fbestfit = new BinaryReader(File.Open("Assets/NormalsFittingTexture_dds", FileMode.Open, FileAccess.Read));
        fbestfit.BaseStream.Seek(128, SeekOrigin.Begin);

        byte[] bytes = fbestfit.ReadBytes(1024 * 1024 * 4);
        fbestfit.Close();
        texBestFit.LoadRawTextureData(bytes);
        texBestFit.Apply();

        Shader.SetGlobalTexture("g_NormalScaleTable", texBestFit);
    }
}
