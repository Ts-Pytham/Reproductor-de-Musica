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
            using (FileStream stream = new FileStream($"{nombre}.pytham", FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
            }
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


        public static List<T> RandomSort(List<T> ls)
        {
            T aux;
            Random random = new Random();
            for (int i = ls.Count - 1, index; i > 0; --i)
            {
                index = random.Next(i + 1);
                aux = ls[i];
                ls[i] = ls[index];
                ls[index] = aux;
            }
            return ls;
        }


        public static String ChangeFormat(string video)
        {
            StringBuilder strb = new StringBuilder(video);
            int len = strb.Length;
            for (int i = 0; i != len; ++i)
            {

                if (strb[i] == '|' || strb[i] == 92 || strb[i] == '/' || strb[i] == ':' || strb[i] == '?' || strb[i] == '<' || strb[i] == '>' || strb[i] == '"')
                {
                    strb[i] = '-';
                }
            }
            return strb.ToString();
        }

    }
}
