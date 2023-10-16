using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MeshKernelNETTest")]

namespace MeshKernelNET.Api
{
    internal class StringBufferSizeAttribute : Attribute
    {
        public int BufferSize { get; set; }
    }
}