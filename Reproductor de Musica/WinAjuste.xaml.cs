using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Reproductor_de_Musica
{
    /// <summary>
    /// Lógica de interacción para WinAjuste.xaml
    /// </summary>
    [Serializable]
    public partial class WinAjuste : Window
    {
        public MainWindow Mainwindow
        {
            get => this.DataContext as MainWindow;
            set => this.DataContext = value;
        }
        

        public WinAjuste(MainWindow Mainwindow)
        {
            this.Mainwindow = Mainwindow;
            InitializeComponent();

            
            if (Mainwindow.theme == 0)
                RB_Modo_Oscuro.IsChecked = true;
            else if (Mainwindow.theme == 1)
                RB_Modo_Claro.IsChecked = true;
            else if (Mainwindow.theme == 2)
                RB_Modo_Opera.IsChecked = true;

        }

        private void WrapPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonX_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

        }

        /* Eventos para cambiar el color de las letras de los textblock*/
        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;

            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(207, 207, 207));
        }

        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;

            textBlock.Foreground = Brushes.White;

            


        }

        private void TextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => TextBlock_MouseEnter(sender, e);


        //=================================================================================

        /* Eventos para cambiar el tema del programa*/

        private void RB_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RadioButton RB = (RadioButton)sender;

            if(RB.Name == "RB_Modo_Claro" && (bool)!RB.IsChecked)
            {
                Modo_Claro();
                Mainwindow.theme = 1;
                List<string> list = new List<string>
                {
                    "#FF72C8F1",
                    "#FFFFFFFF",
                    "#FFdedede",
                    "#FF318d99",
                    "#FF000000",
                    "#FF000000",
                    "1"
                };
                Utilidades.Utilities.SaveData("theme", list);
                RB.IsChecked = true;
            }
            else if (RB.Name == "RB_Modo_Oscuro" && (bool)!RB.IsChecked)
            {
                Modo_Oscuro();
                Mainwindow.theme = 0;
                List<string> list = new List<string>
                {
                    "#FF000000",
                    "#FF2F3136",
                    "#FF151515",
                    "#FF212121",
                    "#FFFFFFFF",
                    "#FFCFCFCF",
                    "0"
                };
                Utilidades.Utilities.SaveData("theme", list);
                RB.IsChecked = true;
            }
            else if (RB.Name == "RB_Modo_Opera" && (bool)!RB.IsChecked)
            {
                Modo_Opera();
                Mainwindow.theme = 2;
                List<string> list = new List<string>
                {
                    "#FF121019",
                    "#FF1e1b2a",
                    "#FF1c1726",
                    "#FF1c1730",
                    "#FFFFFFFF",
                    "#FFCFCFCF",
                    "2"
                };
                Utilidades.Utilities.SaveData("theme", list);
                RB.IsChecked = true;
            }
        }

        private void Modo_Claro()
        {
            Mainwindow.Background = Brushes.White;
            Mainwindow.ListBox.Background = Brushes.White;
            Mainwindow.ListBox.Foreground = Brushes.Black;
            /* Primero creo el diccionario de recurso, aplico su propiedad source y le doy
             * la URL, luego uso ItemContainerStyle y le paso el diccionario. 
             */
            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/Styles/ListBox.xaml")
            };

            Mainwindow.ListBox.ItemContainerStyle = (Style)resourceDictionary["Modo_Claro"];


            Mainwindow.WrapPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom("#FF72C8F1");
            Mainwindow.Button_X.Background = (Brush)new BrushConverter().ConvertFrom("#FF72C8F1");
            Mainwindow.Button_X.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF72C8F1");
            Mainwindow.Button_Minus.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF72C8F1");
            Mainwindow.Button_Minus.Background = (Brush)new BrushConverter().ConvertFrom("#FF72C8F1");



            Mainwindow.StackPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom("#FFdedede");

            Mainwindow.WrapPanel_Secundaria.Background = (Brush)new BrushConverter().ConvertFrom("#FF318d99");

            Mainwindow.TextBlock_Add.Foreground = Brushes.Black;
            Mainwindow.TextBlock_Favorite.Foreground = Brushes.Black;
            Mainwindow.TextBlock_PlayList.Foreground = Brushes.Black;
            Mainwindow.TextBlock_Author_Name.Foreground = Brushes.Black;
            Mainwindow.TextBlock_Ajuste.Foreground = Brushes.Black;
            Mainwindow.Name_Music.Foreground = Brushes.Black;
            Mainwindow.Text_MinLength.Foreground = Brushes.Black;
            Mainwindow.Text_MaxLength.Foreground = Brushes.Black;
            Mainwindow.TextBlock_Info_PlayList.Foreground = Brushes.Black;


        }

        private void Modo_Oscuro()
        {
            Mainwindow.Background = (Brush)new BrushConverter().ConvertFrom("#FF2F3136");
            Mainwindow.ListBox.Background = (Brush)new BrushConverter().ConvertFrom("#FF2F3136");
            Mainwindow.ListBox.Foreground = Brushes.White;
            /* Primero creo el diccionario de recurso, aplico su propiedad source y le doy
             * la URL, luego uso ItemContainerStyle y le paso el diccionario. 
             */
            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/Styles/ListBox.xaml")
            };

            Mainwindow.ListBox.ItemContainerStyle = (Style)resourceDictionary["Modo_Oscuro"];


            Mainwindow.WrapPanel_Principal.Background = Brushes.Black;
            Mainwindow.Button_X.Background = Brushes.Black;
            Mainwindow.Button_X.BorderBrush = Brushes.Black;
            Mainwindow.Button_Minus.BorderBrush = Brushes.Black;
            Mainwindow.Button_Minus.Background = Brushes.Black;



            Mainwindow.StackPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom("#FF151515");

            Mainwindow.WrapPanel_Secundaria.Background = (Brush)new BrushConverter().ConvertFrom("#FF212121");

            Mainwindow.TextBlock_Add.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFCFCFCF");
            Mainwindow.TextBlock_Favorite.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFCFCFCF");
            Mainwindow.TextBlock_PlayList.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFCFCFCF");
            Mainwindow.TextBlock_Author_Name.Foreground = Brushes.White;
            Mainwindow.TextBlock_Ajuste.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFCFCFCF");
            Mainwindow.Name_Music.Foreground = Brushes.White;
            Mainwindow.Text_MinLength.Foreground = Brushes.White;
            Mainwindow.Text_MaxLength.Foreground = Brushes.White;
            Mainwindow.TextBlock_Info_PlayList.Foreground = Brushes.White;
        }


        private void Modo_Opera()
        {
            Mainwindow.Background = (Brush)new BrushConverter().ConvertFrom("#FF1e1b2a");
            Mainwindow.ListBox.Background = (Brush)new BrushConverter().ConvertFrom("#FF1e1b2a");
            Mainwindow.ListBox.Foreground = Brushes.White;
            /* Primero creo el diccionario de recurso, aplico su propiedad source y le doy
             * la URL, luego uso ItemContainerStyle y le paso el diccionario. 
             */
            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/Styles/ListBox.xaml")
            };

            Mainwindow.ListBox.ItemContainerStyle = (Style)resourceDictionary["Modo_Oscuro"];


            Mainwindow.WrapPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom("#FF121019");
            Mainwindow.Button_X.Background = (Brush)new BrushConverter().ConvertFrom("#FF121019"); 
            Mainwindow.Button_X.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF121019"); 
            Mainwindow.Button_Minus.BorderBrush = (Brush)new BrushConverter().ConvertFrom("#FF121019");
            Mainwindow.Button_Minus.Background = (Brush)new BrushConverter().ConvertFrom("#FF121019");



            Mainwindow.StackPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom("#FF1c1726");

            Mainwindow.WrapPanel_Secundaria.Background = (Brush)new BrushConverter().ConvertFrom("#FF1c1730");

            Mainwindow.TextBlock_Add.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFCFCFCF");
            Mainwindow.TextBlock_Favorite.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFCFCFCF");
            Mainwindow.TextBlock_PlayList.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFCFCFCF");
            Mainwindow.TextBlock_Author_Name.Foreground = Brushes.White;
            Mainwindow.TextBlock_Ajuste.Foreground = (Brush)new BrushConverter().ConvertFrom("#FFCFCFCF");
            Mainwindow.Name_Music.Foreground = Brushes.White;
            Mainwindow.Text_MinLength.Foreground = Brushes.White;
            Mainwindow.Text_MaxLength.Foreground = Brushes.White;
            Mainwindow.TextBlock_Info_PlayList.Foreground = Brushes.White;
        }


        //=============================================================================

        private Rectangle CreateRectangle(string fill, Thickness Margin)
        {
            Rectangle rectangle = new Rectangle
            {
                Fill = (Brush)new BrushConverter().ConvertFrom(fill),
                Height = 14,
                Width = 12,
                Stroke = Brushes.White,
                Margin = Margin
            };
            return rectangle;
        }
        private void TextBlock_Theme_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Rectangle_Barra.Visibility == Visibility.Visible)
                return;
            Rectangle_Barra1.Visibility = Visibility.Hidden;
            Rectangle_Barra.Visibility = Visibility.Visible;

            StackPanel_Principal.Children.Clear();

            

            StackPanel_Principal.Children.Add(TextBlock_Modo_Oscuro);

            var rect1 = CreateRectangle("#FF000000", new Thickness(0, 0, 0, 0));
            var rect2 = CreateRectangle("#FF2F3136", new Thickness(5, 0, 0, 0));
            var rect3 = CreateRectangle("#FF151515", new Thickness(5, 0, 0, 0));
            var rect4 = CreateRectangle("#FF212121", new Thickness(5, 0, 0, 0));
            var rect5 = CreateRectangle("#FFFFFFFF", new Thickness(5, 0, 0, 0));

            WrapPanel_Oscuro.Children.Add(rect1);
            WrapPanel_Oscuro.Children.Add(rect2);
            WrapPanel_Oscuro.Children.Add(rect3);
            WrapPanel_Oscuro.Children.Add(rect4);
            WrapPanel_Oscuro.Children.Add(rect5);
            WrapPanel_Oscuro.Children.Add(RB_Modo_Oscuro);

            StackPanel_Principal.Children.Add(WrapPanel_Oscuro);

            // Re-construimos los datos del modo claro
            StackPanel_Principal.Children.Add(TextBlock_Modo_Claro);
            
            WrapPanel_Claro.Children.Add(CreateRectangle("#FF72C8F1", new Thickness(0, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(CreateRectangle("#FFFFFFFF", new Thickness(5, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(CreateRectangle("#FFdedede", new Thickness(5, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(CreateRectangle("#FF318d99", new Thickness(5, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(CreateRectangle("#FF000000", new Thickness(5, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(RB_Modo_Claro);

            StackPanel_Principal.Children.Add(WrapPanel_Claro);

            // Re-construimos los datos del modo Opera
            StackPanel_Principal.Children.Add(TextBlock_Modo_Opera);

            WrapPanel_Opera.Children.Add(CreateRectangle("#FF121019", new Thickness(0, 0, 0, 0)));
            WrapPanel_Opera.Children.Add(CreateRectangle("#FF1e1b2a", new Thickness(5, 0, 0, 0)));
            WrapPanel_Opera.Children.Add(CreateRectangle("#FF1c1726", new Thickness(5, 0, 0, 0)));
            WrapPanel_Opera.Children.Add(CreateRectangle("#FF1c1730", new Thickness(5, 0, 0, 0)));
            WrapPanel_Opera.Children.Add(CreateRectangle("#FFFFFFFF", new Thickness(5, 0, 0, 0)));
            WrapPanel_Opera.Children.Add(RB_Modo_Opera);

            StackPanel_Principal.Children.Add(WrapPanel_Opera);



        }
        private void TextBlock_About_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Rectangle_Barra1.Visibility == Visibility.Visible)
                return;
            Rectangle_Barra1.Visibility = Visibility.Visible;
            Rectangle_Barra.Visibility = Visibility.Hidden;

            

            StackPanel_Principal.Children.Clear();
            WrapPanel_Oscuro.Children.Clear();
            WrapPanel_Claro.Children.Clear();
            WrapPanel_Opera.Children.Clear();
            
            

            TextBlock textBlock = new TextBlock 
            { 
                FontFamily = new FontFamily("Verdana"),
                FontSize = 16,
                Foreground = Brushes.White,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0,40,0,0)

            };
            string str = "Creador: Ts_Pytham.\nEste programa está creado en C#. " +
                "El reproductor de música fue creado en el año 2020.\n" +
                "Versión: 2.0.\nEn el siguiente link podrás encontrar el proyecto" +
                "completo para descargar o puedes ir a los releases para descargar el" +
                "instalador del programa.\n";
            Hyperlink hyperlink = new Hyperlink
            {
                NavigateUri = new Uri("https://github.com/Ts-Pytham/Reproductor-de-Musica"),
                FontFamily = new FontFamily("Verdana"),
                FontSize = 16

            };
            
            textBlock.Text = str;
            hyperlink.Inlines.Add("Click aquí");
            textBlock.Inlines.Add(hyperlink);
            StackPanel_Principal.Children.Add(textBlock);
            
            hyperlink.RequestNavigate += Hyperlink_RequestNavigate;


        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        
    }
}
