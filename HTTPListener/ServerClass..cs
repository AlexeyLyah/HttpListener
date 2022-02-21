using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace HttpListenerExample
{
    class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static string projectDirectory = Directory.GetParent(System.Environment.CurrentDirectory).Parent.FullName;
       
        public static string pageData = File.ReadAllText($"{projectDirectory}/index.html", Encoding.UTF8);

        
        public static async Task HandleIncomingConnections()

  
        {
            bool runServer = true;
            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                string absolutePath = req.Url.AbsolutePath;
                string fileName = absolutePath == "/" ? "/index.html" : absolutePath;
                string pageData = "";
                try
                {
                    pageData = File.ReadAllText($"{projectDirectory}{fileName}", Encoding.UTF8);
                    string ext = Path.GetExtension($"{projectDirectory}{fileName}");
                    if (ext == ".html")
                    {
                        resp.ContentType = "text/html";
                    }
                    else if (ext == ".css")
                    {
                        resp.ContentType = "text/css";
                    }
                    else
                    {
                        resp.ContentType = "application/octet-stream";
                    }
                }
                catch
                {
                  pageData = "page not found";
                    resp.ContentType = "text/html";
                }
                byte[] data = Encoding.UTF8.GetBytes(pageData);
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }
        public static void Main(string[] args)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();
            listener.Close();
        }
    }
}

    