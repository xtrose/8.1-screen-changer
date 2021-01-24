using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;
using System.IO.IsolatedStorage;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Resources;
using Microsoft.Phone.Tasks;





namespace Lockscreen_Swap
{





    public partial class ComponentsSounds : PhoneApplicationPage
    {





        //Timer erstellen
        //---------------------------------------------------------------------------------------------------------
        DispatcherTimer dt = new DispatcherTimer();
        //---------------------------------------------------------------------------------------------------------





        //Allgemeine Variabeln
        //-----------------------------------------------------------------------------------------------------------------
        //IsoStore file erstellen
        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
        //Neue Datenliste erstellen //ClassImages
        ObservableCollection<ClassImages> datalist2 = new ObservableCollection<ClassImages>();
        //System Bauteile
        string[] SystemImages = { "English" };
        //Online Liste erstellen
        ObservableCollection<ClassImagesOnline> datalist3 = new ObservableCollection<ClassImagesOnline>();
        //-----------------------------------------------------------------------------------------------------------------






        //Wird am Anfang der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        public ComponentsSounds()
        {
            //Komponenten laden
            InitializeComponent();

            //Timer erstellen
            dt.Stop();
            dt.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dt.Tick += new EventHandler(dt_Tick);

            //Bilder austauschen
            System.Windows.Media.Color backgroundColor = (System.Windows.Media.Color)Application.Current.Resources["PhoneBackgroundColor"];
            string temp = Convert.ToString(backgroundColor);
            if (temp != "#FF000000")
            {
                //Icons ändern
                ImgLogo.Source = new BitmapImage(new Uri("Images/Music.Light.png", UriKind.Relative));
                ImgLogo.Opacity = 0.1;
                ImgLogo02.Source = new BitmapImage(new Uri("Images/Music.Light.png", UriKind.Relative));
                ImgLogo02.Opacity = 0.1;
                ImgLogo03.Source = new BitmapImage(new Uri("Images/Globe.Light.png", UriKind.Relative));
                ImgLogo03.Opacity = 0.1;
                ImgImgEdit.Source = new BitmapImage(new Uri("Images/ComonentEditor.Light.png", UriKind.Relative));
                ImgImgRename.Source = new BitmapImage(new Uri("Images/Edit.Light.png", UriKind.Relative));
                ImgImgCopy.Source = new BitmapImage(new Uri("Images/Copy.Light.png", UriKind.Relative));
                ImgImgDelete.Source = new BitmapImage(new Uri("Images/Delete.Light.png", UriKind.Relative));
                ImgImgAdd.Source = new BitmapImage(new Uri("Images/Folder.Add.Light.png", UriKind.Relative));
                ImgImgLoad.Source = new BitmapImage(new Uri("Images/Globe.Light.png", UriKind.Relative));
            }
        }
        //-----------------------------------------------------------------------------------------------------------------





        // Wird bei jedem Aufruf der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Menüs schließen
            GRImagesMenu.Margin = new Thickness(-600, 0, 0, 0);
            GRImagesOnline.Margin = new Thickness(-600, 0, 0, 0);

            //Angeben das Menüs geschlossen sind
            MenuOpen = false;

