using MeshKernelNET.Helpers;
using MeshKernelNET.Native;
using ProtoBuf;

namespace MeshKernelNET.Api
{
    [ProtoContract(AsReferenceDefault = true)]
    public sealed class DisposableContacts : DisposableNativeObject<ContactsNative>
    {
        [ProtoMember(1)]
        private int[] mesh1dIndices;

        [ProtoMember(2)]
        private int[] mesh2dIndices;

        [ProtoMember(3)]
        private int numContacts;

        public DisposableContacts()
        {
        }

        public DisposableContacts(int nContacts)
        {
            NumContacts = nContacts;
            Mesh1dIndices = new int[NumContacts];
            Mesh2dIndices = new int[NumContacts];
        }

        ~DisposableContacts()
        {
            Dispose(false);
        }

        public int[] Mesh1dIndices
        {
            get { return mesh1dIndices; }
            set { mesh1dIndices = value; }
        }

        public int[] Mesh2dIndices
        {
            get { return mesh2dIndices; }
            set { mesh2dIndices = value; }
        }

        public int NumContacts
        {
            get { return numContacts; }
            set { numContacts = value; }
        }

        protected override void SetNativeObject(ref ContactsNative nativeObject)
        {
            nativeObject.mesh1d_indices = GetPinnedObjectPointer(Mesh1dIndices);
            nativeObject.mesh2d_indices = GetPinnedObjectPointer(Mesh2dIndices);
            nativeObject.num_contacts = NumContacts;
        }

        public void UpdateFromNativeObject(ref ContactsNative nativeObject)
        {
            Mesh1dIndices = nativeObject.mesh1d_indices.CreateValueArray<int>(nativeObject.num_contacts);
            Mesh2dIndices = nativeObject.mesh2d_indices.CreateValueArray<int>(nativeObject.num_contacts);
            NumContacts = nativeObject.num_contacts;
        }
    }
}