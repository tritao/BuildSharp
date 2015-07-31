using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace BuildSharp
{
    /// <summary>
    /// Represents the kind of the diagnostic.
    /// </summary>
    public enum DiagnosticKind
    {
        Debug,
        Message,
        Warning,
        Error
    }

    /// <summary>
    /// Keeps information related to a single diagnostic.
    /// </summary>
    public struct DiagnosticInfo
    {
        public DiagnosticKind Kind;
        public string Message;
        public string File;
        public int Line;
        public int Column;
    }

    public interface ILog
    {
        bool Verbose { get; set; }
        void Emit(DiagnosticInfo info);
        void PushIndent(int level);
        void PopIndent();
    }

    public static class DiagnosticExtensions
    {
        public static void Debug(this ILog consumer, string msg,
            params object[] args)
        {
            var diagInfo = new DiagnosticInfo
            {
                Kind = DiagnosticKind.Debug,
                Message = string.Format(msg, args)
            };

            consumer.Emit(diagInfo);
        }

        public static void Message(this ILog consumer, string msg,
            params object[] args)
        {
            var diagInfo = new DiagnosticInfo
            {
                Kind = DiagnosticKind.Message,
                Message = string.Format(msg, args)
            };

            consumer.Emit(diagInfo);
        }

        public static void Warning(this ILog consumer, string msg, 
            params object[] args)
        {
            var diagInfo = new DiagnosticInfo
            {
                Kind = DiagnosticKind.Warning,
                Message = string.Format(msg, args)
            };

            consumer.Emit(diagInfo);
        }

        public static void Error(this ILog consumer, string msg,
            params object[] args)
        {
            var diagInfo = new DiagnosticInfo
            {
                Kind = DiagnosticKind.Error,
                Message = string.Format(msg, args)
            };

            consumer.Emit(diagInfo);
        }
    }

    public class ConsoleLog : ILog
    {
        public Stack<int> Indents;

        public ConsoleLog()
        {
            Indents = new Stack<int>();
        }

        public bool Verbose { get; set; }

        public void Emit(DiagnosticInfo info)
        {
            if (info.Kind == DiagnosticKind.Debug && !Verbose)
                return;

            var currentIndent = Indents.Sum();
            var message = new string(' ', currentIndent) + info.Message;

            Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public void PushIndent(int level)
        {
            Indents.Push(level);
        }

        public void PopIndent()
        {
            Indents.Pop();
        }
    }

    public static class Log
    {
        public static ILog Logger = new ConsoleLog { Verbose = true};

        public static void Debug(string msg, params object[] args)
        {
            Logger.Debug(msg, args);
        }

        public static void Message(string msg, params object[] args)
        {
            Logger.Message(msg, args);
        }

        public static void Warning(string msg, params object[] args)
        {
            Logger.Warning(msg, args);
        }

        public static void Error(string msg, params object[] args)
        {
            Logger.Error(msg, args);
        }
    }
}
