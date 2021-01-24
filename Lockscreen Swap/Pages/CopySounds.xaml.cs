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





    public partial class CopySounds : PhoneApplicationPage
    {





        // Wird zum Start der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        public CopySounds()
        {
            //Komponenten laden
            InitializeComponent();

            //Infotext erstellen
            TBInfo.Text = Lockscreen_Swap.AppResx.Z01_CopySoundInfo;

            //Bilder tauschen
            Color backgroundColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            string temp = Convert.ToString(backgroundColor);
            if (temp != "#FF000000")
            {
                ImgTop.Source = new BitmapImage(new Uri("Images/Copy.Light.png", UriKind.Relative));
                ImgTop.Opacity = 0.1;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Allgemeine Variabeln
        //-----------------------------------------------------------------------------------------------------------------
        //IsoStore file erstellen
        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
        //Style Name
        string StyleName;
        string OldName;
        //-----------------------------------------------------------------------------------------------------------------





        // Wird bei jedem Aufruf der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Variable für Ordner ermitteln
            StyleName = Convert.ToString(NavigationContext.QueryString["style"]);
            base.OnNavigatedTo(e);

            //Alten Name erstellen
            OldName = StyleName;
            StyleName = StyleName.Trim();

            //Style Name
            TBStyleName.Text = StyleName;

        }
        //-----------------------------------------------------------------------------------------------------------------





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
                    try
                    {
                        string NameToCreate = TBStyleName.Text;
                        NameToCreate = NameToCreate.Trim();
                        file.CreateDirectory("Sounds/ " + NameToCreate);
                        file.CopyFile("Sounds/" + OldName + "/BatteryLow.mp3", "Sounds/ " + NameToCreate + "/BatteryLow.mp3");
                        file.CopyFile("Sounds/" + OldName + "/BatteryIsCharging.mp3", "Sounds/ " + NameToCreate + "/BatteryIsCharging.mp3");
                        file.CopyFile("Sounds/" + OldName + "/BatteryFullyCharged.mp3", "Sounds/ " + NameToCreate + "/BatteryFullyCharged.mp3");
                        NavigationService.GoBack();
                    }
                    catch
                    {
                        MessageBox.Show(Lockscreen_Swap.AppResx.Z01_InUse);
                        TBStyleName.Text = StyleName;
                    }
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------  





        //Back Button
        //---------------------------------------------------------------------------------------------------------------------------------
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //Prüfen ob Name anders
            if (StyleName != TBStyleName.Text)
            {
                //Speichern
                if (MessageBox.Show("", Lockscreen_Swap.AppResx.Z01_CopySounds, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (TBStyleName.Text.Length > 0)
                    {
                        if (StyleName == TBStyleName.Text)
                        {
                            NavigationService.GoBack();
                        }
                        else
                        {
                            try
                            {
                                string NameToCreate = TBStyleName.Text;
                                NameToCreate = NameToCreate.Trim();
                                file.CreateDirectory("Sounds/ " + NameToCreate);
                                file.CopyFile("Sounds/" + OldName + "/BatteryLow.mp3", "Sounds/ " + NameToCreate + "/BatteryLow.mp3");
                                file.CopyFile("Sounds/" + OldName + "/BatteryIsCharging.mp3", "Sounds/ " + NameToCreate + "/BatteryIsCharging.mp3");
                                file.CopyFile("Sounds/" + OldName + "/BatteryFullyCharged.mp3", "Sounds/ " + NameToCreate + "/BatteryFullyCharged.mp3");
                                NavigationService.GoBack();
                            }
                            catch
                            {
                                MessageBox.Show(Lockscreen_Swap.AppResx.Z01_InUse);
                                TBStyleName.Text = StyleName;
                            }
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------




    }
}