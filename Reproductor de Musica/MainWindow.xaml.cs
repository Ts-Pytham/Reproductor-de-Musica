using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Draw = System.Drawing;
using WinForms = System.Windows.Forms;
using IO = System.IO;
using Reproductor_de_Musica.Utilidades;
using Reproductor_de_Musica.src;
using System.Windows.Media.Animation;
using System.Windows.Controls.Primitives;
using System.Threading.Tasks;

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
        private bool IsFavorited = false;
        private bool IsRandom = false; //Comprueba si la lista musical está en modo aleatorio

        private ObservableCollection<Music> ReproductionList = new ObservableCollection<Music>();
        private readonly ObservableCollection<Music> ReproductionListF = new ObservableCollection<Music>();
        private readonly ObservableCollection<Music> ReproductionListAux = new ObservableCollection<Music>(); //Guarda las canciones de reproductionList cuando estamos en modo Favorito

        public  List<string> LTheme = new List<string>();

        public  string PathFile = "";
        
        public  bool Streamer_Picture; // Comprueba si el usuario quiere descargar la imagen de la canción.
        public  Historial historial = new Historial();
        private readonly DiscordRP discordRP;
        private TimeSpan position;
        public  int IsSelected = -1; // Comprueba que canción está seleccionada para darle color

        private TimeSpan suma = new TimeSpan();
        public  WinAjuste win;
        public  Window_Streamer winStreamer;
        public  WindowDownload winDownload;
        /* Si theme es igual a 0 quiere decir, que el tema será de color negro,
         * Si es igual a 1 es porque el tema es claro, 2 tema Opera GX y si
         * es igual a 3 el tema es personalizado.
         */
        public int theme = 0;

        private bool notify = true;

        public MainWindow()
        {
           
            InitializeComponent();
            //Iniciamos un objeto de la clase DiscordRP (Utilidades)
            discordRP = new DiscordRP();
            DataGridP.DataContext = ReproductionList;

            if (IO.File.Exists("PathFile.pytham"))
            {
                PathFile = Utilities<string>.GetFile("PathFile");
            }
            if (IO.File.Exists("theme.pytham"))
            {
                GetTheme();
            }
            else
            {
                List<string> list = new List<string>
                {
                    "#FF000000",
                    "#FF2F3136",
                    "#FF151515",
                    "#FF212121",
                    "#FFFFFFFF",
                    "#FFfdf008",
                    "#FFCFCFCF",
                    "#FFFFFFFF",
                    "0"
                };
                LTheme = list;
            }

            if (IO.File.Exists("vp.pytham"))
            {
                List<Object> list = Utilities<List<Object>>.GetFile("vp");
                Slider_Volumen.Value = (double)list[0];
               
                suma = (TimeSpan)list[1];

            }


            if (IO.File.Exists("historial.pytham"))
            {
                
                historial = Utilities<Historial>.GetFile("historial");

                var rep = historial.Musics;
                foreach(var music in rep)
                {
                    if (IO.File.Exists(music.Path))
                    {
                        ReproductionList.Add(music);
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró la canción {music.Path}\n{music.Name}");
                    }
                }
            }

            if (IO.File.Exists("historial_favorites.pytham"))
            {
                var rep = Utilities<Historial>.GetFile("historial_favorites").Musics;
                foreach (var music in rep)
                {
                    if (IO.File.Exists(music.Path))
                    {
                        ReproductionListF.Add(music);
                    }
                    else
                    {
                        MessageBox.Show($"No se encontró la canción {music.Path}\n{music.Name}");
                    }
                }
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Guardamos el historial

            SaveHistorial();

            //Guardamos el historial de favoritos

            SaveHistorialFavorite();
            //Guardamos el volumen y la duración total de la playlist.
            SaveVolume_Number_PlayList();

            if (win != null)
                win.Close();
            if (winStreamer != null)
                winStreamer.Close();


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

        private void Button_Notification_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;

            if (notify)
            {
               button.Content = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Notifications/notificacion.png")) };
                notify = false;
            }
            else
            {
                button.Content = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Notifications/notificacion1.png")) };
                notify = true;
            }
        }

        private void ThumbButtonInfo_Pause(object sender, EventArgs e)
        {
            Button_Pause_Click(sender, new RoutedEventArgs());
        }
        private void Button_Pause_Click(object sender, RoutedEventArgs e)
        {
            
            
            if (!IsPaused)
            {
                Image img = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/boton-de-play.png")) };
                Button_Reproductor.Content = img;
                IsPaused = true;
                taskBarItemInfo.ThumbButtonInfos[1].Description = "Play";
                taskBarItemInfo.ThumbButtonInfos[1].ImageSource = img.Source;
                mediaPlayer.Pause();
            }
            else
            {
                Image img = new Image { Source = new BitmapImage(new Uri(uriString: @"pack://application:,,,/IMG/pausa.png")) };
                Button_Reproductor.Content = img;
                IsPaused = false;
                taskBarItemInfo.ThumbButtonInfos[1].Description = "Pause";
                taskBarItemInfo.ThumbButtonInfos[1].ImageSource = img.Source;
                mediaPlayer.Play();

            }

        }
        /* Para hacer el efecto de darle click al textblock y que sea tipo button*/
        private async void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            /* Creo el try catch para comprobar si lo que ingresa es un botón o un imagen
             * Sirve para que tanto el botón como el textBlock puedan acceder a sus bloque de código.
             */
            if (IsFavorited)
            {
                MessageBox.Show("¡No puedes agregar una o varias música cuando estés en modo favorito!", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

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
                Rectangle_Barra2.Visibility = Visibility.Hidden;
                Rectangle_Barra3.Visibility = Visibility.Hidden;
                Rectangle_Barra5.Visibility = Visibility.Hidden;

                using (var fd = new WinForms.OpenFileDialog())
                {
                    fd.Filter = "Music Files(*.MP3; *.Webm; *.wav)|*.MP3; *.Webm; *.wav";

                    fd.FilterIndex = 1;

                    if (fd.ShowDialog() == WinForms.DialogResult.OK)
                    {
                        
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


                        mediaPlayer.Volume = Slider_Volumen.Value;
                        Slider_Volumen.Value = mediaPlayer.Volume;

                        mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

                        TagLib.File tagFile = TagLib.File.Create(fd.FileName, "audio/mp3", TagLib.ReadStyle.Average);
                        string author, album, duration;

                        try
                        {
                            author = $"{(!String.IsNullOrEmpty(tagFile.Tag.Artists[0]) ? tagFile.Tag.Artists[0] : "N/A")}.";
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            author = $"N/A.\n";
                        }

                        duration = $"{tagFile.Properties.Duration:hh\\:mm\\:ss}.";
                        album = $"{(!String.IsNullOrEmpty(tagFile.Tag.Album) ? tagFile.Tag.Album : "N/A")}.";

                        ReproductionList.Add(new Music
                        {
                            Name = IO.Path.GetFileNameWithoutExtension(fd.SafeFileName),
                            Path = fd.FileName,
                            Album = album,
                            Author = author,
                            Duration = duration
                        });
                    
                        DataGridP.Items.Refresh();
                        DataGridP.SelectedIndex = DataGridP.Items.Count - 1;
                        suma += tagFile.Properties.Duration;
                        DataGridP.Resources[SystemColors.HighlightTextBrushKey] = new BrushConverter().ConvertFromString("#FFFE4164");
                    }
                }
            }
            else
            {
                Rectangle_Barra1.Visibility = Visibility.Visible;
                Rectangle_Barra.Visibility = Visibility.Hidden;
                Rectangle_Barra2.Visibility = Visibility.Hidden;
                Rectangle_Barra3.Visibility = Visibility.Hidden;
                Rectangle_Barra5.Visibility = Visibility.Hidden;
                using (var fd = new WinForms.OpenFileDialog())
                {
                    fd.Multiselect = true;

                    fd.Filter = "Music Files(*.MP3; *.Webm)|*.MP3; *.Webm";

                    fd.FilterIndex = 1;

                    if (fd.ShowDialog() == WinForms.DialogResult.OK)
                    {

                        Image img = new Image { Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/pausa.png")) };
                        Button_Reproductor.Content = img;
                        IsPaused = false;

                        DispatcherTimer timer = new DispatcherTimer
                        {
                            Interval = TimeSpan.FromSeconds(1)
                        };
                        timer.Tick += Timer_Tick;
                        timer.Start();


                        mediaPlayer.Volume = Slider_Volumen.Value;
                        Slider_Volumen.Value = mediaPlayer.Volume;

                        mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
                        int len = fd.FileNames.Length;
                        await Task.Run(() =>
                        {
                            for (int i = 0; i != len; ++i)
                            {
                                TagLib.File tagFile = TagLib.File.Create(fd.FileNames[i], "audio/mp3", TagLib.ReadStyle.Average);

                                string author, album, duration;

                                try
                                {
                                    author = $"{(!String.IsNullOrEmpty(tagFile.Tag.Artists[0]) ? tagFile.Tag.Artists[0] : "N/A")}.";
                                }
                                catch (System.IndexOutOfRangeException)
                                {
                                    author = $"N/A.";
                                }

                                duration = $"{tagFile.Properties.Duration:hh\\:mm\\:ss}.";
                                album = $"{(!String.IsNullOrEmpty(tagFile.Tag.Album) ? tagFile.Tag.Album : "N/A")}.";

                                Application.Current.Dispatcher.Invoke(delegate
                                {
                                    ReproductionList.Add(new Music
                                    {
                                        Name = IO.Path.GetFileNameWithoutExtension(fd.SafeFileNames[i]),
                                        Path = fd.FileNames[i],
                                        Album = album,
                                        Author = author,
                                        Duration = duration
                                    });
                                });
                                suma += tagFile.Properties.Duration;
                            }
                        });
                        

                        

                        DataGridP.SelectedIndex = 0;
                        DataGridP.Resources[SystemColors.HighlightTextBrushKey] = new BrushConverter().ConvertFromString("#FFFE4164");
                        mediaPlayer.Open(new Uri(ReproductionList[0].Path));
                    }
                }
            }
        }

        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            try
            {
                mediaPlayer.Play();
                GetFavorite();

                TextBlock_Info_PlayList.Text = $"Duración total: {suma:dd\\:hh\\:mm\\:ss}";
                Music music = ReproductionList[DataGridP.SelectedIndex];
                TagLib.File tagLib = TagLib.File.Create(music.Path, "audio/mp3", TagLib.ReadStyle.Average);

                TextBlock_Author_Name.Text = tagLib.Tag.Artists.Length > 0 ? tagLib.Tag.Artists[0] : "";

                Name_Music.Text = music.Name;
                Title = $"JohMusic: {music.Name}";
                CheckFilesStreamerMode(tagLib);
                // Agregamos la imagen de la música
                if (tagLib.Tag.Pictures.Length > 0)
                {
                    try
                    {
                        TagLib.IPicture picture = tagLib.Tag.Pictures[0];
                        IO.MemoryStream ms = new IO.MemoryStream(picture.Data.Data);

                        BitmapImage bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = ms;
                        bitmap.EndInit();

                        Image_Song.Source = bitmap;

                        if (Streamer_Picture)
                            Download_Image(tagLib);
                    }
                    catch (NotSupportedException)
                    {
                        Image_Song.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/no_found_music.jpg"));
                    }
                }
                else
                {
                    Image_Song.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/no_found_music.jpg"));
                }

                Name_Music.ToolTip = new ToolTip()
                {
                    Content = Name_Music.Text,
                    Background = Brushes.Black,
                    Foreground = Brushes.White

                };

                position = mediaPlayer.NaturalDuration.TimeSpan;


                Slider_Carga.Minimum = 0;
                Slider_Carga.Maximum = position.TotalSeconds;
                Text_MinLength.Text = mediaPlayer.Position.ToString(@"mm\:ss");
                Text_MaxLength.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");

                if (Name_Music.Text.Length > 25)
                    RightToLeftMarquee();
                else
                    Name_Music.BeginAnimation(Canvas.RightProperty, null);

                //Cambia el estado de discord.
                discordRP.Details = Name_Music.Text; //"Escuchando"
                discordRP.State = $"{TextBlock_Author_Name.Text}";
                discordRP.TimeEnd = position.TotalSeconds;
                discordRP.UpdateActivity();

                if (WindowState == WindowState.Minimized && notify) // Comprueba si está minimizado y envia una notificación
                    Notification.Show("Escuchando Música", $"Se está escuchando {Name_Music.Text}.\nDuración: {Text_MaxLength.Text}.");
            }

            catch(Exception ms)
            {
                MessageBox.Show(ms.Message);
            }
        }

        private void TextBlock_Add_MouseLeave(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;
            if (theme == 5)
                textBlock.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[6]);
            else if (theme != 1)
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(207, 207, 207));
            else
                textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        }

        private void TextBlock_Add_MouseEnter(object sender, MouseEventArgs e)
        {
            TextBlock textBlock = (TextBlock)sender;

            textBlock.Foreground = theme == 5 ? (Brush)new BrushConverter().ConvertFrom(LTheme[6]) : Brushes.White;
        }

        private void TextBlock_Add_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            TextBlock_Add_MouseEnter(sender, e);
        }

        /* Eventos del Slider para el volumen*/

        private void Slider_Carga_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int pos = Convert.ToInt32(Slider_Carga.Value);
            mediaPlayer.Position = new TimeSpan(0, 0, 0, pos, 0);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mediaPlayer.Volume = e.NewValue;
        }

        private void Slider_Carga_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //Aún no hay nada
        }

        private void Slider_MouseMove(object sender, MouseEventArgs e)
        {

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var slider = (Slider)sender;
                Point position = e.GetPosition(slider);
                double d = 1.0d / slider.ActualWidth * position.X;
                var p = slider.Maximum * d;
                slider.Value = p;
            }

        }
        //================================================================

        /*            Eventos del DataGrid         */

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();

        }

        private void DataGridP_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (DataGridP.SelectedIndex != -1 && DataGridP.Items.Count != 0) {
               
                switch (e.Key)
                {
                    
                    case Key.Delete:
                        {
                            ReproductionList.RemoveAt(DataGridP.SelectedIndex);
                            DataGridP.Items.Refresh();
                            break;
                        }
                    case Key.Space:
                        {
                            Button_Pause_Click(sender, new RoutedEventArgs());
                            break;
                        }
                    case Key.Enter: case Key.Down:
                        {
                            ThumbButtonInfo_Siguiente(sender, new EventArgs());
                            break;
                        }
                    case Key.Up:
                        {
                            ThumbButtonInfo_Anterior(sender, new EventArgs());
                            break;
                        }
                    case Key.Right:
                        {
                            Slider_Volumen.Value += 0.05d;
                            break;
                        }
                    case Key.Left:
                        {
                            Slider_Volumen.Value -= 0.05d;
                            break;
                        }
                }
            }
        }

        private void DataGridP_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridP.SelectedIndex != -1)
            {
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timer.Tick += Timer_Tick;
                timer.Start();

                mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

                DataGridP.Resources[SystemColors.HighlightTextBrushKey] = new BrushConverter().ConvertFromString("#FFFE4164");
                
                mediaPlayer.Open(new Uri(ReproductionList[DataGridP.SelectedIndex].Path));
                
                IsPaused = true;
                Button_Pause_Click(sender, e);
                mediaPlayer.Play();
            }
        }


        private void DataGridP_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            if (DataGridP.SelectedIndex != -1)
            {
                
                DataGridP.Resources[SystemColors.HighlightTextBrushKey] = new BrushConverter().ConvertFromString("#FFFFFFFF");
            }
        }


        /* Esta función sirve para arrastrar archivos de música*/

        private void ListBox_Drop(object sender, DragEventArgs e)
        {
       
            if (IsFavorited)
            {
                MessageBox.Show("¡No puedes agregar una o varias músicas cuando estés en modo favorito!", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            /*Obtiene la url del archivo*/
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            //MessageBox.Show($"la posición 0 es: {files[0]}");
            Task.Run(() =>
            {
                foreach (var file in files)
                {
                    string GetExtensionFile = IO.Path.GetExtension(file);
                    int CompareResult1 = String.Compare(GetExtensionFile, ".mp3", StringComparison.OrdinalIgnoreCase);
                    int CompareResult2 = String.Compare(GetExtensionFile, ".WebM", StringComparison.OrdinalIgnoreCase);
                    int CompareResult3 = String.Compare(GetExtensionFile, ".wav", StringComparison.OrdinalIgnoreCase);
                    if (CompareResult1 != 0 && CompareResult2 != 0 && CompareResult3 != 0)
                    {
                        MessageBox.Show($"Formato de archivo inválido.\nArchivo: {file}", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);

                    }
                    else
                    {
                        TagLib.File tagLib = TagLib.File.Create(file, "audio/mp3", TagLib.ReadStyle.Average);
                        string author;
                        try
                        {
                            author = $"{(!String.IsNullOrEmpty(tagLib.Tag.Artists[0]) ? tagLib.Tag.Artists[0] : "N/A")}.";
                        }
                        catch (System.IndexOutOfRangeException)
                        {
                            author = $"N/A.";
                        }

                        Application.Current.Dispatcher.Invoke(delegate {
                            ReproductionList.Add(new Music
                            {
                                Name = IO.Path.GetFileNameWithoutExtension(file),
                                Path = file,
                                Author = author,
                                Duration = $"{tagLib.Properties.Duration:hh\\:mm\\:ss}.",
                                Album = $"{(!String.IsNullOrEmpty(tagLib.Tag.Album) ? tagLib.Tag.Album : "N/A")}."
                            });
                        });
                        

                        suma += tagLib.Properties.Duration;
                    }

                }
            });
            

            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;
            timer.Start();

            mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

            TextBlock_Info_PlayList.Text = $"Duración total: {suma:dd\\:hh\\:mm\\:ss}";


        }


        //=======================================================================

        private void Timer_Tick(object sender, EventArgs e)
        {
            Slider_Carga.Value = mediaPlayer.Position.TotalSeconds;
            Text_MinLength.Text = mediaPlayer.Position.ToString(@"mm\:ss");

            
            
            if (Text_MinLength.Text == Text_MaxLength.Text)
            {
                if (IsRandom && index_ra != RA_URLS.Count)
                {
                    PlayListRandom();
                }
                else if (DataGridP.Items.Count != 1 && DataGridP.SelectedIndex != DataGridP.Items.Count - 1)
                {
                    Music music = ReproductionList[DataGridP.SelectedIndex + 1];
                    mediaPlayer.Open(new Uri(music.Path));
                    Name_Music.Text = music.Name;
                    DataGridP.SelectedIndex += 1;
                   
                    mediaPlayer.Play();
                }

            }

        }


        /*     EVENTOS POR DEFECTOS DE LAS IMAGENES    */

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            Image img = (Image)sender;
            if (img.Name == "Siguiente")
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Siguiente/hover_siguiente.png"));
            else if (img.Name == "Anterior")
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Anterior/hover_anterior.png"));
            else if (img.Name == "Repetir")
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

        //Mover las canciones, anterior y siguiente.

        private void ThumbButtonInfo_Siguiente(object sender, EventArgs e)
        {

            Image_MouseLeftButtonDown(new Image() { Name = "Siguiente" }, null);
        }

        private void ThumbButtonInfo_Anterior(object sender, EventArgs e)
        {
            Image_MouseLeftButtonDown(new Image() { Name = "Anterior" }, null);
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            
            if (img.Name == "Siguiente"  && DataGridP.Items.Count != 0)
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Siguiente/enter_siguiente.png"));
                if (IsRandom && index_ra <= RA_URLS.Count - 1)
                {
                    PlayListRandom();
                    if (index_ra != RA_URLS.Count)
                        return;
                    
                }
                else if(IsRandom)
                {
  
                    index_ra = RA_URLS.Count;
                }
                if (DataGridP.Items.Count == 1)
                {
                    mediaPlayer.Open(new Uri(ReproductionList[0].Path));


                }
                else if (DataGridP.SelectedIndex == DataGridP.Items.Count - 1)
                {
                    mediaPlayer.Open(new Uri(ReproductionList[0].Path));
                    DataGridP.SelectedIndex = 0;
                    IsSelected = 0;

                }
                else
                {
                    mediaPlayer.Open(new Uri(ReproductionList[DataGridP.SelectedIndex + 1].Path));
                    DataGridP.SelectedIndex += 1;
                }

                IsPaused = true;
                Button_Pause_Click(sender, e);
                mediaPlayer.Play();
            }
            else if (img.Name == "Anterior" && DataGridP.Items.Count != 0)
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Anterior/enter_anterior.png"));
                if (IsRandom && index_ra > 0)
                {
                    PlayListRandom(true);
                    return;
                }
                else
                {
                    index_ra = -1;
                }
                if (DataGridP.Items.Count == 1)
                {
                    mediaPlayer.Open(new Uri(ReproductionList[0].Path));

                }
                else if(DataGridP.SelectedIndex == 0)
                {
                    return;

                }
                else if (DataGridP.SelectedIndex != 0)
                {
                    


                    mediaPlayer.Open(new Uri(ReproductionList[DataGridP.SelectedIndex - 1].Path));
                    DataGridP.SelectedIndex -= 1;

                }
                IsPaused = true;
                Button_Pause_Click(sender, e);
                mediaPlayer.Play();
            }
            else if (img.Name == "Repetir")
            {
                if (DataGridP.Items.Count != 0) {
                    img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Repetir/click_actualizar.png"));

                    mediaPlayer.Open(new Uri(ReproductionList[DataGridP.SelectedIndex].Path));

                    IsPaused = true;
                    Button_Pause_Click(sender, e);
                    mediaPlayer.Play();
                }
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
        /*               Eventos para una lista aleatoria                       */
        List<int> RA_URLS;
        private int index_ra;
        private void Image_MouseLeftButtonDown_Aleatorio(object sender, MouseButtonEventArgs e)
        {
            if (!IsRandom)
            {
                if (ReproductionList.Count != 0)
                {
                    index_ra = -1;
                    int len = ReproductionList.Count;
                    RA_URLS = new List<int>();
                    for (int i = 0; i != len; ++i)
                        RA_URLS.Add(i);

                    RA_URLS = Utilities<int>.RandomSort(RA_URLS);
                    IMG_Aleatorio.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Aleatorio/clic_barajar.png"));
                    IsRandom = true;
                }
                else
                    MessageBox.Show("¡No hay ninguna canción en la PlayList, agrega una!", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                RA_URLS.Clear();
                IMG_Aleatorio.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Aleatorio/barajar.png"));
                IsRandom = false;
            }
            
        }

        private void PlayListRandom(bool reverse = false)
        {
            if (!reverse)
                index_ra++;
            else
                index_ra--;

            if (index_ra == -1 || index_ra == RA_URLS.Count)
                return;

            int index = RA_URLS[index_ra];
            mediaPlayer.Open(new Uri(ReproductionList[index].Path));
            Name_Music.Text = ReproductionList[index].Name;
            DataGridP.SelectedIndex = index;

            MessageBox.Show(index_ra.ToString());

        }   
        /*               Eventos para guardar las canciones favoritas           */


        private void Image_Favorite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (DataGridP.Items.Count != 0 && DataGridP.SelectedIndex != -1)
            {
                if (IMG_Favorite.Source.ToString() == @"pack://application:,,,/IMG/Favorite/favorite.png")
                {

                    IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/clic_favorite.png"));
                    ReproductionListF.Add((Music)DataGridP.SelectedItem);
                }
                else
                {

                    IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/favorite.png"));
                    if (IsFavorited)
                    {
                        ReproductionList.RemoveAt(DataGridP.SelectedIndex);
                        DataGridP.Items.RemoveAt(DataGridP.SelectedIndex);
                        mediaPlayer.Stop();

                    }

                    ReproductionListF.Remove((Music)DataGridP.SelectedItem);
                }
            }

        }

        private void TextBlock_Favorite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle_Barra.Visibility = Rectangle_Barra1.Visibility = Rectangle_Barra3.Visibility = Rectangle_Barra5.Visibility = Visibility.Hidden;
            Rectangle_Barra2.Visibility = Visibility.Visible;

            // Desactivamos la playlist aleatoria.
            IsRandom = false;
            IMG_Aleatorio.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Aleatorio/barajar.png"));


            if (ReproductionListF.Count != 0)
            {
                if (!IsFavorited)
                {
                    ReproductionListF.Clear();
                    foreach (var data in DataGridP.Items)
                    {
                        ReproductionListAux.Add(data as Music);
                    }

                    //DataGridP.Items.Clear(); *Si no me equivoco, esto se actualiza por el observableCollection*

                    ReproductionList.Clear();

                    ReproductionList = new ObservableCollection<Music>(ReproductionListF);

                    DataGridP.Items.Refresh();

                    /* Si no te actualiza el código, modifico algo de acá*/

                    IsFavorited = true;
                    TextBlock_Favorite.Text = "PlayList";
                }
                else
                {
                    TextBlock_Favorite.Text = "Favoritas";

                    ReproductionList = new ObservableCollection<Music>(ReproductionListAux);
                    IsFavorited = false;

                }
            }
            else if (DataGridP.Items.Count == 0 && IsFavorited)
            {
                TextBlock_Favorite.Text = "Favoritas";
               
                ReproductionList = new ObservableCollection<Music>(ReproductionListAux);
           
                IsFavorited = false;
            }


        }

        private void GetFavorite()
        {
            if (ReproductionListF.Count != 0 && DataGridP.SelectedIndex != -1)
            {
                try
                {
                    List<String> aux = new List<String>();
                    foreach (var song in ReproductionListF)
                    {
                        aux.Add(song.Name);
                    }
                    var N = DataGridP.Items[DataGridP.SelectedIndex] as Music;

                    if (Utilities<String>.BinarySearch(aux, N.Name))
                        IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/clic_favorite.png"));
                    else
                        IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/favorite.png"));
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void SaveHistorialFavorite()
        {
            Historial historiall = new Historial
            {
                Musics = ReproductionListF
            };

            Utilities<Historial>.SaveData("historial_favorites", historiall);
        }
        //=================================================================




        /*            Eventos para el botón de ajustes           */

        private void TextBox_Ajuste_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle_Barra.Visibility = Visibility.Hidden;
            Rectangle_Barra1.Visibility = Visibility.Hidden;
            Rectangle_Barra2.Visibility = Visibility.Hidden;
            Rectangle_Barra5.Visibility = Visibility.Hidden;
            Rectangle_Barra3.Visibility = Visibility.Visible;

            if (win == null)
            {
                win = new WinAjuste(this);
                win.Show();
            }

        }



        /*          Función para obtener los elementos    */

        public void GetTheme()
        {
            LTheme = Utilidades.Utilities<List<string>>.GetFile("theme");
            theme = Convert.ToInt32(LTheme[8]);
            WrapPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);

            Button_X.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);
            Button_X.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);
            Button_Minus.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);
            Button_Minus.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[0]);
            this.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[1]);

            DataGridP.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);
            DataGridP.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);

            var style = new Style(typeof(DataGridColumnHeader));
            style.Setters.Add(new Setter { Property = BackgroundProperty, Value = (Brush)new BrushConverter().ConvertFrom(LTheme[0]) });
            style.Setters.Add(new Setter { Property = HeightProperty, Value = 30d });
            style.Seal();
            DataGridP.ColumnHeaderStyle = style;

            Button_Erase.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
            Button_Erase.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[7]);

            Border1.BorderBrush = Border2.BorderBrush = Border3.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);


            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/Styles/ListBox.xaml")
            };

            if (theme != 1)
                DataGridP.ItemContainerStyle = (Style)resourceDictionary["Modo_Oscuro"];
            else
                DataGridP.ItemContainerStyle = (Style)resourceDictionary["Modo_Claro"];

            StackPanel_Principal.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[2]);
            WrapPanel_Secundaria.Background = (Brush)new BrushConverter().ConvertFrom(LTheme[3]);

            TextBlock_Add.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[6]);
            TextBlock_Favorite.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[6]);
            TextBlock_PlayList.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[6]);
            TextBlock_Author_Name.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[6]);
            TextBlock_Ajuste.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[6]);
            Name_Music.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[6]);
            Text_MinLength.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
            Text_MaxLength.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
            TextBlock_Info_PlayList.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);

        }

        /* Función para guardar las canciones del historial.
            El parámetro varias es true cuando vamos a guardar más de una canción,
            caso contario, false.
         */

        public void SaveHistorial()
        {
            if (IsFavorited)
            {
                ReproductionList = new ObservableCollection<Music>(ReproductionListAux);
            }
            Historial historiall = new Historial
            {
                Musics = new ObservableCollection<Music>(ReproductionList)
            };
       
            Utilities<Historial>.SaveData("historial", historiall);
        }


        public void SaveVolume_Number_PlayList()
        {
            
            List<Object> list = new List<object>
            {
                Slider_Volumen.Value,
                suma
            };
            Utilities<List<Object>>.SaveData("vp", list);
        }

        private void Button_Delete_All_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridP.Items.Count != 0 && !IsFavorited)
            {


                mediaPlayer.Close();
                suma = new TimeSpan();
                TextBlock_Info_PlayList.Text = $"";
                Name_Music.Text = "-";
                Name_Music.BeginAnimation(Canvas.RightProperty, null);

                TextBlock_Author_Name.Text = "-";
                Text_MinLength.Text = "-";
                Text_MaxLength.Text = "-";
                IsSelected = -1;

                ReproductionList.Clear();
                DataGridP.Items.Refresh();
                IO.File.Delete("historial.pytham");
            }
        }

        /*                   Modo Streamer                */
        private void TextBox_Streamer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (winStreamer == null)
            {
                Rectangle_Barra.Visibility = Rectangle_Barra2.Visibility = Rectangle_Barra1.Visibility = Rectangle_Barra3.Visibility = Visibility.Hidden;
                Rectangle_Barra5.Visibility = Visibility.Visible;
                winStreamer = new Window_Streamer(this);
                winStreamer.Show();
            }
        }

        //==================================================================================
        /*               Funciones para el modo streamer 
                        Función para comprobar si existen los archivos (Modo Streamer)*/

        private void CheckFilesStreamerMode(TagLib.File tagLib)
        {
            if (!String.IsNullOrEmpty(PathFile)) {
                
                if (IO.File.Exists($@"{PathFile}\NameSong.txt"))
                {
                    string path = $@"{PathFile}\NameSong.txt";

                    using (var fs = IO.File.Create(path))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(Name_Music.Text);
                        fs.Write(info, 0, info.Length);
                    }
                }
                if (IO.File.Exists($@"{PathFile}\AlbumSong.txt"))
                {
                    string path = $@"{PathFile}\AlbumSong.txt";
                    using (var fs = IO.File.Create(path))
                    {
                        string data = tagLib.Tag.Album;
                        if (string.IsNullOrEmpty(data))
                            data = "";
                        byte[] info = new UTF8Encoding(true).GetBytes(data);

                      
                        fs.Write(info, 0, info.Length);
                    }
                }

                if (IO.File.Exists($@"{PathFile}\YearSong.txt"))
                {
                    string path = $@"{PathFile}\YearSong.txt";
                    using (var fs = IO.File.Create(path))
                    {
                        string data = tagLib.Tag.Year.ToString();
                        if (string.IsNullOrEmpty(data))
                            data = "";
                        byte[] info = new UTF8Encoding(true).GetBytes(data);
                    
                        fs.Write(info, 0, info.Length);
                    }
                }

                if (IO.File.Exists($@"{PathFile}\AuthorSong.txt"))
                {
                    string path = $@"{PathFile}\AuthorSong.txt";
                    using (var fs = IO.File.Create(path))
                    {
                        string data = tagLib.Tag.FirstAlbumArtist;
                        if (string.IsNullOrEmpty(data))
                            data = "";
                        byte[] info = new UTF8Encoding(true).GetBytes(data);
                        fs.Write(info, 0, info.Length);
                    }
                }

            }
        }

        // Función para descargar la imagen

        private void Download_Image(TagLib.File tagLib)
        {
            if (!string.IsNullOrEmpty(PathFile))
            {
            
                TagLib.IPicture picture = tagLib.Tag.Pictures[0];


                using (var ms = new IO.MemoryStream(picture.Data.Data))
                {
                    Draw.Image img = null;

                    img = Draw.Image.FromStream(ms);
                    img.Save($@"{PathFile}\data.jpg");
                }

            }
        }

        // Función para texto marquesita de derecha a izquierda
        private void RightToLeftMarquee()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation
            {
                From = -Canvas_Music.ActualWidth,
                To = Canvas_Music.ActualWidth,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(6))
            };
            
            Name_Music.BeginAnimation(Canvas.RightProperty, doubleAnimation);
            
        }

        /* Descargar música */
        private void TextBlock_Download_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            winDownload = new WindowDownload(this);
            winDownload.Show();
        }

       
    }
}
