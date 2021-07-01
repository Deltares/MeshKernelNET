using MeshKernelNETCore.Native;
using ProtoBuf;

namespace MeshKernelNETCore.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableContacts : DisposableMeshObject
    {
        [ProtoMember(1)]
        public int[] mesh1dIndices;

        [ProtoMember(2)]
        public int[] mesh2dIndices;

        [ProtoMember(3)]
        public int numContacts;


        public DisposableContacts() { }

        public DisposableContacts(int nContacts, int nEdges)
        {
            numContacts = nContacts;
            mesh1dIndices = new int[numContacts];
            mesh2dIndices = new int[numContacts];
        }

        public Contacts CreateContacts()
        {
            if (!IsMemoryPinned)
            {
                PinMemory();
            }

            return new Contacts
            {
                mesh1d_indices = GetPinnedObjectPointer(mesh1dIndices),
                mesh2d_indices = GetPinnedObjectPointer(mesh2dIndices),
                num_contacts = numContacts,
            };
        }

        ~DisposableContacts()
        {
            Dispose(false);
        }
    }
}