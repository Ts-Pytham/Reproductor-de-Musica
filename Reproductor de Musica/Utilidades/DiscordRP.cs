using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiscordRPC;
namespace Reproductor_de_Musica.Utilidades
{
    class DiscordRP
    {
        public DiscordRP(string ClientID = null)
        {
            if (ClientID == null)
                this.ClientID = "802668887073751041";
            else
                this.ClientID = ClientID;

            discord = new DiscordRpcClient(this.ClientID);
    
            discord.OnReady += (sender, e) =>
            {
                Console.WriteLine("Received Ready from user {0}", e.User.Username);
            };

            discord.OnPresenceUpdate += (sender, e) =>
            {
                Console.WriteLine("Received Update! {0}", e.Presence);
            };

            discord.Initialize();
            
            Details = "Iniciando";
            State = "Esperando música";
            
            UpdateActivity();
            
        }

        public void UpdateActivity()
        {
           
            discord.SetPresence(new RichPresence()
            {

                Details = this.Details,
                State = this.State,

                Assets = new Assets()
                {
                    LargeImageKey = "icon"
                },

                Timestamps = new Timestamps()
                {
                    End = DateTime.UtcNow.AddSeconds(TimeEnd)
                }


            }); 
        }

       


        //propierties & variables
        public String ClientID { get; set; }
        public String State { get; set; }
        public String Details { get; set; }
        public double TimeEnd { get; set; }

        private readonly DiscordRpcClient discord;
    }
}
