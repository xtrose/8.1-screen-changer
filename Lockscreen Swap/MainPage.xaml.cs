using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Lockscreen_Swap.Resources;
using System.IO;
using System.IO.IsolatedStorage;
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;
using Microsoft.Phone;
using System.Windows.Media;
using Windows.Phone.Devices.Power;
using Microsoft.Phone.Info;
using System.Collections.ObjectModel;
using Microsoft.Phone.Scheduler;
using System.ComponentModel;
using System.Windows.Threading;
using Windows.Phone.System.UserProfile;
using System.Globalization;
using System.Threading;
using Microsoft.Xna.Framework.Media.PhoneExtensions;






namespace Lockscreen_Swap
{
    public partial class MainPage : PhoneApplicationPage
    {





        // Für ScheduledTaskAgent
        //-------------------------------------------------------------------------------------------------------------------------------------------
        //ScheduledTaskAgent Variabeln
        //*********************************************************************************************************
        PeriodicTask periodicTask;
        string periodicTaskName = "PeriodicAgent";
        public bool agentsAreEnabled = true;
        //*********************************************************************************************************



        //ScheduledTaskAgent Starten
        //*********************************************************************************************************
        private void StartPeriodicAgent()
        {
            //Variable ob Task aktiv ist
            agentsAreEnabled = true;

            //Prüfen ob Task aktiv ist
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            //Wenn Task aktiv ist, dann Task stoppen um einen neuen zu starten
            if (periodicTask != null)
            {
                RemoveAgent(periodicTaskName);
            }

            //Neuen Task erstellen
            periodicTask = new PeriodicTask(periodicTaskName);

            //Beschreibung des Tasks, wird in der Hintergundaufgaben bei den Einstellungen angezeigt
            periodicTask.Description = Lockscreen_Swap.AppResx.TaskInfo;

            //Versuchen Task zu starten
            try
            {
                //Task hinzufügen
                ScheduledActionService.Add(periodicTask);
                //ScheduledActionService.LaunchForTest(periodicTaskName, TimeSpan.FromSeconds(60));

                //Prüfen ob LockscreenApp und Anzeige auf on Stellen
                if (IsLockscreenApp == true)
                {
                    TBLockscreenChanger.Content = Lockscreen_Swap.AppResx.On;
                }
            }

            //Benachrichtigungen ausgeben, falls Task nicht gestartet werden kann
            catch (InvalidOperationException exception)
            {
                //Wenn Task nicht aktiv
                if (exception.Message.Contains("Error: The action is disabled"))
                {
                    //Benachrichtigung ausgeben
                    MessageBox.Show(Lockscreen_Swap.AppResx.ExError);
                }
                //Wenn maximum der Anwendungen erreicht
                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    //Keine Aktion erförderlich, da eine Systemnachricht ausgegeben wird
                }

                //Variable ob Task aktiv ist
                agentsAreEnabled = false;

                //Button des Tasks deaktivieren
                TBLockscreenChanger.Content = Lockscreen_Swap.AppResx.Off;
            }

            //Bei Fehler im Scheduler Service
            catch (SchedulerServiceException)
            {
                //Variable ob Task aktiv ist
                agentsAreEnabled = false;

                //Button des Tasks deaktivieren
                TBLockscreenChanger.Content = Lockscreen_Swap.AppResx.Off;
            }

        }
        //*********************************************************************************************************



        //ScheduledTaskAgent entfernen
        //*********************************************************************************************************
        private void RemoveAgent(string name)
        {
            try
            {
                //Agent erntfernen/Background/Background.jpg
                ScheduledActionService.Remove(name);

                //Lockscreenchanger auf Off stellen
                TBLockscreenChanger.Content = Lockscreen_Swap.AppResx.Off;
            }
            catch (Exception)
            {
            }
        }
        //*********************************************************************************************************



        //Lockhelper Button on (Button Lockscreen TSLockscreen in den Settings)
        //*********************************************************************************************************
        private void BtnLockscreen()
        {
            //Wenn noch keine Lockscreen App
            if (!Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication)
            {
                //Lockhelper aktivieren
                string filePathOfTheImage = "/Background/Background.jpg";
                bool isAppResource = true;
                LockHelper(filePathOfTheImage, isAppResource);

                //ScheduledTaskAgent Agent starten
                StartPeriodicAgent();

                //Lockscreen auf on stellen
                IsLockscreenApp = true;

                //Lockscreen Bild wechseln
                CreateImage();
            }

            //Wenn bereits lockscreen App
            else
            {
                //ScheduledTaskAgent Agent starten
                StartPeriodicAgent();

                //Lockscreen Bild wechseln
                CreateImage();
            }

            //Prüfen ob ScheduledTaskAgent läuft
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            //Prüfen ob momentan Lockscreen App
            if (Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication)
            {
                //IsLockscreenApp auf true stellen
                IsLockscreenApp = true;
            }

            //Prüfen ob Task läuft und App Lockscreen App ist
            if (periodicTask != null & IsLockscreenApp == true)
            {
                //agentsAreEnabled auf true
                agentsAreEnabled = true;

                //Button umstellen
                TBLockscreenChanger.Content = Lockscreen_Swap.AppResx.On;

                //Lockscreen Bild wechseln
                CreateImage();
            }

            //Wenn nicht, Button auf false stellen
            else
            {
                //agentsAreEnabled auf false
                agentsAreEnabled = false;

                //Button umstellen
                TBLockscreenChanger.Content = Lockscreen_Swap.AppResx.Off;
            }
        }
        //*********************************************************************************************************



