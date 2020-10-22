using System;
using System.IO;
using XmlFileManager;

namespace Server.Common

{
    public static class CommonUtilities
    {
        private static Random random = new Random(DateTime.Now.Millisecond);
        private static ServerConfig _data = null;
        private static string _FilePath = null;

        private static string FilePath
        {
            get
            {
                if (_FilePath is null)
                {
                    _FilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "..", "..", "Config", "Config.xml"));
                }
                return _FilePath;
            }
        }

        public static ServerConfig ServerConfigData
        {
            get
            {
                if (_data is null)
                {
                    if (File.Exists(FilePath))
                    {
                        _data = XmlManager.XmlDataReaderServer(FilePath);
                    }
                    else
                    {
                        _data = new ServerConfig();
                    }
                }
                return _data;
            }
        }

        public static int GetRandomNumber
        {
            get
            {
                return random.Next(ServerConfigData.StartNumber, ServerConfigData.EndNumber + 1);
            }

        }


    }
}

