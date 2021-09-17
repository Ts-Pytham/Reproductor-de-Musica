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
using System.IO;
using WinForms = System.Windows.Forms;
using Reproductor_de_Musica.Utilidades;

namespace Reproductor_de_Musica.src
{
    /// <summary>
    /// Lógica de interacción para Window_Streamer.xaml
    /// </summary>
    public partial class Window_Streamer : Window
    {
        public MainWindow Mainwindow
        {
            get => this.DataContext as MainWindow;
            set => this.DataContext = value;
        }


        public Window_Streamer(MainWindow Mainwindow)
        {
            this.Mainwindow = Mainwindow;
            InitializeComponent();
            ComboBox_Ruta.SelectedIndex = 0;
            if (File.Exists("PathFile.pytham"))
            {
                string data = Utilities<string>.GetFile("PathFile");
                if(data != "")
                    ComboBox_Ruta.Items.Add(data);
            }
            
        }

        private void WrapPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Utilities<string>.CreateFile("PathFile", Mainwindow.PathFile);
            Mainwindow.winStreamer = null;
        }

        private void ButtonX_Click(object sender, RoutedEventArgs e)
        {
            
            this.Close();

        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        //===================== Eventos para el checkbox de text ===================
        private void CheckBox_Text_Checked(object sender, RoutedEventArgs e)
        {
            WrapPanel_Album.Visibility = Visibility.Visible;
            WrapPanel_Year.Visibility = Visibility.Visible;
            WrapPanel_Author.Visibility = Visibility.Visible;
            WrapPanel_NameSong.Visibility = Visibility.Visible;
        }

        private void CheckBox_Text_Unchecked(object sender, RoutedEventArgs e)
        {
            WrapPanel_Album.Visibility = Visibility.Hidden;
            WrapPanel_Year.Visibility = Visibility.Hidden;
            WrapPanel_Author.Visibility = Visibility.Hidden;
            WrapPanel_NameSong.Visibility = Visibility.Hidden;
        }
        //=======================================================================================

        //========================= Botón para guardar cambios ==================================
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Mainwindow.Streamer_Picture = (bool)Check_Image.IsChecked;

            if (ComboBox_Ruta.Text.Length == 0)
            {
                MessageBox.Show("¡No ha seleccionado una ruta para guardar los datos!", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            else
                Mainwindow.PathFile = (string)ComboBox_Ruta.Items[ComboBox_Ruta.SelectedIndex];

            if ((bool)!Check_Image.IsChecked && (bool)!Check_Text.IsChecked)
            {
                MessageBox.Show("¡No ha seleccionado por lo menos una de las 2 opciones!", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if ((bool)Check_Text.IsChecked)
            {
                if ((bool)!Check_NameSong.IsChecked && (bool)!Check_Album.IsChecked && (bool)!Check_Year.IsChecked && (bool)!Check_Author.IsChecked)
                {
                    MessageBox.Show("¡No ha seleccionado ninguna opción para mostrar el texto!", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                else
                {
                    if ((bool)Check_NameSong.IsChecked)
                        File.Create($@"{(string)ComboBox_Ruta.Items[ComboBox_Ruta.SelectedIndex]}\NameSong.txt");

                    if ((bool)Check_Album.IsChecked)
                        File.Create($@"{(string)ComboBox_Ruta.Items[ComboBox_Ruta.SelectedIndex]}\AlbumSong.txt");

                    if ((bool)Check_Year.IsChecked)
                        File.Create($@"{(string)ComboBox_Ruta.Items[ComboBox_Ruta.SelectedIndex]}\YearSong.txt");

                    if ((bool)Check_Author.IsChecked)
                        File.Create($@"{(string)ComboBox_Ruta.Items[ComboBox_Ruta.SelectedIndex]}\AuthorSong.txt");

                    MessageBox.Show("Se ha guardado correctamente las configuración, en la música siguiente se mostrarán los cambios");
                    this.Close();
                }
            }
            if ((bool)Check_Image.IsChecked)
            {
                MessageBox.Show("Se ha guardado correctamente las configuración, en la música siguiente se mostrarán los cambios");
                this.Close();
            }

            

        }

        //Función para obtener la carpeta para guardar los archivos.
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            using(var fd = new WinForms.FolderBrowserDialog())
            {
                var result = fd.ShowDialog();
                if (result == WinForms.DialogResult.OK && !string.IsNullOrWhiteSpace(fd.SelectedPath))
                {
                    ComboBox_Ruta.Items.Add(fd.SelectedPath);
                    ComboBox_Ruta.SelectedIndex = ComboBox_Ruta.Items.Count - 1;
                }
            }
        }

       
    }
}
