using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Reproductor_de_Musica.Utilidades
{
    [Serializable]
    public static class Utilities
    {
        public static void CreateFile(string nombre, List<string> data)
        {
            FileStream stream = new FileStream($"{nombre}.pytham", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);

            stream.Close();
        }
       public static List<string> GetFile(string nombre)
       {
            
            FileStream stream = new FileStream($"{nombre}.pytham", FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            List<string> ls = (List<string>)formatter.Deserialize(stream);

            stream.Close();

            return ls;
       }

       public static void SaveData(string nombre, List<string> list)
       {
            if (File.Exists($"{nombre}.pytham"))
                File.Delete($"{nombre}.pytham");

            CreateFile(nombre, list);
       }
    }
}
