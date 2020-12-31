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
    public static class Utilities<T>
    {
        public static void CreateFile(string nombre, T data)
        {
            FileStream stream = new FileStream($"{nombre}.pytham", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(stream, data);

            stream.Close();
        }
       public static T GetFile(string nombre)
       {
            
            FileStream stream = new FileStream($"{nombre}.pytham", FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();
            T ls = (T)formatter.Deserialize(stream);

            stream.Close();

            return ls;
       }

       public static void SaveData(string nombre, T list)
       {
            if (File.Exists($"{nombre}.pytham"))
                File.Delete($"{nombre}.pytham");

            CreateFile(nombre, list);
       }

       public static bool BinarySearch(List<T> ls, T item)
        {
            ls.Sort();
            if (ls.BinarySearch(item) > -1)
                return true;
            return false;
        }
    }
}
