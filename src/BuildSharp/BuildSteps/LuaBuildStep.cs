using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using BuildSharp.Builds;
using BuildSharp.Toolsets;
using Eluant;
using ProcessReadWriteUtils;

namespace BuildSharp.Steps
{
    /// <summary>
    /// Represents the output of a process execution.
    /// </summary>
    public struct CommandOutput
    {
        public string Executable;
        public string Arguments;
        public List<string> Lines;
        public List<string> Raw;
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class BindLuaAttribute : Attribute
    {
        public BindLuaAttribute(string name = null)
        {
            Name = name;
        }

        public string Name;
    }

    /// <summary>
    /// Build step responsible for calling a Lua script.
    /// </summary>
    public class LuaBuildStep : BuildStep, IDisposable
    {
        /// <summary>
        /// Lua runtime instance.
        /// </summary>
        private readonly LuaRuntime luaRuntime;

        /// <summary>
        /// Outputs for each command execution.
        /// </summary>
        public List<CommandOutput> Outputs;

        public LuaBuildStep(BuildAgent agent, Build build)
            : base(agent, build)
        {
            luaRuntime = new LuaRuntime();
            Outputs = new List<CommandOutput>();
        }

        public override void Run()
        {
            SetupEnvironment();
            LoadScripts(".");
        }

        public void Dispose()
        {
            luaRuntime.Dispose();
        }

        #region Process execution

        private void ExecuteProcess(string cmd, string args,
            Action<string, CommandOutput> handleOutput)
        {
            var command = new CommandOutput
            {
                Executable = cmd,
                Arguments = args,
                Lines = new List<string>(),
                Raw = new List<string>()
            };

            var info = new ProcessStartInfo
            {
                WorkingDirectory = Directory.GetCurrentDirectory(),
                FileName = cmd,
                Arguments = args,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = info })
            {
                try
                {
                    process.Start();
                }
                catch (Exception ex)
                {
                    throw new BuildException(
                        string.Format("Error launching process: {0} {1}",
                        cmd, args));
                }

                var handleProcessOutput = new Action<string>(text =>
                {
                    lock (this) { handleOutput(text, command); }
                });

                var io = new ProcessIoManager(process);
                io.StdoutTextRead += new StringReadEventHandler(handleProcessOutput);
                io.StderrTextRead += new StringReadEventHandler(handleProcessOutput);
                io.StartProcessOutputRead();

                process.WaitForExit();

                Thread.Sleep(100);

                process.Close();
                io.StopMonitoringProcessOutput();
            }

            Outputs.Add(command);
        }

        #endregion

        #region Lua extensions

        [BindLua("buildstep")]
        void BuildStep(string name, LuaFunction step)
        {
            Output.PushGroup("Build step '{0}'", name);
            step.Call();
            Output.PopGroup();
        }

        [BindLua("netframeworkpath")]
        string NETFrameworkPath()
        {
            string path;
            MSVCToolchain.GetNetFrameworkDir(out path);

            return path;
        }

        [BindLua("msbuildpath")]
        string MSBuildPath()
        {
            string path;
            MSVCToolchain.GetMSBuildSDKDir(out path);

            return path;
        }

        [BindLua("push")]
        void Push(string name)
        {
            Output.PushGroup(name);
        }

        [BindLua("pop")]
        void Pop()
        {
            Output.PopGroup();
        }

        [BindLua("execute")]
        void Execute(string cmd, string args, LuaFunction filter)
        {
            var handleOutput = new Action<string, CommandOutput>(
                (text, command) =>
            {
                // Clean up new lines in the end of the string.
                if (text.EndsWith(Environment.NewLine))
                    text = text.Replace(Environment.NewLine, string.Empty);

                command.Raw.Add(text);

                if (filter == null)
                    goto SkipFilter;

                var result = filter.Call(text);
                if (result.Count == 0)
                    return;

                text = result[0].ToString();

            SkipFilter:

                command.Lines.Add(text);
                Build.Output.WriteLine(text);
            });

            ExecuteProcess(cmd, args, handleOutput);
        }

        #endregion

        #region Lua runtime

        /// <summary>
        /// Sets up the Lua environment with builtins.
        /// </summary>
        public void SetupEnvironment()
        {
            foreach (var step in Build.Steps)
                RegisterLuaMethods(step);
        }

        /// <summary>
        /// Registers methods with [LuaBind] attributes in Lua.
        /// </summary>
        private void RegisterLuaMethods(object obj)
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Instance
                                       | BindingFlags.Public | BindingFlags.NonPublic;
            foreach (var method in obj.GetType().GetMethods(flags))
            {
                var attr = method.GetCustomAttributes(typeof(BindLuaAttribute),
                    inherit: false).FirstOrDefault() as BindLuaAttribute;
                if (attr == null) continue;

                var wrapper = new LuaRuntime.MethodWrapper(this, method);
                var fn = luaRuntime.CreateFunctionFromMethodWrapper(wrapper);

                var name = string.IsNullOrEmpty(attr.Name) ? method.Name : attr.Name;
                luaRuntime.Globals[name] = fn;
            }
        }

        /// <summary>
        /// Loads the Lua scripts found in the path.
        /// </summary>
        public void LoadScripts(string path)
        {
            Log.Message("Loading Lua scripts...");

            // Searching for a Lua script describing the builds.
            foreach (var file in Directory.EnumerateFiles(path))
            {
                if (!file.EndsWith(".lua"))
                    continue;

                var code = File.ReadAllText(file);
                var function = CompileCode(code);

                Log.Message("Running {0}...", Path.GetFileName(file));
                RunCode(function);
            }
        }

        private static void RunCode(LuaFunction function)
        {
            try
            {
                function.Call();
            }
            catch (Exception ex)
            {
                throw new BuildException("Error in Lua script: " + ex.ToString());
            }
        }

        private LuaFunction CompileCode(string code)
        {
            LuaFunction function;
            try
            {
                function = luaRuntime.CompileString(code);
            }
            catch (Exception ex)
            {
                throw new BuildException("Error parsing Lua: " + ex.ToString());
            }

            return function;
        }

        #endregion
    }
}
