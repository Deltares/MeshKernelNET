using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("MeshKernelNETCoreTest")]

namespace MeshKernelNETCore.Api
{
    internal class StringBufferSizeAttribute : Attribute
    {
        public int BufferSize { get; set; }
    }
}