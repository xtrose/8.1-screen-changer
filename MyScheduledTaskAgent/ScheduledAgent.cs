using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MyScheduledTaskAgent;
//Zum laden von Qellcodes und zum erstellen und auslesem von Dateien
using System.IO;
//Zum erstellen und auslesem von Dateien
using System.IO.IsolatedStorage;
//Zum erweiterten schneiden von strings
using System.Text.RegularExpressions;
//Zum speichern von Bildern in den Isolated Storage
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Microsoft.Xna.Framework.Media;
using Microsoft.Phone.Tasks;
using Microsoft.Phone;
using System.Windows.Media;
//Um Battery Leistung auszulesen
using Windows.Phone.Devices.Power;

//Um auszulesen ob Handy geladen wird;
using Microsoft.Phone.Info;
//Background Agent
using Microsoft.Phone.Scheduler;










namespace MyScheduledTaskAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {










        static ScheduledAgent()
        //------------------------------------------------------------------------
        {
            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }
        //------------------------------------------------------------------------



        /// Code to execute on Unhandled Exceptions
        //------------------------------------------------------------------------
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }
        //------------------------------------------------------------------------










        //Allgemeine Variabeln
        //------------------------------------------------------------------------
        //Einstellungen
        string Settings;
        string SetFolder = "*"; //"*" Zufälliger Ordner
        int SetRandom = 1;
        int SetImageNow = 1;
        int SetBatteryWarning = 20;
        int SetNoteCharging = 1;
        int SetNoteFullyCharged = 1;
        string SetNoteSounds = "*";
        string BatteryStatus = "none";
        bool LogoStart = true;

        //Sprachvariabeln
        string BatteryFullyCharged = "Battery fully charged";
        string BatteryIsCharging = "Battery is charging";
        string BatteryLow = "BatteryLow";
        string Day = "day";
        string Days = "days";
        string Hour = "hour";
        string Hours = "hours";
        string Minute = "minute";
        string Minutes = "minutes";

        //Battery Level
        int BatteryLevel = Battery.GetDefault().RemainingChargePercent;
        string PowerSource = DeviceStatus.PowerSource.ToString();

        //Anzahl erstellter Bilder
        int SwapImage = 0;

        //Prüfen ob Lockscreenapp
        bool IsLockscreenApp = false;
        //Prüfen ob Vollversion
        bool FullVersion = false;
        //Gibt an ob Bild im Trial erstellt wird
        bool Run = true;

        //Startzeit
        DateTime dt = DateTime.Now;
        //Aktuelle Zeit
        DateTime dt_Now = DateTime.Now;

        //IsoStore file erstellen
        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();


        //Toast Daten erstellen
        // Set the minimum version number that supports custom toast sounds
        private static Version TargetVersion = new Version(8, 0, 10492);

        // Function to determine if the current device is running the target version.
        public static bool IsTargetedVersion { get { return Environment.OSVersion.Version >= TargetVersion; } }

        // Function for setting a property value using reflection.
        private static void SetProperty(object instance, string name, object value)
        {
            var setMethod = instance.GetType().GetProperty(name).GetSetMethod();
            setMethod.Invoke(instance, new object[] { value });
        }
        //------------------------------------------------------------------------










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










        //Eigentlicher Task
        //--------------------------------------------------------------------------------------------------------------------
        protected override void OnInvoke(ScheduledTask task)
        {
            
            //PeriodicTask, Periodischer Task
            //****************************************************************************************************
            if (task is PeriodicTask)
            {
                try
                {
                    //Einstellungen laden
                    //------------------------------------------------------------------------
                    //First Time// DateTime der ersten installation
                    IsolatedStorageFileStream filestream = file.OpenFile("Settings/FirstTime.dat", FileMode.Open);
                    StreamReader sr = new StreamReader(filestream);
                    string temp = sr.ReadToEnd();
                    filestream.Close();
                    dt = Convert.ToDateTime(temp);

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

                    //Einstellungen laden
                    filestream = file.OpenFile("Settings/Settings.dat", FileMode.Open);
                    sr = new StreamReader(filestream);
                    Settings = sr.ReadToEnd();
                    filestream.Close();

                    //Einstellungen umsetzen
                    string[] SplitSettings = Regex.Split(Settings, ";");
                    //Einstellungen Ordner
                    SetFolder = SplitSettings[0];
                    //Einstellungen Zufällige Wiedergabe
                    SetRandom = Convert.ToInt32(SplitSettings[1]);
                    //Momentan verwendetes Bild
                    SetImageNow = Convert.ToInt32(SplitSettings[2]);
                    //Batterie Warning
                    SetBatteryWarning = Convert.ToInt32(SplitSettings[3]);
                    //Note Battery Charging
                    SetNoteCharging = Convert.ToInt32(SplitSettings[4]);
                    //Note fully Charged
                    SetNoteFullyCharged = Convert.ToInt32(SplitSettings[5]);
                    //Note Sounds
                    SetNoteSounds = SplitSettings[6];
                    BatteryStatus = SplitSettings[7];
                    //SetNoteSounds abspielen
                    LogoStart = Convert.ToBoolean(SplitSettings[8]);

                    //Sprachdatei laden
                    filestream = file.OpenFile("Lang.dat", FileMode.Open);
                    sr = new StreamReader(filestream);
                    string lang = sr.ReadToEnd();
                    filestream.Close();
                    string[] langSplit = Regex.Split(lang, ";");
                    //Sprachdatei umwandeln
                    BatteryFullyCharged = langSplit[0];
                    BatteryIsCharging = langSplit[1];
                    BatteryLow = langSplit[2];
                    Day = langSplit[3];
                    Days = langSplit[4];
                    Hour = langSplit[5];
                    Hours = langSplit[6];
                    Minute = langSplit[7];
                    Minutes = langSplit[8];


                    //Bei Vollversion
                    if (FullVersion == true)
                    {
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
                        }
                    }
                    //------------------------------------------------------------------------


                    //Bild neu erstellen
                    //------------------------------------------------------------------------
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
                            filestream = file.OpenFile("FoldersDat/" + SetFolder + ".dat", FileMode.Open);
                            sr = new StreamReader(filestream);
                            temp = sr.ReadToEnd();
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
                                filestream = file.OpenFile("FoldersDat/" + FoldersSplit[i], FileMode.Open);
                                sr = new StreamReader(filestream);
                                temp = sr.ReadToEnd();
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
                            filestream = file.OpenFile("Settings/SwapImage.dat", FileMode.Open);
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
                            filestream = file.OpenFile("Settings/SwapImage.dat", FileMode.Open);
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
                            Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);
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
                        filestream = file.OpenFile("Settings/SwapImage.dat", FileMode.Open);
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
                        Windows.Phone.System.UserProfile.LockScreen.SetImageUri(uri);
                    }
                }
                catch
                {
                    file.CreateDirectory("Dreck geht nicht");
                }
                //------------------------------------------------------------------------





                //Benachrichtigungen erstellen
                //------------------------------------------------------------------------
                string ToastNote = "none";
                string ToastTitle = "";
                string ToastContent = "";

                bool doSilentToast = false;
                if (SetNoteSounds == "*")
                {
                    doSilentToast = true;
                }
                bool CustomSounds = true;
                if (SetNoteSounds == "**")
                {
                    CustomSounds = false;
                }


                if (Run == true)
                {
                    //Benachrichtigung das Battery voll aufgeladen ist
                    if (BatteryLevel == 100 & PowerSource == "External" & BatteryStatus != "full")
                    {
                        //Batterie Status neu erstellen
                        BatteryStatus = "full";
                        CreateSettings();
                        //Toast Variabeln erstellen
                        ToastNote = "BatteryFullyCharged";
                        ToastTitle = BatteryFullyCharged;
                        ToastContent = BatteryLevel + " %";
                    }

                    //Benachrichtigung wenn Battery geladen wird
                    else if (PowerSource == "External" & BatteryStatus != "loading" & BatteryStatus != "full")
                    {
                        //Batterie Status neu erstellen
                        BatteryStatus = "loading";
                        CreateSettings();
                        //Toast Variabeln erstellen
                        ToastNote = "BatteryIsCharging";
                        ToastTitle = BatteryIsCharging;
                        ToastContent = BatteryLevel + " %";
                    }

                    //Benachrichtigen wenn Battery schwach
                    else if (SetBatteryWarning >= BatteryLevel & BatteryStatus != "loading")
                    {
                        //Toast Variabeln erstellen
                        ToastNote = "BatteryLow";
                        ToastTitle = BatteryLow;
                        ToastContent = BatteryLevel + " %";
                    }

                    //Wenn keine Benachrichtigung
                    else if (PowerSource != "External")
                    {
                        //Batterie Status neu erstellen
                        BatteryStatus = "none";
                        CreateSettings();
                    }

                    //Benachrichtigung ausgeben
                    if ((ToastNote == "BatteryFullyCharged" & SetNoteFullyCharged == 1) | (ToastNote == "BatteryIsCharging" & SetNoteCharging == 1) | (ToastNote == "BatteryLow"))
                    {
                        ShowToast(ToastNote, ToastTitle, ToastContent, CustomSounds, doSilentToast);
                    }
                }
                //------------------------------------------------------------------------


                //Iconic Tile updaten
                //-----------------------------------------------------------------------------------------------------------------
                //Wenn Anzeige ausgeführt wird
                if (Run == true)
                {
                    try
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
                                tempText += "20 " + Days + ", ";
                            }
                            else
                            {
                                if (tempDays == 1)
                                {
                                    tempText += "1 " + Day + ", ";
                                }
                                else
                                {
                                    tempText += tempDays + " " + Days + ", ";
                                }
                            }
                        }
                        if (tempHours != 0)
                        {
                            if (tempHours == 1)
                            {
                                tempText += "1 " + Hour + ", ";
                            }
                            else
                            {
                                tempText += tempHours + " " + Hours + ", ";
                            }
                        }
                        else
                        {
                            if (tempDays != 0)
                            {
                                tempText += "0 " + Hours + ", ";
                            }
                        }
                        if (tempMinutes == 1)
                        {
                            tempText += "1 " + Minute;
                        }
                        else
                        {
                            tempText += tempMinutes + " " + Minutes;
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
                    }
                    catch
                    {
                    }
                }
                //Wenn Anzeige nicht ausgeführt wird
                else
                {
                    try
                    {
                        //Tile neu erstellen
                        ShellTile tile = ShellTile.ActiveTiles.First();
                        IconicTileData TileData = new IconicTileData()
                        {
                            //Title = TileTitle,
                            //WideBackContent = "[back of wide Tile size content]",
                            Count = 0,
                            //SmallBackgroundImage = new Uri(tempUri, UriKind.Absolute),
                            //BackgroundImage = new Uri(tempUri, UriKind.Absolute),
                            //BackBackgroundImage = [back of medium Tile size URI],
                            //WideBackgroundImage = [front of wide Tile size URI],
                            //WideBackBackgroundImage = [back of wide Tile size URI],
                        };
                        tile.Update(TileData);
                    }
                    catch
                    {
                    }
                }
                //-----------------------------------------------------------------------------------------------------------------
            }
            //****************************************************************************************************

            //ScheduledActionService.LaunchForTest("PeriodicAgent", TimeSpan.FromSeconds(60));
            NotifyComplete();
        }










        //Toast erstellen
        //---------------------------------------------------------------------------------------------------------
        public void ShowToast(string ToastNote, string ToastTitle, string ToastContent, bool useCustomSound, bool doSilentToast)
        {
            //Toast erstellen
            ShellToast toast = new ShellToast();
            toast.Title = ToastTitle;
            toast.Content = ToastContent;


            //Bei benutzerdefinierten Tönen
            if (useCustomSound == true)
            {
                //If the device is running the right version and a custom sound is requested
                if ((IsTargetedVersion) && (useCustomSound) & doSilentToast == false)
                {
                    //Prüfen was für ein Sound ausgegeben wird
                    if (ToastNote == "BatteryLow")
                    {
                        SetProperty(toast, "Sound", new Uri("isostore:/Shared/ShellContent/BatteryLow.mp3", UriKind.RelativeOrAbsolute));
                    }
                    else if (ToastNote == "BatteryFullyCharged")
                    {
                        SetProperty(toast, "Sound", new Uri("isostore:/Shared/ShellContent/BatteryFullyCharged.mp3", UriKind.RelativeOrAbsolute));
                    }
                    else
                    {
                        SetProperty(toast, "Sound", new Uri("isostore:/Shared/ShellContent/BatteryIsCharging.mp3", UriKind.Absolute));
                    }
                }
                // For a silent toast, check the version and then set the Sound property to an empty string.
                else if ((IsTargetedVersion) && (doSilentToast))
                {
                    //Do the reflection to get the new Sound property added to the toast
                    SetProperty(toast, "Sound", new Uri("", UriKind.RelativeOrAbsolute));
                }
            }

            //Bei Standard Tönen
            else
            {
                //Wenn kein Ton abgespielt wird
                if ((IsTargetedVersion) && (doSilentToast))
                {
                    //Do the reflection to get the new Sound property added to the toast
                    SetProperty(toast, "Sound", new Uri("", UriKind.RelativeOrAbsolute));
                }
            }


            //Toast ausgeben
            toast.Show();
        }
        //---------------------------------------------------------------------------------------------------------
    }
}