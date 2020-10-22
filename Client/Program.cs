using Client.Common;
using PackageManager;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static object locker = new object();

        const int intMax = Int32.MaxValue;

        static void Main(string[] args)
        {
            Console.WriteLine("Client Started ...");

            UdpClient udpclient = new UdpClient(CommonUtilities.ClientConfigData.Port);
            IPAddress multicastaddress = IPAddress.Parse(CommonUtilities.ClientConfigData.MulticastAddress);
            udpclient.JoinMulticastGroup(multicastaddress);

            IPEndPoint remote = null;
            Encoding enc = Encoding.Unicode;

            Quotation quotations = new Quotation();



            List<int> Items = new List<int>();
            UdpPackage udpPackage = new UdpPackage();


            Task.Run(() =>
            {

                while (true)
                {
                    bool acquiredLockReceive = false;

                    try
                    {
                        Monitor.Enter(locker, ref acquiredLockReceive);

                        if (Items.Count < intMax)
                        {
                            Thread.Sleep(CommonUtilities.ClientConfigData.Delay);

                            Byte[] data = udpclient.Receive(ref remote);

                            udpPackage.Desserialize(data);
                            Items.Add(udpPackage.Number);

                        }
                        else
                        {
                            quotations.Init(Items, udpPackage.PackageNumber);
                            Items = new List<int>();
                        }


                        Console.WriteLine(udpPackage.Number.ToString());
                    }
                    catch (Exception ex) {      /*Console.WriteLine(ex.Message);   */       }
                    catch { }
                    finally
                    {
                        if (acquiredLockReceive) Monitor.Exit(locker);
                    }



                }
            });

            Console.WriteLine("Press Enter");
            ulong ItemsCount = 0;



            while (true)
            {
                bool acquiredLockStatistics = false;

                ConsoleKeyInfo keyInfo = Console.ReadKey();

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    var taskSt = Task.Run(() =>
                      {
                          try
                          {
                              Monitor.Enter(locker, ref acquiredLockStatistics);

                              quotations.Init(Items, udpPackage.PackageNumber);
                              Items = new List<int>();

                          }
                          finally
                          {
                              if (acquiredLockStatistics) Monitor.Exit(locker);
                          }
                      });
                    Task.WaitAll(taskSt);
                    Task.Run(() =>
                    {
                        quotations.CalculateStatistics();

                        Console.WriteLine("Press Enter");
                    });

                }
            }




        }
    }
}
