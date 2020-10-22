using PackageManager;
using Server.Common;
using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
    class Program
    {

        static void Main(string[] args)
        {
            Console.WriteLine("Server Started ...");

            UdpClient udpclient = new UdpClient();

            IPAddress multicastaddress = IPAddress.Parse(CommonUtilities.ServerConfigData.MulticastAddress);
            udpclient.JoinMulticastGroup(multicastaddress);
            IPEndPoint remoteep = new IPEndPoint(multicastaddress, CommonUtilities.ServerConfigData.Port);

            Byte[] buffer = null;
            UdpPackage udpPackage = new UdpPackage();

            ulong PackageNumber = 1;

            while (true)
            {
                try
                {
                    udpPackage.Number = CommonUtilities.GetRandomNumber;
                    udpPackage.PackageNumber = PackageNumber++;
                    buffer = udpPackage.ToByteArray();
                    udpclient.Send(buffer, buffer.Length, remoteep);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch { }


            }
        }
    }
}




