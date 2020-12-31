using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Reproductor_de_Musica.Utilidades;

namespace Reproductor_de_Musica
{
    /// <summary>
    /// Lógica de interacción para Colors.xaml
    /// </summary>
    [Serializable]
    public partial class ColorsRGB : Window
    {
        public WinAjuste Window
        {
            get => this.DataContext as WinAjuste;
            set => this.DataContext = value;
        }
        private readonly Rectangle rect;
        private readonly bool colors = false;

        public ColorsRGB(WinAjuste window, Rectangle rect)
        {
            this.Window = window;
            this.rect = rect;
            InitializeComponent();

            R.Value = ((SolidColorBrush)rect.Fill).Color.R;
            TextBox_R.Text = R.Value.ToString();

            G.Value = ((SolidColorBrush)rect.Fill).Color.G;
            TextBox_G.Text = G.Value.ToString();

            B.Value = ((SolidColorBrush)rect.Fill).Color.B;
            TextBox_B.Text = B.Value.ToString();

            Background = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
            
            colors = true;

        }

        private void WrapPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            List<String> list = new List<String> {
                Window.Rectangle1.Fill.ToString(),
                Window.Rectangle2.Fill.ToString(),
                Window.Rectangle3.Fill.ToString(),
                Window.Rectangle4.Fill.ToString(),
                Window.Rectangle5.Fill.ToString(),
                Window.Rectangle6.Fill.ToString(),
                Window.Rectangle7.Fill.ToString()
            };

            Utilities<List<string>>.SaveData("theme_p", list);
        }

        private void ButtonX_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (colors)
            {
                
                var slider = sender as Slider;
                if (slider.Name == "R")
                    TextBox_R.Text = slider.Value.ToString();
                else if (slider.Name == "G")
                    TextBox_G.Text = slider.Value.ToString();
                else if (slider.Name == "B")
                    TextBox_B.Text = slider.Value.ToString();

                Background = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                rect.Fill = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                if ((bool)Window.RB_Modo_P.IsChecked)
                {

                    
                    if (rect.Name == "Rectangle1")
                    {
                        Window.Mainwindow.WrapPanel_Principal.Background = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Button_X.Background = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Button_Minus.Background = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Button_Minus.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Button_X.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Button_X.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Button_Minus.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                    }
                    else if (rect.Name == "Rectangle2")
                    {
                        Window.Mainwindow.Background = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                    }
                    else if (rect.Name == "Rectangle3")
                    {
                        Window.Mainwindow.StackPanel_Principal.Background = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                    }
                    else if (rect.Name == "Rectangle4")
                    {
                        Window.Mainwindow.WrapPanel_Secundaria.Background = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                    }
                    else if (rect.Name == "Rectangle5")
                    {
                        Window.Mainwindow.Button_Erase.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.ListBox.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.TextBlock_Add.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.TextBlock_Favorite.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.TextBlock_PlayList.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.TextBlock_Author_Name.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.TextBlock_Ajuste.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Name_Music.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Text_MinLength.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Text_MaxLength.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.TextBlock_Info_PlayList.Foreground = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                    }
                    else if (rect.Name == "Rectangle6")
                    {
                        Window.Mainwindow.Border1.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                        Window.Mainwindow.Border2.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                    }
                    else if (rect.Name == "Rectangle7") 
                    {
                        Window.Mainwindow.ListBox.BorderBrush = new SolidColorBrush(Color.FromRgb((byte)R.Value, (byte)G.Value, (byte)B.Value));
                    }

                }
            }

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (colors)
            {
                var tb = sender as TextBox;
                if (tb.Name == "TextBox_R" && tb.Text != "")
                    R.Value = Convert.ToDouble(TextBox_R.Text);
                else if (tb.Name == "TextBox_G" && tb.Text != "")
                    G.Value = Convert.ToDouble(TextBox_G.Text);
                else if (tb.Name == "TextBox_B" && tb.Text != "")
                    B.Value = Convert.ToDouble(TextBox_B.Text);
            }
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e) //Función de la persona principal.
        {
            if ((e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9))
                e.Handled = false;
            else
                e.Handled = true;
        }

        
    }
}
