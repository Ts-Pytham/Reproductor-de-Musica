using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reproductor_de_Musica.Utilidades
{
    [Serializable]
    public class Music
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string Album { get; set; }
        public string Duration { get; set; }
        public string Path { get; set; }

        public Music() { }

        public override string ToString()
        {
            return $"Nombre: {Name}, Autor: {Author}, Album: {Album}, Duración: {Duration}";
        }
    }
}
