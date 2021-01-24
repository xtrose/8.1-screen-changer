﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using System.Windows.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System.IO;
using System.ComponentModel;
using System.Windows.Navigation;
using System.IO.IsolatedStorage;
using System.Windows.Threading;





namespace Lockscreen_Swap.Pages
{
    public partial class ImagesImporter : PhoneApplicationPage
    {





        //Allgemeine Variabeln
        //---------------------------------------------------------------------------------------------------------
        //Allgemeine Variabeln
        string Folder;
        int ImageCount = 0;
        int ImagesAll = 0;
        //Bild Area Variabeln
        int imgGes = 0;
        int imgStart = 0;
        int imgEnd = 0;
        int imgArea = 0;
        int imgAreaGes = 0;
        string imgPictures = "all";

        //Neue Datenliste erstellen
        ObservableCollection<ClassMediaImage> datalist = new ObservableCollection<ClassMediaImage>();

        //Timer für die Animation
        DispatcherTimer dtt = new DispatcherTimer();
        //---------------------------------------------------------------------------------------------------------





        //Wird am Start der Seite geladen
        //---------------------------------------------------------------------------------------------------------
        public ImagesImporter()
        {
            //Komponenten laden
            InitializeComponent();

            //Bilder ändern
            Color backgroundColor = (Color)Application.Current.Resources["PhoneBackgroundColor"];
            string temp = Convert.ToString(backgroundColor);
            if (temp != "#FF000000")
            {
                ImgTop.Source = new BitmapImage(new Uri("Images/Cut.Light.png", UriKind.Relative));
                ImgTop.Opacity = 0.1;
            }

            //Timer erstellen
            dtt.Stop();
            dtt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dtt.Tick += dt_Tick;
        }
        //---------------------------------------------------------------------------------------------------------





        //Wird bei jedem Aufruf der Seite ausgeführt
        //---------------------------------------------------------------------------------------------------------------------------------
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //Variable für Ordner ermitteln
            Folder = Convert.ToString(NavigationContext.QueryString["folder"]);
            base.OnNavigatedTo(e);

            //File erstellen
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();

            //ImageCount laden //Anzahl erstellter Bilder
            FileStream filestream = file.OpenFile("Settings/ImageCount.dat", FileMode.Open);
            StreamReader sr = new StreamReader(filestream);
            String tempSr = sr.ReadToEnd();
            filestream.Close();
            ImageCount = Convert.ToInt32(tempSr);

            //Images.dat laden
            filestream = file.OpenFile("FoldersDat/" + Folder + ".dat", FileMode.Open);
            sr = new StreamReader(filestream);
            string temp = sr.ReadToEnd();
            filestream.Close();
            ImagesAll = Convert.ToInt32(temp);
            
            //Bilder laden
            GetImages("first");
        }
        //---------------------------------------------------------------------------------------------------------------------------------





        //Bilder in Listbox laden
        //---------------------------------------------------------------------------------------------------------
        void GetImages(string action)
        {
            //Listbox leeren
            datalist.Clear();
            
            //Wenn alle Bilder ausgewählt werden
            if (action == "allPictures")
            {
                imgPictures = "all";
                action = "first";
            }

            //Wenn gespeicherte Bilder ausgewählt werden
            if (action == "savedPictures")
            {
                imgPictures = "saved";
                action = "first";
            }

            //Bei allen Bildern
            if (imgPictures == "all")
            {
                //Bei allen Bildern
                MediaLibrary mediaLibrary = new MediaLibrary();
                var pictures = mediaLibrary.Pictures;

                //Beim ersten laden
                if (action == "first")
                {
                    //Variabeln erstellen
                    imgGes = pictures.Count;
                    imgStart = 0;
                    if (imgGes >= (imgStart + 199))
                    {
                        imgEnd = imgStart + 199;
                    }
                    else
                    {
                        imgEnd = (pictures.Count - 1);
                    }
                    imgArea = 1;
                    imgAreaGes = (imgGes / 200) + 1;
                }

                //Wenn nächste
                if (action == "next")
                {
                    //Prüfen ob möglich
                    if (imgArea < imgAreaGes)
                    {
                        imgArea++;
                        imgStart = imgStart + 200;
                        if (imgGes >= (imgStart + 199))
                        {
                            imgEnd = imgStart + 199;
                        }
                        else
                        {
                            imgEnd = (pictures.Count - 1);
                        }
                    }
                }
                
                //Wenn vorherige
                if (action == "previous")
                {
                    //Prüfen ob möglich
                    if (imgArea > 1)
                    {
                        imgArea--;
                        imgStart = imgStart - 200;
                        if (imgGes >= (imgStart + 199))
                        {
                            imgEnd = imgStart + 199;
                        }
                        else
                        {
                            imgEnd = (pictures.Count - 1);
                        }
                    }
                }

                //Bilder auslesen und in ListBox schreiben
                for (int i = imgStart; i <= imgEnd; i++)
                {
                    try
                    {
                        BitmapImage image = new BitmapImage();
                        image.SetSource(pictures[i].GetThumbnail());
                        datalist.Add(new ClassMediaImage((i), image));
                    }
                    catch
                    {
                    }
                }
            }

            //Bei gespeicherten Bildern
            else
            {
                //Bei saved Pictures
                MediaLibrary mediaLibrary = new MediaLibrary();
                var pictures = mediaLibrary.SavedPictures;

                //Beim ersten laden
                if (action == "first")
                {
                    //Variabeln erstellen
                    imgGes = pictures.Count;
                    imgStart = 0;
                    if (imgGes >= (imgStart + 199))
                    {
                        imgEnd = imgStart + 199;
                    }
                    else
                    {
                        imgEnd = (pictures.Count - 1);
                    }
                    imgArea = 1;
                    imgAreaGes = (imgGes / 200) + 1;
                }

                //Wenn nächste
                if (action == "next")
                {
                    //Prüfen ob möglich
                    if (imgArea < imgAreaGes)
                    {
                        imgArea++;
                        imgStart = imgStart + 200;
                        if (imgGes >= (imgStart + 199))
                        {
                            imgEnd = imgStart + 199;
                        }
                        else
                        {
                            imgEnd = (pictures.Count - 1);
                        }
                    }
                }

                //Wenn vorherige
                if (action == "previous")
                {
                    //Prüfen ob möglich
                    if (imgArea > 1)
                    {
                        imgArea--;
                        imgStart = imgStart - 200;
                        if (imgGes >= (imgStart + 199))
                        {
                            imgEnd = imgStart + 199;
                        }
                        else
                        {
                            imgEnd = (pictures.Count - 1);
                        }
                    }
                }

                //Bilder auslesen und in ListBox schreiben
                for (int i = imgStart; i <= imgEnd; i++)
                {
                    try
                    {
                        BitmapImage image = new BitmapImage();
                        image.SetSource(pictures[i].GetThumbnail());
                        datalist.Add(new ClassMediaImage((i), image));
                    }
                    catch
                    {
                    }
                }
            }

            //Daten in Listbox schreiben
            LBImages.ItemsSource = datalist;

            //AppBar erstellen
            CreateAppBar();
        }
        //---------------------------------------------------------------------------------------------------------





