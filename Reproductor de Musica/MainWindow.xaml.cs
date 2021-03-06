﻿using System;
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
using Draw = System.Drawing;
using WinForms = System.Windows.Forms;
using IO = System.IO;
using Reproductor_de_Musica.Utilidades;
using Reproductor_de_Musica.src;
using System.Windows.Media.Animation;

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
        private List<string> URLS = new List<string>();
        private readonly List<string> F_URLS = new List<string>();
        private readonly List<string> ListFavorites = new List<string>();
        public  List<string> LAux_Songs = new List<string>();//Guarda temporalmente las canciones.
        public  List<string> LAux_URLS;//Guarda temporalmente las urls de las canciones.
        public  List<string> LTheme = new List<string>();
        private string Name_Song_URL;
        public  string PathFile = "";
        
        public  bool Streamer_Picture; // Comprueba si el usuario quiere descargar la imagen de la canción.
        public  Historial historial = new Historial();
        private TimeSpan position;
        public  int IsSelected = -1; // Comprueba que canción está seleccionada para darle color

        private TimeSpan suma = new TimeSpan();
        public  WinAjuste win;
        public  Window_Streamer winStreamer;
        /* Si theme es igual a 0 quiere decir, que el tema será de color negro,
         * Si es igual a 1 es porque el tema es claro, 2 tema Opera GX y si
         * es igual a 3 el tema es personalizado.
         */
        public int theme = 0;



        public MainWindow()
        {

            InitializeComponent();

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
                URLS = historial.LURL;
                int i = 0;
                foreach (var data in historial.LHistory)
                {
                    TagLib.File tagFile = TagLib.File.Create(URLS[i], "audio/mp3", TagLib.ReadStyle.Average);
                    string str = $"Autor: {(!String.IsNullOrEmpty(tagFile.Tag.FirstAlbumArtist) ? tagFile.Tag.FirstAlbumArtist : "N/A")}.\n" +
                            $"Duración: {tagFile.Properties.Duration:hh\\:mm\\:ss}.\n" +
                            $"Tamaño: {Math.Round(new IO.FileInfo(URLS[i]).Length / 1048576d, 3)} MB.\n" +
                            $"Album: {(!String.IsNullOrEmpty(tagFile.Tag.Album) ? tagFile.Tag.Album : "N/A")}.";
                    ToolTip toolTip = new ToolTip
                    {
                        Content = str
                    };
                    TextBlock tb = new TextBlock
                    {
                        Text = data,
                        ToolTip = toolTip
                    };

                    ListBox.Items.Add(tb);
                    i++;
                }
            }

            if (IO.File.Exists("historial_favorites.pytham"))
            {
                ListFavorites = Utilities<Historial>.GetFile("historial_favorites").LHistory;
                F_URLS = Utilities<Historial>.GetFile("historial_favorites").LURL;
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

                        string str = $"Autor: {(!String.IsNullOrEmpty(tagFile.Tag.FirstAlbumArtist) ? tagFile.Tag.FirstAlbumArtist : "N/A")}.\n" +
                            $"Duración: {tagFile.Properties.Duration:hh\\:mm\\:ss}.\n" +
                            $"Tamaño: {Math.Round(new IO.FileInfo(fd.FileName).Length / 1048576d, 3)} MB.\n" +
                            $"Album: {(!String.IsNullOrEmpty(tagFile.Tag.Album) ? tagFile.Tag.Album : "N/A")}.";
                        ToolTip toolTip = new ToolTip
                        {
                            Content = str
                        };
                        TextBlock tb = new TextBlock
                        {
                            Text = $"{ListBox.Items.Count + 1} - {IO.Path.GetFileNameWithoutExtension(fd.SafeFileName)}",
                            ToolTip = toolTip
                        };
                        ListBox.Items.Add(tb);
                        URLS.Add(fd.FileName);

                        //Comprueba si el objeto anterior está seleccionado, si es así, se lo quita para ponerselo al actual.
                        if (IsSelected != -1)
                            ((TextBlock)ListBox.Items[IsSelected]).Foreground = Brushes.White;

                        ListBox.SelectedIndex = ListBox.Items.Count - 1;

                        ((TextBlock)ListBox.Items[ListBox.SelectedIndex]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");

                        IsSelected = ListBox.SelectedIndex;

                        suma += tagFile.Properties.Duration;

                    }
                }
            }
            else
            {
                Rectangle_Barra1.Visibility = Visibility.Visible;
                Rectangle_Barra.Visibility = Visibility.Hidden;
                Rectangle_Barra2.Visibility = Visibility.Hidden;
                Rectangle_Barra3.Visibility = Visibility.Hidden;

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

                        bool IsEmpty = true;
                        if (ListBox.Items.Count != 0)
                            IsEmpty = false;

                        int len = fd.FileNames.Length;

                        for (int i = 0; i != len; ++i)
                        {
                            TagLib.File tagFile = TagLib.File.Create(fd.FileNames[i], "audio/mp3", TagLib.ReadStyle.Average);

                            string str = $"Autor: {(!String.IsNullOrEmpty(tagFile.Tag.FirstAlbumArtist) ? tagFile.Tag.FirstAlbumArtist : "N/A")}.\n" +
                            $"Duración: {tagFile.Properties.Duration:hh\\:mm\\:ss}.\n" +
                            $"Tamaño: {Math.Round(new IO.FileInfo(fd.FileNames[i]).Length / 1048576d, 3)} MB.\n" +
                            $"Album: {(!String.IsNullOrEmpty(tagFile.Tag.Album) ? tagFile.Tag.Album : "N/A")}.";

                            ToolTip toolTip = new ToolTip
                            {
                                Content = str
                            };
                            TextBlock tb = new TextBlock
                            {
                                Text = $"{ListBox.Items.Count + 1} - {IO.Path.GetFileNameWithoutExtension(fd.SafeFileNames[i])}",
                                ToolTip = toolTip
                            };

                            ListBox.Items.Add(tb);
                            //ListBox.Items.Add($"{ListBox.Items.Count + 1} - {tagFile.Properties.N}");
                            URLS.Add(fd.FileNames[i]);


                            suma += tagFile.Properties.Duration;
                        }

                        if (IsSelected != -1)
                            ((TextBlock)ListBox.Items[IsSelected]).Foreground = Brushes.White;

                        // Comprueba si en la lista hay música, si no hay pone el index en 0.
                        if (IsEmpty)
                        {
                            ListBox.SelectedIndex = 0;
                            ((TextBlock)ListBox.Items[0]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");
                            mediaPlayer.Open(new Uri(URLS[0]));
                            IsSelected = 0;
                        }
                        else
                        {
                            ListBox.SelectedIndex = ListBox.Items.Count - 1;
                            ((TextBlock)ListBox.Items[ListBox.SelectedIndex]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");
                            mediaPlayer.Open(new Uri(URLS[URLS.Count - 1]));
                            IsSelected = ListBox.Items.Count - 1;
                        }
                    }
                }
            }
        }


        private void MediaPlayer_MediaOpened(object sender, EventArgs e)
        {
            // Comprueba que IsSelected ya tenga un valor establecido (el anterior) y le cambia de color.
            if (IsSelected != -1)
            {
                ((TextBlock)ListBox.Items[IsSelected]).Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
            }

            IsSelected = ListBox.SelectedIndex;
            ((TextBlock)ListBox.Items[IsSelected]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");


            mediaPlayer.Play();
            GetFavorite();

            TextBlock_Info_PlayList.Text = $"Duración total: {suma:dd\\:hh\\:mm\\:ss}";
            TagLib.File tagLib = TagLib.File.Create(URLS[ListBox.SelectedIndex], "audio/mp3", TagLib.ReadStyle.Average);
            TextBlock_Author_Name.Text = tagLib.Tag.FirstAlbumArtist;
            
            Name_Music.Text = IO.Path.GetFileNameWithoutExtension(URLS[ListBox.SelectedIndex]);
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
                catch (System.NotSupportedException)
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
            Name_Song_URL = URLS[ListBox.SelectedIndex];
            Slider_Carga.Minimum = 0;
            Slider_Carga.Maximum = position.TotalSeconds;
            Text_MinLength.Text = mediaPlayer.Position.ToString(@"mm\:ss");
            Text_MaxLength.Text = mediaPlayer.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
            RightToLeftMarquee();
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


            if (theme == 5)
            {
                textBlock.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[6]);
            }
            else
            {
                textBlock.Foreground = Brushes.White;
            }


        }

        private void TextBlock_Add_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => TextBlock_Add_MouseEnter(sender, e);

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

        /* Eventos del ListBox         */
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {



        }

        private void ListBox_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ListBox.SelectedIndex != -1)
            {
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromSeconds(1)
                };
                timer.Tick += Timer_Tick;
                timer.Start();

                mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;

                mediaPlayer.Open(new Uri(URLS[ListBox.SelectedIndex]));

                if (IsSelected != -1 && IsSelected != ListBox.SelectedIndex)
                {
                    try
                    {

                        ((TextBlock)ListBox.Items[IsSelected]).Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
                    }
                    catch (System.ArgumentOutOfRangeException)
                    {
                        ((TextBlock)ListBox.Items[IsSelected - 1]).Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
                    }
                }
                ((TextBlock)ListBox.Items[ListBox.SelectedIndex]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");
                IsSelected = ListBox.SelectedIndex;
                IsPaused = true;
                Button_Pause_Click(sender, e);
                mediaPlayer.Play();

                GetFavorite();
            }
        }

        private void ListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

            ListBox.SelectedIndex = -1;
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

                    string str = $"Autor: {(!String.IsNullOrEmpty(tagLib.Tag.FirstAlbumArtist) ? tagLib.Tag.FirstAlbumArtist : "N/A")}.\n" +
                            $"Duración: {tagLib.Properties.Duration:hh\\:mm\\:ss}.\n" +
                            $"Tamaño: {Math.Round(new IO.FileInfo(file).Length / 1048576d, 3)} MB.\n" +
                            $"Album: {(!String.IsNullOrEmpty(tagLib.Tag.Album) ? tagLib.Tag.Album : "N/A")}.";

                    ToolTip toolTip = new ToolTip
                    {
                        Content = str
                    };
                    TextBlock tb = new TextBlock
                    {
                        Text = $"{ListBox.Items.Count + 1} - {IO.Path.GetFileNameWithoutExtension(file)}",
                        ToolTip = toolTip
                    };
                    ListBox.Items.Add(tb);
                    URLS.Add(file);
                    suma += tagLib.Properties.Duration;
                }

            }

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

        void Timer_Tick(object sender, EventArgs e)
        {
            Slider_Carga.Value = mediaPlayer.Position.TotalSeconds;
            Text_MinLength.Text = mediaPlayer.Position.ToString(@"mm\:ss");

            if (Text_MinLength.Text == Text_MaxLength.Text)
            {
                if (ListBox.Items.Count != 1 && ListBox.SelectedIndex != ListBox.Items.Count - 1)
                {
                    mediaPlayer.Open(new Uri(URLS[ListBox.SelectedIndex + 1]));
                    Name_Music.Text = ((TextBlock)ListBox.Items[ListBox.SelectedIndex + 1]).Text;
                    ListBox.SelectedIndex += 1;
                   
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
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = (Image)sender;
            
            if (img.Name == "Siguiente" && IsSelected != -1 && ListBox.Items.Count != 0)
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Siguiente/enter_siguiente.png"));
                if (ListBox.Items.Count == 1)
                {
                    mediaPlayer.Open(new Uri(URLS[0]));


                }
                else if (ListBox.SelectedIndex == ListBox.Items.Count - 1 || IsSelected == ListBox.Items.Count - 1)
                {
                    
                    int valor;
                    // Comprueba si Selected es diferente a la última canción 
                    if (IsSelected != ListBox.Items.Count - 1 )
                    {
                        valor = IsSelected;
                        mediaPlayer.Open(new Uri(URLS[valor + 1]));

                        ((TextBlock)ListBox.Items[valor]).Foreground = Brushes.White;
                        ((TextBlock)ListBox.Items[valor + 1]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");
                        ListBox.SelectedIndex = valor + 1;
                        IsSelected = valor + 1;

                        return;
                    }
                    //Esta condición sirve para comprobar cuando el usuario deja de seleccionar una canción.
                    else if ((ListBox.SelectedIndex == -1 && IsSelected == ListBox.Items.Count - 1) || ListBox.SelectedIndex != IsSelected)
                    {
                        
                        valor = IsSelected;
                    }
                    
                    else
                    {
                        valor = ListBox.SelectedIndex;
                    }

                    mediaPlayer.Open(new Uri(URLS[0]));
                
                    ((TextBlock)ListBox.Items[valor]).Foreground = Brushes.White;
                    ((TextBlock)ListBox.Items[0]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");
                    ListBox.SelectedIndex = 0;
                    IsSelected = 0;

                }
                else
                {
                    
                    int valor = ListBox.SelectedIndex;
                    //Esta condición sirve para comprobar cuando el usuario deja de seleccionar una canción.
                    if (ListBox.SelectedIndex == -1 && IsSelected != - 1 || ListBox.SelectedIndex != IsSelected)
                    {
                        valor = IsSelected;
                        ListBox.SelectedIndex = IsSelected;
                    }
                   
                    mediaPlayer.Open(new Uri(URLS[valor + 1]));

      
                    ListBox.SelectedIndex += 1;
                    valor = ListBox.SelectedIndex;
                    IsSelected = valor;
                    ((TextBlock)ListBox.Items[valor - 1]).Foreground = Brushes.White;
                    ((TextBlock)ListBox.Items[valor]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");
                    
                }

                IsPaused = true;
                Button_Pause_Click(sender, e);
                mediaPlayer.Play();
            }
            else if (img.Name == "Anterior" && IsSelected != -1 && ListBox.Items.Count != 0)
            {
                img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Models_Anterior/enter_anterior.png"));
                if (ListBox.Items.Count == 1)
                {
                    mediaPlayer.Open(new Uri(URLS[0]));


                }
                else if(ListBox.SelectedIndex == 0 && IsSelected > 0)
                {
                    int valor = IsSelected;
                    ((TextBlock)ListBox.Items[valor]).Foreground = Brushes.White;
                    ((TextBlock)ListBox.Items[valor - 1]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");
                    mediaPlayer.Open(new Uri(URLS[valor - 1]));
                    ListBox.SelectedIndex = valor - 1;
                    IsSelected = valor - 1;

                }
                else if (ListBox.SelectedIndex != 0)
                {
                    
                    int valor = ListBox.SelectedIndex;
                    if (IsSelected == 0)
                        return;
                    if(ListBox.SelectedIndex == -1 && IsSelected != -1 || ListBox.SelectedIndex != IsSelected)
                    {
                        valor = IsSelected;
                        ListBox.SelectedIndex = IsSelected;
                    }
                    
                    mediaPlayer.Open(new Uri(URLS[valor - 1]));

                    ((TextBlock)ListBox.Items[valor]).Foreground = Brushes.White;
                    ((TextBlock)ListBox.Items[valor - 1]).Foreground = (Brush)new BrushConverter().ConvertFrom("#FFFE4164");
                    ListBox.SelectedIndex -= 1;
                    valor = ListBox.SelectedIndex;
                    IsSelected = valor;

                }
                IsPaused = true;
                Button_Pause_Click(sender, e);
                mediaPlayer.Play();
            }
            else if (img.Name == "Repetir")
            {
                if (ListBox.Items.Count != 0) {
                    img.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Repetir/click_actualizar.png"));
                    int valor = ListBox.SelectedIndex;
                    if (ListBox.SelectedIndex != IsSelected)
                        valor = IsSelected;

                    mediaPlayer.Open(new Uri(URLS[valor]));

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


        /*               Eventos para guardar las canciones favoritas           */


        private void Image_Favorite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ListBox.Items.Count != 0 && ListBox.SelectedIndex != -1)
            {

                if (IMG_Favorite.Source.ToString() == @"pack://application:,,,/IMG/Favorite/favorite.png")
                {

                    IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/clic_favorite.png"));

                    ListFavorites.Add(Name_Music.Text);
                    F_URLS.Add(Name_Song_URL);
                }
                else
                {

                    IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/favorite.png"));
                    if (IsFavorited)
                    {
                        URLS.RemoveAt(ListBox.SelectedIndex);
                        ListBox.Items.RemoveAt(ListBox.SelectedIndex);
                        mediaPlayer.Stop();

                    }

                    ListFavorites.Remove(Name_Music.Text);
                    F_URLS.Remove(Name_Song_URL);
                }
            }

        }

        private void TextBlock_Favorite_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ListFavorites.Count != 0)
            {
                if (!IsFavorited)
                {
                    //LAux_Songs = ListBox.Items.OfType<string>().ToList();
                    LAux_Songs.Clear();
                    foreach (var data in ListBox.Items)
                    {
                        LAux_Songs.Add(((TextBlock)data).Text);
                    }
                    LAux_URLS = new List<string>(URLS);

                    ListBox.Items.Clear();
                    URLS.Clear();

                    URLS = F_URLS.ToList();
                    int i = 0;
                    foreach (var data in ListFavorites)
                    {
                        TagLib.File tagFile = TagLib.File.Create(URLS[i], "audio/mp3", TagLib.ReadStyle.Average);

                        string str = $"Autor: {(!String.IsNullOrEmpty(tagFile.Tag.FirstAlbumArtist) ? tagFile.Tag.FirstAlbumArtist : "N/A")}.\n" +
                        $"Duración: {tagFile.Properties.Duration:hh\\:mm\\:ss}.\n" +
                        $"Tamaño: {Math.Round(new IO.FileInfo(URLS[i]).Length / 1048576d, 3)} MB.\n" +
                        $"Album: {(!String.IsNullOrEmpty(tagFile.Tag.Album) ? tagFile.Tag.Album : "N/A")}.";

                        ToolTip toolTip = new ToolTip
                        {
                            Content = str
                        };
                        TextBlock tb = new TextBlock
                        {
                            Text = data,
                            ToolTip = toolTip
                        };

                        ListBox.Items.Add(tb);
                    }
                    IsFavorited = true;
                    TextBlock_Favorite.Text = "PlayList";
                }
                else
                {
                    TextBlock_Favorite.Text = "Favoritas";
                    ListBox.Items.Clear();
                    URLS.Clear();
                    int i = 0;
                    URLS = LAux_URLS.ToList();

                    foreach (var data in LAux_Songs)
                    {
                        TagLib.File tagFile = TagLib.File.Create(URLS[i], "audio/mp3", TagLib.ReadStyle.Average);

                        string str = $"Autor: {(!String.IsNullOrEmpty(tagFile.Tag.FirstAlbumArtist) ? tagFile.Tag.FirstAlbumArtist : "N/A")}.\n" +
                        $"Duración: {tagFile.Properties.Duration:hh\\:mm\\:ss}.\n" +
                        $"Tamaño: {Math.Round(new IO.FileInfo(URLS[i]).Length / 1048576d, 3)} MB.\n" +
                        $"Album: {(!String.IsNullOrEmpty(tagFile.Tag.Album) ? tagFile.Tag.Album : "N/A")}.";

                        ToolTip toolTip = new ToolTip
                        {
                            Content = str
                        };
                        TextBlock tb = new TextBlock
                        {
                            Text = data,
                            ToolTip = toolTip
                        };

                        ListBox.Items.Add(tb);
                        i++;
                    }


                    IsFavorited = false;

                }
            }
            else if (ListBox.Items.Count == 0 && IsFavorited)
            {
                TextBlock_Favorite.Text = "Favoritas";
                int i = 0;
                URLS = LAux_URLS.ToList();

                foreach (var data in LAux_Songs)
                {
                    TagLib.File tagFile = TagLib.File.Create(URLS[i], "audio/mp3", TagLib.ReadStyle.Average);

                    string str = $"Autor: {(!String.IsNullOrEmpty(tagFile.Tag.FirstAlbumArtist) ? tagFile.Tag.FirstAlbumArtist : "N/A")}.\n" +
                    $"Duración: {tagFile.Properties.Duration:hh\\:mm\\:ss}.\n" +
                    $"Tamaño: {Math.Round(new IO.FileInfo(URLS[i]).Length / 1048576d, 3)} MB.\n" +
                    $"Album: {(!String.IsNullOrEmpty(tagFile.Tag.Album) ? tagFile.Tag.Album : "N/A")}.";

                    ToolTip toolTip = new ToolTip
                    {
                        Content = str
                    };
                    TextBlock tb = new TextBlock
                    {
                        Text = data,
                        ToolTip = toolTip
                    };

                    ListBox.Items.Add(tb);
                    i++;
                }


                IsFavorited = false;
            }


        }

        private void GetFavorite()
        {
            if (ListFavorites.Count != 0 && ListBox.SelectedIndex != -1)
            {
                List<string> aux = new List<string>(ListFavorites);

                string str = ((TextBlock)ListBox.Items[ListBox.SelectedIndex]).Text;

                if (!IsFavorited)
                    str = str.Remove(0, 4);

                if (Utilities<String>.BinarySearch(aux, str))
                    IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/clic_favorite.png"));
                else
                    IMG_Favorite.Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/Favorite/favorite.png"));
            }
        }

        private void SaveHistorialFavorite()
        {
            Historial historiall = new Historial
            {
                LHistory = ListFavorites,
                LURL = F_URLS
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

            ListBox.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[7]);
            ListBox.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);

            Button_Erase.Foreground = (Brush)new BrushConverter().ConvertFrom(LTheme[4]);
            Button_Erase.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[7]);

            Border1.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);
            Border2.BorderBrush = (Brush)new BrushConverter().ConvertFrom(LTheme[5]);

            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri(@"pack://application:,,,/Styles/ListBox.xaml")
            };

            if (theme != 1)
                ListBox.ItemContainerStyle = (Style)resourceDictionary["Modo_Oscuro"];
            else
                ListBox.ItemContainerStyle = (Style)resourceDictionary["Modo_Claro"];

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

        public void SaveHistorial(string pathname = "", bool varias = true)
        {
            if (IsFavorited)
            {
                ListBox.Items.Clear();
                foreach (var data in LAux_Songs)
                {
                    ListBox.Items.Add(new TextBlock { Text = data });
                }

                URLS.Clear();

                URLS = LAux_URLS.ToList();
            }
            Historial historiall = new Historial
            {
                LURL = URLS
            };
            if (!varias)
                historiall.LHistory.Add(IO.Path.GetFileNameWithoutExtension(pathname));
            else
            {
                foreach (var data in ListBox.Items)
                {
                    historiall.LHistory.Add(((TextBlock)data).Text);
                }
            }
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
            if (ListBox.Items.Count != 0)
            {
                ListBox.Items.Clear();
                URLS.Clear();
                IO.File.Delete("historial.pytham");
                mediaPlayer.Stop();
                suma = new TimeSpan();
                TextBlock_Info_PlayList.Text = $"";
                Name_Music.Text = "-";
  
                TextBlock_Author_Name.Text = "-";
                Text_MinLength.Text = "-";
                Text_MaxLength.Text = "-";
                IsSelected = -1;
            }
        }

        /*                   Modo Streamer                */
        private void TextBox_Streamer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (winStreamer == null)
            {
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
                    IO.File.Delete($@"{PathFile}\NameSong.txt");

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
                        byte[] info = new UTF8Encoding(true).GetBytes(tagLib.Tag.Album);
                        fs.Write(info, 0, info.Length);
                    }
                }

                if (IO.File.Exists($@"{PathFile}\YearSong.txt"))
                {
                    string path = $@"{PathFile}\YearSong.txt";
                    using (var fs = IO.File.Create(path))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(tagLib.Tag.Year.ToString());
                        fs.Write(info, 0, info.Length);
                    }
                }

                if (IO.File.Exists($@"{PathFile}\AuthorSong.txt"))
                {
                    string path = $@"{PathFile}\AuthorSong.txt";
                    using (var fs = IO.File.Create(path))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes(tagLib.Tag.FirstAlbumArtist);
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
    }
}
