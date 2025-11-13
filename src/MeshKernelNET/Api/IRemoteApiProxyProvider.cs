namespace MeshKernelNET.Api
{
    /// <summary>
    /// Provides a way to create and release remote API proxy instances.
    /// </summary>
    public interface IRemoteApiProxyProvider
    {
        /// <summary>
        /// Creates a new API proxy instance.
        /// </summary>
        IMeshKernelApi CreateProxy();

        /// <summary>
        /// Releases or disposes of the API proxy instance.
        /// </summary>
        void ReleaseProxy(IMeshKernelApi proxy);

        /// <summary>
        /// Checks whether the API proxy instance is still alive.
        /// </summary>
        bool IsProxyAlive(IMeshKernelApi proxy);
    }
}