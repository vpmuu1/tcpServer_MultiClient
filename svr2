using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    private static TcpClient client1;
    private static TcpClient client2;
    static TcpListener listener;
    static async Task Main(string[] args)
    {
        listener = new TcpListener(IPAddress.Any, 5000);
        listener.Start();
        while (true)
        {
            Console.WriteLine("Server started...");
            Console.WriteLine("Waiting for clients...");
            await Main1(args);
        }
        
    }
    static async Task Main1(string[] args)
    {
        
        client1 = await listener.AcceptTcpClientAsync();
        Console.WriteLine("Client 1 connected...");

        client2 = await listener.AcceptTcpClientAsync();
        Console.WriteLine("Client 2 connected...");

        Task client1ToClient2 = ForwardMessages(client1, client2);
        Task client2ToClient1 = ForwardMessages(client2, client1);

        await Task.WhenAll(client1ToClient2, client2ToClient1);
    }

    private static async Task ForwardMessages(TcpClient fromClient, TcpClient toClient)
    {
        NetworkStream fromStream = fromClient.GetStream();
        NetworkStream toStream = toClient.GetStream();

        byte[] buffer = new byte[10240];
        int bytesRead;
        try
        {

            while ((bytesRead = await fromStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                await toStream.WriteAsync(buffer, 0, bytesRead);
                Console.WriteLine($"Forwarded {bytesRead} bytes");
            }
            toClient.Close();
            toClient.Dispose();

        }
        catch (Exception eee)
        {
            Console.WriteLine(eee.Message);
        }
        //Environment.Exit(0);
    }
}
