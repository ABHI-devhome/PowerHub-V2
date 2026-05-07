// Polyfill for C# 9 init-only setters on .NET Framework (no runtime BCL type).
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