        //Lockhelper ausführen
        //*********************************************************************************************************
        private async void LockHelper(string filePathOfTheImage, bool isAppResource)
        {
            try
            {
                var isProvider = Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication;
                if (!isProvider)
                {
                    // If you're not the provider, this call will prompt the user for permission.
                    // Calling RequestAccessAsync from a background agent is not allowed.
                    var op = await Windows.Phone.System.UserProfile.LockScreenManager.RequestAccessAsync();

                    // Only do further work if the access was granted.
                    isProvider = op == Windows.Phone.System.UserProfile.LockScreenRequestResult.Granted;
                }

                if (isProvider)
                {
                    // At this stage, the app is the active lock screen background provider.

                    // The following code example shows the new URI schema.
                    // ms-appdata points to the root of the local app data folder.
                    // ms-appx points to the Local app install folder, to reference resources bundled in the XAP package.
                    var schema = isAppResource ? "ms-appx:///" : "ms-appdata:///Local/";
                    var uri = new Uri(schema + filePathOfTheImage, UriKind.Absolute);

                    // Set the lock screen background image.
                    Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);

                    // Get the URI of the lock screen background image.
                    var currentImage = Windows.Phone.System.UserProfile.LockScreen.GetImageUri();
                    System.Diagnostics.Debug.WriteLine("The new lock screen background image is set to {0}", currentImage.ToString());

                    //ScheduledTaskAgent Agent starten
                    StartPeriodicAgent();
                }
                else
                {
                    MessageBox.Show(Lockscreen_Swap.AppResx.LockhelperInfo);
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        //*********************************************************************************************************










        //Animation
        //---------------------------------------------------------------------------------------------------------------------------------
        int UhrzeitMS;
        int StartMs;
        string Animation = "PauseStart";
        void dt_Tick(object sender, object e)
        {
            //uhrzeitms neu erstellen
            DateTime Uhrzeit = DateTime.Now;
            //Aktuelle Uhrzeit Millisekunden erstellen
            UhrzeitMS = (Uhrzeit.Hour * 3600000) + (Uhrzeit.Minute * 60000) + (Uhrzeit.Second * 1000) + Uhrzeit.Millisecond;

            //Animation Pause1
            if (Animation == "PauseStart")
            {
                //Prüfen ob Animation schon gestartet
                if (StartMs == 0)
                {
                    StartMs = UhrzeitMS;
                }
                //Wenn Animation schon gestartet
                else
                {
                    //Wenn Animation beendet
                    if ((UhrzeitMS - StartMs) > 300)
                    {
                        //Nächste Animation Starten
                        Animation = "Img1";
                        StartMs = 0;
                    }
                }
            }

            //Animation Img1
            if (Animation == "Img1")
            {
                //Prüfen ob Animation schon gestartet
                if (StartMs == 0)
                {
                    StartMs = UhrzeitMS;
                }
                //Wenn Animation schon gestartet
                else
                {
                    //Wenn Animation beendet
                    if ((UhrzeitMS - StartMs) > 400)
                    {
                        //Bild endgültig ausrichten
                        Img1.Width = 221;
                        Img1.Opacity = 1.0;
                        //Nächste Animation Starten
                        Animation = "Pause1";
                        StartMs = 0;
                    }
                    //Wenn Animation noch läuft
                    else
                    {
                        //Prozent errechnen
                        int Prozent = 100 * 100000 / 400 * (UhrzeitMS - StartMs) / 100000;
                        //Bild Größe berechnen
                        int NewSize = 1800 * 100000 / 100 * Prozent / 100000;
                        //Bild Transparenz berechnen
                        double opa = 0.1;
                        //string temp = Convert.ToString(Prozent);
                        //opa = Convert.ToDouble("0," + temp);
                        if (Prozent >= 100)
                        {
                            opa = 1.0;
                        }
                        else if (Prozent >= 90)
                        {
                            opa = 0.9;
                        }
                        else if (Prozent >= 80)
                        {
                            opa = 0.8;
                        }
                        else if (Prozent >= 70)
                        {
                            opa = 0.7;
                        }
                        else if (Prozent >= 60)
                        {
                            opa = 0.6;
                        }
                        else if (Prozent >= 50)
                        {
                            opa = 0.5;
                        }
                        else if (Prozent >= 40)
                        {
                            opa = 0.4;
                        }
                        else if (Prozent >= 30)
                        {
                            opa = 0.3;
                        }
                        else if (Prozent >= 20)
                        {
                            opa = 0.2;
                        }
                        else if (Prozent >= 10)
                        {
                            opa = 0.1;
                        }
                        //Bild neu erstellen
                        Img1.Width = 2021 - NewSize;
                        Img1.Opacity = opa;
                    }
                }
            }

            //Animation Pause1
            if (Animation == "Pause1")
            {
                //Prüfen ob Animation schon gestartet
                if (StartMs == 0)
                {
                    StartMs = UhrzeitMS;
                }
                //Wenn Animation schon gestartet
                else
                {
                    //Wenn Animation beendet
                    if ((UhrzeitMS - StartMs) > 200)
                    {
                        //Nächste Animation Starten
                        Animation = "Schein";
                        StartMs = 0;
                    }
                }
            }

            //Animation Img1
            if (Animation == "Schein")
            {
                //Prüfen ob Animation schon gestartet
                if (StartMs == 0)
                {
                    //Bilder sichtbar machen
                    ImgSchein.Visibility = System.Windows.Visibility.Visible;
                    Img2.Visibility = System.Windows.Visibility.Visible;
                    //Zeit neu erstellen
                    StartMs = UhrzeitMS;
                }
                //Wenn Animation schon gestartet
                else
                {
                    //Wenn Animation beendet
                    if ((UhrzeitMS - StartMs) > 400)
                    {
                        //Bild unsichtbar machen
                        ImgSchein.Visibility = System.Windows.Visibility.Collapsed;
                        //Animation umstellen
                        Animation = "Pause2";
                    }
                    //Wenn Animation noch läuft
                    else
                    {
                        //Prozent errechnen
                        int Prozent = 100 * 100000 / 400 * (UhrzeitMS - StartMs) / 100000;
                        //Margin neu berechnen
                        int NewMargin = 2000 * 100000 / 100 * Prozent / 100000;
                        ImgSchein.Margin = new Thickness((NewMargin - 150), 159, 0, 159);
                    }
                }
            }

            //Animation Pause2
            if (Animation == "Pause2")
            {
                //Prüfen ob Animation schon gestartet
                if (StartMs == 0)
                {
                    StartMs = UhrzeitMS;
                }
                //Wenn Animation schon gestartet
                else
                {
                    //Wenn Animation beendet
                    if ((UhrzeitMS - StartMs) > 800)
                    {
                        //Nächste Animation Starten
                        Animation = "Ausblenden";
                        StartMs = 0;
                    }
                }
            }

            //Animation Img1
            if (Animation == "Ausblenden")
            {
                //Prüfen ob Animation schon gestartet
                if (StartMs == 0)
                {
                    //Bilder sichtbar machen
                    ImgSchein.Visibility = System.Windows.Visibility.Visible;
                    Img2.Visibility = System.Windows.Visibility.Visible;
                    //Zeit neu erstellen
                    StartMs = UhrzeitMS;
                }
                //Wenn Animation schon gestartet
                else
                {
                    //Wenn Animation beendet
                    if ((UhrzeitMS - StartMs) > 400)
                    {
                        //Bilder entfernen
                        Img1.Visibility = System.Windows.Visibility.Collapsed;
                        Img2.Visibility = System.Windows.Visibility.Collapsed;
                        //Animation umstellen
                        Animation = "PauseEnde";
                    }
                    //Wenn Animation noch läuft
                    else
                    {
                        //Prozent errechnen
                        int Prozent = 100 * 100000 / 400 * (UhrzeitMS - StartMs) / 100000;
                        Prozent = 100 - Prozent;
                        //Bild Transparenz berechnen
                        double opa = 0.0;
                        if (Prozent >= 100)
                        {
                            opa = 1.0;
                        }
                        else if (Prozent >= 90)
                        {
                            opa = 0.9;
                        }
                        else if (Prozent >= 80)
                        {
                            opa = 0.8;
                        }
                        else if (Prozent >= 70)
                        {
                            opa = 0.7;
                        }
                        else if (Prozent >= 60)
                        {
                            opa = 0.6;
                        }
                        else if (Prozent >= 50)
                        {
                            opa = 0.5;
                        }
                        else if (Prozent >= 40)
                        {
                            opa = 0.4;
                        }
                        else if (Prozent >= 30)
                        {
                            opa = 0.3;
                        }
                        else if (Prozent >= 20)
                        {
                            opa = 0.2;
                        }
                        else if (Prozent >= 10)
                        {
                            opa = 0.1;
                        }
                        //Transparenz auf Bilder anwenden
                        Img1.Opacity = opa;
                        Img2.Opacity = opa;
                    }
                }
            }

            //Animation PauseEnde
            if (Animation == "PauseEnde")
            {
                //Prüfen ob Animation schon gestartet
                if (StartMs == 0)
                {
                    StartMs = UhrzeitMS;
                }
                //Wenn Animation schon gestartet
                else
                {
                    //Wenn Animation beendet
                    if ((UhrzeitMS - StartMs) > 300)
                    {
                        //Seite wechseln
                        dtt.Stop();
                        GRAnimation.Visibility = System.Windows.Visibility.Collapsed;
                    }
                }
            }
        }


        //Animation abbrechen
        private void AnimationStop(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Seite wechseln
            dtt.Stop();
            GRAnimation.Visibility = System.Windows.Visibility.Collapsed;
        }
        //---------------------------------------------------------------------------------------------------------------------------------










        // Allgemeine Variabeln
        //---------------------------------------------------------------------------------------------------------
        //Globalisierung
        string cul = "";

        //Einstellungen
        string Settings;
        string SetFolder = "*"; //"*" Zufälliger Ordner
        int SetRandom = 1;
        int SetImageNow = 1;
        int SetBatteryWarning = 20;
        int SetNoteCharging = 1;
        int SetNoteFullyCharged = 1;
        string SetNoteSounds = "English";
        string BatteryStatus = "none";
        bool LogoStart = true;


        //Anzahl Ordner
        int CFolders = 0;
        //Anzahl Bilder
        int CPictures = 0;

        //Version
        int AppVersion = 0;

        //Anzahl importierter Bilder
        int ImageCount = 0;
        //Anzahl erstellter Bilder
        int SwapImage = 0;

        //Prüfen ob Lockscreenapp
        bool IsLockscreenApp = false;
        //Prüfen ob Vollversion
        bool FullVersion = false;
        //Gibt an ob Bild im Trial erstellt wird
        bool Run = true;
        //Ob Trial Benachrichtigung bereits angezeigt wurde
        bool TrialMSG = false;
        
        //BackgroundTask
        bool IsSetBGTask = false;

        //Startzeit
        DateTime dt = DateTime.Now;
        //Aktuelle Zeit
        DateTime dt_Now = DateTime.Now;

        //Timer für die Animation
        DispatcherTimer dtt = new DispatcherTimer();

        //IsoStore file erstellen
        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
        IsolatedStorageFileStream filestream;
        StreamReader sr;
        StreamWriter sw;

        //Datenlisten
        ObservableCollection<ClassFolders> datalist = new ObservableCollection<ClassFolders>();
        ObservableCollection<ClassFolders> datalist2 = new ObservableCollection<ClassFolders>();
        ObservableCollection<ClassFolders> datalist3 = new ObservableCollection<ClassFolders>();
        ObservableCollection<ClassNumbers> datalist4 = new ObservableCollection<ClassNumbers>();

        //Battery Level
        int BatteryLevel = Battery.GetDefault().RemainingChargePercent;

        //Angeben das Update installiert wurde
        bool InstallingUpdate = false;
        //---------------------------------------------------------------------------------------------------------










        // Wird am Anfang der Seite geladen
        //---------------------------------------------------------------------------------------------------------
        public MainPage()
        {
            //Prüfen ob eine Sprachdatei besteht
            if (file.FileExists("Cul.dat"))
            {
                //Spachdatei laden
                filestream = file.OpenFile("Cul.dat", FileMode.Open);
                sr = new StreamReader(filestream);
                cul = sr.ReadToEnd();
                cul = cul.TrimEnd(new char[] { '\r', '\n' });
                filestream.Close();
                //Sprache einstellen
                CultureInfo newCulture = new CultureInfo(cul);
                Thread.CurrentThread.CurrentUICulture = newCulture;

            }

            //Prüfen ob lang.dat exestiert
            if (!file.FileExists("Lang.dat"))
            {
                //Sprachdatei erstellen
                string Lang = Lockscreen_Swap.AppResx.BatteryFullyCharged + ";" + Lockscreen_Swap.AppResx.BatteryIsCharging + ";" + Lockscreen_Swap.AppResx.BatteryLow + ";" + Lockscreen_Swap.AppResx.Day + ";" + Lockscreen_Swap.AppResx.Days + ";" + Lockscreen_Swap.AppResx.Hour + ";" + Lockscreen_Swap.AppResx.Hours + ";" + Lockscreen_Swap.AppResx.Minute + ";" + Lockscreen_Swap.AppResx.Minutes + ";";
                //Sprachdatei speichern
                filestream = file.CreateFile("Lang.dat");
                sw = new StreamWriter(filestream);
                sw.WriteLine(Convert.ToString(Lang));
                sw.Flush();
                filestream.Close();
            }

            //Komponenten laden
            InitializeComponent();

            //Animation vorbereiten
            Color backgroundColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            string temp = Convert.ToString(backgroundColor);
            if (temp == "#FF000000")
            {
                ImgSchein.Source = new BitmapImage(new Uri("Images/StartUp/Schein_black.png", UriKind.Relative));
                Img2.Source = new BitmapImage(new Uri("Images/StartUp/Logo800_2_black.png", UriKind.Relative));
                StartUpInfo.Foreground = new SolidColorBrush(Colors.White);
                Greetings.Foreground = new SolidColorBrush(Colors.White);
            }

            //Animation sichtbar machen
            GRAnimation.Visibility = System.Windows.Visibility.Visible;

            //Timer erstellen
            dtt.Stop();
            dtt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dtt.Tick += dt_Tick;
            dtt.Start();

            //Icons ändern
            if (temp != "#FF000000")
            {
                //Icons ändern
                ImgPictures01.Source = new BitmapImage(new Uri("Images/Folder.Add.Light.png", UriKind.Relative));
                ImgFolderOpen.Source = new BitmapImage(new Uri("Images/Folder.Light.png", UriKind.Relative));
                ImgFolderCopy.Source = new BitmapImage(new Uri("Images/Copy.Light.png", UriKind.Relative));
                ImgFolderEdit.Source = new BitmapImage(new Uri("Images/Edit.Light.png", UriKind.Relative));
                ImgFolderDelete.Source = new BitmapImage(new Uri("Images/Delete.Light.png", UriKind.Relative));
                ImgLogo.Source = new BitmapImage(new Uri("Images/Logo.Light.png", UriKind.Relative));
                ImgLogo.Opacity = 0.1;
                ImgFolderMenu.Source = new BitmapImage(new Uri("Images/Folder.Light.png", UriKind.Relative));
                ImgFolderMenu.Opacity = 0.1;
                ImgFolderMenu2.Source = new BitmapImage(new Uri("Images/Folder.Light.png", UriKind.Relative));
                ImgFolderMenu2.Opacity = 0.1;
                ImgFolderMenu3.Source = new BitmapImage(new Uri("Images/Battery.Light.png", UriKind.Relative));
                ImgFolderMenu3.Opacity = 0.1;
                ImgCreateNew.Source = new BitmapImage(new Uri("Images/Logo.Light.png", UriKind.Relative));
                ImgGlobe.Source = new BitmapImage(new Uri("Images/Globe.Light.png", UriKind.Relative));
                ImgInstructions.Source = new BitmapImage(new Uri("Images/Instruction.Light.png", UriKind.Relative));
                ImgSupport.Source = new BitmapImage(new Uri("Images/Support.Light.png", UriKind.Relative));
                ImgCompSounds.Source = new BitmapImage(new Uri("Images/Music.Light.png", UriKind.Relative));
            }

            //Batterie Warnung ListBox erstellen
            CreateBatteryWarning();
        }
        //---------------------------------------------------------------------------------------------------------










        // Wird bei jedem Aufruf der Seite geladen
        //---------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Prüfen ob Einstellungen bereits vorhanden
            if(!file.DirectoryExists("Settings"))
            {
                //Ordner erstellen
                file.CreateDirectory("Settings");
                file.CreateDirectory("Folders");
                file.CreateDirectory("Thumbs");
                file.CreateDirectory("Version");
                file.CreateDirectory("FoldersDat");

                //Leeres Hintergrundbild in Storage laden
                using (Stream input = Application.GetResourceStream(new Uri("Images/LockscreenImage.jpg", UriKind.Relative)).Stream)
                {
                    // Create a stream for the new file in the local folder.
                    using (IsolatedStorageFileStream output = file.CreateFile("LockscreenImage.jpg"))
                    {
                        // Initialize the buffer.
                        byte[] readBuffer = new byte[4096];
                        int bytesRead = -1;

                        // Copy the file from the installation folder to the local folder. 
                        while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                        {
                            output.Write(readBuffer, 0, bytesRead);
                        }
                    }
                }

                //Startzeit erstellen //Anzahl erstellter Wallpapers
                filestream = file.CreateFile("Settings/FirstTime.dat");
                sw = new StreamWriter(filestream);
                sw.WriteLine(Convert.ToString(dt));
                sw.Flush();
                filestream.Close();

                //Image Count erstellen //Anzahl importierter Bilder
                filestream = file.CreateFile("Settings/ImageCount.dat");
                sw = new StreamWriter(filestream);
                sw.WriteLine("0");
                sw.Flush();
                filestream.Close();

                //SwapImage erstellen //Anzahl erstellter Bilder
                filestream = file.CreateFile("Settings/SwapImage.dat");
                sw = new StreamWriter(filestream);
                sw.WriteLine("0");
                sw.Flush();
                filestream.Close();

                //FullVersion erstellen
                filestream = file.CreateFile("Settings/FullVersion.dat");
                sw = new StreamWriter(filestream);
                sw.WriteLine("0");
                sw.Flush();
                filestream.Close();

                //Versions Datei erstellen
                filestream = file.CreateFile("Version/Version.dat");
                sw = new StreamWriter(filestream);
                sw.WriteLine("20450");
                sw.Flush();
                filestream.Close();

                //Settings erstellen
                filestream = file.CreateFile("Settings/Settings.dat");
                sw = new StreamWriter(filestream);
                sw.WriteLine("0");
                sw.Flush();
                filestream.Close();

                //Einstellungen neu erstellen
                CreateSettings();
            }

            //Wenn Einstellungen bereits vorhanden
            else
            {
                //First Time// DateTime der ersten installation
                filestream = file.OpenFile("Settings/FirstTime.dat", FileMode.Open);
                sr = new StreamReader(filestream);
                string temp = sr.ReadToEnd();
                filestream.Close();
                dt = Convert.ToDateTime(temp);

                //ImageCount laden //Anzahl importierter Bilder
                filestream = file.OpenFile("Settings/ImageCount.dat", FileMode.Open);
                sr = new StreamReader(filestream);
                temp = sr.ReadToEnd();
                filestream.Close();
                ImageCount = Convert.ToInt32(temp);

                //SwapImage //Anzahl erstellter Bilder 
                filestream = file.OpenFile("Settings/SwapImage.dat", FileMode.Open);
                sr = new StreamReader(filestream);
                temp = sr.ReadToEnd();
                filestream.Close();
                SwapImage = Convert.ToInt32(temp);

                //FullVersion laden
                filestream = file.OpenFile("Settings/FullVersion.dat", FileMode.Open);
                sr = new StreamReader(filestream);
                temp = sr.ReadToEnd();
                filestream.Close();
                int temp2 = Convert.ToInt32(temp);
                if (temp2 == 1)
                {
                    FullVersion = true;
                }

                //Version Datei laden
                filestream = file.OpenFile("Version/Version.dat", FileMode.Open);
                sr = new StreamReader(filestream);
                temp = sr.ReadToEnd();
                filestream.Close();
                AppVersion = Convert.ToInt32(temp);

                //Einstellungen laden
                filestream = file.OpenFile("Settings/Settings.dat", FileMode.Open);
                sr = new StreamReader(filestream);
                Settings = sr.ReadToEnd();
                filestream.Close();
            }



            //Update auf 2.1.45.0
            //****************************************************************************************************************************************************
            if (AppVersion < 2001045000)
            {
                //Versions Datei neu erstellen
                filestream = file.CreateFile("Version/Version.dat");
                sw = new StreamWriter(filestream);
                sw.WriteLine("2001045000");
                sw.Flush();
                filestream.Close();

                //Rate File erstellen
                DateTime datetime = DateTime.Now;
                datetime = datetime.AddDays(4);
                filestream = file.CreateFile("Settings/RateReminder.txt");
                sw = new StreamWriter(filestream);
                sw.WriteLine(datetime.ToString());
                sw.Flush();
                filestream.Close();

                //Sounds in Isostore schreiben
                string[] Sounds = { "BatteryFullyCharged.mp3", "BatteryIsCharging.mp3", "BatteryLow.mp3" };
                file.CreateDirectory("Sounds");
                file.CreateDirectory("Sounds/English");
                for (int i = 0; i < Sounds.Count(); i++)
                {
                    //Prüfen ob Datei bereits existiert
                    if (file.FileExists("Sounds/English/" + Sounds[i]))
                    {
                        file.DeleteFile("Sounds/English/" + Sounds[i]);
                    }
                    // Create a stream for the file in the installation folder.
                    using (Stream input = Application.GetResourceStream(new Uri("Sounds/English/" + Sounds[i], UriKind.Relative)).Stream)
                    {
                        // Create a stream for the new file in the local folder.
                        using (IsolatedStorageFileStream output = file.CreateFile("Sounds/English/" + Sounds[i]))
                        {
                            // Initialize the buffer.
                            byte[] readBuffer = new byte[4096];
                            int bytesRead = -1;

                            // Copy the file from the installation folder to the local folder. 
                            while ((bytesRead = input.Read(readBuffer, 0, readBuffer.Length)) > 0)
                            {
                                output.Write(readBuffer, 0, bytesRead);
                            }

                        }
                    }
                }

                //Sounds in Shell Shared Content kopieren
                for (int i = 0; i < Sounds.Count(); i++)
                {
                    if (file.FileExists("Shared/ShellContent/" + Sounds[i]))
                    {
                        file.DeleteFile("Shared/ShellContent/" + Sounds[i]);
                    }
                    file.CopyFile("Sounds/English/" + Sounds[i], "Shared/ShellContent/" + Sounds[i]);
                }

                //Update Installieren
                InstallingUpdate = true;
            }
            //****************************************************************************************************************************************************


            //Prüfen ob Reminder noch vorhanden und wenn ja, laden
            if (file.FileExists("Settings/RateReminder.txt"))
            {
                //Daten laden
                filestream = file.OpenFile("Settings/RateReminder.txt", FileMode.Open);
                sr = new StreamReader(filestream);
                string temp = sr.ReadToEnd();
                filestream.Close();
                temp = temp.TrimEnd(new char[] { '\r', '\n' });

                //Prüfen of Benachrichtigung ausgegeben wird
                DateTime DT_Reminder = Convert.ToDateTime(temp);
                DateTime DT_Now = DateTime.Now;
                int result = DateTime.Compare(DT_Reminder, DT_Now);
                if (result < 0)
                {
                    //Bewertung öffnen
                    GRRate.Margin = new Thickness(0, 0, 0, 0);
                    MenuOpen = true;
                }
            }


            //Einstellungen umsetzen
            string[] SplitSettings = Regex.Split(Settings, ";");
            //Einstellungen Ordner
            SetFolder = SplitSettings[0];
            if (SetFolder == "*")
            {
                BtnLockscreenFolder.Content = "(" + Lockscreen_Swap.AppResx.Random + ")";
            }
            else
            {
                BtnLockscreenFolder.Content = SetFolder;
            }
            //Einstellungen Zufällige Wiedergabe
            SetRandom = Convert.ToInt32(SplitSettings[1]);
            if (SetRandom == 1)
            {
                BtnRandom.Content = Lockscreen_Swap.AppResx.On;
            }
            else
            {
                BtnRandom.Content = Lockscreen_Swap.AppResx.Off;
            }
            //Momentan verwendetes Bild
            SetImageNow = Convert.ToInt32(SplitSettings[2]);
            //Batterie Warning
            SetBatteryWarning = Convert.ToInt32(SplitSettings[3]);
            if (SetBatteryWarning == 0)
            {
                BtnBatteryWarning.Content = Lockscreen_Swap.AppResx.Off;
            }
            else
            {
                BtnBatteryWarning.Content = SetBatteryWarning + " %";
            }
            //Note Battery Charging
            SetNoteCharging = Convert.ToInt32(SplitSettings[4]);
            if (SetNoteCharging == 0)
            {
                BtnIsCharging.Content = Lockscreen_Swap.AppResx.Off;
            }
            else
            {
                BtnIsCharging.Content = Lockscreen_Swap.AppResx.On;
            }
            //Note fully Charged
            SetNoteFullyCharged = Convert.ToInt32(SplitSettings[5]);
            if (SetNoteFullyCharged == 0)
            {
                BtnFullyCharged.Content = Lockscreen_Swap.AppResx.Off;
            }
            else
            {
                BtnFullyCharged.Content = Lockscreen_Swap.AppResx.On;
            }
            //Note Sounds
            SetNoteSounds = SplitSettings[6];
            if (SetNoteSounds == "*")
            {
                BtnNoteSounds.Content = Lockscreen_Swap.AppResx.Z01_SoundSilent;
            }
            else if (SetNoteSounds == "**")
            {
                BtnNoteSounds.Content = Lockscreen_Swap.AppResx.Z01_SoundStandard;
            }
            else
            {
                BtnNoteSounds.Content = SetNoteSounds;
            }            
            

            //Wenn Update installiert wurde, Einstellungen speichern
            if (InstallingUpdate == true)
            {
                //Sound umstellen
                SetNoteSounds = "English";
                //Logo Start
                LogoStart = true;
                InstallingUpdate = false;
                //Einstellungen speichern
                CreateSettings();
            }
            else
            //Wenn Update bereits installiert
            {
                LogoStart = Convert.ToBoolean(SplitSettings[8]);
            }

            if (LogoStart == true)
            {
                //Button einstellen
                BtnLogoStart.Content = Lockscreen_Swap.AppResx.On;
            }
            else
            {
                //Button einstellen
                BtnLogoStart.Content = Lockscreen_Swap.AppResx.Off;
                //Logo deaktivieren
                dtt.Stop();
                GRAnimation.Visibility = System.Windows.Visibility.Collapsed;
            }

            //Menüs schließen
            GRFolderMenu.Margin = new Thickness(-600, 0, 0, 0);
            GRFolder.Margin = new Thickness(-600, 0, 0, 0);
            GRBatteryWarning.Margin = new Thickness(-600, 0, 0, 0);
            GRSoundSelect.Margin = new Thickness(-600, 0, 0, 0);
            //Angeben das Menüs geschlossen sind
            MenuOpen = false;


            //Prüfen ob App gerade gekauft wurde und in Einstellungen speichern
            if ((Application.Current as App).IsTrial)
            {
            }
            //Bei Kaufversion
            else
            {
                if (FullVersion == false)
                {
                    //Settings neu erstellen
                    IsolatedStorageFileStream filestream = file.CreateFile("Settings/FullVersion.dat");
                    StreamWriter sw = new StreamWriter(filestream);
                    sw.WriteLine("1");
                    sw.Flush();
                    filestream.Close();
                    //FullVersion umstellen
                    FullVersion = true;
                    //Benachrichtigung ausgeben
                    MessageBox.Show(Lockscreen_Swap.AppResx.PurchaseNote);
                }
            }


            //Bei Vollversion
            if (FullVersion == true)
            {
                SPTrial.Visibility = System.Windows.Visibility.Collapsed;
            }
            //Bei Demoversion
            else
            {
                //Prüfen ob Trial Zeit abgelaufen
                TimeSpan diff = dt_Now - dt;
                int MinToGo = 1440 - Convert.ToInt32(diff.TotalMinutes);
                //Wenn Zeit abgelaufen
                if (MinToGo <= 0)
                {
                    //Angeben das Bild auf Leer gestellt wird
                    Run = false;
                    if (TrialMSG == false)
                    {
                        MessageBox.Show(Lockscreen_Swap.AppResx.TrialNote);
                        TrialMSG = true;
                        TBTrial.Text = Lockscreen_Swap.AppResx.TrialExpired;
                        TBTialTime.Text = "";
                    }
                }
                //Wenn Zeit nicht abgelaufen
                else
                {
                    //Restliche Zeit erstellen
                    string tTime = "";
                    int tH = MinToGo / 60;
                    if (tH == 1)
                    {
                        tTime += tH + " " + Lockscreen_Swap.AppResx.Hour + "   ";
                    }
                    else
                    {
                        tTime += tH + " " + Lockscreen_Swap.AppResx.Hours + "   ";
                    }
                    int TM = MinToGo - (tH * 60);
                    if (TM == 1)
                    {
                        tTime += TM + " " + Lockscreen_Swap.AppResx.Minute;
                    }
                    else
                    {
                        tTime += TM + " " + Lockscreen_Swap.AppResx.Minutes;
                    }
                    //Zeit ausgeben
                    TBTialTime.Text = tTime;
                }
            }


            //Prüfen ob ScheduledTaskAgent läuft
            periodicTask = ScheduledActionService.Find(periodicTaskName) as PeriodicTask;

            //Prüfen ob momentan Lockscreen App
            if (Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication)
            {
                //IsLockscreenApp auf true stellen
                IsLockscreenApp = true;
            }

            //Prüfen ob Task läuft und App Lockscreen App ist
            if (periodicTask != null & IsLockscreenApp == true)
            {
                //agentsAreEnabled auf true
                agentsAreEnabled = true;

                //Button umstellen
                TBLockscreenChanger.Content = Lockscreen_Swap.AppResx.On;
            }

            //Wenn nicht, Button auf false stellen
            else
            {
                //agentsAreEnabled auf false
                agentsAreEnabled = false;

                //Button umstellen
                TBLockscreenChanger.Content = Lockscreen_Swap.AppResx.Off;
            }

            //Abfragen ob App als Lockscreen App gesetzt werden soll
            if ((IsLockscreenApp == false | agentsAreEnabled == false) & IsSetBGTask == false)
            {
                //Lockhelper aktivieren
                string filePathOfTheImage = "/Images/LockscreenImage.jpg";
                bool isAppResource = true;
                LockHelper(filePathOfTheImage, isAppResource);

                //Button umstellen
                TBLockscreenChanger.Content = Lockscreen_Swap.AppResx.On;
            }

            //Hintergrund Task einmalig neu aktivieren //Und Lockscreen einmalig neu erstellen
            if (IsSetBGTask == false)
            {
                //Task neu Starten
                StartPeriodicAgent();
                //Angeben das Task gestartet wurde
                IsSetBGTask = true;

                //Lockscreen Bild wechseln
                if (IsLockscreenApp == true)
                {
                    CreateImage();
                }
            }

            //Ordnerlisten erstellen
            CreateFolders();

            //Liste der Töne erstellen
            CreateSounds();

            //Tile neu erstellen
            CreateTile();

            // Prüfen ob Werbung bereits angezeigt wurde
            if (!file.FileExists("Settings/ShowAd.txt"))
            {
                // Angeben das Werbung beriets angezeigt wurde
                filestream = file.CreateFile("Settings/ShowAd.txt");
                sw = new StreamWriter(filestream);
                sw.WriteLine("1");
                sw.Flush();
                filestream.Close();

                // 10.0 Lock Screen Seite anzeigen
                NavigationService.Navigate(new Uri("/Pages/10_0_Screen_Changer.xaml", UriKind.Relative));
            }
        }
        //---------------------------------------------------------------------------------------------------------









        //Einstellungsn neu erstellen
        //---------------------------------------------------------------------------------------------------------
        void CreateSettings()
        {
            //Einstellungen zusammensetzten
            Settings = SetFolder + ";" + SetRandom + ";" + SetImageNow + ";" + SetBatteryWarning + ";" + SetNoteCharging + ";" + SetNoteFullyCharged + ";" + SetNoteSounds + ";" + BatteryStatus + ";" + LogoStart.ToString() +";;;;;;;;;;;;;;;;;;;;;;;;;;;";

            //Einstellungen speichern
            IsolatedStorageFileStream filestream = file.OpenFile("Settings/Settings.dat", FileMode.Open);
            StreamWriter sw = new StreamWriter(filestream);
            sw.WriteLine(Settings);
            sw.Flush();
            filestream.Close();
        }
        //---------------------------------------------------------------------------------------------------------










        //Sound Auswahl neu erstellen
        //-----------------------------------------------------------------------------------------------------------------
        void CreateSounds()
        {
            //Alte Datenliste löschen
            datalist4.Clear();

            //Lautlos hinzufügen
            datalist4.Add(new ClassNumbers(Lockscreen_Swap.AppResx.Z01_SoundSilent));
            //Standard hinzufügen
            datalist4.Add(new ClassNumbers(Lockscreen_Swap.AppResx.Z01_SoundStandard));

            //Sound Ordner durchsuchen
            string[] soundList = file.GetDirectoryNames("Sounds/");

            //Sounds hinzufügen
            for (int i = 0; i < soundList.Count(); i++)
            {
                datalist4.Add(new ClassNumbers(soundList[i]));
            }

            //Sounds mit Listbox verknüpfen
            LBSoundSelect.ItemsSource = datalist4;
        }
        //-----------------------------------------------------------------------------------------------------------------










        //Einstellungsn neu erstellen
        //---------------------------------------------------------------------------------------------------------
        void CreateBatteryWarning()
        {
            //Datenliste neu erstellen
            datalist3.Clear();
            datalist3.Add(new ClassFolders("(" + Lockscreen_Swap.AppResx.Off + ")", "0"));
            datalist3.Add(new ClassFolders("5 %", "5"));
            datalist3.Add(new ClassFolders("10 %", "10"));
            datalist3.Add(new ClassFolders("15 %", "15"));
            datalist3.Add(new ClassFolders("20 %", "20"));
            datalist3.Add(new ClassFolders("25 %", "25"));
            datalist3.Add(new ClassFolders("30 %", "30"));
            datalist3.Add(new ClassFolders("35 %", "35"));
            datalist3.Add(new ClassFolders("40 %", "40"));
            //Daten in Listbox schreiben
            LBBatteryWarning.ItemsSource = datalist3;
        }
        //---------------------------------------------------------------------------------------------------------










        //Ordner erstellen
        //---------------------------------------------------------------------------------------------------------
        void CreateFolders()
        {
            //Alte Daten löschen
            datalist.Clear();
            datalist2.Clear();
            datalist2.Add(new ClassFolders("(" + Lockscreen_Swap.AppResx.Random + ")", "0"));

            //Dateien Laden "Styles"
            string[] tempFolders = file.GetDirectoryNames("Thumbs/");
            int tempFolders_c = tempFolders.Count();

            //Anzahl Ordner ausgeben
            CFolders = tempFolders_c;

            //Anzahl Bilder löschen
            CPictures = 0;
            
            //Ordner durchlaufen
            for (int i = 0; i < tempFolders_c; i++)
            {
                //Anzahl der Bilder laden
                string[] tempPictures = file.GetFileNames("Thumbs/" + tempFolders[i] + "/");
                //Anzahl Bilder erstellen
                string tempBilder = "";
                int temp2 = tempPictures.Count();
                if (temp2 == 1)
                {
                    tempBilder = "1 " + Lockscreen_Swap.AppResx.Picture;
                }
                else
                {
                    tempBilder = temp2 + " " + Lockscreen_Swap.AppResx.Pictures;
                }

                //Bilder gesamt zählen
                CPictures += temp2;

                //Daten in Klasse schreiben
                datalist.Add(new ClassFolders(tempFolders[i], tempBilder));
                datalist2.Add(new ClassFolders(tempFolders[i], tempBilder));
            }

            //Daten in Listbox schreiben
            LBFolders.ItemsSource = datalist;
            LBFolder.ItemsSource = datalist2;
        }
        //---------------------------------------------------------------------------------------------------------










        //Neues Bild erstellen
        //*********************************************************************************************************
        void CreateImage()
        {
            //Wichtig, bool "Run" beachten
            if (Run == true)
            {
                //Variabeln
                bool NoPicture = false;
                string ShowPicture = "";


                //Wenn bestimmter Ordner ausgewählt
                if (SetFolder != "*")
                {
                    //Variabeln
                    string AllFolders = "";
                    string RunFolder;
                    
                    //Ordner.dat auslesen ob Bilder vorhanden
                    IsolatedStorageFileStream filestream = file.OpenFile("FoldersDat/" + SetFolder + ".dat", FileMode.Open);
                    StreamReader sr = new StreamReader(filestream);
                    string temp = sr.ReadToEnd();
                    filestream.Close();

                    int cTempPictures = Convert.ToInt32(temp);
                    //Prüfen ob Dateien in Ordner vorhanden
                    if (cTempPictures > 0)
                    {
                        //Der Reihe nach
                        if (SetRandom != 1)
                        {
                            //Alle Bilder laden
                            string[] AllPictures = file.GetFileNames("Thumbs/" + SetFolder + "/");
                            int cAllPictures = AllPictures.Count();
                            //SetImageNow prüfen
                            if (SetImageNow >= cAllPictures)
                            {
                                SetImageNow = 1;
                            }
                            else
                            {
                                SetImageNow++;
                            }
                            //Settings neu erstellen
                            CreateSettings();
                            //ShowPicture erstellen
                            ShowPicture = "Folders/" + SetFolder + "/" + AllPictures[(SetImageNow - 1)];
                        }

                        //Zufälliges Bild aus Ordner wählen
                        else
                        {
                            Random Rand = new Random();
                            string[] AllPictures = file.GetFileNames("Thumbs/" + SetFolder + "/");
                            int cAllPictures = AllPictures.Count();
                            int iPicture = Rand.Next(1, cAllPictures + 1);
                            if (iPicture > cAllPictures)
                            {
                                iPicture = cAllPictures;
                            }
                            ShowPicture = "Folders/" + SetFolder + "/" + AllPictures[iPicture - 1];
                        }
                    }
                    //Wenn keine Dateien im Ordner vorhanden
                    else
                    {
                        //Ordner Einstellung zurücksetzten
                        SetFolder = "*";
                        CreateSettings();
                        //Random in Ordner schreiben
                        BtnLockscreenFolder.Content = "(" + Lockscreen_Swap.AppResx.Random + ")";
                    }

                }

     
                //Prüfen ob Random ausgewählt ist
                if (SetFolder == "*")
                {
                    //Ordner auslesen
                    string[] FoldersSplit = file.GetFileNames("FoldersDat/");
                    int cFoldersSplit = FoldersSplit.Count();
                    string AllFolders = "";
                    string RunFolder;

                    //Ordner durchlaufen
                    for (int i = 0; i < cFoldersSplit; i++)
                    {
                        //Ordner.dat auslesen ob Bilder vorhanden
                        IsolatedStorageFileStream filestream = file.OpenFile("FoldersDat/" + FoldersSplit[i], FileMode.Open);
                        StreamReader sr = new StreamReader(filestream);
                        string temp = sr.ReadToEnd();
                        filestream.Close();
                        int cTempPictures = Convert.ToInt32(temp);
                        //Prüfen ob Dateien in Ordner vorhanden
                        if (cTempPictures > 0)
                        {
                            AllFolders += FoldersSplit[i] + "///";
                        }
                    }
                    //Zufälligen Ordner auswählen
                    string[] AllFoldersSplit = Regex.Split(AllFolders, "///");
                    int cAllFoldersSplit = AllFoldersSplit.Count() - 1;
                    if (cAllFoldersSplit > 0)
                    {
                        Random Rand = new Random();
                        int iFolder = Rand.Next(1, (cAllFoldersSplit + 1));
                        if (iFolder > cAllFoldersSplit)
                        {
                            iFolder = cAllFoldersSplit;
                        }
                        RunFolder = AllFoldersSplit[iFolder - 1];
                        string[] SplitRunFolder = Regex.Split(RunFolder, ".dat");
                        int cSplitRunFolder = SplitRunFolder.Count();
                        RunFolder = SplitRunFolder[0];
                        for (int i2 = 1; i2 < (cSplitRunFolder - 1); i2++)
                        {
                            RunFolder += ".dat" + SplitRunFolder[i2];
                        }

                        //Zufälliges Bild aus Ordner wählen
                        string[] AllPictures = file.GetFileNames("Thumbs/" + RunFolder + "/");
                        int cAllPictures = AllPictures.Count();
                        int iPicture = Rand.Next(1, cAllPictures + 1);
                        if (iPicture > cAllPictures)
                        {
                            iPicture = cAllPictures;
                        }
                        ShowPicture = "Folders/" + RunFolder + "/" + AllPictures[iPicture - 1];
                    }
                    else
                    {
                        NoPicture = true;
                    }

                }


                //Prüfen ob Bild vorhanden und Lockscreen erstellen
                if (NoPicture == false)
                {
                    //Prüfen ob Ordner schon vorhanden
                    if (!file.DirectoryExists("Background"))
                    {
                        file.CreateDirectory("Background");
                    }
                    //Prüfen ob Bilder vorhanden und alte Bilder löschen
                    string SaveFile;
                    if (file.FileExists("Background/" + (SwapImage - 1) + ".jpg"))
                    {
                        file.DeleteFile("Background/" + (SwapImage - 1) + ".jpg");
                    }
                    if (file.FileExists("Background/" + (SwapImage - 2) + ".jpg"))
                    {
                        file.DeleteFile("Background/" + (SwapImage - 2) + ".jpg");
                    }

                    //SwapImage erhöhen
                    SwapImage++;
                    //SaveFile neu erstellen
                    SaveFile = "Background/" + SwapImage + ".jpg";

                    //SwapImage speichern
                    IsolatedStorageFileStream filestream = file.OpenFile("Settings/SwapImage.dat", FileMode.Open);
                    StreamWriter sw = new StreamWriter(filestream);
                    sw.WriteLine(SwapImage);
                    sw.Flush();
                    filestream.Close();

                    //Datei kopieren
                    file.CopyFile(ShowPicture, SaveFile);

                    //Lockscreen erstellen
                    string filePathOfTheImage = SaveFile;
                    var schema = "ms-appdata:///Local/";
                    var uri = new Uri(schema + filePathOfTheImage, UriKind.RelativeOrAbsolute);
                    if (Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication)
                    {
                        Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);
                    }

                    //Bild in ShowImage laden
                    var Background = new WriteableBitmap(0, 0);
                    byte[] tempData;
                    MemoryStream tempMs;
                    using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream isfs = isf.OpenFile(SaveFile, FileMode.Open, FileAccess.Read))
                        {
                            tempData = new byte[isfs.Length];
                            isfs.Read(tempData, 0, tempData.Length);
                            isfs.Close();
                        }
                    }
                    tempMs = new MemoryStream(tempData);
                    Background.SetSource(tempMs);
                    DemoImage.Source = Background;
                }

