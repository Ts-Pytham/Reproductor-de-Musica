using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IO = System.IO;
using Reproductor_de_Musica.Utilidades;
using System.Windows.Controls.Primitives;

namespace Reproductor_de_Musica
{
    /// <summary>
    /// Lógica de interacción para WinAjuste.xaml
    /// </summary>
    [Serializable]
    public partial class WinAjuste : Window
    {

        private readonly string version = "4.0";
        public ColorsRGB colors;


        public MainWindow Mainwindow
        {
            get => this.DataContext as MainWindow;
            set => this.DataContext = value;
        }
        

        public WinAjuste(MainWindow Mainwindow)
        {
            this.Mainwindow = Mainwindow;
            InitializeComponent();

            switch (Mainwindow.theme)
            {
                case 0:
                    RB_Modo_Oscuro.IsChecked = true;
                    break;
                case 1:
                    RB_Modo_Claro.IsChecked = true;
                    break;
                case 2:
                    RB_Modo_Opera.IsChecked = true;
                    break;
                case 3:
                    RB_Modo_Amazul.IsChecked = true;
                    break;
                case 4:
                    RB_Modo_Quartz.IsChecked = true;
                    break;
                case 5:
                    RB_Modo_P.IsChecked = true;
                    break;
            }

            // Comprobamos si el usuario tiene una plantilla personalizada

            if (IO.File.Exists("theme_p.pytham"))
            {
                List<string> ls = Utilities<List<string>>.GetFile("theme_p");
                Rectangle1.Fill = (Brush)new BrushConverter().ConvertFrom(ls[0]);
                Rectangle2.Fill = (Brush)new BrushConverter().ConvertFrom(ls[1]);
                Rectangle3.Fill = (Brush)new BrushConverter().ConvertFrom(ls[2]);
                Rectangle4.Fill = (Brush)new BrushConverter().ConvertFrom(ls[3]);
                Rectangle5.Fill = (Brush)new BrushConverter().ConvertFrom(ls[4]);
                Rectangle6.Fill = (Brush)new BrushConverter().ConvertFrom(ls[5]);
                Rectangle7.Fill = (Brush)new BrushConverter().ConvertFrom(ls[6]);
            }


        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((bool)RB_Modo_P.IsChecked)
            {
                List<string> list = new List<string>
                {
                    Rectangle1.Fill.ToString(),
                    Rectangle2.Fill.ToString(),
                    Rectangle3.Fill.ToString(),
                    Rectangle4.Fill.ToString(),
                    Rectangle5.Fill.ToString(),
                    Rectangle6.Fill.ToString(),
                    Rectangle5.Fill.ToString(),
                    Rectangle7.Fill.ToString(),
                    "5"
                };

                Utilities<List<string>>.SaveData("theme", list);
            }
            Mainwindow.win = null;
            if(colors != null)
                colors.Close();
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

        public void RB_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
         
            RadioButton RB = (RadioButton)sender;
            
            if(RB.Name == "RB_Modo_Claro" && (bool)!RB.IsChecked)
            {
                Mainwindow.theme = 1;
               
                Change_Theme("#FF72C8F1", "#FFFFFFFF", "#FFdedede", "#FF318d99", Brushes.Black, "#FF000000", "#FF000000", "#FF000000");
               
                List<string> list = new List<string>
                {
                    "#FF72C8F1",
                    "#FFFFFFFF",
                    "#FFdedede",
                    "#FF318d99",
                    "#FF000000",
                    "#FF000000",
                    "#FF000000",
                    "#FF000000",
                    "1"
                };
                Utilities<List<string>>.SaveData("theme", list);
                Mainwindow.LTheme = list;
                RB.IsChecked = true;
            }
            else if (RB.Name == "RB_Modo_Oscuro" && (bool)!RB.IsChecked)
            {
                Mainwindow.theme = 0;
                
                Change_Theme("#FF000000", "#FF2F3136", "#FF151515", "#FF212121", Brushes.White, "#FFfdf008", "#FFCFCFCF", "#FFfdf008");


                List<string> list = new List<string>
                {
                    "#FF000000",
                    "#FF2F3136",
                    "#FF151515",
                    "#FF212121",
                    "#FFFFFFFF",
                    "#FFfdf008",
                    "#FFCFCFCF",
                    "#FFfdf008",
                    "0"
                };
                Utilities<List<string>>.SaveData("theme", list);
                Mainwindow.LTheme = list;
                RB.IsChecked = true;
            }
            else if (RB.Name == "RB_Modo_Opera" && (bool)!RB.IsChecked)
            {
                Mainwindow.theme = 2;
               
                Change_Theme("#FF121019", "#FF1e1b2a", "#FF1c1726", "#FF1c1730", Brushes.White, "#FFde1927", "#FFCFCFCF", "#FFde1927");
                List<string> list = new List<string>
                {
                    "#FF121019",
                    "#FF1e1b2a",
                    "#FF1c1726",
                    "#FF1c1730",
                    "#FFFFFFFF",
                    "#FFde1927",
                    "#FFCFCFCF",
                    "#FFde1927",
                    "2"
                };
                Utilities<List<string>>.SaveData("theme", list);
                Mainwindow.LTheme = list;
                RB.IsChecked = true;
            }
            else if (RB.Name == "RB_Modo_Amazul" && (bool)!RB.IsChecked)
            {
                Mainwindow.theme = 3;

                Change_Theme("#FF002420", "#FF002429", "#FF00363d", "#FF002729", Brushes.White, "#FFfdf008", "#FFCFCFCF", "#FFfdf008");
                
                List<string> list = new List<string>
                {
                    "#FF002420",
                    "#FF002429",
                    "#FF00363d",
                    "#FF002729",
                    "#FFFFFFFF",
                    "#FFfdf008",
                    "#FFCFCFCF",
                    "#FFfdf008",
                    "3"
                };
                Utilities<List<string>>.SaveData("theme", list);
                Mainwindow.LTheme = list;
                RB.IsChecked = true;
            }

            else if (RB.Name == "RB_Modo_Quartz" && (bool)!RB.IsChecked)
            {
                Mainwindow.theme = 4;

                Change_Theme("#FF1d0c13", "#FF1d0c13", "#FF2b121c", "#FF351622", Brushes.White, "#FFf2688d", "#FFCFCFCF", "#FFf2688d");


                List<string> list = new List<string>
                {
                    "#FF1d0c13",
                    "#FF1d0c13",
                    "#FF2b121c",
                    "#FF351622",
                    "#FFFFFFFF",
                    "#FFf2688d",
                    "#FFCFCFCF",
                    "#FFf2688d",
                    "4"
                };
                Mainwindow.LTheme = list;
                Utilities<List<string>>.SaveData("theme", list);
                RB.IsChecked = true;
            }
            else if (RB.Name == "RB_Modo_P" && (bool)!RB.IsChecked)
            {
                Mainwindow.theme = 5;
                Change_Theme(Rectangle1.Fill.ToString(), Rectangle2.Fill.ToString(), Rectangle3.Fill.ToString(), Rectangle4.Fill.ToString(), Rectangle5.Fill, Rectangle6.Fill.ToString(), Rectangle5.Fill.ToString(), Rectangle7.Fill.ToString());
                
                List<string> list = new List<string>
                {
                    Rectangle1.Fill.ToString(),
                    Rectangle2.Fill.ToString(),
                    Rectangle3.Fill.ToString(),
                    Rectangle4.Fill.ToString(),
                    Rectangle5.Fill.ToString(),
                    Rectangle6.Fill.ToString(),
                    Rectangle5.Fill.ToString(),
                    Rectangle7.Fill.ToString(),
                    "5"
                };
                Utilities<List<string>>.SaveData("theme", list);
                RB.IsChecked = true;
            }
        }
        private void Change_Theme(string color1, string color2, string color3, string color4, Brush color5, string color7, string color6, string color8)
        {
            Mainwindow.Background = (Brush)new BrushConverter().ConvertFrom(color2);
            Mainwindow.DataGridP.BorderBrush = (Brush)new BrushConverter().ConvertFrom(color8);


            Mainwindow.Button_Erase.Foreground = color5;
            Mainwindow.Button_Erase.BorderBrush = (Brush)new BrushConverter().ConvertFrom(color8);

            Mainwindow.Border1.BorderBrush = Mainwindow.Border2.BorderBrush = Mainwindow.Border3.BorderBrush = (Brush)new BrushConverter().ConvertFrom(color7);
            //MessageBox.Show(Mainwindow.DataGridP.ColumnHeaderStyle.Resources[Control.ForegroundProperty].ToString());

            Style style = new Style(typeof(DataGridColumnHeader));
            style.Setters.Add(new Setter { Property = BackgroundProperty, Value = (Brush)new BrushConverter().ConvertFrom(color1) });
            style.Setters.Add(new Setter { Property = HeightProperty, Value = 30d });
            style.Seal();
            Mainwindow.DataGridP.ColumnHeaderStyle = style;

            Style style2 = new Style(typeof(DataGridCell));


            if (Mainwindow.theme == 1)
            {
                Mainwindow.DataGridP.Foreground = Brushes.Black;
                style2.Setters.Add(new Setter { Property = ForegroundProperty, Value = Brushes.Black });
            }
            else if (Mainwindow.theme != 5)
            {
                Mainwindow.DataGridP.Foreground = Brushes.White;
                style2.Setters.Add(new Setter { Property = ForegroundProperty, Value = Brushes.White });
            }
            else
            {
                Mainwindow.DataGridP.Foreground = color5;
                style2.Setters.Add(new Setter { Property = ForegroundProperty, Value = Brushes.White });
            }
            style2.Seal();
            Mainwindow.DataGridP.CellStyle = style2;


            Mainwindow.WrapPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom(color1);
            Mainwindow.Button_X.Background = (Brush)new BrushConverter().ConvertFrom(color1);
            Mainwindow.Button_X.BorderBrush = (Brush)new BrushConverter().ConvertFrom(color1);
            Mainwindow.Button_Minus.BorderBrush = (Brush)new BrushConverter().ConvertFrom(color1);
            Mainwindow.Button_Minus.Background = (Brush)new BrushConverter().ConvertFrom(color1);



            Mainwindow.StackPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom(color3);

            Mainwindow.WrapPanel_Secundaria.Background = (Brush)new BrushConverter().ConvertFrom(color4);

            Mainwindow.TextBlock_Add.Foreground = (Brush)new BrushConverter().ConvertFrom(color6);
            Mainwindow.TextBlock_Favorite.Foreground = (Brush)new BrushConverter().ConvertFrom(color6);
            Mainwindow.TextBlock_PlayList.Foreground = (Brush)new BrushConverter().ConvertFrom(color6);
            Mainwindow.TextBlock_Author_Name.Foreground = color5;
            Mainwindow.TextBlock_Ajuste.Foreground = (Brush)new BrushConverter().ConvertFrom(color6);
            Mainwindow.Name_Music.Foreground = color5;
            Mainwindow.Text_MinLength.Foreground = color5;
            Mainwindow.Text_MaxLength.Foreground = color5;
            Mainwindow.TextBlock_Info_PlayList.Foreground = color5;
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
            var rect6 = CreateRectangle("#FFfdf008", new Thickness(5, 0, 0, 0));
            var rect7 = CreateRectangle("#FFFFFFFF", new Thickness(5, 0, 0, 0));

            WrapPanel_Oscuro.Children.Add(rect1);
            WrapPanel_Oscuro.Children.Add(rect2);
            WrapPanel_Oscuro.Children.Add(rect3);
            WrapPanel_Oscuro.Children.Add(rect4);
            WrapPanel_Oscuro.Children.Add(rect5);
            WrapPanel_Oscuro.Children.Add(rect6);
            WrapPanel_Oscuro.Children.Add(rect7);
            WrapPanel_Oscuro.Children.Add(RB_Modo_Oscuro);

            StackPanel_Principal.Children.Add(WrapPanel_Oscuro);

            // Re-construimos los datos del modo claro
            StackPanel_Principal.Children.Add(TextBlock_Modo_Claro);
            
            WrapPanel_Claro.Children.Add(CreateRectangle("#FF72C8F1", new Thickness(0, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(CreateRectangle("#FFFFFFFF", new Thickness(5, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(CreateRectangle("#FFdedede", new Thickness(5, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(CreateRectangle("#FF318d99", new Thickness(5, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(CreateRectangle("#FF000000", new Thickness(5, 0, 0, 0)));
            WrapPanel_Claro.Children.Add(CreateRectangle("#FFFFFFFF", new Thickness(5, 0, 0, 0)));
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
            WrapPanel_Opera.Children.Add(CreateRectangle("#FFde1927", new Thickness(5, 0, 0, 0)));
            WrapPanel_Opera.Children.Add(CreateRectangle("#FFde1927", new Thickness(5, 0, 0, 0)));
            WrapPanel_Opera.Children.Add(RB_Modo_Opera);

            StackPanel_Principal.Children.Add(WrapPanel_Opera);

            // Re-construimos los datos del modo Amazul
            StackPanel_Principal.Children.Add(TextBlock_Modo_Amazul);

            WrapPanel_Amazul.Children.Add(CreateRectangle("#FF002420", new Thickness(0, 0, 0, 0)));
            WrapPanel_Amazul.Children.Add(CreateRectangle("#FF002429", new Thickness(5, 0, 0, 0)));
            WrapPanel_Amazul.Children.Add(CreateRectangle("#FF00363d", new Thickness(5, 0, 0, 0)));
            WrapPanel_Amazul.Children.Add(CreateRectangle("#FF002729", new Thickness(5, 0, 0, 0)));
            WrapPanel_Amazul.Children.Add(CreateRectangle("#FFFFFFFF", new Thickness(5, 0, 0, 0)));
            WrapPanel_Amazul.Children.Add(CreateRectangle("#FFfdf008", new Thickness(5, 0, 0, 0)));
            WrapPanel_Amazul.Children.Add(CreateRectangle("#FFfdf008", new Thickness(5, 0, 0, 0)));
            WrapPanel_Amazul.Children.Add(RB_Modo_Amazul);

            
            StackPanel_Principal.Children.Add(WrapPanel_Amazul);

            // Re-construimos los datos del modo Quartz
            StackPanel_Principal.Children.Add(TextBlock_Modo_Quartz);

            WrapPanel_Quartz.Children.Add(CreateRectangle("#FF1d0c13", new Thickness(0, 0, 0, 0)));
            WrapPanel_Quartz.Children.Add(CreateRectangle("#FF1d0c13", new Thickness(5, 0, 0, 0)));
            WrapPanel_Quartz.Children.Add(CreateRectangle("#FF2b121c", new Thickness(5, 0, 0, 0)));
            WrapPanel_Quartz.Children.Add(CreateRectangle("#FF351622", new Thickness(5, 0, 0, 0)));
            WrapPanel_Quartz.Children.Add(CreateRectangle("#FFFFFFFF", new Thickness(5, 0, 0, 0)));
            WrapPanel_Quartz.Children.Add(CreateRectangle("#FFf2688d", new Thickness(5, 0, 0, 0)));
            WrapPanel_Quartz.Children.Add(CreateRectangle("#FFf2688d", new Thickness(5, 0, 0, 0)));
            WrapPanel_Quartz.Children.Add(RB_Modo_Quartz);


            StackPanel_Principal.Children.Add(WrapPanel_Quartz);


            // Re-construimos los datos del modo Personalizado
            StackPanel_Principal.Children.Add(TextBlock_Modo_P);

            WrapPanel_P.Children.Add(Rectangle1);
            WrapPanel_P.Children.Add(Rectangle2);
            WrapPanel_P.Children.Add(Rectangle3);
            WrapPanel_P.Children.Add(Rectangle4);
            WrapPanel_P.Children.Add(Rectangle5);
            WrapPanel_P.Children.Add(Rectangle6);
            WrapPanel_P.Children.Add(Rectangle7);
            WrapPanel_P.Children.Add(RB_Modo_P);


            StackPanel_Principal.Children.Add(WrapPanel_P);



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
            WrapPanel_Amazul.Children.Clear();
            WrapPanel_Quartz.Children.Clear();
            WrapPanel_P.Children.Clear();
            
            

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
                $"Versión: {version}.\nEn el siguiente link podrás encontrar el proyecto " +
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

        private void Color_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rec = sender as Rectangle;

            colors = new ColorsRGB(this, rec);
            colors.Show();
        }

       
    }
}
