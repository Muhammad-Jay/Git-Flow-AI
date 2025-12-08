using System.Net;

namespace GitFlowAi.Server
{
    public class WebServer
    {
        public static async Task StartServer(string htmlTemplate)
        {
            using (HttpListener listener = new HttpListener())
            {
                listener.Prefixes.Add("http://localhost:8080/");
                listener.Prefixes.Add("http://127.0.0.1:8080/");

                try
                {
                    listener.Start();
                    Console.WriteLine("HTTP server started on http://localhost:8080/");

                    while (true)
                    {
                        HttpListenerContext context = await listener.GetContextAsync();
                        HttpListenerRequest request = context.Request;
                        HttpListenerResponse response = context.Response;

                        Console.WriteLine($"Request URL: {request.Url}");
                        
                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(htmlTemplate);
                        response.ContentLength64 = buffer.Length;
                        System.IO.Stream output = response.OutputStream;

                        await output.WriteAsync(buffer, 0, buffer.Length);
                        output.Close();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
                finally
                {
                    listener.Stop();
                }
            }
        }
        
    }
}