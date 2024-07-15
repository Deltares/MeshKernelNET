using MeshKernelNET.Api;
using System;

// Define the MeshKernelState class
public class MeshKernelState : IDisposable
{
    private DisposableMesh2D disposableMesh2D;
    private bool disposed = false; // To detect redundant calls

    // Default constructor
    public MeshKernelState()
    {
        disposableMesh2D = new DisposableMesh2D();
    }

    // Example method that returns a reference to the managed DisposableMesh2D
    public DisposableMesh2D DisposableMesh2D
    {
        get { return disposableMesh2D; }
        set
        {
            if (disposableMesh2D != null && !disposed)
            {
                disposableMesh2D.Dispose();
            }
            disposableMesh2D = value;
        }
    }

    // Public implementation of Dispose pattern callable by consumers.
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    // Protected implementation of Dispose pattern.
    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
        {
            // Free any other managed objects here.
            if (disposableMesh2D != null)
            {
                disposableMesh2D.Dispose();
                disposableMesh2D = null;
            }
        }

        // Free any unmanaged objects here.
        disposed = true;
    }

    // Destructor
    ~MeshKernelState()
    {
        Dispose(false);
    }
}