        //AppBar erstellen //Haupt Panel
        //---------------------------------------------------------------------------------------------------------
        void CreateAppBar()
        {
            //neue AppBar anlegen
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;

            //IDs zum festlegen der Aktionen erstellen
            int buttonID = 0;
            int itemID = 0;

            //AppBar //Items //Select erstellen
            ApplicationBarMenuItem item0 = new ApplicationBarMenuItem(Lockscreen_Swap.AppResx.Select);
            ApplicationBar.MenuItems.Add(item0);
            (ApplicationBar.MenuItems[itemID] as ApplicationBarMenuItem).Click += BtnSelect;
            itemID++;


            //Button zurück anlegen
            if (imgArea > 1)
            {
                ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/Images/appbar.arrow.left.png", UriKind.Relative));
                button1.Text = Lockscreen_Swap.AppResx.Previous;
                ApplicationBar.Buttons.Add(button1);
                (ApplicationBar.Buttons[buttonID] as ApplicationBarIconButton).Click += BtnPrevious;
                buttonID++;

                ApplicationBarMenuItem item1 = new ApplicationBarMenuItem(Lockscreen_Swap.AppResx.Previous);
                ApplicationBar.MenuItems.Add(item1);
                (ApplicationBar.MenuItems[itemID] as ApplicationBarMenuItem).Click += BtnPrevious;
                itemID++;
            }

            //Button select anlegen
            ApplicationBarIconButton button2 = new ApplicationBarIconButton(new Uri("/Images/appbar.add.multiple.png", UriKind.Relative));
            button2.Text = Lockscreen_Swap.AppResx.Select;
            ApplicationBar.Buttons.Add(button2);
            (ApplicationBar.Buttons[buttonID] as ApplicationBarIconButton).Click += BtnSelect;
            buttonID++;

            //Button zurück anlegen
            if (imgArea < imgAreaGes)
            {
                ApplicationBarIconButton button3 = new ApplicationBarIconButton(new Uri("/Images/appbar.arrow.right.png", UriKind.Relative));
                button3.Text = Lockscreen_Swap.AppResx.Next;
                ApplicationBar.Buttons.Add(button3);
                (ApplicationBar.Buttons[buttonID] as ApplicationBarIconButton).Click += BtnNext;
                buttonID++;

                ApplicationBarMenuItem item3 = new ApplicationBarMenuItem(Lockscreen_Swap.AppResx.Next);
                ApplicationBar.MenuItems.Add(item3);
                (ApplicationBar.MenuItems[itemID] as ApplicationBarMenuItem).Click += BtnNext;
                itemID++;
            }

            //Item Alle Bilder
            ApplicationBarMenuItem item4 = new ApplicationBarMenuItem(Lockscreen_Swap.AppResx.AllPictures);
            ApplicationBar.MenuItems.Add(item4);
            (ApplicationBar.MenuItems[itemID] as ApplicationBarMenuItem).Click += BtnAllPictures;
            itemID++;

            //Item Gespeicherte Bilder
            ApplicationBarMenuItem item5 = new ApplicationBarMenuItem(Lockscreen_Swap.AppResx.SavedPictures);
            ApplicationBar.MenuItems.Add(item5);
            (ApplicationBar.MenuItems[itemID] as ApplicationBarMenuItem).Click += BtnSavedPictures;
            itemID++;
        }
        //---------------------------------------------------------------------------------------------------------





