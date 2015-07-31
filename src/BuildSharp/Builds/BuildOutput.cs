using BuildSharp.Utils;

namespace BuildSharp.Builds
{
    /// <summary>
    /// Represents the output of a build.
    /// </summary>
    public class BuildOutput : BlockGenerator
    {
        public void PushGroup(string msg, params object[] args)
        {
            PushBlock(BlockKind.Group);
            PushIndent();
            WriteLine(msg, args);
        }

        public void PopGroup()
        {
            PopIndent();
            PopBlock();
        }

        //public override string ToString()
        //{
        //    return base.Generate();
        //}
    }
}
