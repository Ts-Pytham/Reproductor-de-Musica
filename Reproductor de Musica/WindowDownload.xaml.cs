using System;
using IO = System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Search;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Converter;
using System.Net;
using WPF = Ookii.Dialogs.Wpf;

namespace Reproductor_de_Musica
{
    /// <summary>
    /// Lógica de interacción para WindowDownload.xaml
    /// </summary>
    public partial class WindowDownload : Window
    {
        readonly YoutubeClient youtube;
     
        readonly SearchClient ysearch;
        IReadOnlyList<PlaylistVideo> playlists;
        private readonly Button[] buttons;

        public MainWindow Mainwindow
        {
            get => this.DataContext as MainWindow;
            set => this.DataContext = value;
        }

        public WindowDownload(MainWindow mainWindow)
        {
            this.Mainwindow = mainWindow;
            InitializeComponent();

            this.Background = mainWindow.Background;

            Border.BorderBrush = Buscar.BorderBrush = TB_Bind.BorderBrush = Border2.BorderBrush = mainWindow.Border1.BorderBrush;

            TB_Bind.Foreground = TB_Bind.CaretBrush = Buscar.Foreground = mainWindow.Name_Music.Foreground;

            youtube = new YoutubeClient();

            ysearch = youtube.Search;
            buttons = new Button[6];
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

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

        private void DownloadMusic(object sender, RoutedEventArgs e)
        {
            if (TB_Bind.Text.Length == 0)
                MessageBox.Show("¡No has escrito nada!", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
            {
                BindInfo();
                
            }
        }


        private async void BindInfo()
        {
            try
            {
                ProgressBar.IsIndeterminate = true;
                playlists = await ysearch.GetVideosAsync(TB_Bind.Text).BufferAsync(6);
                
           
                ProgressBar.IsIndeterminate = false;
                int count = playlists.Count;

                if (StackPanel_Main.Children.Count != 1)
                    StackPanel_Main.Children.Clear();

                for (int i = 0; i != count; ++i)
                {

                    WrapPanel wrapPanelP = new WrapPanel
                    {
                        Height = 55,
                        Width = 784,
                        Margin = i == 0 ? new Thickness(0, 0, 0, 0) : new Thickness(0, 10, 0, 0)
                    };

                    WrapPanel wrapPanel = new WrapPanel
                    {
                        Width = 695,
                        Height = 49,
                        Margin = new Thickness(5, 3, 84, 0)
                    };

                    Hyperlink hyperlink = new Hyperlink
                    {
                        NavigateUri = new Uri(playlists[i].Url),
                        FontFamily = new FontFamily("Verdana"),
                        FontSize = 14
                    };

                    hyperlink.Inlines.Add(playlists[i].Url);

                    hyperlink.RequestNavigate += Hyperlink_RequestNavigate;

                    TextBlock Name = new TextBlock
                    {
                        Text = playlists[i].Title + "\n",
                        FontFamily = new FontFamily("Verdana"),
                        FontSize = 14,
                        Foreground = Brushes.White
                    };


                    Name.Inlines.Add(hyperlink);

                    wrapPanel.Children.Add(Name);
                    wrapPanelP.Children.Add(wrapPanel);


                    Image myImage = new Image
                    {
                        Source = new BitmapImage(new Uri(@"pack://application:,,,/IMG/download.png"))
                    };

                    buttons[i] = new Button
                    {
                        Name = "b" + i,
                        Background = Brushes.Transparent,
                        BorderBrush = Brushes.Transparent,
                        Margin = new Thickness(728, -52, 0, 0),
                        Content = myImage,
                        Cursor = Cursors.Hand

                    };

                    buttons[i].Click += WindowDownload_Click;
                    wrapPanelP.Children.Add(buttons[i]);

                    StackPanel_Main.Children.Add(wrapPanelP);

                }

            }
            catch (System.Net.Http.HttpRequestException)
            {
                MessageBox.Show("Al parecer hay un error con el servidor, intente comprobar si tiene internet.", "Advertencia", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void WindowDownload_Click(object sender, RoutedEventArgs e)
        {
            int value = ((Button)sender).Name[1] - '0';

            var fd = new WPF.VistaFolderBrowserDialog();
            {
                if(fd.ShowDialog() == true && !string.IsNullOrWhiteSpace(fd.SelectedPath))
                {
                    ProgressBar.IsIndeterminate = true;
                    
                    StreamManifest stream = await youtube.Videos.Streams.GetManifestAsync(playlists[value].Id);
                    ProgressBar.IsIndeterminate = false;

                    
                    IProgress<double> progress = new Progress<double>(p => ProgressBar.Value = 100 * p);


                    string path = $@"{fd.SelectedPath}\{Utilidades.Utilities<string>.ChangeFormat(playlists[value].Title)}.mp3";

                    try
                    {

                        // Modo normal
                        string path2 = IO.Path.Combine(IO.Directory.GetCurrentDirectory(), @"..\..\ffmpeg\ffmpeg.exe");
                        //string path2 = $@"{IO.Directory.GetCurrentDirectory()}\ffmpeg\ffmpeg.exe"; //Modo publicado
                        await youtube.Videos.DownloadAsync(playlists[value].Id, path, p => p.SetFormat("mp3").SetPreset(ConversionPreset.UltraFast).SetFFmpegPath(path2), progress);
                        AddInfo(path, playlists[value]);
                        MessageBox.Show("¡Descarga completada!", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                        ProgressBar.Value = 0;
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        if (MessageBox.Show("No se ha encontrado la ruta del programa ffmpeg.exe, ¿Desea agregar la ruta? (Se descargará con el formato webM)", "Advertencia", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {
                            var ofd = new WPF.VistaOpenFileDialog
                            {
                                Filter = "Program Files|ffmpeg.exe",
                                InitialDirectory = IO.Directory.GetCurrentDirectory(),
                                FilterIndex = 1
                            };
                            if (ofd.ShowDialog() == true)
                            {
                                if (ofd.FileName.Contains("ffmpeg.exe"))
                                {
                                    await youtube.Videos.DownloadAsync(playlists[value].Id, path, p => p.SetFormat("mp3").SetPreset(ConversionPreset.UltraFast).SetFFmpegPath(ofd.FileName), progress);
                                    MessageBox.Show("¡Descarga completada!", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ProgressBar.Value = 0;
                                    AddInfo(path, playlists[value]);
                                    return;
                                }
                            }
                            else
                            {
                                MessageBox.Show("No se ha encontrado el programa, se descargará el archivo en formato webM.", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                                IStreamInfo streamInfo = stream.GetAudioOnly().Where(p => p.Container == Container.WebM).WithHighestBitrate();
                                path = $@"{fd.SelectedPath}\{Utilidades.Utilities<string>.ChangeFormat(playlists[value].Title)}.{streamInfo.Container}";
                                if (streamInfo != null)
                                {
                                    await youtube.Videos.Streams.DownloadAsync(streamInfo, path, progress);
                                    MessageBox.Show("¡Descarga completada!", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
                                    ProgressBar.Value = 0;
                                }
                            }  
                        }
                        
                    }
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }


        private void AddInfo(string path, PlaylistVideo video)
        {
            TagLib.File lib = TagLib.File.Create(path, TagLib.ReadStyle.Average);
            string[] data = new string[2];
            data[0] = video.Author;

            lib.Tag.Title = video.Title;
            lib.Tag.AlbumArtists = data;
            lib.Tag.Artists = data;
            lib.Tag.Description = video.Description;
            

            DownloadImage(video.Thumbnails.HighResUrl);

            var pic = new TagLib.IPicture[1];
            pic[0] = new TagLib.Picture(@"C:\Windows\Temp\Miniatura.jpg");

            lib.Tag.Pictures = pic;
            lib.Save();
            
        }


        private void DownloadImage(string URL)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile(new Uri(URL), @"C:\Windows\Temp\Miniatura.jpg");

            }
        }
    }
}
