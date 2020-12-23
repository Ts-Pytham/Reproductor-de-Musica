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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WinForms = System.Windows.Forms;
using IO = System.IO;


namespace Reproductor_de_Musica
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MediaPlayer mediaPlayer = new MediaPlayer();
        private bool IsPaused = true;
        private readonly List<string> URLS = new List<string>();
        private TimeSpan position;
        private DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            //mediaPlayer = new MediaPlayer();
            InitializeComponent();
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

        private void Button_Pause_Click(object sender, RoutedEventArgs e)
        {

            if (!IsPaused)
            {
                Image img = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/boton-de-play.png")) };
                Button_Reproductor.Content = img;
                IsPaused = true;
                mediaPlayer.Pause();
            }
            else
            {
                Image img = new Image { Source = new BitmapImage(new Uri(uriString: @"pack://application:,,,/IMG/pausa.png")) };
                Button_Reproductor.Content = img;
                IsPaused = false;
                mediaPlayer.Play();

            }

        }
        /* Para hacer el efecto de darle click al textblock y que sea tipo button*/
        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
          
            TextBlock textBlock = (TextBlock)sender;

            textBlock.Foreground = Brushes.Gray; 
            /* El if comprueba cual textblock fue el que se presionó, 
             * solo sirve para ocultar/mostrar el rectángulo y llama a la función para 
             * agregar la música, es decir, si entra en el else es porque el usuario ingresa 
             * una playlist. */
            if (textBlock.Name == "TextBlock_Add") 
            {
                Rectangle_Barra.Visibility = Visibility.Visible;
                Rectangle_Barra1.Visibility = Visibility.Hidden;

                using (var fd = new WinForms.OpenFileDialog())
                {
                    fd.Filter = "Image Files(*.MP3; *.Webm)|*.MP3; *.Webm";

                    fd.FilterIndex = 1;

                    if (fd.ShowDialog() == WinForms.DialogResult.OK)
                    {

                        Name_Music.Text = fd.SafeFileName;

                        mediaPlayer.Open(new Uri(fd.FileName));


                        Image img = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/pausa.png")) };
                        Button_Reproductor.Content = img;
                        IsPaused = false;

                        DispatcherTimer timer = new DispatcherTimer
                        {
                            Interval = TimeSpan.FromSeconds(1)
                        };
                        timer.Tick += Timer_Tick;
                        timer.Start();


                        mediaPlayer.Volume = 0.5;
                        Slider_Volumen.Value = mediaPlayer.Volume;

                        mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

                        ListBox.Items.Add(fd.SafeFileName);

                        URLS.Add(fd.FileName);

                        ListBox.SelectedIndex = ListBox.Items.Count - 1;

                    }
                }
            }
            else 
            { 
                Rectangle_Barra1.Visibility = Visibility.Visible;
                Rectangle_Barra.Visibility = Visibility.Hidden;

                using (var fd = new WinForms.OpenFileDialog())
                {
                    fd.Multiselect = true;

                    fd.Filter = "Image Files(*.MP3; *.Webm)|*.MP3; *.Webm";

                    fd.FilterIndex = 1;

                    if (fd.ShowDialog() == WinForms.DialogResult.OK)
                    {

                        Name_Music.Text = fd.SafeFileName;

                        mediaPlayer.Open(new Uri(fd.FileName));


                        Image img = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/pausa.png")) };
                        Button_Reproductor.Content = img;
                        IsPaused = false;

                        DispatcherTimer timer = new DispatcherTimer
                        {
                            Interval = TimeSpan.FromSeconds(1)
                        };
                        timer.Tick += Timer_Tick;
                        timer.Start();


                        mediaPlayer.Volume = 0.5;
                        Slider_Volumen.Value = mediaPlayer.Volume;

                        mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

                        bool IsEmpty = true;
                        if (ListBox.Items.Count != 0)
                            IsEmpty = false;
                        
                        int len = fd.FileNames.Length;
                        for(int i = 0; i != len; ++i)
                        {
                            ListBox.Items.Add(fd.SafeFileNames[i]);
                            URLS.Add(fd.FileNames[i]);
                        }

                        // Comprueba si en la lista hay música, si no hay pone el index en 0.
                        if (IsEmpty)
                            ListBox.SelectedIndex = 0;
                        else
                            ListBox.SelectedIndex = ListBox.Items.Count - 1;
                    }
                }
            }

            
        }

        
        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            mediaPlayer.Play();
            position = mediaPlayer.NaturalDuration.TimeSpan;
            Slider_Carga.Minimum = 0;
            Slider_Carga.Maximum = position.TotalSeconds;

            Text_MinLength.Text = mediaPlayer.Position.ToString(@"mm\:ss");
            Text_MaxLength.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }

        private void TextBlock_Add_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;

            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(207, 207, 207));
        }

        private void TextBlock_Add_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;

            textBlock.Foreground = Brushes.White;

            
        }

        private void TextBlock_Add_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => TextBlock_Add_MouseEnter(sender, e);

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = e.NewValue;
        }

        /* Eventos del ListBox         */
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
            mediaPlayer.Open(new Uri(URLS[ListBox.SelectedIndex]));
            Name_Music.Text = ListBox.SelectedItem.ToString();
            IsPaused = true;
            Button_Pause_Click(sender, e);
            mediaPlayer.Play();
            
        }
        //=======================================================================

        void Timer_Tick(object sender, EventArgs e)
        {
            Slider_Carga.Value = mediaPlayer.Position.TotalSeconds;
            Text_MinLength.Text = mediaPlayer.Position.ToString(@"mm\:ss");
            if(Text_MinLength.Text == Text_MaxLength.Text)
            {
                if(ListBox.Items.Count != 1 && ListBox.SelectedIndex != ListBox.Items.Count - 1)
                {
                    mediaPlayer.Open(new Uri(URLS[ListBox.SelectedIndex + 1]));
                    Name_Music.Text = ListBox.Items[ListBox.SelectedIndex + 1].ToString();
                    ListBox.SelectedIndex += 1;
                    mediaPlayer.Play();
                }
          
            }


        }

        private void Slider_Carga_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int pos = Convert.ToInt32(Slider_Carga.Value);
            mediaPlayer.Position = new TimeSpan(0, 0, 0, pos, 0);
        }





        /*     EVENTOS POR DEFECTOS DE LAS IMAGENES    */

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            if(img.Name == "Siguiente")
              img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Siguiente/hover_siguiente.png"));
            else if(img.Name == "Anterior")
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Anterior/hover_anterior.png"));
            else if(img.Name == "Repetir")
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Repetir/hover_actualizar.png"));
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            if (img.Name == "Siguiente")
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Siguiente/siguiente.png"));
            else if (img.Name == "Anterior")
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Anterior/anterior.png"));
            else if (img.Name == "Repetir")
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Repetir/actualizar.png"));
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            if (img.Name == "Siguiente")
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Siguiente/enter_siguiente.png"));
                if (ListBox.Items.Count == 1)
                {
                    mediaPlayer.Open(new Uri(URLS[0]));

                }
                else if(ListBox.SelectedIndex == ListBox.Items.Count - 1)
                {
                    mediaPlayer.Open(new Uri(URLS[0]));
                    Name_Music.Text = ListBox.Items[0].ToString();
                    ListBox.SelectedIndex = 0;
                }
                else
                {
                    mediaPlayer.Open(new Uri(URLS[ListBox.SelectedIndex + 1]));
                    Name_Music.Text = ListBox.Items[ListBox.SelectedIndex + 1].ToString();
                    ListBox.SelectedIndex += 1;
                }

                IsPaused = true;
                Button_Pause_Click(sender, e);
                mediaPlayer.Play();
            }
            else if (img.Name == "Anterior")
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Anterior/enter_anterior.png"));
                if (ListBox.Items.Count == 1)
                {
                    mediaPlayer.Open(new Uri(URLS[0]));
                    
                    
                }
                else if(ListBox.SelectedIndex != 0)
                {

                    mediaPlayer.Open(new Uri(URLS[ListBox.SelectedIndex - 1]));
                    Name_Music.Text = ListBox.Items[ListBox.SelectedIndex - 1].ToString();
                    ListBox.SelectedIndex -= 1;
                }
                IsPaused = true;
                Button_Pause_Click(sender, e);
                mediaPlayer.Play();
            }
            else if(img.Name == "Repetir")
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Repetir/click_actualizar.png"));
                mediaPlayer.Open(new Uri(URLS[ListBox.SelectedIndex]));

                IsPaused = true;
                Button_Pause_Click(sender, e);
                mediaPlayer.Play();
            }
        }

        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            if (img.Name == "Siguiente")
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Siguiente/siguiente.png"));
            else if (img.Name == "Anterior")
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Anterior/anterior.png"));
        }



        /* Esta función sirve para arrastrar archivos de música*/

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            /*Obtiene la url del archivo*/
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            MessageBox.Show($"la posición 0 es: {files[0]}");
            foreach (var file in files)
            {
                if (IO.Path.GetExtension(file) != ".mp3" && IO.Path.GetExtension(file) != ".WebM")
                {
                    MessageBox.Show("Formato de archivo inválido", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            }
            Name_Music.Text = IO.Path.GetFileName(files[0]);

            int len = files.Length;
            for(int i = 0; i != len; ++i)
            {
                ListBox.Items.Add(IO.Path.GetFileName(files[i]));
                URLS.Add(files[i]);
            }

            ListBox.SelectedIndex = 0;

        }
        /*               Eventos para guardar las canciones favoritas           */

        
        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if(ListBox.Items.Count != 0)
                IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/clic_favorite.png"));
        }

       
    }
}
