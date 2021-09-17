using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reproductor_de_Musica.Utilidades
{
    [Serializable]
    public class Historial
    {

        public Historial()
        {
            LHistory = new List<string>();
            LURL = new List<string>();
            Musics = new ObservableCollection<Music>();
        }

        public Historial(List<string> LHistory, List<string> LURL)
        {
            this.LHistory = LHistory;
            this.LURL = LURL;
        }


        public List<string> LHistory { set; get; }
        public List<string> LURL { set; get; }
        public ObservableCollection<Music> Musics { get; set; }
    }
}
