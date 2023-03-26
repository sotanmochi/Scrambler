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
    /// Texture encryption based on Arnold Cat Map.
    /// </summary>
    public sealed class AcmTextureScrambler : ITextureScrambler
    {
        struct Vector2Int
        {
            public int x;
            public int y;
        }
        
        public const int BytesPerPixel = 4;
        
        public byte[] ScrambledPixels32Bytes => _scrambledPixels32Bytes;
        
        private readonly byte[] _scrambledPixels32Bytes;
        private readonly Vector2Int[] _coordinates;
        private readonly int _width;
        private readonly int _height;
        private readonly int _a;
        private readonly int _b;
        
        private byte[] _sourcePixels32Bytes;
        
        public AcmTextureScrambler(byte[] pixels32Bytes, int width, int height, int a = 1, int b = 1)
        {
            if (width != height || (width & (width - 1)) != 0)
            {
                throw new ArgumentException("Input texture must be square and have dimensions power of 2.");
            }
            
            _width = width;
            _height = height;
            _a = a;
            _b = b;
            
            _scrambledPixels32Bytes = new byte[width * height * BytesPerPixel];
            _coordinates = new Vector2Int[width * height];
            
            Initialize(pixels32Bytes);
        }
        
        public void Initialize(byte[] pixels32Bytes)
        {
            _sourcePixels32Bytes = pixels32Bytes;
            
            for (var y = 0; y < _height; y++)
            {
                for (var x = 0; x < _width; x++)
                {
                    var pixelIndex = x * _height + y;
                    _coordinates[pixelIndex].x = x;
                    _coordinates[pixelIndex].y = y;
                }
            }
        }
        
        public void Scramble(int iteration)
        {
            var ab1 = (_a * _b + 1);
            for (var iter = 0; iter < iteration; iter++)
            {
                for (var index = 0; index < _coordinates.Length; index++)
                {
                    var x = _coordinates[index].x;
                    var y = _coordinates[index].y;
                    
                    // Reference:
                    // "2. Arnold Transform" from
                    // "A New Transformation of 3D Models Using Chaotic Encryption Based on Arnold Cat Map".
                    _coordinates[index].x = (ab1 * x + y) % _width;
                    _coordinates[index].y = (_a * x + _b * y) % _height;
                    // _coordinates[index].x = (_a * x + _b * y) % _width;
                    // _coordinates[index].y = (ab1 * x + y) % _height;
                }
            }
            
            for (var i = 0; i < _scrambledPixels32Bytes.Length / BytesPerPixel; i++)
            {
                var x = _coordinates[i].x;
                var y = _coordinates[i].y;
                var pixelIndex = x * _height + y;
                
                _scrambledPixels32Bytes[i * BytesPerPixel + 0] = _sourcePixels32Bytes[pixelIndex * BytesPerPixel + 0];
                _scrambledPixels32Bytes[i * BytesPerPixel + 1] = _sourcePixels32Bytes[pixelIndex * BytesPerPixel + 1];
                _scrambledPixels32Bytes[i * BytesPerPixel + 2] = _sourcePixels32Bytes[pixelIndex * BytesPerPixel + 2];
                _scrambledPixels32Bytes[i * BytesPerPixel + 3] = _sourcePixels32Bytes[pixelIndex * BytesPerPixel + 3];
            }
        }
    }
}