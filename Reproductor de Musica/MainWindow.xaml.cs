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

            Rectangle_Barra.Visibility = Visibility.Visible;

            using (var fd = new WinForms.OpenFileDialog())
            {
                fd.Filter = "Image Files(*.MP3)|*.MP3";

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
                    Console.WriteLine("2");
                    URLS.Add(fd.FileName);
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

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            mediaPlayer.Open(new Uri(URLS[ListBox.SelectedIndex]));
            Name_Music.Text = ListBox.SelectedItem.ToString();

            mediaPlayer.Play();
        }


        void Timer_Tick(object sender, EventArgs e)
        {
            Slider_Carga.Value = mediaPlayer.Position.TotalSeconds;
            Text_MinLength.Text = mediaPlayer.Position.ToString(@"mm\:ss");

        }

        private void Slider_Carga_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int pos = Convert.ToInt32(Slider_Carga.Value);
            mediaPlayer.Position = new TimeSpan(0, 0, 0, pos, 0);
        }
    }
}