                //Wenn kein Bild vorhanden
                else
                {
                    //Bild auf LockscreenImage setzen
                    ShowPicture = "LockscreenImage.jpg";

                    //Prüfen ob Ordner schon vorhanden
                    if (!file.DirectoryExists("Background"))
                    {
                        file.CreateDirectory("Background");
                    }
                    //Prüfen ob Bilder vorhanden und alte Bilder löschen
                    string SaveFile;
                    if (file.FileExists("Background/" + (SwapImage - 1) + ".jpg"))
                    {
                        file.DeleteFile("Background/" + (SwapImage - 1) + ".jpg");
                    }
                    if (file.FileExists("Background/" + (SwapImage - 2) + ".jpg"))
                    {
                        file.DeleteFile("Background/" + (SwapImage - 2) + ".jpg");
                    }

                    //SwapImage erhöhen
                    SwapImage++;
                    //SaveFile neu erstellen
                    SaveFile = "Background/" + SwapImage + ".jpg";

                    //SwapImage speichern
                    IsolatedStorageFileStream filestream = file.OpenFile("Settings/SwapImage.dat", FileMode.Open);
                    StreamWriter sw = new StreamWriter(filestream);
                    sw.WriteLine(SwapImage);
                    sw.Flush();
                    filestream.Close();

                    //Datei kopieren
                    file.CopyFile(ShowPicture, SaveFile);

                    //Lockscreen erstellen
                    string filePathOfTheImage = SaveFile;
                    var schema = "ms-appdata:///Local/";
                    var uri = new Uri(schema + filePathOfTheImage, UriKind.RelativeOrAbsolute);
                    if (Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication)
                    {
                        Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);
                    }

