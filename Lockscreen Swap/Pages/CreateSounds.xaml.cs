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
using System.ComponentModel;





namespace Lockscreen_Swap.Pages
{





    public partial class CreateSounds : PhoneApplicationPage
    {





        //Wird am Anfang der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        public CreateSounds()
        {
            //Komponenten laden
            InitializeComponent();

            //Bilder tauschen
            Color backgroundColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            string temp = Convert.ToString(backgroundColor);
            if (temp != "#FF000000")
            {
                ImgTop.Source = new BitmapImage(new Uri("Images/Folder.Add.Light.png", UriKind.Relative));
                ImgTop.Opacity = 0.1;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Allgemeine Variabeln
        //-----------------------------------------------------------------------------------------------------------------
        //IsoStore file erstellen
        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
        //-----------------------------------------------------------------------------------------------------------------





        // Wird bei jedem Aufruf der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Info erstellen
            TBInfo.Text = Lockscreen_Swap.AppResx.Z01_CreateSoundsInfo;
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Prüfen ob Return gedrückt wurde
        //-----------------------------------------------------------------------------------------------------------------
        private void TBStyleName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Prüfen ob Name vorhanden
            if (TBStyleName.Text.Length > 0)
            {
                //Prüfen ob Return gedrückt wurde
                string tempkey = Convert.ToString(e.Key);
                if (tempkey == "Enter")
                {
                    if (TBStyleName.Text.Length > 0)
                    {
                        try
                        {
                            string NameToCreate = TBStyleName.Text;
                            NameToCreate = NameToCreate.Trim();
                            file.CreateDirectory("Sounds/ " + NameToCreate);
                            file.CopyFile("Sounds/English/BatteryLow.mp3", "Sounds/ " + NameToCreate + "/BatteryLow.mp3");
                            file.CopyFile("Sounds/English/BatteryFullyCharged.mp3", "Sounds/ " + NameToCreate + "/BatteryFullyCharged.mp3");
                            file.CopyFile("Sounds/English/BatteryIsCharging.mp3", "Sounds/ " + NameToCreate + "/BatteryIsCharging.mp3");
                            NavigationService.GoBack();
                        }
                        catch
                        {
                            MessageBox.Show(Lockscreen_Swap.AppResx.Z01_InUse);
                            TBStyleName.Text = "";
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------  





        //Back Button
        //---------------------------------------------------------------------------------------------------------------------------------
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (TBStyleName.Text.Length > 0)
            {
                //Speichern
                if (MessageBox.Show("", Lockscreen_Swap.AppResx.Z01_CreateSounds, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (TBStyleName.Text.Length > 0)
                    {
                        try
                        {
                            string NameToCreate = TBStyleName.Text;
                            NameToCreate = NameToCreate.Trim();
                            file.CreateDirectory("Sounds/ " + NameToCreate);
                            file.CopyFile("Sounds/English/BatteryLow.mp3", "Sounds/ " + NameToCreate + "/BatteryLow.mp3");
                            file.CopyFile("Sounds/English/BatteryFullyCharged.mp3", "Sounds/ " + NameToCreate + "/BatteryFullyCharged.mp3");
                            file.CopyFile("Sounds/English/BatteryIsCharging.mp3", "Sounds/ " + NameToCreate + "/BatteryIsCharging.mp3");
                            NavigationService.GoBack();
                        }
                        catch
                        {
                            MessageBox.Show(Lockscreen_Swap.AppResx.Z01_InUse);
                            TBStyleName.Text = "";
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------




    }
}