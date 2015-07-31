using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BuildSharp
{
    /// <summary>
    /// Listens to HTTP requests from outside (GitHub).
    /// </summary>
    public class HttpServer
    {
        readonly HttpListener listener;
        bool stopServer;

        public HttpServer()
        {
            listener = new HttpListener();
        }

        public void Start()
        {
            if (listener.IsListening)
                return;

            // To be able to listen in Windows we need to register:
            //  netsh http add urlacl url=http://+:80/ user=triton
            listener.Prefixes.Add("http://+:80/");
            listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
            listener.Start();
        }

        public void Stop()
        {
            if (!listener.IsListening)
                return;

            listener.Stop();
            stopServer = true;
        }

        public async Task ProcessAsync()
        {
            while (!stopServer)
            {
                var ctx = await listener.GetContextAsync();
                Perform(ctx);
            }
        }

        private static int i = 0;

        static void Perform(HttpListenerContext ctx)
        {
            Console.WriteLine(ctx.Request.Url);

            var html = "<HTML><BODY> Hello world!" + i++ + "</BODY></HTML>";
            var buffer = Encoding.UTF8.GetBytes(html);

            // Get a response stream and write the response to it.
            using (var response = ctx.Response)
            {
                ctx.Response.ContentLength64 = buffer.Length;
                ctx.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
        }
    }
}
