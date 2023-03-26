// -----
//
// MIT License
//
// Copyright (c) 2023 Soichiro Sugimoto
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// -----

using System;

namespace Scrambler
{
    /// <summary>
    /// Encryption based on Arnold Cat Map.
    /// </summary>
    public sealed class AcmTriangleMeshIndicesScrambler : IMeshIndicesScrambler
    {
        public int[] ScrambledIndices => _scrambledIndices;
        
        private readonly int[] _scrambledIndices;
        private readonly int _vertexCount;
        private readonly int _a;
        private readonly int _b;
        
        public AcmTriangleMeshIndicesScrambler(int[] triangles, int vertexCount, int a = 1, int b = 1)
        {
            if (triangles.Length % 3 != 0)
            {
                throw new ArgumentException();
            }
            
            _scrambledIndices = new int[triangles.Length];
            _vertexCount = vertexCount;
            _a = a;
            _b = b;
            
            Initialize(triangles);
        }
        
        public void Initialize(int[] indices)
        {
            for (var i = 0; i < indices.Length; i++)
            {
                _scrambledIndices[i] = indices[i];
            }
        }
        
        public void Scramble(int iteration)
        {
            var ab1 = (_a * _b + 1);
            for (var iter = 0; iter < iteration; iter++)
            {
                for (var i = 0; i < _scrambledIndices.Length / 3; i++)
                {
                    var x = _scrambledIndices[3 * i + 0];
                    var y = _scrambledIndices[3 * i + 1];
                    var z = _scrambledIndices[3 * i + 2];
                    
                    // Reference:
                    // "2. Arnold Transform" from
                    // "A New Transformation of 3D Models Using Chaotic Encryption Based on Arnold Cat Map".
                    _scrambledIndices[3 * i + 0] = (x + _a * z) % _vertexCount;
                    _scrambledIndices[3 * i + 1] = y % _vertexCount;
                    _scrambledIndices[3 * i + 2] = (_b * x + ab1 * z) % _vertexCount;
                }
            }
        }
    }
}