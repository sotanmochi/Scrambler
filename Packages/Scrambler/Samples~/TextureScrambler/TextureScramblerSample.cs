using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scrambler.Samples
{
    public class TextureScramblerSample : MonoBehaviour
    {
        [SerializeField] private Texture image;
        [SerializeField] private RawImage sourceImage;
        [SerializeField] private RawImage scrambledImage;
        [SerializeField] private Text message;

        async void Start()
        {
            sourceImage.texture = image;

            var source = sourceImage.texture as Texture2D;
            if (source is null)
            {
                message.color = Color.red;
                message.text = "The source image is not Texture2D.";
                return;
            }

            var srcPixels = source.GetPixels32();
            var srcPixels32Bytes = new byte[srcPixels.Length * 4];            
            for (int i = 0; i < srcPixels.Length; i++)
            {
                srcPixels32Bytes[i * 4 + 0] = srcPixels[i].r;
                srcPixels32Bytes[i * 4 + 1] = srcPixels[i].g;
                srcPixels32Bytes[i * 4 + 2] = srcPixels[i].b;
                srcPixels32Bytes[i * 4 + 3] = srcPixels[i].a;
            }

            var dst = new Texture2D(source.width, source.height, TextureFormat.RGBA32, false);
            var scrambler = new AcmTextureScrambler(srcPixels32Bytes, source.width, source.height);
            for (int iter = 0; iter < 3000; iter++)
            {
                await Task.Run(() => scrambler.Scramble(1));

                dst.LoadRawTextureData(scrambler.ScrambledPixels32Bytes);
                scrambledImage.texture = dst;
                dst.Apply();

                if (scrambler.ScrambledPixels32Bytes.SequenceEqual(srcPixels32Bytes))
                {
                    message.text = $"Iteration: {iter}\nReconstructed the original image. ";
                    return;
                }

                message.text = $"Iteration: {iter}";
            }
        }
    }
}