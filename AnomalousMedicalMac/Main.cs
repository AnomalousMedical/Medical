using System;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using Medical;
using System.Net.Http;
using MonoMac.HttpClient;

namespace AnomalousMedicalMac
{
    class MainClass
    {
        static void Main(string[] args)
        {
            ServerConnection.HttpClientProvider = () => new HttpClient(new NativeMessageHandler());

            Medical.Main.Run();
        }
    }
}