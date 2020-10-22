using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace PackageManager
{
    [Serializable]
    public class UdpPackage
    {
        public int Number { get; set; }
        public ulong PackageNumber { get; set; }

        public byte[] ToByteArray()
        {
            using (MemoryStream m = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(m))
                {
                    writer.Write(Number);
                    writer.Write(PackageNumber);
                }
                return m.ToArray();
            }
        }

        public  void Desserialize(byte[] data)
        {
            
            using (MemoryStream m = new MemoryStream(data))
            {
                using (BinaryReader reader = new BinaryReader(m))
                {
                    Number = reader.ReadInt32();
                    PackageNumber = reader.ReadUInt64();
                }
            }
        }

    }


}
