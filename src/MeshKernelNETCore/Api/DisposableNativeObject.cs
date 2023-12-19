using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using MeshKernelNETCore.Helpers;

namespace MeshKernelNETCore.Api
{
    /// <summary>
    /// Base class for disposable mesh objects. Provides pinning of arrays in memory for exchange with native calls.
    /// </summary>
    public abstract class DisposableNativeObject<TNative> : IDisposable where TNative : new()
    {
        private readonly Dictionary<object,GCHandle> objectGarbageCollectHandles = new Dictionary<object, GCHandle>();
        private bool disposed;

        /// <summary>
        /// Disposes the unmanaged resources
        /// </summary>
        ~DisposableNativeObject()
        {
            Dispose(false);
        }

        /// <summary>
        /// Indicates if arrays are pinned in memory
        /// </summary>
        private bool IsMemoryPinned
        {
            get { return objectGarbageCollectHandles.Count > 0; }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a native structure for this object
        /// </summary>
        /// <returns></returns>
        public TNative CreateNativeObject()
        {
            if (!IsMemoryPinned)
            {
                PinMemory();
            }

            var nativeObject = new TNative();
            SetNativeObject(ref nativeObject);
            return nativeObject;
        }

        /// <summary>
        /// Maps the <typeparamref name="TNative"/> object with current state (used in <see cref="CreateNativeObject"/>)
        /// </summary>
        /// <param name="nativeObject">Newly created native object</param>
        protected abstract void SetNativeObject(ref TNative nativeObject);

        /// <summary>
        /// Get the pointer to the pinned object
        /// </summary>
        /// <param name="objectToLookUp">Object to get </param>
        /// <returns></returns>
        protected IntPtr GetPinnedObjectPointer(object objectToLookUp)
        {
            return objectGarbageCollectHandles[objectToLookUp].AddrOfPinnedObject();
        }

        /// <summary>
        /// Disposes resources
        /// </summary>
        /// <param name="disposing">Boolean indicating that the call is called from the dispose method
        /// not the destructor</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            ReleaseUnmanagedResources();

            disposed = true;
        }

        /// <summary>
        /// Pins the arrays in memory (no garbage collect until unpinned (done in dispose))
        /// </summary>
        private void PinMemory()
        {
            var arrayProperties = GetType().GetProperties().Where(f => f.PropertyType.IsArray);
            
            // force initialization
            foreach (var arrayProperty in arrayProperties)
            {
                var elementType = arrayProperty.PropertyType.GetElementType();
                var objectToPin = arrayProperty.GetValue(this);

                if (objectToPin == null)
                {
                    objectToPin = Array.CreateInstance(elementType, 0);
                    arrayProperty.SetValue(this, objectToPin);
                }

                if (elementType == typeof(string))
                {
                    var bufferSize = GetType().GetBufferSize(arrayProperty.Name);
                    if (bufferSize == 0) continue;

                    var bytes = ((string[])objectToPin).GetFlattenedAsciiCodedStringArray(bufferSize);
                    AddObjectToPin(bytes, objectToPin);
                }
                else
                {
                    AddObjectToPin(objectToPin);
                }
            }
        }

        private void UnPinMemory()
        {
            foreach (var valuePair in objectGarbageCollectHandles)
            {
                valuePair.Value.Free();
            }

            objectGarbageCollectHandles.Clear();
        }

        private void AddObjectToPin(object objectToPin, object lookupObject = null)
        {
            var key = lookupObject ?? objectToPin;
            objectGarbageCollectHandles.Add(key, GCHandle.Alloc(objectToPin, GCHandleType.Pinned));
        }

        private void ReleaseUnmanagedResources()
        {
            UnPinMemory();
        }
    }
}