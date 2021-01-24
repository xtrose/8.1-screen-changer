using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework.Media;
using System.Collections.ObjectModel;
using System.ComponentModel;




namespace Lockscreen_Swap.Pages
{





    public partial class SoundsEditor : PhoneApplicationPage
    {





        // Wird zum Start der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        public SoundsEditor()
        {
            //Komponenten laden
            InitializeComponent();

            //Bilder tauschen
            Color backgroundColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            string temp = Convert.ToString(backgroundColor);
            if (temp != "#FF000000")
            {
                ImgLogo.Source = new BitmapImage(new Uri("Images/ComonentEditor.Light.png", UriKind.Relative));
                ImgLogo.Opacity = 0.1;
                ImgLogo2.Source = new BitmapImage(new Uri("Images/ComonentEditor.Light.png", UriKind.Relative));
                ImgLogo2.Opacity = 0.1;
                ImgLogo3.Source = new BitmapImage(new Uri("Images/ComonentEditor.Light.png", UriKind.Relative));
                ImgLogo3.Opacity = 0.1;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Allgemeine Variabeln
        //-----------------------------------------------------------------------------------------------------------------
        //IsoStore file erstellen
        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
        //Style Name
        string StyleName;
        //Neue Datenliste erstellen
        ObservableCollection<ClassSongs> datalist = new ObservableCollection<ClassSongs>();
        //-----------------------------------------------------------------------------------------------------------------





        // Wird bei jedem Aufruf der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Variable für Ordner ermitteln
            StyleName = Convert.ToString(NavigationContext.QueryString["style"]);
            base.OnNavigatedTo(e);

            //Überschrift erstellen
            TBImagesOnlineName.Text = StyleName;

            //Menüs schließen
            GRSoundSelect.Margin = new Thickness(-600, 0, 0, 0);
            GRSoundDownload.Margin = new Thickness(-600, 0, 0, 0);
            //Angeben das Menüs geschlossen sind
            MenuOpen = false;

            //Info Text erstellen
            TBInfo.Text = Lockscreen_Swap.AppResx.Z01_SoundChangeInfo;

            //Songs neu laden
            //CreateSongs();
        }
        //-----------------------------------------------------------------------------------------------------------------





        // Wird bei jedem Aufruf der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        void CreateSongs()
        {
            //Songs aus MediaLibrary laden
            MediaLibrary library = new MediaLibrary();
            var songs = library.Songs;

            //Datenliste leeren
            datalist.Clear();

            //Neue Datenliste erstellen
            for (int i = 0; i < songs.Count; i++)
            {
                datalist.Add(new ClassSongs(i, songs[i].Name));
            }

            //Liste verknüpfen
            LBSoundSelect.ItemsSource = datalist;
        }
        //-----------------------------------------------------------------------------------------------------------------




        //Ausgewählte töne Abspielen
        //---------------------------------------------------------------------------------------------------------
        //Battery Low
        private void PlayBatteryLow(object sender, RoutedEventArgs e)
        {
            //MediaElement Anhalten
            MediaElement.Stop();

            //Prüfen ob Datei existiert
            try
            {
                if (file.FileExists("Sounds/" + StyleName + "/BatteryLow.mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("Sounds/" + StyleName + "/BatteryLow.mp3", FileMode.OpenOrCreate, isf))
                        {
                            MediaElement.SetSource(isfs);
                        }
                    }
                }
            }
            catch
            {
            }

            //Abspielen
            MediaElement.Play();
        }

        //Battery is charging
        private void PlayBatteryIsCharging(object sender, RoutedEventArgs e)
        {
            //MediaElement Anhalten
            MediaElement.Stop();

            //Prüfen ob Datei existiert
            try
            {
                if (file.FileExists("Sounds/" + StyleName + "/BatteryIsCharging.mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("Sounds/" + StyleName + "/BatteryIsCharging.mp3", FileMode.OpenOrCreate, isf))
                        {
                            MediaElement.SetSource(isfs);
                        }
                    }
                }
            }
            catch
            {
            }

            //Abspielen
            MediaElement.Play();
        }

        //Battery fully charged
        private void PlayBatteryFullyCharged(object sender, RoutedEventArgs e)
        {
            //MediaElement Anhalten
            MediaElement.Stop();

            //Prüfen ob Datei existiert
            try
            {
                if (file.FileExists("Sounds/" + StyleName + "/BatteryFullyCharged.mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("Sounds/" + StyleName + "/BatteryFullyCharged.mp3", FileMode.OpenOrCreate, isf))
                        {
                            MediaElement.SetSource(isfs);
                        }
                    }
                }
            }
            catch
            {
            }

            //Abspielen
            MediaElement.Play();
        }
        //---------------------------------------------------------------------------------------------------------





        //Töne tauschen
        //---------------------------------------------------------------------------------------------------------
        //Variabeln erstellen
        string SelectFor = "none";
        bool MenuOpen = false;
        //Aktionen
        private void ChangeBatteryLow(object sender, RoutedEventArgs e)
        {
            //Auswählen für was
            SelectFor = "BatteryLow";
            //Auswahl öffnen
            GRSoundDownload.Margin = new Thickness(0, 0, 0, 0);
            //Textbox http:// eingeben
            TBStyleName.Text = "http://";
            //Stackpanle verbergen
            SpNewSound.Visibility = System.Windows.Visibility.Collapsed;
            TBDownloadStatus.Visibility = System.Windows.Visibility.Collapsed;
            //Angeben das Menü offen ist
            MenuOpen = true;
        }

        private void ChangeBatteryIsCharging(object sender, RoutedEventArgs e)
        {
            //Auswählen für was
            SelectFor = "BatteryIsCharging";
            //Auswahl öffnen
            GRSoundDownload.Margin = new Thickness(0, 0, 0, 0);
            //Textbox http:// eingeben
            TBStyleName.Text = "http://";
            //Stackpanle verbergen
            SpNewSound.Visibility = System.Windows.Visibility.Collapsed;
            TBDownloadStatus.Visibility = System.Windows.Visibility.Collapsed;
            //Angeben das Menü offen ist
            MenuOpen = true;
        }

        private void ChangeBatteryFullyCharged(object sender, RoutedEventArgs e)
        {
            //Auswählen für was
            SelectFor = "BatteryFullyCharged";
            //Auswahl öffnen
            GRSoundDownload.Margin = new Thickness(0, 0, 0, 0);
            //Textbox http:// eingeben
            TBStyleName.Text = "http://";
            //Stackpanle verbergen
            SpNewSound.Visibility = System.Windows.Visibility.Collapsed;
            TBDownloadStatus.Visibility = System.Windows.Visibility.Collapsed;
            //Angeben das Menü offen ist
            MenuOpen = true;
        }
        //---------------------------------------------------------------------------------------------------------





        //Töne aus Listbox wählen
        //---------------------------------------------------------------------------------------------------------
        private void SoundChange(object sender, SelectionChangedEventArgs e)
        {
            /*
            //Songs aus MediaLibrary laden
            MediaLibrary library = new MediaLibrary();
            var songs = library.Songs;
             * */
        }
        //---------------------------------------------------------------------------------------------------------





        //Wenn url eingegeben
        //---------------------------------------------------------------------------------------------------------
        private void TBStyleName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Prüfen ob Return gedrückt wurde
            string tempkey = Convert.ToString(e.Key);
            if (tempkey == "Enter")
            {
                if (TBStyleName.Text.Length > 0)
                {
                    try
                    {
                        //Url erstellen
                        string url = TBStyleName.Text;

                        //Focus zurücksetzen
                        Focus();

                        //Downloadstatus ändern
                        TBDownloadStatus.Text = Lockscreen_Swap.AppResx.Z01_DownloadingSound;
                        TBDownloadStatus.Visibility = System.Windows.Visibility.Visible;

                        //Datei öffnen
                        WebClient client = new WebClient();
                        client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                        client.OpenReadAsync(new Uri(url));
                    }
                    catch
                    {
                    }
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------





        //Geladenes Bild vom Internet in den Isolated Storage speichern
        //---------------------------------------------------------------------------------------------------------
        //Variabeln
        BitmapImage bi = new BitmapImage();

        //Aktion
        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //Bild vom Internet in den Isolated Storage speichern
            try
            {
                //Prüfen ob Downloadordner existiert
                if (file.DirectoryExists("TempSounds"))
                {
                }
                else
                {
                    file.CreateDirectory("TempSounds");
                }

                //Prüfen ob Datei bereits vorhanden
                if (file.FileExists("TempSounds/" + SelectFor + ".mp3"))
                {
                    file.DeleteFile("TempSounds/" + SelectFor + ".mp3");
                }

                //Datei in Isolated Storage laden
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("TempSounds/" + SelectFor + ".mp3", System.IO.FileMode.Create, file))
                {
                    byte[] buffer = new byte[1024];
                    while (e.Result.Read(buffer, 0, buffer.Length) > 0)
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }

                //Downloastatus verbergen
                SpNewSound.Visibility = System.Windows.Visibility.Visible;
                TBDownloadStatus.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------------------------------------





        //Neuen Ton abspielen
        //---------------------------------------------------------------------------------------------------------
        private void PlayTempSound(object sender, RoutedEventArgs e)
        {
            //MediaElement Anhalten
            MediaElement.Stop();
            try
            {
                //Prüfen ob Datei existiert
                if (file.FileExists("TempSounds/" + SelectFor + ".mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("TempSounds/" + SelectFor + ".mp3", FileMode.OpenOrCreate, isf))
                        {
                            MediaElement.SetSource(isfs);
                        }
                    }
                }
            }
            catch
            {
            }
            //Abspielen
            MediaElement.Play();
        }
        //---------------------------------------------------------------------------------------------------------





        //Neuen Ton kopieren
        //---------------------------------------------------------------------------------------------------------
        private void ChangeTempSound(object sender, RoutedEventArgs e)
        {
            //Alte Datei löschen
            if (file.FileExists("Sounds/" + StyleName + "/" + SelectFor + ".mp3"))
            {
                file.DeleteFile("Sounds/" + StyleName + "/" + SelectFor + ".mp3");
            }
            //Datei kopieren
            file.CopyFile("TempSounds/" + SelectFor + ".mp3", "Sounds/" + StyleName + "/" + SelectFor + ".mp3");

            //Grid verbergen
            GRSoundDownload.Margin = new Thickness(-600, 0, 0, 0);

            //Angeben das Menüs geschlossen sind
            MenuOpen = false;
        }
        //---------------------------------------------------------------------------------------------------------





        //Back Button
        //---------------------------------------------------------------------------------------------------------------------------------
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //Prüfen ob Menü offen ist und alle Menüs schließen
            if (MenuOpen == true)
            {
                //Media Elemet stoppen
                MediaElement.Stop();

                //Menüs schließen
                GRSoundSelect.Margin = new Thickness(-600, 0, 0, 0);
                GRSoundDownload.Margin = new Thickness(-600, 0, 0, 0);

                //Angeben das Menüs geschlossen sind
                MenuOpen = false;

                //Zurück oder beenden abbrechen
                e.Cancel = true;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------




    }
}