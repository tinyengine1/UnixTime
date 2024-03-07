using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        // Specify the NTP server to query
        string ntpServer = "pool.ntp.org";

        try
        {
            while (true)
            {
                // Connect to the NTP server
                using (var client = new UdpClient(ntpServer, 123))
                {
                    // Create a request packet (the first byte is the request flags, see RFC 5905 for details)
                    byte[] requestPacket = new byte[48];
                    requestPacket[0] = 0x1B;

                    // Send the request packet to the NTP server
                    client.Send(requestPacket, requestPacket.Length);

                    // Receive the response packet from the NTP server
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] responsePacket = client.Receive(ref remoteEndPoint);

                    // Close the connection to the NTP server
                    client.Close();

                    // Extract the transmit timestamp from the response packet (bytes 40-43)
                    ulong transmitTimestamp = ((ulong)responsePacket[40] << 24) | ((ulong)responsePacket[41] << 16) |
                                               ((ulong)responsePacket[42] << 8) | responsePacket[43];

                    // Convert the transmit timestamp to Unix timestamp format
                    long unixTimestamp = (long)((transmitTimestamp - 2208988800UL) & 0x00000000FFFFFFFF);

                    // Clear the console screen
                    Console.Clear();

                    // Display the current time in Unix timestamp format
                    Console.WriteLine("Unix Time: " + unixTimestamp);
                    Console.WriteLine();
                    Console.WriteLine();

                    // Display the current time left before the Y2K38 problem
                    Console.WriteLine("Time left until Y2K38: " + ((unixTimestamp - 2147483647).ToString().Substring(1)));

                    Thread.Sleep(1000);
                }
            }
        }
        catch (Exception ex)
        {
            Console.Beep(1000, 500);
            Console.WriteLine("Error: " + ex.Message);
            Console.ReadKey();
        }
    }
}