            //Bilder Liste neu erstellen
            CreateImages();
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Liste der Töne neu erstellen
        //-----------------------------------------------------------------------------------------------------------------
        void CreateImages()
        {
            //Alte Daten löschen
            datalist2.Clear();
            //Dateien Laden "Styles"
            string[] tempStyles = file.GetDirectoryNames("/Sounds/");
            int tempStyles_c = tempStyles.Count();
            for (int i = 0; i < tempStyles_c; i++)
            {
                datalist2.Add(new ClassImages(tempStyles[i]));
            }
            //Daten in Listbox schreiben
            LBImages.ItemsSource = datalist2;
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Menüs Variabeln
        //-----------------------------------------------------------------------------------------------------------------
        //Gibt an ob Menü offen ist
        bool MenuOpen = false;

        //Images Menü Variabeln
        int SelectedImages = -1;
        //-----------------------------------------------------------------------------------------------------------------





        //Sounds Menü
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnOpenImageMenu(object sender, SelectionChangedEventArgs e)
        {
            //Prüfen ob Menü bereits offen
            if (MenuOpen == false)
            {
                //Index auswählen
                SelectedImages = Convert.ToInt32(LBImages.SelectedIndex);
                TBImagesMenuName.Text = (datalist2[SelectedImages] as ClassImages).name;

                //Menü öffnen
                GRImagesMenu.Margin = new Thickness(0, 0, 0, 0);
                //Angeben das ein Menü offen ist
                MenuOpen = true;

                //Auswahl aufheben
                try
                {
                    LBImages.SelectedIndex = -1;
                }
                catch
                {
                }
            }
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Neuen Sound erstellen
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnNewImages(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Create Images öffnen
            NavigationService.Navigate(new Uri("/Pages/CreateSounds.xaml", UriKind.Relative));
        }
        //----------------------------------------------------------------------------------------------------------------





        //Sound umbenennen
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnRemaneImages(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Style auswählen
            string tempNewStyle = (datalist2[SelectedImages] as ClassImages).name;
            //Prüfen ob System Images
            bool editable = true;
            for (int i = 0; i < SystemImages.Count(); i++)
            {
                if (tempNewStyle == SystemImages[i])
                {
                    editable = false;
                    break;
                }
            }
            if (editable == false)
            {
                MessageBox.Show(Lockscreen_Swap.AppResx.Z01_Error + " " + tempNewStyle + " " + Lockscreen_Swap.AppResx.Z01_ErrorSounds);
            }
            else
            {
                //Rename Images öffnen
                NavigationService.Navigate(new Uri("/Pages/RenameSounds.xaml?style=" + tempNewStyle, UriKind.Relative));
            }
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Sounds kopieren
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnCopyImages(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Style auswählen
            string tempNewStyle = (datalist2[SelectedImages] as ClassImages).name;
            //Rename Images öffnen
            NavigationService.Navigate(new Uri("/Pages/CopySounds.xaml?style=" + tempNewStyle, UriKind.Relative));
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Sounds löschen
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnDeleteImages(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Style auswählen
            string tempNewStyle = (datalist2[SelectedImages] as ClassImages).name;
            //Prüfen ob System Images
            bool editable = true;
            for (int i = 0; i < SystemImages.Count(); i++)
            {
                if (tempNewStyle == SystemImages[i])
                {
                    editable = false;
                    break;
                }
            }
            if (editable == false)
            {
                MessageBox.Show(Lockscreen_Swap.AppResx.Z01_Error + " " + tempNewStyle + " " + Lockscreen_Swap.AppResx.Z01_ErrorSounds);
            }
            else
            {
                //Sounds löschen
                if (MessageBox.Show(Lockscreen_Swap.AppResx.Z01_NoteDeleteSound + " " + TBImagesMenuName.Text, Lockscreen_Swap.AppResx.Z01_Warning, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    try
                    {
                        file.DeleteFile("Sounds/" + tempNewStyle + "/BatteryFullyCharged.mp3");
                        file.DeleteFile("Sounds/" + tempNewStyle + "/BatteryIsCharging.mp3");
                        file.DeleteFile("Sounds/" + tempNewStyle + "/BatteryLow.mp3");
                        file.DeleteDirectory("Sounds/" + tempNewStyle);
                    }
                    catch
                    {
                    }
                    //Listbox neu erstellen
                    CreateImages();

                    //Menü schließen
                    GRImagesMenu.Margin = new Thickness(-600, 0, 0, 0);
                    //Angeben das ein Menü geschlossen ist
                    MenuOpen = false;
                }
            }
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Sounds beartbeiten
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnEditImages(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Style auswählen
            string tempNewStyle = (datalist2[SelectedImages] as ClassImages).name;
            //Prüfen ob System Images
            bool editable = true;
            for (int i = 0; i < SystemImages.Count(); i++)
            {
                if (tempNewStyle == SystemImages[i])
                {
                    editable = false;
                    break;
                }
            }
            if (editable == false)
            {
                MessageBox.Show(Lockscreen_Swap.AppResx.Z01_Error + " " + tempNewStyle + " " + Lockscreen_Swap.AppResx.Z01_ErrorSounds);
            }
            else
            {
                //ImagesEdit öffnen
                NavigationService.Navigate(new Uri("/Pages/SoundsEditor.xaml?style=" + tempNewStyle, UriKind.Relative));
            }
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Online Componenten laden
        //---------------------------------------------------------------------------------------------------------------------------------
        //Variabeln
        string SourceWebsite = "http://www.xtrose.de/xtrose/apps/battery_live_tile_editor/files/Sounds/ListSounds.php";
        string SourceUrl = "http://www.xtrose.de/xtrose/apps/battery_live_tile_editor/files/Sounds/";
        string StatusTimer = "none";
        string Source = "";
        string CheckSum = "";
        private void BtnConnect(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (StatusTimer == "none")
            {
                //Button umstellen
                TxtConnect.Text = Lockscreen_Swap.AppResx.Z01_ConnectingNow;
                //Timer starten
                dt.Start();
                //Timer Status angeben
                StatusTimer = "LoadWebsite";
                //Seite versuchen zu erreichen
                GetSourceCode();
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------





        //Aktion versuchen Seite zu erreichen und Quelltext in String laden
        //---------------------------------------------------------------------------------------------------------
        //Webseite versuchen zu erreichen
        public void GetSourceCode()
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp(SourceWebsite);
                request.BeginGetResponse(new AsyncCallback(HandleResponse), request);
            }
            catch
            {
                //Wenn Webseite nicht erreichbar, Fehlermeldung ausgeben
                MessageBox.Show(Lockscreen_Swap.AppResx.Z01_ConnectionFailed);
                //Zurück zum Start
                NavigationService.GoBack();
            }
        }

        //Quelltext in String speichern
        public void HandleResponse(IAsyncResult result)
        {
            HttpWebRequest request = result.AsyncState as HttpWebRequest;

            try
            {
                if (request != null)
                {
                    using (WebResponse response = request.EndGetResponse(result))
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            //Quelltext laden
                            Source = reader.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------------------------------------





        //Timer, Ablauf aller aktionen
        //---------------------------------------------------------------------------------------------------------
        //Variabeln
        int StartMS = 0;
        int UhrzeiMS = 0;
        //Aktion
        void dt_Tick(object sender, EventArgs e)
        {
            //Styles laden
            if (StatusTimer == "LoadWebsite")
            {
                //Prüfen ob Timeout
                bool TimeOut = false;


                //Aktuelle Uhrzeit Millisekunden erstellen
                DateTime Uhrzeit = DateTime.Now;
                UhrzeiMS = (Uhrzeit.Hour * 3600000) + (Uhrzeit.Minute * 60000) + (Uhrzeit.Second * 1000) + Uhrzeit.Millisecond;


                //Prüfen ob StartMS vorhanden
                if (StartMS == 0)
                {
                    StartMS = UhrzeiMS;
                }
                else
                {
                    if ((StartMS + 10000) < UhrzeiMS)
                    {
                        TimeOut = true;
                    }
                }


                //Preüfen ob Time out
                if (TimeOut == false)
                {
                    //Prüfen ob Quelle geladen
                    if (Source != "")
                    {
                        //wenn feedtemp = feed, Quelltext komplett geladen
                        if (Source == CheckSum)
                        {
                            //Timer Status löschen
                            StatusTimer = "none";
                            //feedtemp löschen
                            CheckSum = "";
                            //StartMS löschen
                            StartMS = 0;
                            //StackPanel verstecken
                            StpConnect.Visibility = System.Windows.Visibility.Collapsed;
                            //Timer Stoppen
                            dt.Stop();
                            //Listbox erstellen
                            CreateOnlineImages();
                        }
                        //wenn feedtemp != feed, Quelltext noch nicht komplett geladen
                        else
                        {
                            //feedtemp zu aktuellem Feed machen
                            CheckSum = Source;
                        }
                    }
                }
                //Bei TimeOut
                else
                {
                    MessageBox.Show(Lockscreen_Swap.AppResx.Z01_ConnectionTimeOut);
                    //Timer Status löschen
                    StatusTimer = "none";
                    //feedtemp löschen
                    CheckSum = "";
                    //StartMS löschen
                    StartMS = 0;
                    //Button umstellen
                    TxtConnect.Text = Lockscreen_Swap.AppResx.Z01_Connecting;
                    //Timer Stoppen
                    dt.Stop();
                }
            }


            //Bilder downloaden
            if (StatusTimer == "DownloadImages")
            {
                //Datei Herunterladen wenn nicht bereits eine andere runtergeladen wird
                if (DownloadID != TempID & DownloadID < 3)
                {
                    string url = "";
                    //TempID auf DownloadID stellen
                    TempID = DownloadID;
                    //Dateiname anhand der DownloadID erstellen
                    if (DownloadID == 0)
                    {
                        url = SourceUrl + OnlineDirectory + "/BatteryLow.mp3";
                    }
                    else if (DownloadID == 1)
                    {
                        url = SourceUrl + OnlineDirectory + "/BatteryIsCharging.mp3";
                    }
                    else if (DownloadID == 2)
                    {
                        url = SourceUrl + OnlineDirectory + "/BatteryFullyCharged.mp3";
                    }

                    //Downloadstatus ändern
                    TBDownloadStatus.Text = Lockscreen_Swap.AppResx.Z01_DownloadingSound + " " + (DownloadID + 1);

                    //Datei öffnen
                    WebClient client = new WebClient();
                    client.OpenReadCompleted += new OpenReadCompletedEventHandler(client_OpenReadCompleted);
                    client.OpenReadAsync(new Uri(url));

                }

                //Wenn alle Dateien Heruntergeladen sind
                else if (DownloadID == 3)
                {
                    //Download Status verbergen
                    TBDownloadStatus.Visibility = System.Windows.Visibility.Collapsed;
                    //Save Eingabe Hinzufügen
                    SPSaveImages.Visibility = System.Windows.Visibility.Visible;
                    //Stackpanel abspeilen hinzufügen
                    SPSounds.Visibility = System.Windows.Visibility.Visible;
                    //Name in Textbox eintragen
                    TBStyleName.Text = OnlineName;
                    //Timer Status ändern
                    StatusTimer = "none";
                    //Timer stoppen
                    dt.Stop();
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------





        //Bilderliste erstellen
        //---------------------------------------------------------------------------------------------------------
        //Variabekn
        bool ComponentsOnline = false;

        //Aktion
        void CreateOnlineImages()
        {
            //Quelle verarbeiten
            Source = Source.TrimEnd(new char[] { '\r', '\n' });

            //Wenn Inhalte verfügbar
            if (Source != "none")
            {
                //Angeben das Komponenten online sind
                ComponentsOnline = true;
                //Komponenten aufteilen
                string[] SourceSplit = Regex.Split(Source, ";;;");
                int cSourceSplit = SourceSplit.Count();
                for (int i = 0; i < (cSourceSplit - 1); i++)
                {
                    SourceSplit[i] = SourceSplit[i].Trim(new char[] { '\r', '\n' });
                    string[] SourceSplitSplit = Regex.Split(SourceSplit[i], ";");
                    datalist3.Add(new ClassImagesOnline(SourceSplitSplit[0], SourceSplitSplit[1], SourceSplitSplit[2]));
                }
                //Listbox neu erstellen
                LBOnlineImages.ItemsSource = datalist3;
            }
            //Wenn keine Inhalte verfügbar
            else
            {
                //Dateienliste leeren
                datalist3.Clear();
                //Überschrift erstellen
                datalist3.Add(new ClassImagesOnline(Lockscreen_Swap.AppResx.Z01_NoComponentsOnline, "",""));
                //Listbox neu erstellen
                LBOnlineImages.ItemsSource = datalist3;
            }
        }
        //---------------------------------------------------------------------------------------------------------





        //Online Komponenten laden
        //---------------------------------------------------------------------------------------------------------
        //Variabeln
        bool OpenComponent = true;
        string OnlineName = "";
        string OnlineDirectory = "";
        int DownloadID = 0;

        //Aktionen
        private void BtnOpenImageOnline(object sender, SelectionChangedEventArgs e)
        {
            //Prüfen Komponenten online sind
            if (ComponentsOnline == true)
            {
                //Prüfen ob Komponenten Fenster geöffnet wird
                if (OpenComponent == true)
                {
                    //Index laden
                    int SI = LBOnlineImages.SelectedIndex;

                    //Bilderset öffnen
                    OnlineName = (datalist3[SI] as ClassImagesOnline).name;
                    TBImagesOnlineName.Text = OnlineName;
                    OnlineDirectory = (datalist3[SI] as ClassImagesOnline).directory;

                    //Download Status sichtbar machen
                    TBDownloadStatus.Visibility = System.Windows.Visibility.Visible;
                    //Save Eingabe verbergen
                    SPSaveImages.Visibility = System.Windows.Visibility.Collapsed;
                    //Abspielpanel verbergen
                    SPSounds.Visibility = System.Windows.Visibility.Collapsed;

                    //Timer starten
                    DownloadID = 0;
                    TempID = 5000;
                    StatusTimer = "DownloadImages";
                    dt.Start();

                    //Menü öffnen
                    GRImagesOnline.Margin = new Thickness(0, 0, 0, 0);
                    //Angeben das ein Menü offen ist
                    MenuOpen = true;

                    //Auswahl aufheben
                    OpenComponent = false;
                    try
                    {
                        LBOnlineImages.SelectedIndex = -1;
                    }
                    catch
                    {
                    }
                    OpenComponent = true;
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------





        //Geladenes Bild vom Internet in den Isolated Storage speichern
        //---------------------------------------------------------------------------------------------------------
        //Variabeln
        int TempID = 5000;
        string FileName = "none";
        BitmapImage bi = new BitmapImage();

        //Aktion
        void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            //Bild vom Internet in den Isolated Storage speichern
            try
            {
                //Format prüfen

                //Dateiname anhand der DownloadID erstellen
                if (DownloadID == 0)
                {
                    FileName = "BatteryLow.mp3";
                }
                else if (DownloadID == 1)
                {
                    FileName = "BatteryIsCharging.mp3";
                }
                else if (DownloadID == 2)
                {
                    FileName = "BatteryFullyCharged.mp3";
                }

                //Prüfen ob Downloadordner existiert
                if (file.DirectoryExists("TempSounds"))
                {
                }
                else
                {
                    file.CreateDirectory("TempSounds");
                }

                //Prüfen ob Datei bereits vorhanden
                if (file.FileExists("TempSounds/" + FileName))
                {
                    file.DeleteFile("TempSounds/" + FileName);
                }

                //Datei in Isolated Storage laden
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream("TempSounds/" + FileName, System.IO.FileMode.Create, file))
                {
                    byte[] buffer = new byte[1024];
                    while (e.Result.Read(buffer, 0, buffer.Length) > 0)
                    {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch
            {
            }
            //DownloadID erhöhen um nächstes Bild aus aus dem Netz herunterzuladen
            DownloadID++;
        }
        //---------------------------------------------------------------------------------------------------------





        //Heruntergeladene Töne abspielen
        //---------------------------------------------------------------------------------------------------------
        //Battery Low
        private void PlayBatteryLowTemp(object sender, RoutedEventArgs e)
        {
            //MediaElement stoppen
            MediaElement.Stop();

            //Prüfen ob Datei existiert
            try
            {
                if (file.FileExists("TempSounds/BatteryLow.mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("TempSounds/BatteryLow.mp3", FileMode.OpenOrCreate, isf))
                        {
                            MediaElement.SetSource(isfs);
                        }
                    }
                }
            }
            catch
            {
            }

            //Sound abspielen
            MediaElement.Play();
        }

        //Battery is charging
        private void PlayBatteryIsChargingTemp(object sender, RoutedEventArgs e)
        {
            //MediaElement stoppen
            MediaElement.Stop();

            //Prüfen ob Datei existiert
            try
            {
                if (file.FileExists("TempSounds/BatteryIsCharging.mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("TempSounds/BatteryIsCharging.mp3", FileMode.OpenOrCreate, isf))
                        {
                            MediaElement.SetSource(isfs);
                        }
                    }
                }
            }
            catch
            {
            }

            //Sound abspielen
            MediaElement.Play();
        }

        //Battery fully charged
        private void PlayBatteryFullyChargedTemp(object sender, RoutedEventArgs e)
        {
            //MediaElement stoppen
            MediaElement.Stop();

            //Prüfen ob Datei existiert
            try
            {
                if (file.FileExists("TempSounds/BatteryFullyCharged.mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("TempSounds/BatteryFullyCharged.mp3", FileMode.OpenOrCreate, isf))
                        {
                            MediaElement.SetSource(isfs);
                        }
                    }
                }
            }
            catch
            {
            }

            //Sound abspielen
            MediaElement.Play();
        }
        //---------------------------------------------------------------------------------------------------------





        //Prüfen ob Return gedrückt wurde
        //-----------------------------------------------------------------------------------------------------------------
        private void TBStyleName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Prüfen ob Return gedrückt wurde
            string tempkey = Convert.ToString(e.Key);
            if (tempkey == "Enter")
            {
                if (TBStyleName.Text.Length > 0)
                {
                    SaveImages();
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------  





        //Button Save Images
        //---------------------------------------------------------------------------------------------------------
        private void BtnSaveImagesClick(object sender, RoutedEventArgs e)
        {
            //Prüfen ob Name eingegeben wurde
            if (TBStyleName.Text.Length > 0)
            {
                SaveImages();
            }
        }
        //---------------------------------------------------------------------------------------------------------




        //Bilder Speichern
        //---------------------------------------------------------------------------------------------------------  
        void SaveImages()
        {
            try
            {
                //Bilder in neuen Style kopieren
                string NameToCreate = TBStyleName.Text;
                NameToCreate = NameToCreate.Trim();
                file.CreateDirectory("Sounds/ " + NameToCreate);
                file.CopyFile("TempSounds/BatteryLow.mp3", "Sounds/ " + NameToCreate + "/BatteryLow.mp3");
                file.CopyFile("TempSounds/BatteryIsCharging.mp3", "Sounds/ " + NameToCreate + "/BatteryIsCharging.mp3");
                file.CopyFile("TempSounds/BatteryFullyCharged.mp3", "Sounds/ " + NameToCreate + "/BatteryFullyCharged.mp3");
                //Bilderliste neu erstellen
                CreateImages();
                //Menü verbergen
                SPSaveImages.Visibility = System.Windows.Visibility.Collapsed;
                GRImagesOnline.Margin = new Thickness(-600, 0, 0, 0);
                MenuOpen = false;
            }
            catch
            {
                MessageBox.Show(Lockscreen_Swap.AppResx.Z01_InUse);
                TBStyleName.Text = "";
            }
        }
        //---------------------------------------------------------------------------------------------------------  





        //Ausgewählte töne Abspielen
        //---------------------------------------------------------------------------------------------------------
        //Battery Low
        private void PlayBatteryLow(object sender, RoutedEventArgs e)
        {
            //MediaElement stoppen
            MediaElement.Stop();

            //Prüfen ob Datei existiert
            try
            {
                if (file.FileExists("Sounds/" + TBImagesMenuName.Text + "/BatteryLow.mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("Sounds/" + TBImagesMenuName.Text + "/BatteryLow.mp3", FileMode.OpenOrCreate, isf))
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
            //MediaElement stoppen
            MediaElement.Stop();

            //Prüfen ob Datei existiert
            try
            {
                if (file.FileExists("Sounds/" + TBImagesMenuName.Text + "/BatteryIsCharging.mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("Sounds/" + TBImagesMenuName.Text + "/BatteryIsCharging.mp3", FileMode.OpenOrCreate, isf))
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
            //Prüfen ob Datei existiert
            try
            {
                if (file.FileExists("Sounds/" + TBImagesMenuName.Text + "/BatteryFullyCharged.mp3"))
                {
                    //MediaElement MediaElement = new MediaElement();
                    using (var isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (var isfs = new IsolatedStorageFileStream("Sounds/" + TBImagesMenuName.Text + "/BatteryFullyCharged.mp3", FileMode.OpenOrCreate, isf))
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





        //Back Button
        //---------------------------------------------------------------------------------------------------------------------------------
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //Prüfen ob Menü offen ist und alle Menüs schließen
            if (MenuOpen == true)
            {
                //MediaElement stoppen
                MediaElement.Stop();
                MediaElement.Source = null;

                //Menüs schließen
                GRImagesMenu.Margin = new Thickness(-600, 0, 0, 0);
                GRImagesOnline.Margin = new Thickness(-600, 0, 0, 0);

                //Angeben das Menüs geschlossen sind
                MenuOpen = false;

                //Zurück oder beenden abbrechen
                e.Cancel = true;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------





    }
}