        //AppBar erstellen //Cut Panel
        //---------------------------------------------------------------------------------------------------------
        void CreateAppBar2()
        {
            //neue AppBar anlegen
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = true;
            ApplicationBar.IsVisible = true;

            //IDs zum festlegen der Aktionen erstellen
            int buttonID = 0;
            int itemID = 0;

            //Button cut anlegen
            ApplicationBarIconButton button1 = new ApplicationBarIconButton(new Uri("/Images/appbar.cut.png", UriKind.Relative));
            button1.Text = Lockscreen_Swap.AppResx.Cut;
            ApplicationBar.Buttons.Add(button1);
            (ApplicationBar.Buttons[buttonID] as ApplicationBarIconButton).Click += BtnCut;
            buttonID++;

            ApplicationBarMenuItem item1 = new ApplicationBarMenuItem(Lockscreen_Swap.AppResx.Cut);
            ApplicationBar.MenuItems.Add(item1);
            (ApplicationBar.MenuItems[itemID] as ApplicationBarMenuItem).Click += BtnCut;
            itemID++;

            if (ImagesToAdd_c > 1)
            {
                //Button cut anlegen
                ApplicationBarIconButton button2 = new ApplicationBarIconButton(new Uri("/Images/appbar.cut.auto.png", UriKind.Relative));
                button2.Text = Lockscreen_Swap.AppResx.CutAuto;
                ApplicationBar.Buttons.Add(button2);
                (ApplicationBar.Buttons[buttonID] as ApplicationBarIconButton).Click += BtnAutoCut;
                buttonID++;

                ApplicationBarMenuItem item2 = new ApplicationBarMenuItem(Lockscreen_Swap.AppResx.CutAuto);
                ApplicationBar.MenuItems.Add(item2);
                (ApplicationBar.MenuItems[itemID] as ApplicationBarMenuItem).Click += BtnAutoCut;
                itemID++;
            }

            //AppBar Button drehen links
            ApplicationBarIconButton button3 = new ApplicationBarIconButton(new Uri("/Images/appbar.rotate.counterclockwise.png", UriKind.Relative));
            button3.Text = Lockscreen_Swap.AppResx.Left;
            ApplicationBar.Buttons.Add(button3);
            ApplicationBarMenuItem item3 = new ApplicationBarMenuItem(Lockscreen_Swap.AppResx.RotateLeft);
            ApplicationBar.MenuItems.Add(item3);
            (ApplicationBar.Buttons[buttonID] as ApplicationBarIconButton).Click += BtnRotateLeft;
            (ApplicationBar.MenuItems[itemID] as ApplicationBarMenuItem).Click += BtnRotateLeft;
            buttonID++;
            itemID++;

            //AppBar Button drehen rechts
            ApplicationBarIconButton button4 = new ApplicationBarIconButton(new Uri("/Images/appbar.rotate.clockwise.png", UriKind.Relative));
            button4.Text = Lockscreen_Swap.AppResx.Right;
            ApplicationBar.Buttons.Add(button4);
            ApplicationBarMenuItem item4 = new ApplicationBarMenuItem(Lockscreen_Swap.AppResx.RotateRight);
            ApplicationBar.MenuItems.Add(item4);
            (ApplicationBar.Buttons[buttonID] as ApplicationBarIconButton).Click += BtnRotateRight;
            (ApplicationBar.MenuItems[itemID] as ApplicationBarMenuItem).Click += BtnRotateRight;
            buttonID++;
            itemID++;
        }
        //---------------------------------------------------------------------------------------------------------





        //AppBar entfernen
        //---------------------------------------------------------------------------------------------------------
        //neue AppBar anlegen
        void DeleteAppBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.IsMenuEnabled = false;
            ApplicationBar.IsVisible = false;
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Select
        //---------------------------------------------------------------------------------------------------------
        //Variablen erstellen
        bool MenuOpen = false;
        int ImagesToAdd_c;
        int[] ImagesToAdd;
        int ImageNow = 0;
        //Aktion ausführen
        private void BtnSelect(object sender, EventArgs e)
        {
            //Errechnen wie viele Bilder ausgewählt sind
            ImagesToAdd_c = LBImages.SelectedItems.Count;

            //Prüfen ob ein oder mehrere Bilder ausgewählt sind
            if (ImagesToAdd_c >= 1)
            {
                //Momenatane Bild zurücksetzen
                ImageNow = 0;

                //BilderListe erstellen
                ImagesToAdd = new int[ImagesToAdd_c];
                for (int i = 0; i < ImagesToAdd_c; i++)
                {
                    ImagesToAdd[i] = (LBImages.SelectedItems[i] as ClassMediaImage).imgID;
                }

                //Cut Panel aufrufen
                CutRoot.Visibility = System.Windows.Visibility.Visible;
                CutRoot.Margin = new Thickness(0, 0, 0, 0);
                MenuOpen = true;

                //Bild in Cut Panel laden
                LoadImageToPanel();

                //AppBar neu erstellen
                CreateAppBar2();
            }

            //Wenn keiner Bilder ausgewählt sind
            else
            {
                MessageBox.Show(Lockscreen_Swap.AppResx.SelectImages);
            }
        }
        //---------------------------------------------------------------------------------------------------------





        //Nächstes Bild in Cut Panel laden
        //---------------------------------------------------------------------------------------------------------
        //Function Double Runden
        static int MyRound(double d)
        {
            return (int)(d + 0.5);
        }

        //Bild Variabeln
        WriteableBitmap tempBitmap = new WriteableBitmap(0, 0);
        WriteableBitmap endBitmap = new WriteableBitmap(0, 0);
        double nWidth;
        double nHeight;
        double minWidth = 240;
        double minHeight = 427;

