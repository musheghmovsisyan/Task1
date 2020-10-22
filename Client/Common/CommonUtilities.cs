using System;
using System.Collections.Generic;
using System.IO;
using XmlFileManager;

namespace Client.Common

{
    public static class CommonUtilities
    {

        private static ClientConfig _data = null;
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

        public static ClientConfig ClientConfigData
        {
            get
            {
                if (_data is null)
                {
                    if (File.Exists(FilePath))
                    {
                        _data = XmlManager.XmlDataReaderClient(FilePath);
                    }
                    else
                    {
                        _data = new ClientConfig();
                    }
                }
                return _data;
            }
        }

      
    }
}
