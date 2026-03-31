using System;
using System.Runtime.CompilerServices;

namespace MeshKernelNET.Api
{
    internal class StringBufferSizeAttribute : Attribute
    {
        public int BufferSize { get; set; }
    }
}