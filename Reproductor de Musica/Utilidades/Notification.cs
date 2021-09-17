using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reproductor_de_Musica.Utilidades
{
    static class Notification
    {
        public static void Show(String Title, String Body)
        {
            new ToastContentBuilder()
                .AddArgument("action", "viewConversation")
                .AddArgument("conversationId", 9813)
                .AddText(Title)
                .AddText(Body)
                .Show(toast =>
                {
                    toast.ExpirationTime = DateTime.Now.AddSeconds(3);

                });

        }

    }
}