        //Aktion ausführen
        void LoadImageToPanel()
        {
            //Verschiebung zurücksetzen
            transform.TranslateX = 0;
            transform.TranslateY = 0;

            //Bei allen Bildern
            if (imgPictures == "all")
            {
                //Bei allen Bildern
                MediaLibrary mediaLibrary = new MediaLibrary();
                var pictures = mediaLibrary.Pictures;

                //Bilder auslesen und in Cut Panel schreiben
                try
                {
                    //Bild in Temp Bitmap laden
                    tempBitmap.SetSource(pictures[ImagesToAdd[ImageNow]].GetImage());

                    //Errechnen ob Höhe Größer als 427px wenn breite 240px
                    double Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelWidth) * minWidth;
                    double tempHeight = Convert.ToDouble(tempBitmap.PixelHeight) / Convert.ToDouble(100) * Prozent;

                    //Wenn Höhe Größer als 427px
                    if (tempHeight >= minHeight)
                    {
                        //Neue Größe erstellen
                        nWidth = minWidth;
                        nHeight = tempHeight;
                    }

                    //Wenn Höhe nicht Größer als 427px
                    else
                    {
                        //Neue Größe erstellen
                        nHeight = minHeight;
                        Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelHeight) * minHeight;
                        nWidth = Convert.ToDouble(tempBitmap.PixelWidth) / Convert.ToDouble(100) * Prozent;
                    }

                    //Bild, Größe ändern
                    tempBitmap = tempBitmap.Resize(Convert.ToInt32(nWidth), Convert.ToInt32(nHeight), WriteableBitmapExtensions.Interpolation.Bilinear);

                    //Bild in Cut Bild ausgeben
                    CutImage.Height = nHeight;
                    CutImage.Width = nWidth;
                    CutImage.Source = tempBitmap;

                    //Endgültiges Bild in Writeable Bitmap schreiben
                    endBitmap.SetSource(pictures[ImagesToAdd[ImageNow]].GetImage());
                }
                catch
                {
                }
            }

            //Bei gespeicherten Bildern
            else
            {
                //Bei saved Pictures
                MediaLibrary mediaLibrary = new MediaLibrary();
                var pictures = mediaLibrary.SavedPictures;

                //Bilder auslesen und in Cut Panel schreiben
                try
                {
                    //Bild in Temp Bitmap laden
                    tempBitmap.SetSource(pictures[ImagesToAdd[ImageNow]].GetImage());

                    //Errechnen ob Höhe Größer als 427px wenn breite 240px
                    double Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelWidth) * minWidth;
                    double tempHeight = Convert.ToDouble(tempBitmap.PixelHeight) / Convert.ToDouble(100) * Prozent;

                    //Wenn Höhe Größer als 427px
                    if (tempHeight >= minHeight)
                    {
                        //Neue Größe erstellen
                        nWidth = minWidth;
                        nHeight = tempHeight;
                    }

                    //Wenn Höhe nicht Größer als 427px
                    else
                    {
                        //Neue Größe erstellen
                        nHeight = minHeight;
                        Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelHeight) * minHeight;
                        nWidth = Convert.ToDouble(tempBitmap.PixelWidth) / Convert.ToDouble(100) * Prozent;
                    }

                    //Bild, Größe ändern
                    tempBitmap = tempBitmap.Resize(Convert.ToInt32(nWidth), Convert.ToInt32(nHeight), WriteableBitmapExtensions.Interpolation.Bilinear);

                    //Bild in Cut Bild ausgeben
                    CutImage.Height = nHeight;
                    CutImage.Width = nWidth;
                    CutImage.Source = tempBitmap;

                    //Endgültiges Bild in Writeable Bitmap schreiben
                    endBitmap.SetSource(pictures[ImagesToAdd[ImageNow]].GetImage());
                }
                catch
                {
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Zurück
        //---------------------------------------------------------------------------------------------------------
        private void BtnPrevious(object sender, EventArgs e)
        {
            GetImages("previous");
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Next
        //---------------------------------------------------------------------------------------------------------
        private void BtnNext(object sender, EventArgs e)
        {
            GetImages("next");
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Alle Bilder
        //---------------------------------------------------------------------------------------------------------
        private void BtnAllPictures(object sender, EventArgs e)
        {
            GetImages("allPictures");
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Gespeicherte Bilder
        //---------------------------------------------------------------------------------------------------------
        private void BtnSavedPictures(object sender, EventArgs e)
        {
            GetImages("savedPictures");
        }
        //---------------------------------------------------------------------------------------------------------





        //Button Schneiden
        //---------------------------------------------------------------------------------------------------------
        private void BtnCut(object sender, EventArgs e)
        {
            //Endgültiges Bild verkleinern wenn zu groß
            if (nWidth == minWidth & Convert.ToDouble(endBitmap.PixelWidth) > Convert.ToDouble(1080))
            {
                double Prozent;
                Prozent = Convert.ToDouble(100) / Convert.ToDouble(endBitmap.PixelWidth) * Convert.ToDouble(1080);
                double tHeight = Convert.ToDouble(endBitmap.PixelHeight) / Convert.ToDouble(100) * Prozent;
                if (tHeight < Convert.ToDouble(1920))
                {
                    tHeight = Convert.ToDouble(1920);
                }
                endBitmap = endBitmap.Resize(1080, MyRound(tHeight), WriteableBitmapExtensions.Interpolation.Bilinear);
            }
            else if (nHeight == minHeight & Convert.ToDouble(endBitmap.PixelHeight) > Convert.ToDouble(1920))
            {
                double Prozent;
                Prozent = Convert.ToDouble(100) / Convert.ToDouble(endBitmap.PixelHeight) * Convert.ToDouble(1920);
                double tWidth = Convert.ToDouble(endBitmap.PixelWidth) / Convert.ToDouble(100) * Prozent;
                if (tWidth < Convert.ToDouble(1080))
                {
                    tWidth = Convert.ToDouble(1080);
                }
                endBitmap = endBitmap.Resize(MyRound(tWidth), 1920, WriteableBitmapExtensions.Interpolation.Bilinear);
            }

            //neues Bild erstellen
            var CutBitmap = new WriteableBitmap(0, 0);

            //Wenn X Achse verschoben werden kann
            if (nHeight == 427)
            {
                //Prozent der Verschiebung errechnen
                double temp = Convert.ToDouble(transform.TranslateX) + ((nWidth - minWidth) / 2);
                double Prozent = Convert.ToDouble(100) / (nWidth - minWidth) * temp;

                //erförderliche Pixel errechnen
                double tProzenzt = Convert.ToDouble(100) / Convert.ToDouble(1920) * Convert.ToDouble(endBitmap.PixelHeight);
                double ePix = Convert.ToDouble(1080) / Convert.ToDouble(100) * tProzenzt;

                //Restlichen Pixel errechnen
                double rPix = Convert.ToDouble(endBitmap.PixelWidth) - ePix;

                //Verschiebung errechnen
                double verschiebung = rPix / Convert.ToDouble(100) * Prozent;
                verschiebung = rPix - verschiebung;

                //EndBild schneiden
                CutBitmap = new WriteableBitmap(MyRound(ePix), endBitmap.PixelHeight);
                CutBitmap.Blit(new Rect(0, 0, MyRound(ePix), endBitmap.PixelHeight), endBitmap, new Rect(MyRound(verschiebung), 0, MyRound(ePix), endBitmap.PixelHeight));
            }

            //Wenn Y Achse verschoben werden kann
            else if (nWidth == 240)
            {
                //Prozent der Verschiebung errechnen
                double temp = Convert.ToDouble(transform.TranslateY) + ((nHeight - minHeight) / 2);
                double Prozent = Convert.ToDouble(100) / (nHeight - minHeight) * temp;

                //erförderliche Pixel errechnen
                double tProzenzt = Convert.ToDouble(100) / Convert.ToDouble(1080) * Convert.ToDouble(endBitmap.PixelWidth);
                double ePix = Convert.ToDouble(1920) / Convert.ToDouble(100) * tProzenzt;

                //Restlichen Pixel errechnen
                double rPix = Convert.ToDouble(endBitmap.PixelHeight) - ePix;

                //Verschiebung errechnen
                double verschiebung = rPix / Convert.ToDouble(100) * Prozent;
                verschiebung = rPix - verschiebung;

                //EndBild schneiden
                CutBitmap = new WriteableBitmap(endBitmap.PixelWidth, MyRound(ePix));
                CutBitmap.Blit(new Rect(0, 0, endBitmap.PixelWidth, MyRound(ePix)), endBitmap, new Rect(0, MyRound(verschiebung), endBitmap.PixelWidth, MyRound(ePix)));
            }

            //Wenn keine Achse verschoben werden kann
            else
            {
                //Cut Bitmap von endBitmap übernehmen
                CutBitmap = endBitmap;
            }


            //Image Count erhöhen
            ImageCount++;
            string ImageName = Convert.ToString(ImageCount);
            while (ImageName.Length < 8)
            {
                ImageName = "0" + ImageName;
            }
            //Image Count speichern
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
            FileStream filestream = file.CreateFile("Settings/ImageCount.dat");
            StreamWriter sw = new StreamWriter(filestream);
            sw.WriteLine(Convert.ToString(ImageCount));
            sw.Flush();
            filestream.Close();


            //Datei in Isolated Storage schreiben
            var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
            var isolatedStorageFileStream = userStoreForApplication.CreateFile("/Folders/" + Folder + "/" + ImageName + ".jpg");
            int ImgHeight = CutBitmap.PixelHeight;
            int ImgWidth = CutBitmap.PixelWidth;
            CutBitmap.SaveJpeg(isolatedStorageFileStream, CutBitmap.PixelWidth, CutBitmap.PixelHeight, 0, 80);
            isolatedStorageFileStream.Close();

            //Thumbnail erstellen
            int percent;
            int newWidth;
            int newHeight;
            //Wenn breiter als hoch
            if (CutBitmap.PixelWidth > CutBitmap.PixelHeight)
            {
                percent = 100 * 1000 / CutBitmap.PixelWidth * 150 / 1000;
                newHeight = CutBitmap.PixelHeight * 1000 / 100 * percent / 1000;
                newWidth = 150;
            }
            //Wenn höher als breit
            else
            {
                percent = 100 * 1000 / CutBitmap.PixelHeight * 150 / 1000;
                newWidth = CutBitmap.PixelWidth * 1000 / 100 * percent / 1000;
                newHeight = 150;
            }
            //Bild verkleinern
            CutBitmap = CutBitmap.Resize(newWidth, newHeight, WriteableBitmapExtensions.Interpolation.Bilinear);

            //Thumbnail speichern
            isolatedStorageFileStream = userStoreForApplication.CreateFile("/Thumbs/" + Folder + "/" + ImageName + ".jpg");
            CutBitmap.SaveJpeg(isolatedStorageFileStream, CutBitmap.PixelWidth, CutBitmap.PixelHeight, 0, 80);
            isolatedStorageFileStream.Close();

            //Images.dat erstellen
            ImagesAll++;
            //Neue Image Datei erstellen
            filestream = file.CreateFile("/FoldersDat/" + Folder + ".dat");
            sw = new StreamWriter(filestream);
            sw.WriteLine(ImagesAll);
            sw.Flush();
            filestream.Close();

            //Aktuelles Bild erhöhen
            ImageNow++;
            //Prüfen ob noch ein Bild vorhanden
            if (ImageNow < ImagesToAdd_c)
            {
                LoadImageToPanel();
            }
            else
            {
                //Cut zurücksetzen
                CutRoot.Margin = new Thickness(-600, 0, 0, 0);
                CutRoot.Visibility = System.Windows.Visibility.Collapsed;
                //AppBar zurückstellen
                CreateAppBar();
                //MenuOpene zurücksetzen
                MenuOpen = false;
            }


        }
        //---------------------------------------------------------------------------------------------------------





        //Timer AutoCut
        //---------------------------------------------------------------------------------------------------------------------------------
        string dt_Action = "Stop";
        void dt_Tick(object sender, object e)
        {
            //Beim Start
            if (dt_Action == "Start")
            {
                dt_Action = "Next";
            }

            //Prüfen ob nächstes Bild geschnitten wird
            else if (dt_Action == "Next")
            {
                //Prüfen ob noch Bild zum schneiden
                if (ImageNow < ImagesToAdd_c)
                {
                    WaitGrid.Visibility = System.Windows.Visibility.Visible;
                    dt_Action = "Stop";
                    AutoCut();
                }

                //Wenn kein Bild mehr zum schneiden
                else
                {
                    WaitGrid.Visibility = System.Windows.Visibility.Collapsed;
                    CutRoot.Margin = new Thickness(-600, 0, 0, 0);
                    CutRoot.Visibility = System.Windows.Visibility.Collapsed;
                    //AppBar zurückstellen
                    CreateAppBar();
                    //MenuOpene zurücksetzen
                    MenuOpen = false;
                    dtt.Stop();
                }
            }

        }
        //---------------------------------------------------------------------------------------------------------------------------------





        //Button AutoCut
        //---------------------------------------------------------------------------------------------------------------------------------
        private void BtnAutoCut(object sender, EventArgs e)
        {
            //Action umstellen
            dt_Action = "Start";
            dtt.Start();
        }
        //---------------------------------------------------------------------------------------------------------------------------------





        //Aktion Automatisch schneiden
        //---------------------------------------------------------------------------------------------------------
        private void AutoCut()
        {
            //CutBitmap erstellen
            WriteableBitmap CutBitmap = new WriteableBitmap(0, 0);

            //Bei allen Bildern
            if (imgPictures == "all")
            {
                //Bei allen Bildern
                MediaLibrary mediaLibrary = new MediaLibrary();
                var pictures = mediaLibrary.Pictures;

                //Bilder auslesen und in Cut Panel schreiben
                try
                {
                    //Bild in Temp Bitmap laden
                    tempBitmap.SetSource(pictures[ImagesToAdd[ImageNow]].GetImage());

                    //Bild Größe Prüfen und schneiden
                    if (tempBitmap.PixelHeight > 1920)
                    {
                        //Wenn Breite beschnitten werden muss
                        double Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelHeight) * 1920;
                        double tempWidth = Convert.ToDouble(tempBitmap.PixelWidth) / Convert.ToDouble(100) * Prozent;
                        if (MyRound(tempWidth) >= 1080)
                        {
                            tempBitmap = tempBitmap.Resize(MyRound(tempWidth), 1920, WriteableBitmapExtensions.Interpolation.Bilinear);
                        }
                        //Wenn Höhe beschnitten werden muss
                        else
                        {
                            Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelWidth) * 1080;
                            double tempHeight = Convert.ToDouble(tempBitmap.PixelHeight) / Convert.ToDouble(100) * Prozent;
                            tempBitmap = tempBitmap.Resize(1080, MyRound(tempHeight), WriteableBitmapExtensions.Interpolation.Bilinear);
                        }
                    }

                    //Bild Verhältnis prüfen
                    double ProzentWidth = Convert.ToDouble(100) / Convert.ToDouble(1080) * Convert.ToDouble(tempBitmap.PixelWidth);
                    double ProzentHeight = Convert.ToDouble(100) / Convert.ToDouble(1920) * Convert.ToDouble(tempBitmap.PixelHeight);

                    //Wenn Breite größer als Höhe
                    if (ProzentWidth > ProzentHeight)
                    {
                        double PixelCut = Convert.ToDouble(1080) / Convert.ToDouble(100) * ProzentHeight;
                        double PixelRest = (Convert.ToDouble(tempBitmap.PixelWidth) - PixelCut) / Convert.ToDouble(2);
                        CutBitmap = new WriteableBitmap(MyRound(PixelCut), tempBitmap.PixelHeight);
                        CutBitmap.Blit(new Rect(0, 0, MyRound(PixelCut), tempBitmap.PixelHeight), tempBitmap, new Rect(MyRound(PixelRest), 0, MyRound(PixelCut), tempBitmap.PixelHeight));
                    }

                    //Wenn Höhe größer als Breite
                    else if (ProzentHeight > ProzentWidth)
                    {
                        double PixelCut = Convert.ToDouble(1920) / Convert.ToDouble(100) * ProzentWidth;
                        double PixelRest = (Convert.ToDouble(tempBitmap.PixelHeight) - PixelCut) / Convert.ToDouble(2);
                        CutBitmap = new WriteableBitmap(tempBitmap.PixelWidth, MyRound(PixelCut));
                        CutBitmap.Blit(new Rect(0, 0, tempBitmap.PixelWidth, MyRound(PixelCut)), tempBitmap, new Rect(0, MyRound(PixelRest), tempBitmap.PixelWidth, MyRound(PixelCut)));
                    }

                    //Wenn gleich groß
                    else
                    {
                        CutBitmap = tempBitmap;
                    }

                    //Zurück umwandeln
                    tempBitmap = CutBitmap;

                    //Image Count erhöhen
                    ImageCount++;
                    string ImageName = Convert.ToString(ImageCount);
                    while (ImageName.Length < 8)
                    {
                        ImageName = "0" + ImageName;
                    }
                    //Image Count speichern
                    IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                    FileStream filestream = file.CreateFile("Settings/ImageCount.dat");
                    StreamWriter sw = new StreamWriter(filestream);
                    sw.WriteLine(Convert.ToString(ImageCount));
                    sw.Flush();
                    filestream.Close();

                    //Datei in Isolated Storage schreiben
                    var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
                    var isolatedStorageFileStream = userStoreForApplication.CreateFile("/Folders/" + Folder + "/" + ImageName + ".jpg");
                    int ImgHeight = tempBitmap.PixelHeight;
                    int ImgWidth = tempBitmap.PixelWidth;
                    tempBitmap.SaveJpeg(isolatedStorageFileStream, tempBitmap.PixelWidth, tempBitmap.PixelHeight, 0, 80);
                    isolatedStorageFileStream.Close();

                    //Thumbnail erstellen
                    int percent;
                    int newWidth;
                    int newHeight;
                    //Wenn breiter als hoch
                    if (tempBitmap.PixelWidth > tempBitmap.PixelHeight)
                    {
                        percent = 100 * 1000 / tempBitmap.PixelWidth * 150 / 1000;
                        newHeight = tempBitmap.PixelHeight * 1000 / 100 * percent / 1000;
                        newWidth = 150;
                    }
                    //Wenn höher als breit
                    else
                    {
                        percent = 100 * 1000 / tempBitmap.PixelHeight * 150 / 1000;
                        newWidth = tempBitmap.PixelWidth * 1000 / 100 * percent / 1000;
                        newHeight = 150;
                    }
                    //Bild verkleinern
                    tempBitmap = tempBitmap.Resize(newWidth, newHeight, WriteableBitmapExtensions.Interpolation.Bilinear);

                    //Thumbnail speichern
                    isolatedStorageFileStream = userStoreForApplication.CreateFile("/Thumbs/" + Folder + "/" + ImageName + ".jpg");
                    tempBitmap.SaveJpeg(isolatedStorageFileStream, tempBitmap.PixelWidth, tempBitmap.PixelHeight, 0, 80);
                    isolatedStorageFileStream.Close();

                    //Images.dat erstellen
                    ImagesAll++;
                    //Neue Image Datei erstellen
                    filestream = file.CreateFile("/FoldersDat/" + Folder + ".dat");
                    sw = new StreamWriter(filestream);
                    sw.WriteLine(ImagesAll);
                    sw.Flush();
                    filestream.Close();

                    //ImgNow erhöhen
                    ImageNow++;
                }
                catch
                {
                }
            }

            //Bei gespeicherten Bildern
            else
            {
                //Bei saved Pictures
                MediaLibrary mediaLibrary = new MediaLibrary();
                var pictures = mediaLibrary.SavedPictures;

                //Bilder auslesen und in Cut Panel schreiben
                try
                {
                    //Bild in Temp Bitmap laden
                    tempBitmap.SetSource(pictures[ImagesToAdd[ImageNow]].GetImage());

                    //Bild Größe Prüfen und schneiden
                    if (tempBitmap.PixelHeight > 1920)
                    {
                        //Wenn Breite beschnitten werden muss
                        double Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelHeight) * 1920;
                        double tempWidth = Convert.ToDouble(tempBitmap.PixelWidth) / Convert.ToDouble(100) * Prozent;
                        if (MyRound(tempWidth) >= 1080)
                        {
                            tempBitmap = tempBitmap.Resize(MyRound(tempWidth), 1920, WriteableBitmapExtensions.Interpolation.Bilinear);
                        }
                        //Wenn Höhe beschnitten werden muss
                        else
                        {
                            Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelWidth) * 1080;
                            double tempHeight = Convert.ToDouble(tempBitmap.PixelHeight) / Convert.ToDouble(100) * Prozent;
                            tempBitmap = tempBitmap.Resize(1080, MyRound(tempHeight), WriteableBitmapExtensions.Interpolation.Bilinear);
                        }
                    }

                    //Bild Verhältnis prüfen
                    double ProzentWidth = Convert.ToDouble(100) / Convert.ToDouble(1080) * Convert.ToDouble(tempBitmap.PixelWidth);
                    double ProzentHeight = Convert.ToDouble(100) / Convert.ToDouble(1920) * Convert.ToDouble(tempBitmap.PixelHeight);

                    //Wenn Breite größer als Höhe
                    if (ProzentWidth > ProzentHeight)
                    {
                        double PixelCut = Convert.ToDouble(1080) / Convert.ToDouble(100) * ProzentHeight;
                        double PixelRest = (Convert.ToDouble(tempBitmap.PixelWidth) - PixelCut) / Convert.ToDouble(2);
                        CutBitmap = new WriteableBitmap(MyRound(PixelCut), tempBitmap.PixelHeight);
                        CutBitmap.Blit(new Rect(0, 0, MyRound(PixelCut), tempBitmap.PixelHeight), tempBitmap, new Rect(MyRound(PixelRest), 0, MyRound(PixelCut), tempBitmap.PixelHeight));
                    }

                    //Wenn Höhe größer als Breite
                    else if (ProzentHeight > ProzentWidth)
                    {
                        double PixelCut = Convert.ToDouble(1920) / Convert.ToDouble(100) * ProzentWidth;
                        double PixelRest = (Convert.ToDouble(tempBitmap.PixelHeight) - PixelCut) / Convert.ToDouble(2);
                        CutBitmap = new WriteableBitmap(tempBitmap.PixelWidth, MyRound(PixelCut));
                        CutBitmap.Blit(new Rect(0, 0, tempBitmap.PixelWidth, MyRound(PixelCut)), tempBitmap, new Rect(0, MyRound(PixelRest), tempBitmap.PixelWidth, MyRound(PixelCut)));
                    }

                    //Wenn gleich groß
                    else
                    {
                        CutBitmap = tempBitmap;
                    }

                    //Zurück umwandeln
                    tempBitmap = CutBitmap;

                    //Image Count erhöhen
                    ImageCount++;
                    string ImageName = Convert.ToString(ImageCount);
                    while (ImageName.Length < 8)
                    {
                        ImageName = "0" + ImageName;
                    }
                    //Image Count speichern
                    IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForApplication();
                    FileStream filestream = file.CreateFile("Settings/ImageCount.dat");
                    StreamWriter sw = new StreamWriter(filestream);
                    sw.WriteLine(Convert.ToString(ImageCount));
                    sw.Flush();
                    filestream.Close();

                    //Datei in Isolated Storage schreiben
                    var userStoreForApplication = IsolatedStorageFile.GetUserStoreForApplication();
                    var isolatedStorageFileStream = userStoreForApplication.CreateFile("/Folders/" + Folder + "/" + ImageName + ".jpg");
                    int ImgHeight = tempBitmap.PixelHeight;
                    int ImgWidth = tempBitmap.PixelWidth;
                    tempBitmap.SaveJpeg(isolatedStorageFileStream, tempBitmap.PixelWidth, tempBitmap.PixelHeight, 0, 80);
                    isolatedStorageFileStream.Close();

                    //Thumbnail erstellen
                    int percent;
                    int newWidth;
                    int newHeight;
                    //Wenn breiter als hoch
                    if (tempBitmap.PixelWidth > tempBitmap.PixelHeight)
                    {
                        percent = 100 * 1000 / tempBitmap.PixelWidth * 150 / 1000;
                        newHeight = tempBitmap.PixelHeight * 1000 / 100 * percent / 1000;
                        newWidth = 150;
                    }
                    //Wenn höher als breit
                    else
                    {
                        percent = 100 * 1000 / tempBitmap.PixelHeight * 150 / 1000;
                        newWidth = tempBitmap.PixelWidth * 1000 / 100 * percent / 1000;
                        newHeight = 150;
                    }
                    //Bild verkleinern
                    tempBitmap = tempBitmap.Resize(newWidth, newHeight, WriteableBitmapExtensions.Interpolation.Bilinear);

                    //Thumbnail speichern
                    isolatedStorageFileStream = userStoreForApplication.CreateFile("/Thumbs/" + Folder + "/" + ImageName + ".jpg");
                    tempBitmap.SaveJpeg(isolatedStorageFileStream, tempBitmap.PixelWidth, tempBitmap.PixelHeight, 0, 80);
                    isolatedStorageFileStream.Close();

                    //Images.dat erstellen
                    ImagesAll++;
                    //Neue Image Datei erstellen
                    filestream = file.CreateFile("/FoldersDat/" + Folder + ".dat");
                    sw = new StreamWriter(filestream);
                    sw.WriteLine(ImagesAll);
                    sw.Flush();
                    filestream.Close();

                    //ImgNow erhöhen
                    ImageNow++;
                }
                catch
                {
                }
            }

            //Action Umstellen
            dt_Action = "Next";
        }
        //---------------------------------------------------------------------------------------------------------





        //Bild ausrichten
        //---------------------------------------------------------------------------------------------------------------------------------
        //Variabeln
        double maxX;
        double minX;
        double maxY;
        double minY;

        //Funktion
        private void OnDoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            transform.ScaleX = transform.ScaleY = 1;
        }

        private void DoubleTap(object sender, Microsoft.Phone.Controls.GestureEventArgs e)
        {
            transform.TranslateX = transform.TranslateY = 0;
            transform.ScaleX = transform.ScaleY = 1;
            transform.Rotation = 0;
        }

        private void OnDragStarted(object sender, DragStartedGestureEventArgs e)
        {
            CutImage.Opacity = 0.3;
        }

        private void OnDragDelta(object sender, DragDeltaGestureEventArgs e)
        {

            transform.TranslateX += e.HorizontalChange;
            transform.TranslateY += e.VerticalChange;

            maxX = (nWidth - minWidth) / Convert.ToDouble(2);
            minX = 0 - maxX;
            maxY = (nHeight - minHeight) / Convert.ToDouble(2);
            minY = 0 - maxY;

            if (maxY < transform.TranslateY)
            {
                transform.TranslateY = maxY;
            }
            if (minY > transform.TranslateY)
            {
                transform.TranslateY = minY;
            }

            if (maxX < transform.TranslateX)
            {
                transform.TranslateX = maxX;
            }
            if (minX > transform.TranslateX)
            {
                transform.TranslateX = minX;
            }
        }

        private void OnDragCompleted(object sender, DragCompletedGestureEventArgs e)
        {
            CutImage.Opacity = 1.0;
        }
        //---------------------------------------------------------------------------------------------------------------------------------





        //Bild drehen links
        //---------------------------------------------------------------------------------------------------------------------------------
        private void BtnRotateLeft(object sender, EventArgs e)
        {
            //Bild drehen
            endBitmap = endBitmap.Rotate(270);

            //Verschiebung zurücksetzen
            transform.TranslateX = 0;
            transform.TranslateY = 0;

            //tempBild in Writeable Bitmap speichern
            tempBitmap = endBitmap;

            //Errechnen ob Höhe Größer als 427px wenn breite 240px
            double Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelWidth) * minWidth;
            double tempHeight = Convert.ToDouble(tempBitmap.PixelHeight) / Convert.ToDouble(100) * Prozent;

            //Wenn Höhe Größer als 427px
            if (tempHeight >= minHeight)
            {
                //Neue Größe erstellen
                nWidth = minWidth;
                nHeight = tempHeight;
            }

            //Wenn Höhe nicht Größer als 427px
            else
            {
                //Neue Größe erstellen
                nHeight = minHeight;
                Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelHeight) * minHeight;
                nWidth = Convert.ToDouble(tempBitmap.PixelWidth) / Convert.ToDouble(100) * Prozent;
            }

            //Bild, Größe ändern
            tempBitmap = tempBitmap.Resize(Convert.ToInt32(nWidth), Convert.ToInt32(nHeight), WriteableBitmapExtensions.Interpolation.Bilinear);

            //Bild in Cut Bild ausgeben
            CutImage.Height = nHeight;
            CutImage.Width = nWidth;
            CutImage.Source = tempBitmap;
        }
        //---------------------------------------------------------------------------------------------------------------------------------





        //Bild drehen rechts
        //---------------------------------------------------------------------------------------------------------------------------------
        private void BtnRotateRight(object sender, EventArgs e)
        {
            //Bild drehen
            endBitmap = endBitmap.Rotate(90);

            //Verschiebung zurücksetzen
            transform.TranslateX = 0;
            transform.TranslateY = 0;

            //tempBild in Writeable Bitmap speichern
            tempBitmap = endBitmap;

            //Errechnen ob Höhe Größer als 427px wenn breite 240px
            double Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelWidth) * minWidth;
            double tempHeight = Convert.ToDouble(tempBitmap.PixelHeight) / Convert.ToDouble(100) * Prozent;

            //Wenn Höhe Größer als 427px
            if (tempHeight >= minHeight)
            {
                //Neue Größe erstellen
                nWidth = minWidth;
                nHeight = tempHeight;
            }

            //Wenn Höhe nicht Größer als 427px
            else
            {
                //Neue Größe erstellen
                nHeight = minHeight;
                Prozent = Convert.ToDouble(100) / Convert.ToDouble(tempBitmap.PixelHeight) * minHeight;
                nWidth = Convert.ToDouble(tempBitmap.PixelWidth) / Convert.ToDouble(100) * Prozent;
            }

            //Bild, Größe ändern
            tempBitmap = tempBitmap.Resize(Convert.ToInt32(nWidth), Convert.ToInt32(nHeight), WriteableBitmapExtensions.Interpolation.Bilinear);

            //Bild in Cut Bild ausgeben
            CutImage.Height = nHeight;
            CutImage.Width = nWidth;
            CutImage.Source = tempBitmap;
        }
        //---------------------------------------------------------------------------------------------------------------------------------





        //Back Button
        //---------------------------------------------------------------------------------------------------------------------------------
        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            if (MenuOpen == true)
            {
                //Cut zurücksetzen
                CutRoot.Margin = new Thickness(-600, 0, 0, 0);
                CutRoot.Visibility = System.Windows.Visibility.Collapsed;
                //AppBar zurückstellen
                CreateAppBar();
                //MenuOpene zurücksetzen
                MenuOpen = false;
                //Zurück abbrechen
                e.Cancel = true;
            }
        }
        //---------------------------------------------------------------------------------------------------------------------------------       





    }
}
