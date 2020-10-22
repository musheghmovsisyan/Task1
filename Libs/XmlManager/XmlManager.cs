using System.IO;
using System.Xml.Serialization;


namespace XmlFileManager
{
    public class XmlManager
    {

        //Xml reader
        public static T XmlDataReader<T>(string filename)     where T : new()
        {
            T obj = new T();
            XmlSerializer xs = new XmlSerializer(typeof(T));
            FileStream reader = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            obj = (T)xs.Deserialize(reader);
            reader.Close();
            return obj;
        }

        public static ClientConfig XmlDataReaderClient(string filename)
        {
            return XmlDataReader<ClientConfig>(filename);
        }

        public static ServerConfig XmlDataReaderServer(string filename)
        {
            return XmlDataReader<ServerConfig>(filename);
        }


    }
}
