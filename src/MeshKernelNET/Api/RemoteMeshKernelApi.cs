using System;

namespace MeshKernelNET.Api
{
    /// <summary>
    /// Remote wrapper for <see cref="IMeshKernelApi"/> that executes operations in a separate process.
    /// </summary>
    public partial class RemoteMeshKernelApi : IMeshKernelApi
    {
        private readonly IMeshKernelApi api;
        private readonly IRemoteApiProxyProvider provider;
        private bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteMeshKernelApi"/> class.
        /// </summary>
        /// <param name="provider">Provides remote API proxy instances.</param>
        public RemoteMeshKernelApi(IRemoteApiProxyProvider provider)
        {
            this.provider = provider;

            api = provider.CreateProxy();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing && api != null)
            {
                if (provider.IsProxyAlive(api))
                {
                    api.Dispose();
                }

                provider.ReleaseProxy(api);
            }

            disposed = true;
        }
    }
}