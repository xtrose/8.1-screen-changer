using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
//Zum laden von Qellcodes und zum erstellen und auslesem von Dateien
using System.IO;
//Zum erstellen und auslesem von Dateien
using System.IO.IsolatedStorage;
//Zum erweiterten schneiden von strings
using System.Text.RegularExpressions;
//Zum speichern von Bildern in den Isolated Storage
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Microsoft.Phone.Tasks;
using Microsoft.Phone;
using System.Windows.Media;
//Um auszulesen ob Handy geladen wird;
using Microsoft.Phone.Info;
//Für Listbox Update
using System.Collections.ObjectModel;
//Background Agent
using Microsoft.Phone.Scheduler;
//Für Benachrichtigung vor beenden
using System.ComponentModel;





namespace Lockscreen_Swap.Pages
{
    public partial class RenameFolder : PhoneApplicationPage
    {





        // Wird zum Start der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        public RenameFolder()
        {
            //Komponenten laden
            InitializeComponent();

            //Icons ändern
            Color backgroundColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            string tempCol = Convert.ToString(backgroundColor);
            if (tempCol != "#FF000000")
            {
                ImgTop.Source = new BitmapImage(new Uri("Images/Edit.Light.png", UriKind.Relative));
                ImgTop.Opacity = 0.1;
            }
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Allgemeine Variabeln
        //-----------------------------------------------------------------------------------------------------------------
        //IsoStore file erstellen
        IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
        //Style Name
        string FolderName;
        //-----------------------------------------------------------------------------------------------------------------





        // Wird bei jedem Aufruf der Seite geladen
        //-----------------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Variable für Ordner ermitteln
            FolderName = Convert.ToString(NavigationContext.QueryString["folder"]);
            base.OnNavigatedTo(e);

            //Style Name
            TBFolderName.Text = FolderName;
        }
        //-----------------------------------------------------------------------------------------------------------------





        //Prüfen ob Return gedrückt wurde
        //-----------------------------------------------------------------------------------------------------------------
        private void TBFolderName_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            //Prüfen ob Return gedrückt wurde
            string tempkey = Convert.ToString(e.Key);
            if (tempkey == "Enter")
            {
                if (TBFolderName.Text.Length > 0)
                {
                    //Prüfen ob leere Eingabe und zurücksetzen
                    //bool temp = Regex.IsMatch(TBFolderName.Text, @"^[a-zA-Z0-9 ]+$");
                    bool temp = true;
                    if (temp == false)
                    {
                        MessageBox.Show("ERROR!\n\nInvalid characters\nOnly a-z A-Z 0-9 ");
                        TBFolderName.Text = FolderName;
                    }
                    else
                    {
                        if (FolderName == TBFolderName.Text)
                        {
                            NavigationService.GoBack();
                        }
                        else
                        {
                            //Prüfen ob Ordner bereits besteht
                            if (!file.DirectoryExists("/Folders/" + TBFolderName.Text))
                            {
                                try
                                {
                                    file.CreateDirectory("Folders/" + TBFolderName.Text);
                                    string[] files = file.GetFileNames("/Folders/" + FolderName + "/");
                                    foreach (string file2 in files)
                                    {
                                        file.MoveFile("/Folders/" + FolderName + "/" + file2, "Folders/" + TBFolderName.Text + "/" + file2);
                                    }
                                    file.DeleteDirectory("/Folders/" + FolderName);
                                    file.CreateDirectory("Thumbs/" + TBFolderName.Text);
                                    string[] files2 = file.GetFileNames("/Thumbs/" + FolderName + "/");
                                    foreach (string file2 in files2)
                                    {
                                        file.MoveFile("/Thumbs/" + FolderName + "/" + file2, "Thumbs/" + TBFolderName.Text + "/" + file2);
                                    }
                                    file.DeleteDirectory("/Thumbs/" + FolderName);
                                    file.MoveFile("/FoldersDat/" + FolderName + ".dat", "/FoldersDat/" + TBFolderName.Text + ".dat");
                                    NavigationService.GoBack();
                                }
                                catch
                                {
                                    MessageBox.Show(Lockscreen_Swap.AppResx.ErrorName);
                                    TBFolderName.Text = FolderName;
                                }
                            }
                            //Wenn Ordner bereits besteht
                            else
                            {
                                MessageBox.Show(Lockscreen_Swap.AppResx.ErrorName);
                                TBFolderName.Text = FolderName;
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show(Lockscreen_Swap.AppResx.ErrorEnterNewName);
                    TBFolderName.Text = FolderName;
                }

                //Focus zurücksetzen
                //Focus();
            }
        }
        //---------------------------------------------------------------------------------------------------------  





        //Back Button
        //---------------------------------------------------------------------------------------------------------------------------------
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            //Prüfen ob Name anders
            if (FolderName != TBFolderName.Text & TBFolderName.Text.Length > 0)
            {
                //Speichern
                if (MessageBox.Show("", Lockscreen_Swap.AppResx.RenameFolder, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (TBFolderName.Text.Length > 0)
                    {
                        //Prüfen ob leere Eingabe und zurücksetzen
                        //bool temp = Regex.IsMatch(TBFolderName.Text, @"^[a-zA-Z0-9 ]+$");
                        bool temp = true;
                        if (temp == false)
                        {
                            MessageBox.Show("ERROR!\n\nInvalid characters\nOnly a-z A-Z 0-9 ");
                            TBFolderName.Text = FolderName;
                            //Zurück oder beenden abbrechen
                            e.Cancel = true;
                        }
                        else
                        {
                            if (FolderName == TBFolderName.Text)
                            {
                                NavigationService.GoBack();
                            }
                            else
                            {
                                //Prüfen ob Ordner bereits besteht
                                if (!file.DirectoryExists("/Folders/" + TBFolderName.Text))
                                {
                                    try
                                    {
                                        file.CreateDirectory("Folders/" + TBFolderName.Text);
                                        string[] files = file.GetFileNames("/Folders/" + FolderName + "/");
                                        foreach (string file2 in files)
                                        {
                                            file.MoveFile("/Folders/" + FolderName + "/" + file2, "Folders/" + TBFolderName.Text + "/" + file2);
                                        }
                                        file.DeleteDirectory("/Folders/" + FolderName);
                                        file.CreateDirectory("Thumbs/" + TBFolderName.Text);
                                        string[] files2 = file.GetFileNames("/Thumbs/" + FolderName + "/");
                                        foreach (string file2 in files2)
                                        {
                                            file.MoveFile("/Thumbs/" + FolderName + "/" + file2, "Thumbs/" + TBFolderName.Text + "/" + file2);
                                        }
                                        file.DeleteDirectory("/Thumbs/" + FolderName);
                                        file.MoveFile("/FoldersDat/" + FolderName + ".dat", "/FoldersDat/" + TBFolderName.Text + ".dat");
                                        NavigationService.GoBack();
                                    }
                                    catch
                                    {
                                        MessageBox.Show(Lockscreen_Swap.AppResx.ErrorName);
                                        TBFolderName.Text = FolderName;
                                        //Zurück oder beenden abbrechen
                                        e.Cancel = true;
                                    }
                                }
                                //Wenn Ordner bereits besteht
                                else
                                {
                                    MessageBox.Show(Lockscreen_Swap.AppResx.ErrorName);
                                    TBFolderName.Text = FolderName;
                                    //Zurück oder beenden abbrechen
                                    e.Cancel = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show(Lockscreen_Swap.AppResx.ErrorEnterNewName);
                        TBFolderName.Text = FolderName;
                        //Zurück oder beenden abbrechen
                        e.Cancel = true;
                    }
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------





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




    }
}