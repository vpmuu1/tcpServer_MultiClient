using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class TcpServer
{
    private TcpListener _listener;
    private ConcurrentBag<TcpClient> _clients = new();

    public TcpServer(string ipAddress, int port)
    {
        _listener = new TcpListener(IPAddress.Parse(ipAddress), port);
    }

    public void Start()
    {
        Console.WriteLine("Server is starting...");
        _listener.Start();
        Console.WriteLine("Server started.");
        AcceptClientsAsync();
    }


    ConcurrentBag<TcpClient> remove(ConcurrentBag<TcpClient> bag, TcpClient c)
    {
        var newBag = new ConcurrentBag<TcpClient>();
        foreach (var item in bag)
        {
            if (!item.Equals(c))
            {
                newBag.Add(item);
            }
        }

        return newBag;
    }
    private async void AcceptClientsAsync()
    {
        while (true)
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine("Client connected.");
                _clients.Add(client);
                HandleClientAsync(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accepting client: {ex.Message}");
            }
        }
    }

    private async void HandleClientAsync(TcpClient client)
    {
        var buffer = new byte[1024];
        var stream = client.GetStream();

        try
        {
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Console.WriteLine("Client disconnected.");
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message.Trim()}");
                BroadcastMessage(client, message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            client.Close();            
            var tt = remove(_clients, client);
            _clients = tt;
            
        }
    }

    private void BroadcastMessage(TcpClient sender, string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);

        foreach (var client in _clients)
        {
            if (client == sender) 
                continue;

            try
            {
                var stream = client.GetStream();
                stream.WriteAsync(messageBytes, 0, messageBytes.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message to client: {ex.Message}");
            }
        }
    }

    public void Stop()
    {
        Console.WriteLine("Stopping server...");
        _listener.Stop();
        foreach (var client in _clients)
        {
            client.Close();
        }
        Console.WriteLine("Server stopped.");
    }

    public static void Main(string[] args)
    {
        //Console.WriteLine("Enter the server IP address (default 127.0.0.1):");
        //string ipAddress = Console.ReadLine();
        //if (string.IsNullOrWhiteSpace(ipAddress))
        //{

        //}

        string ipAddress = "127.0.0.1";
        //Console.WriteLine("Enter the server port (default 5000):");
        //if (!int.TryParse(Console.ReadLine(), out int port))
        //{
        //    port = 5000;
        //}

        int port = 5000;
        var server = new TcpServer(ipAddress, port);
        server.Start();

        Console.WriteLine("server 127.0.0.1:5000...");
        Console.WriteLine("Press Enter to stop the server...");
        Console.ReadLine();

        server.Stop();
    }
}
