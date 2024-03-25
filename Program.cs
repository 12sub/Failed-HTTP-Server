// See https://aka.ms/new-console-template for more information
using System;
using System.Net;
using System.IO;

public class HttpServer
{
    public int Port = 8080;
    private HttpListener Hlistener;

    public void Start()
    {
        Hlistener = new HttpListener();
        Hlistener.Prefixes.Add("http://127.0.0.1:" + Port.ToString() + "/");
        Hlistener.Start();
        Receive();
        
    }
    public void Stop()
    {
        Hlistener.Stop();
    }
    private void Receive()
    {
        Hlistener.BeginGetContext(new AsyncCallback(ListenerCallback), Hlistener);
    }
    private void ListenerCallback(IAsyncResult ar)
    {
        if (Hlistener.IsListening)
        {
            var context = Hlistener.EndGetContext(ar);
            var request = context.Request;

            //Reporting on queries
            Console.WriteLine($"{request.HttpMethod} {request.Url}");
            if (request.HasEntityBody)
            {
                var body = request.InputStream;
                var encoding = request.ContentEncoding;
                var readln = new StreamReader(body, encoding);
                if (request.ContentType != null)
                {
                    Console.WriteLine("Client data content type {0}");
                }
                Console.WriteLine("Client data content length {0}");

                Console.WriteLine("Start of Data: ");
                string stt = readln.ReadToEnd(); 
                Console.WriteLine(stt);
                Console.WriteLine("End of Data stream");
                readln.Close();
                body.Close();
            }
            // Providing a response
            var response = context.Response;
            response.StatusCode = (int) HttpStatusCode.OK;
            response.ContentType = "text/plain";
            response.OutputStream.Write(new byte[] { }, 0, 0);
            response.OutputStream.Close();
            Receive();
        }
    }
}


class Program
{
    private static bool _keepRunning = true;
    static void Main(string[] args)
    {
        Console.CancelKeyPress += delegate (object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            Program._keepRunning = false;
        };
        Console.WriteLine("Starting HTTP listener.....");
        var httpServer = new HttpServer();
        httpServer.Start();
        while (Program._keepRunning)
        {
           // Console.WriteLine("Thats all folks!!!");
            httpServer.Stop();
          
        }
    }
}