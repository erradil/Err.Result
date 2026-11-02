// Polyfill required to use C# records (init-only setters) on netstandard2.0.
#if NETSTANDARD2_0
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
#endif
