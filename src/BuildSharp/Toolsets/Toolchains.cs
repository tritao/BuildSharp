using System.Collections.Generic;

namespace BuildSharp.Toolsets
{
    public enum ToolchainKind
    {
        Clang,
        GCC,
        MSVC
    }

    public abstract class Toolchain
    {
        public ToolchainKind Kind { get; protected set; }

        protected Toolchain(ToolchainKind kind)
        {

        }

        public abstract List<string> GetSystemIncludes();
    }
}
