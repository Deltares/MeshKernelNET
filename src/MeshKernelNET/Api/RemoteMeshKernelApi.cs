namespace MeshKernelNET.Api
{
    /// <summary>
    /// Remote wrapper for <see cref="IMeshKernelApi"/> that executes operations in a separate process.
    /// </summary>
    public sealed partial class RemoteMeshKernelApi : IMeshKernelApi
    {
        private readonly IRemoteApiProxyProvider provider;
        private IMeshKernelApi api;

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
            if (api == null)
            {
                return;
            }

            if (provider.IsProxyAlive(api))
            {
                api.Dispose();
            }

            provider.ReleaseProxy(api);
            api = null;
        }
    }
}