                    //Bild in ShowImage laden
                    var Background = new WriteableBitmap(0, 0);
                    byte[] tempData;
                    MemoryStream tempMs;
                    using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        using (IsolatedStorageFileStream isfs = isf.OpenFile(SaveFile, FileMode.Open, FileAccess.Read))
                        {
                            tempData = new byte[isfs.Length];
                            isfs.Read(tempData, 0, tempData.Length);
                            isfs.Close();
                        }
                    }
                    tempMs = new MemoryStream(tempData);
                    Background.SetSource(tempMs);
                    DemoImage.Source = Background;
                }



            }
            //Wenn Bild nicht gewechselt wird
            else
            {
                //Bild auf LockscreenImage setzen
                string ShowPicture = "LockscreenImage.jpg";

                //Prüfen ob Ordner schon vorhanden
                if (!file.DirectoryExists("Background"))
                {
                    file.CreateDirectory("Background");
                }
                //Prüfen ob Bilder vorhanden und alte Bilder löschen
                string SaveFile;
                if (file.FileExists("Background/" + (SwapImage - 1) + ".jpg"))
                {
                    file.DeleteFile("Background/" + (SwapImage - 1) + ".jpg");
                }
                if (file.FileExists("Background/" + (SwapImage - 2) + ".jpg"))
                {
                    file.DeleteFile("Background/" + (SwapImage - 2) + ".jpg");
                }

                //SwapImage erhöhen
                SwapImage++;
                //SaveFile neu erstellen
                SaveFile = "Background/" + SwapImage + ".jpg";

                //SwapImage speichern
                IsolatedStorageFileStream filestream = file.OpenFile("Settings/SwapImage.dat", FileMode.Open);
                StreamWriter sw = new StreamWriter(filestream);
                sw.WriteLine(SwapImage);
                sw.Flush();
                filestream.Close();

                //Datei kopieren
                file.CopyFile(ShowPicture, SaveFile);

                //Lockscreen erstellen
                string filePathOfTheImage = SaveFile;
                var schema = "ms-appdata:///Local/";
                var uri = new Uri(schema + filePathOfTheImage, UriKind.RelativeOrAbsolute);
                if (Windows.Phone.System.UserProfile.LockScreenManager.IsProvidedByCurrentApplication)
                {
                    Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);
                }

                //Bild in ShowImage laden
                var Background = new WriteableBitmap(0, 0);
                byte[] tempData;
                MemoryStream tempMs;
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream isfs = isf.OpenFile(SaveFile, FileMode.Open, FileAccess.Read))
                    {
                        tempData = new byte[isfs.Length];
                        isfs.Read(tempData, 0, tempData.Length);
                        isfs.Close();
                    }
                }
                tempMs = new MemoryStream(tempData);
                Background.SetSource(tempMs);
                DemoImage.Source = Background;
            }
            
        }
        //*********************************************************************************************************










        //Tile Updaten
        //*********************************************************************************************************
        void CreateTile()
        {
            //Restliche Zeit anzeigen
            TimeSpan tempTime = Battery.GetDefault().RemainingDischargeTime;
            int tempDays = tempTime.Days;
            int tempHours = tempTime.Hours;
            int tempMinutes = tempTime.Minutes;
            string tempText = "";
            if (tempDays != 0)
            {
                if (tempDays >= 20)
                {
                    tempText += "20 " + Lockscreen_Swap.AppResx.Days + ", ";
                }
                else
                {
                    if (tempDays == 1)
                    {
                        tempText += "1 " + Lockscreen_Swap.AppResx.Day + ", ";
                    }
                    else
                    {
                        tempText += tempDays + " " + Lockscreen_Swap.AppResx.Days + ", ";
                    }
                }
            }
            if (tempHours != 0)
            {
                if (tempHours == 1)
                {
                    tempText += "1 " + Lockscreen_Swap.AppResx.Hour + ", ";
                }
                else
                {
                    tempText += tempHours + " " + Lockscreen_Swap.AppResx.Hours + ", ";
                }
            }
            else
            {
                if (tempDays != 0)
                {
                    tempText += "0 " + Lockscreen_Swap.AppResx.Hours + ", ";
                }
            }
            if (tempMinutes == 1)
            {
                tempText += "1 " + Lockscreen_Swap.AppResx.Minute;
            }
            else
            {
                tempText += tempMinutes + " " + Lockscreen_Swap.AppResx.Minutes;
            }


            //Batterie Leistung umwandeln
            if (BatteryLevel == 100)
            {
                BatteryLevel = 99;
            }
            //Tile neu erstellen
            ShellTile tile = ShellTile.ActiveTiles.First();
            IconicTileData TileData = new IconicTileData()
            {
                //Title = TileTitle,
                //WideBackContent = "[back of wide Tile size content]",
                Count = BatteryLevel,
                WideContent1 = tempText,
                //SmallBackgroundImage = new Uri(tempUri, UriKind.Absolute),
                //BackgroundImage = new Uri(tempUri, UriKind.Absolute),
                //BackBackgroundImage = [back of medium Tile size URI],
                //WideBackgroundImage = [front of wide Tile size URI],
                //WideBackBackgroundImage = [back of wide Tile size URI],
            };
            tile.Update(TileData);
            //Startseite updaten
            TBBatteryStatus.Text = tempText;
        }
        //*********************************************************************************************************










        //Menü Buttons
        //*********************************************************************************************************
        //Menü Variabeln
        //-----------------------------------------------------------------------------------------------------------------
        //Gibt an ob Menü offen ist
        bool MenuOpen = false;

        //Folder Menü Variabeln
        int SelectedFolder = -1;
        private byte tbyte;
        //-----------------------------------------------------------------------------------------------------------------


        //Menü öffnen
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnOpenFolderMenu(object sender, SelectionChangedEventArgs e)
        {
            //Prüfen ob Menü bereits offen
            if (MenuOpen == false)
            {
                //Index auswählen
                SelectedFolder = Convert.ToInt32(LBFolders.SelectedIndex);
                TBFolderMenuName.Text = (datalist[SelectedFolder] as ClassFolders).name;
                TBFolderMenuPictures.Text = (datalist[SelectedFolder] as ClassFolders).images;

                //Menü öffnen
                GRFolderMenu.Margin = new Thickness(0, 0, 0, 0);
                //Angeben das ein Menü offen ist
                MenuOpen = true;

                //Auswahl aufheben
                try
                {
                    LBFolders.SelectedIndex = -1;
                }
                catch
                {
                }
            }
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Folder bearbeiten
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnEditFolder(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Style auswählen
            string tempFolder = (datalist[SelectedFolder] as ClassFolders).name;
            //Editor öffnen
            NavigationService.Navigate(new Uri("/Pages/EditFolder.xaml?folder=" + tempFolder, UriKind.Relative));
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Folder umbenennen
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnRemaneFolder(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Style auswählen
            string tempFolder = (datalist[SelectedFolder] as ClassFolders).name;

            //Editor öffnen
            NavigationService.Navigate(new Uri("/Pages/RenameFolder.xaml?folder=" + tempFolder, UriKind.Relative));
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Folder kopieren
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnCopyFolder(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Style auswählen
            string tempFolder = (datalist[SelectedFolder] as ClassFolders).name;
            //Editor öffnen
            NavigationService.Navigate(new Uri("/Pages/CopyFolder.xaml?folder=" + tempFolder, UriKind.Relative));
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Folder löschen
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnDeleteFolder(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Style auswählen
            string tempFolder = (datalist[SelectedFolder] as ClassFolders).name;
            if (MessageBox.Show(Lockscreen_Swap.AppResx.Delete + " " + tempFolder, Lockscreen_Swap.AppResx.WARNING, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {

                //Ordner löschen
                DeleteDirectory("Folders/" + tempFolder + "/");
                //Thumbnail Ordner löschen
                DeleteDirectory("Thumbs/" + tempFolder + "/");
                //FolderDat löschen
                file.DeleteFile("FoldersDat/" + tempFolder + ".dat");

                //Folders neu laden
                CreateFolders();
                //Menü schließen
                GRFolderMenu.Margin = new Thickness(-600, 0, 0, 0);
                MenuOpen = false;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Folder erstellen
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnCreateFolder(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Editor öffnen
            NavigationService.Navigate(new Uri("/Pages/CreateFolder.xaml", UriKind.Relative));
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Ordner mit gesamten Inhalt löschen
        //---------------------------------------------------------------------------------------------------------
        public void DeleteDirectory(string target_dir)
        {
            try
            {
                //Ordner löschen
                IsolatedStorageFile file2 = IsolatedStorageFile.GetUserStoreForApplication();
                string[] files = file2.GetFileNames(target_dir);
                //string[] dirs = file11.GetDirectoryNames(target_dir);
                foreach (string file in files)
                {
                    file2.DeleteFile(target_dir + file);
                }
                file2.DeleteDirectory(target_dir);
            }
            catch
            {
            }
        }
        //---------------------------------------------------------------------------------------------------------
        //*********************************************************************************************************










        //Einstellungen
        //*********************************************************************************************************
        //Ordner Auswahl öffnen
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnSelectFolder(object sender, RoutedEventArgs e)
        {
            //Ordner Auswahl öffnen
            GRFolder.Margin = new Thickness(0, 0, 0, 0);
            MenuOpen = true;
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Ordner aus Listbox wählen
        //-----------------------------------------------------------------------------------------------------------------
        bool folderAction = true;
        private void FolderChange(object sender, SelectionChangedEventArgs e)
        {
            if (folderAction == true)
            {
                //Frequenz setzen
                int SI = LBFolder.SelectedIndex;

                if (SI == 0)
                {
                    //Einstellung erstellen
                    SetFolder = "*";
                    CreateSettings();
                    //Button umstellen
                    BtnLockscreenFolder.Content = "(" + Lockscreen_Swap.AppResx.Random + ")";
                    //Angeben das Menü geschlossen ist
                    MenuOpen = false;
                    //Listbox schließen
                    GRFolder.Margin = new Thickness(-600, 0, 0, 0);
                    //Lockscreenbild wechseln
                    CreateImage();
                }
                else
                {
                    //Prüfen ob Bilder in Ordner vorhanden
                    IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                    string[] temp = file.GetFileNames("/Folders/" + (datalist2[SI] as ClassFolders).name + "/");
                    //Wenn Bilder in Ornder vorhanden
                    if (temp.Count() >= 1)
                    {
                        SetFolder = (datalist2[SI] as ClassFolders).name;
                        BtnLockscreenFolder.Content = SetFolder;
                        //Settings neu erstellen
                        CreateSettings();
                        //Angeben das Menü geschlossen ist
                        MenuOpen = false;
                        //Listbox schließen
                        GRFolder.Margin = new Thickness(-600, 0, 0, 0);
                        //Lockscreenbild wechseln
                        CreateImage();
                    }
                    //Wenn keine Bilder in Ordner vorhanden
                    else
                    {
                        MessageBox.Show(Lockscreen_Swap.AppResx.MsgNoPicture);
                    }
                }

                //Listbox deselectieren
                folderAction = false;
                try
                {
                    LBFolder.SelectedIndex = -1;
                }
                catch
                {
                }
                folderAction = true;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------
        //*********************************************************************************************************










        //Bild im Hintergrund wechseln
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnSettingsChanger(object sender, RoutedEventArgs e)
        {
            if (Convert.ToString(TBLockscreenChanger.Content) == Lockscreen_Swap.AppResx.On)
            {
                RemoveAgent(periodicTaskName);
            }
            else
            {
                BtnLockscreen();
            }
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Button neu erstellen
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnGenerateNew(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CreateImage();
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Button Random Pictures
        //-----------------------------------------------------------------------------------------------------------------
        private void BtnShufflePictures(object sender, RoutedEventArgs e)
        {
            if (SetRandom == 1)
            {
                SetRandom = 0;
                SetImageNow = 1;
                BtnRandom.Content = Lockscreen_Swap.AppResx.Off;
                CreateSettings();
                CreateImage();
            }
            else
            {
                SetRandom = 1;
                BtnRandom.Content = Lockscreen_Swap.AppResx.On;
                CreateSettings();
                CreateImage();
            }
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Button Lock Screen Settings
        //-----------------------------------------------------------------------------------------------------------------
        private async void BtnLockScreenSettings(object sender, RoutedEventArgs e)
        {
            var op = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Button Low Battery Warning
        //-----------------------------------------------------------------------------------------------------------------
        //Button
        private void BtnLowBatteryWarning(object sender, RoutedEventArgs e)
        {
            //Auswahl rein fahren
            GRBatteryWarning.Margin = new Thickness(0, 0, 0, 0);
            MenuOpen = true;
        }

        //Auswahl
        bool warningAction = true;
        private void BatteryWarningChange(object sender, SelectionChangedEventArgs e)
        {
            if (warningAction == true)
            {
                //Index auswählen
                int SI = LBBatteryWarning.SelectedIndex;
                
                //Auswahl erfassen
                string batteryWarning = (datalist3[SI] as ClassFolders).images;
                //Batterie Warnung umstellen
                SetBatteryWarning = Convert.ToInt32(batteryWarning);
                //Speichern
                CreateSettings();

                //Button neu einstellen
                if (SetBatteryWarning == 0)
                {
                    BtnBatteryWarning.Content = Lockscreen_Swap.AppResx.Off;

                }
                else
                {
                    BtnBatteryWarning.Content = SetBatteryWarning + " %";
                }
                
                //Menü raus fahren
                GRBatteryWarning.Margin = new Thickness(-600, 0, 0, 0);
                MenuOpen = false;

                //Listbox deselectieren
                warningAction = false;
                try
                {
                    LBBatteryWarning.SelectedIndex = -1;
                }
                catch
                {
                }
                warningAction = true;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Button Battery is Charging
        //-----------------------------------------------------------------------------------------------------------------
        private void ClickIsCharging(object sender, RoutedEventArgs e)
        {
            if (SetNoteCharging == 0)
            {
                BtnIsCharging.Content = Lockscreen_Swap.AppResx.On;
                SetNoteCharging = 1;
            }
            else
            {
                BtnIsCharging.Content = Lockscreen_Swap.AppResx.Off;
                SetNoteCharging = 0;
            }
            CreateSettings();
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Button Battery fully charged
        //-----------------------------------------------------------------------------------------------------------------
        private void ClickFullyCharged(object sender, RoutedEventArgs e)
        {
            if (SetNoteFullyCharged == 0)
            {
                BtnFullyCharged.Content = Lockscreen_Swap.AppResx.On;
                SetNoteFullyCharged = 1;
            }
            else
            {
                BtnFullyCharged.Content = Lockscreen_Swap.AppResx.Off;
                SetNoteFullyCharged = 0;
            }
            CreateSettings();
        }
        //-----------------------------------------------------------------------------------------------------------------


        //Button Note Sounds
        //-----------------------------------------------------------------------------------------------------------------
        private void ClickNoteSounds(object sender, RoutedEventArgs e)
        {
            //Sound Grid öffnen
            GRSoundSelect.Margin = new Thickness(0, 0, 0, 0);
            //Angeben das Menü offen
            MenuOpen = true;
        }

        //Button Sound Auswahl
        bool ChangeSounds = true;
        private void SoundChange(object sender, SelectionChangedEventArgs e)
        {
            if (ChangeSounds == true)
            {
                //Index auswählen
                int SI = LBSoundSelect.SelectedIndex;

                //Wenn lautlos
                if (SI == 0)
                {
                    //Benachrichtigung Sound umstellen
                    SetNoteSounds = "*";
                    BtnNoteSounds.Content = Lockscreen_Swap.AppResx.Z01_SoundSilent;
                }
                //Bei Standard Sounds
                else if (SI == 1)
                {
                    //Benachrichtigung Sound umstellen
                    SetNoteSounds = "**";
                    BtnNoteSounds.Content = Lockscreen_Swap.AppResx.Z01_SoundStandard;
                }
                //Bei Custom Souns
                else
                {
                    //Benachrichtigungs Sound umstellen
                    SetNoteSounds = (datalist4[SI] as ClassNumbers).name;
                    BtnNoteSounds.Content = SetNoteSounds;

                    //Neue Sound kopieren
                    string[] Sounds = { "BatteryFullyCharged.mp3", "BatteryIsCharging.mp3", "BatteryLow.mp3" };
                    for (int i = 0; i < Sounds.Count(); i++)
                    {
                        if (file.FileExists("Shared/ShellContent/" + Sounds[i]))
                        {
                            file.DeleteFile("Shared/ShellContent/" + Sounds[i]);
                        }
                        file.CopyFile("Sounds/" + SetNoteSounds + "/" + Sounds[i], "Shared/ShellContent/" + Sounds[i]);
                    }
                }
                //Einstellungen speichern
                CreateSettings();

                //Auswahl zurücksetzen
                ChangeSounds = false;
                try
                {
                    LBSoundSelect.SelectedIndex = -1;
                }
                catch
                {
                }
                ChangeSounds = true;

                //Grid ausblenden
                GRSoundSelect.Margin = new Thickness(-600, 0, 0, 0);
                MenuOpen = false;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------










        //About Buttons
        //*********************************************************************************************************

        //Button Buy
        //---------------------------------------------------------------------------------------------------------
        private void BtnBuy(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();
            _marketPlaceDetailTask.Show();
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Rate
        //---------------------------------------------------------------------------------------------------------
        private void BtnRate(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Other Apps
        //---------------------------------------------------------------------------------------------------------
        private void BtnOther(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MarketplaceSearchTask marketplaceSearchTask = new MarketplaceSearchTask();
            marketplaceSearchTask.SearchTerms = "xtrose";
            marketplaceSearchTask.Show();
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Facebook
        //---------------------------------------------------------------------------------------------------------
        private void BtnFacebook(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var wb = new WebBrowserTask();
            wb.URL = "http://www.facebook.com/xtrose.xtrose";
            wb.Show();
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Support
        //---------------------------------------------------------------------------------------------------------
        private void BtnSupport(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Rename Profile öffnen
            NavigationService.Navigate(new Uri("/Pages/Support.xaml", UriKind.Relative));
            /*
            EmailComposeTask emailcomposer = new EmailComposeTask();
            emailcomposer.To = "xtrose@hotmail.com";
            emailcomposer.Subject = "8.1 Screen changer Support";
            emailcomposer.Body = "";
            emailcomposer.Show();
             */
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Sprache
        //---------------------------------------------------------------------------------------------------------
        private void BtnLanguage(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Rename Profile öffnen
            NavigationService.Navigate(new Uri("/Pages/Language.xaml", UriKind.Relative));
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Instructions
        //---------------------------------------------------------------------------------------------------------
        private void BtnInstructions(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/Instructions.xaml", UriKind.Relative));
        }
        //---------------------------------------------------------------------------------------------------------
        //*********************************************************************************************************










        //Bewertungsaufruf
        //---------------------------------------------------------------------------------------------------------
        //Bewerten
        private void BtnRateRate_click(object sender, RoutedEventArgs e)
        {
            //Zu bewertungen gehen
            MarketplaceReviewTask review = new MarketplaceReviewTask();
            review.Show();
            //Rate File löschen
            file.DeleteFile("Settings/RateReminder.txt");
            //Rate verbergen
            GRRate.Margin = new Thickness(-600, 0, 0, 0);
            MenuOpen = false;
        }
        //Später
        private void BtnRateLater_click(object sender, RoutedEventArgs e)
        {
            //Rate File neu erstellen
            DateTime datetime = DateTime.Now;
            datetime = datetime.AddDays(4);
            filestream = file.CreateFile("Settings/RateReminder.txt");
            sw = new StreamWriter(filestream);
            sw.WriteLine(datetime.ToString());
            sw.Flush();
            filestream.Close();
            //Rate verbergen
            GRRate.Margin = new Thickness(-600, 0, 0, 0);
            MenuOpen = false;
        }
        //Nie
        private void BtnRateNever_click(object sender, RoutedEventArgs e)
        {
            //Rate File löschen
            file.DeleteFile("Settings/RateReminder.txt");
            //Rate verbergen
            GRRate.Margin = new Thickness(-600, 0, 0, 0);
            MenuOpen = false;
        }
        //---------------------------------------------------------------------------------------------------------









        //Benachrichtigungstöne
        //---------------------------------------------------------------------------------------------------------
        private void BtnCompSound(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/ComponentsSounds.xaml", UriKind.Relative));
        }
        //---------------------------------------------------------------------------------------------------------









        //Button Logo Start
        //---------------------------------------------------------------------------------------------------------
        private void ClickLogoStart(object sender, RoutedEventArgs e)
        {
            if (LogoStart == true)
            {
                LogoStart = false;
                CreateSettings();
                BtnLogoStart.Content = Lockscreen_Swap.AppResx.Off;
            }
            else
            {
                LogoStart = true;
                CreateSettings();
                BtnLogoStart.Content = Lockscreen_Swap.AppResx.On;
            }
        }
        //---------------------------------------------------------------------------------------------------------










        // Button Lock Screen 10
        //---------------------------------------------------------------------------------------------------------
        private void BtnLS10(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            // 10.0 Lock Screen Seite anzeigen
            NavigationService.Navigate(new Uri("/Pages/10_0_Screen_Changer.xaml", UriKind.Relative));
        }
        //---------------------------------------------------------------------------------------------------------
        
        
        
        
        
        
        
        
        
        
        //Back Button
        //---------------------------------------------------------------------------------------------------------------------------------
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //Prüfen ob Menü offen ist und alle Menüs schließen
            if (MenuOpen == true)
            {
                //Menüs schließen
                GRFolderMenu.Margin = new Thickness(-600, 0, 0, 0);
                GRFolder.Margin = new Thickness(-600, 0, 0, 0);
                GRBatteryWarning.Margin = new Thickness(-600, 0, 0, 0);
                GRSoundSelect.Margin = new Thickness(-600, 0, 0, 0);

                //Angeben das Menüs geschlossen sind
                MenuOpen = false;

                //Zurück oder beenden abbrechen
                e.Cancel = true;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------
    }
}









        










        











        