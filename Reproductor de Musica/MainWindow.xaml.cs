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
using Reproductor_de_Musica.Utilidades;

namespace Reproductor_de_Musica
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    [Serializable]
    public partial class MainWindow : Window
    {

        private readonly MediaPlayer mediaPlayer = new MediaPlayer();
        private bool IsPaused = true;
        private readonly List<string> URLS = new List<string>();
        public List<string> LTheme = new List<string>();
        public Historial historial = new Historial();
        private TimeSpan position;
        private TimeSpan suma = new TimeSpan();
        private WinAjuste win;
        /* Si theme es igual a 0 quiere decir, que el tema será de color negro,
         * Si es igual a 1 es porque el tema es claro, 2 tema Opera GX y si
         * es igual a 3 el tema es personalizado.
         */
        public int theme = 0;

      


        

        
        public MainWindow()
        {
            
            InitializeComponent();
            if (IO.File.Exists("theme.pytham"))
            {
                GetTheme();
            }

            if (IO.File.Exists("historial.pytham"))
            {
                historial = Utilities<Historial>.GetFile("historial");
                URLS = historial.LURL;
         
                foreach(var data in historial.LHistory)
                {
                    ListBox.Items.Add(data);
                }
            }
        }
        
        
        private void WrapPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ButtonX_Click(object sender, RoutedEventArgs e)
        {
            if(win != null)
                win.Close();
 
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
            /* Creo el try catch para comprobar si lo que ingresa es un botón o un imagen
             * Sirve para que tanto el botón como el textBlock puedan acceder a sus bloque de código.
             */
            TextBlock textBlock = new TextBlock();
            Image image = new Image();
            try
            {
                 textBlock = (TextBlock)sender;
            }
            catch (System.InvalidCastException)
            {
                image = (Image)sender;
            }

            textBlock.Foreground = Brushes.Gray; 
            /* El if comprueba cual textblock fue el que se presionó, 
             * solo sirve para ocultar/mostrar el rectángulo y llama a la función para 
             * agregar la música, es decir, si entra en el else es porque el usuario ingresa 
             * una playlist. */
            if (textBlock.Name == "TextBlock_Add" || image.Name == "Image_Add") 
            {
                Rectangle_Barra.Visibility = Visibility.Visible;
                Rectangle_Barra1.Visibility = Visibility.Hidden;

                using (var fd = new WinForms.OpenFileDialog())
                {
                    fd.Filter = "Image Files(*.MP3; *.Webm)|*.MP3; *.Webm";

                    fd.FilterIndex = 1;

                    if (fd.ShowDialog() == WinForms.DialogResult.OK)
                    {

                        Name_Music.Text = IO.Path.GetFileNameWithoutExtension(fd.SafeFileName);

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

                        TagLib.File tagFile = TagLib.File.Create(fd.FileName);
                        ListBox.Items.Add($"{ListBox.Items.Count + 1} - {IO.Path.GetFileNameWithoutExtension(fd.SafeFileName)}");

                        URLS.Add(fd.FileName);
                        //TextBlock_Author_Name.Text = TagLib.File.Create(fd.FileName).Tag.FirstAlbumArtist;
                        ListBox.SelectedIndex = ListBox.Items.Count - 1;

                        suma += tagFile.Properties.Duration;


                        // Guardamos el historial

                        SaveHistorial(IO.Path.GetFileNameWithoutExtension(fd.SafeFileName));

                        
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

                        Name_Music.Text = IO.Path.GetFileNameWithoutExtension(fd.SafeFileName);

                        


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
                            TagLib.File tagFile = TagLib.File.Create(fd.FileNames[i]);
                            ListBox.Items.Add($"{ListBox.Items.Count + 1} - {IO.Path.GetFileNameWithoutExtension(fd.SafeFileNames[i])}");
                            //ListBox.Items.Add($"{ListBox.Items.Count + 1} - {tagFile.Properties.N}");
                            URLS.Add(fd.FileNames[i]);
                            
                            suma += tagFile.Properties.Duration;
                        }

                        
                        mediaPlayer.Open(new Uri(fd.FileName));
                       
                        

                        // Comprueba si en la lista hay música, si no hay pone el index en 0.
                        if (IsEmpty)
                            ListBox.SelectedIndex = 0;
                        else
                            ListBox.SelectedIndex = ListBox.Items.Count - 1;

                        //Guardamos el historial
                        SaveHistorial();
                    }
                }
            }

            
        }

        
        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            
            mediaPlayer.Play();

            TextBlock_Info_PlayList.Text = $"Duración total: {suma:dd\\:hh\\:mm\\:ss}";
            TextBlock_Author_Name.Text = TagLib.File.Create(URLS[ListBox.SelectedIndex]).Tag.FirstAlbumArtist;
            position = mediaPlayer.NaturalDuration.TimeSpan;
            Slider_Carga.Minimum = 0;
            Slider_Carga.Maximum = position.TotalSeconds;
            Text_MinLength.Text = mediaPlayer.Position.ToString(@"mm\:ss");
            Text_MaxLength.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }

        private void TextBlock_Add_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            if(theme == 0 || theme == 2)
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(207, 207, 207));
            else
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
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
           
            
            
        }

        private void ListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mediaPlayer.Open(new Uri(URLS[ListBox.SelectedIndex]));
            Name_Music.Text = ListBox.SelectedItem.ToString();
            IsPaused = true;
            Button_Pause_Click(sender, e);
            mediaPlayer.Play();
        }

        private void ListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox.SelectedIndex = -1;
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

            //MessageBox.Show($"la posición 0 es: {files[0]}");
            foreach (var file in files)
            {
                if (IO.Path.GetExtension(file) != ".mp3" && IO.Path.GetExtension(file) != ".WebM")
                {
                    MessageBox.Show("Formato de archivo inválido", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

            }

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

            int len = files.Length;
            for(int i = 0; i != len; ++i)
            {
                TagLib.File tagLib = TagLib.File.Create(files[i]);

                ListBox.Items.Add($"{ListBox.Items.Count + 1} - {IO.Path.GetFileNameWithoutExtension(files[i])}");
                URLS.Add(files[i]);
                suma += tagLib.Properties.Duration;
            }


            //Guardamos el historial
            SaveHistorial();
            


            TextBlock_Info_PlayList.Text = $"Duración total: {suma:dd\\:hh\\:mm\\:ss}";

        }
        /*               Eventos para guardar las canciones favoritas           */

        
        private void Image_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            if(ListBox.Items.Count != 0)
                IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/clic_favorite.png"));
        }






        /*            Eventos para el botón de ajustes           */

        private void TextBox_Ajuste_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            win = new WinAjuste(this);
            
            win.Show();
        }



        /*          Función para obtener los elementos    */

        public void GetTheme()
        {
            LTheme = Utilidades.Utilities<List<string>>.GetFile("theme");

            WrapPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);

            Button_X.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);
            Button_X.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);
            Button_Minus.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);
            Button_Minus.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);
            this.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[1]);

            ListBox.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[1]);
            ListBox.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/Styles/ListBox.xaml")
            };

            theme = Convert.ToInt32(LTheme[6]);

            if (theme == 0 || theme == 2)
                ListBox.ItemContainerStyle = (Style)resourceDictionary["Modo_Oscuro"];
            else
                ListBox.ItemContainerStyle = (Style)resourceDictionary["Modo_Claro"];

            StackPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[2]);
            WrapPanel_Secundaria.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[3]);

            TextBlock_Add.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);
            TextBlock_Favorite.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);
            TextBlock_PlayList.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);
            TextBlock_Author_Name.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);
            TextBlock_Ajuste.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);
            Name_Music.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);
            Text_MinLength.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
            Text_MaxLength.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
            TextBlock_Info_PlayList.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);


            
        }

        /* Función para guardar las canciones del historial.
            El parámetro varias es true cuando vamos a guardar más de una canción,
            caso contario, false.
         */

        public void SaveHistorial(string pathname = "", bool varias = true)
        {
            
            Historial historiall = new Historial
            {
                LURL = URLS
            };
            if(!varias)
                historiall.LHistory.Add(IO.Path.GetFileNameWithoutExtension(pathname));
            else
            {
                foreach(var data in ListBox.Items)
                {
                    historiall.LHistory.Add((string)data);
                }
            }
            Utilities<Historial>.SaveData("historial", historiall);
        }

        private void Button_Delete_All_Click(object sender, RoutedEventArgs e)
        {
            if (ListBox.Items.Count != 0) 
            {
                ListBox.Items.Clear();
                IO.File.Delete("historial.pytham");
                mediaPlayer.Stop();
            }
                
        }

    }
}
