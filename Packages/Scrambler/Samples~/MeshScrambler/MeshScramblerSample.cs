using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Scrambler.Samples
{
    public class MeshScramblerSample : MonoBehaviour
    {
        [SerializeField] private Renderer meshRenderer;
        [SerializeField] private Text message;
        
        private Mesh mesh;
        private int[] _srcTriangles;
        
        async void Start()
        {
            if (meshRenderer is SkinnedMeshRenderer skinnedMeshRenderer)
            {
                mesh = skinnedMeshRenderer.sharedMesh;
            }
            else
            {
                var meshFilter = meshRenderer.gameObject.GetComponent<MeshFilter>();
                if (meshFilter != null) { mesh = meshFilter.mesh; }
            }

            if (mesh is null)
            {
                message.color = Color.red;
                message.text = "Mesh is null.";
                return;
            }

            var triangles = mesh.triangles;
            var vertexCount = mesh.vertexCount;

            _srcTriangles = new int[triangles.Length];
            for (var i = 0; i < triangles.Length; i++)
            {
                _srcTriangles[i] = triangles[i];
            }

            var scrambler = new AcmTriangleMeshIndicesScrambler(mesh.triangles, vertexCount);
            for (var iter = 0; iter < 3000; iter++)
            {
                await Task.Run(() => { scrambler.Scramble(1); });
                mesh.SetTriangles(scrambler.ScrambledIndices, 0);

                if (scrambler.ScrambledIndices.SequenceEqual(_srcTriangles))
                {
                    message.text = $"Iteration: {iter}\nReconstructed the original mesh. ";
                    return;
                }

                message.text = $"Iteration: {iter}";
            }
        }
        
        private void OnDestroy()
        {
            mesh.SetTriangles(_srcTriangles, 0);
        }
    }
}