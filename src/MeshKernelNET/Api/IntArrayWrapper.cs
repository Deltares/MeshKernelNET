using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract]
    public sealed class IntArrayWrapper
    {
        public IntArrayWrapper()
        {
        }

        public IntArrayWrapper(int size)
        {
            Values = new int[size];
        }

        [ProtoMember(1)]
        public int[] Values { get; set; }
    }
}