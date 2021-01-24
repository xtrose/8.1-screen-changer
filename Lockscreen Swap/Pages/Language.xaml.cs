using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Globalization;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace Lockscreen_Swap.Pages
{
    public partial class Language : PhoneApplicationPage
    {






        //Variabeln erstellen
        //---------------------------------------------------------------------------------------------------------
        string[] LangCodes = { "az-Latn-AZ", "id-ID", "ms-MY", "ca-ES", "cs-CZ", "da-DK", "de-DE", "en-GB", "en", "es-ES", "es-MX", "fil-PH", "fr-FR", "hr-HR", "it-IT", "lt-LT", "lv-LV", "hu-HU", "nl-NL", "nb-NO", "pl-PL", "pt-BR", "pt-PT", "ro-RO", "sq-AL", "fi-FI", "sk-SK", "sv-SE", "vi-VN", "tr-TR", "el-GR", "be-BY", "bg-BG", "mk-MK", "ru-RU", "he-IL", "ar-SA", "fa-IR", "hi-IN", "th-TH", "ko-KR", "zh-CN", "zh-TW", "ja-JP", "sr-Latn-CS", "uk-UA" };
        string[] LangNames = { "Azərbaycan", "Bahasa Indonesia", "Behasa Melayu", "català", "Čeština", "dansk", "deutsch", "English (United States)", "English (international)", "español (España)", "Español (México)", "Filipino", "Français", "hrvatski", "italiano", "Lietuvių", "Latviešu", "magyar", "Nederlands", "norsk", "polski", "português (Brasil)", "português (Portugal)", "română", "Shqip", "suomi", "Slovenský", "Svenska", "Tiếng Việt", "Türkçe", "Ελληνικά", "Беларуска", "Български", "македонски", "русский", "עברית", "العربية", "فارسی", "हिंदी", "ไทย", "한국어", "简体中文", "繁體中文", "日本語", "српски", "Український" };
        //Neue Datenliste erstellen //ClassStyles
        ObservableCollection<ClassLanguages> datalist = new ObservableCollection<ClassLanguages>();
        //---------------------------------------------------------------------------------------------------------










        //Wird am Anfang der Seite geladen
        //---------------------------------------------------------------------------------------------------------
        public Language()
        {
            //Komponenten laden
            InitializeComponent();

            //Hintergrundfarbe prüfen
            Color backgroundColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            string temp = Convert.ToString(backgroundColor);

            //Icons ändern
            if (temp != "#FF000000")
            {
                //Icons ändern
                ImgTop.Source = new BitmapImage(new Uri("Images/Globe.Light.png", UriKind.Relative));
                ImgTop.Opacity = 0.1;
            }

            //Sprachen erstellen
            CreateLanguages();
        }
        //---------------------------------------------------------------------------------------------------------










        //Wird am Anfang der Seite geladen
        //---------------------------------------------------------------------------------------------------------
        void CreateLanguages()
        {
            //Prüfen wieviel Sprachen
            int cLang = LangNames.Count();

            //Sprachen durchlaufen
            datalist.Clear();
            for (int i = 0; i < cLang; i++)
            {
                datalist.Add(new ClassLanguages(LangNames[i], LangCodes[i]));
            }

            //Sprachen in Listbox Setzen
            LBLangList.ItemsSource = datalist;

        }
        //---------------------------------------------------------------------------------------------------------










        //Neue Sprachdatei erstellen
        //---------------------------------------------------------------------------------------------------------
        //Variabeln
        bool SelectLang = true;
        //Aktion
        private void LocList_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            //Prüfen ob Aktion ausgefüfrt wird
            if (SelectLang == true)
            {
                //Index ermitteln
                int SI = LBLangList.SelectedIndex;

                //Code aus Array laden
                string cul = (datalist[SI] as ClassLanguages).code;
                string lang = (datalist[SI] as ClassLanguages).name;

                //Abfrage ob Sprache geändert werden soll
                if (MessageBox.Show(Lockscreen_Swap.AppResx.NoteLanguage + "\n" + lang, Lockscreen_Swap.AppResx.WARNING, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //Sprache ändern
                    CultureInfo newCulture = new CultureInfo(cul);
                    Thread.CurrentThread.CurrentUICulture = newCulture;

                    //IsoStore file erstellen
                    IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                    //Prüfen ob alte Datei vorhanden
                    if (file.FileExists("Cul.dat"))
                    {
                        file.DeleteFile("Cul.dat");
                    }
                    //Neue Datei erstellen
                    IsolatedStorageFileStream filestream = file.CreateFile("Cul.dat");
                    StreamWriter sw = new StreamWriter(filestream);
                    sw.WriteLine(Convert.ToString(cul));
                    sw.Flush();
                    filestream.Close();

                    //Lang.dat für Hintergrunddienst neu erstellen
                    string Lang = Lockscreen_Swap.AppResx.BatteryFullyCharged + ";" + Lockscreen_Swap.AppResx.BatteryIsCharging + ";" + Lockscreen_Swap.AppResx.BatteryLow + ";" + Lockscreen_Swap.AppResx.Day + ";" + Lockscreen_Swap.AppResx.Days + ";" + Lockscreen_Swap.AppResx.Hour + ";" + Lockscreen_Swap.AppResx.Hours + ";" + Lockscreen_Swap.AppResx.Minute + ";" + Lockscreen_Swap.AppResx.Minutes + ";";
                    //Sprachdatei speichern
                    if (file.FileExists("Lang.dat"))
                    {
                        file.DeleteFile("Lang.dat");
                    }
                    filestream = file.CreateFile("Lang.dat");
                    sw = new StreamWriter(filestream);
                    sw.WriteLine(Convert.ToString(Lang));
                    sw.Flush();
                    filestream.Close();

                    //Benachrichtigung ausgeben
                    MessageBox.Show(Lockscreen_Swap.AppResx.NoteLanguage2);

                    //Zurück
                    NavigationService.GoBack();
                }
            }

            //Auswahl zurücksetzen
            SelectLang = false;
            try
            {
                LBLangList.SelectedIndex = -1;
            }
            catch
            {
            }
            SelectLang = true;

        }
        //---------------------------------------------------------------------------------------------------------





    }
}