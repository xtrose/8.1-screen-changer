﻿#pragma checksum "D:\Moses\Projekte\Windows\Windows Phone\8.1 Screen Changer\Lockscreen Swap\Lockscreen Swap\Pages\ImagesImporter.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "05DB259600A60EA66AC069B4659412E7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace Lockscreen_Swap.Pages {
    
    
    public partial class ImagesImporter : Microsoft.Phone.Controls.PhoneApplicationPage {
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.ListBox LBImages;
        
        internal System.Windows.Controls.Grid CutRoot;
        
        internal System.Windows.Controls.Image CutImage;
        
        internal System.Windows.Media.CompositeTransform transform;
        
        internal System.Windows.Controls.Image ImgTop;
        
        internal System.Windows.Controls.Grid WaitGrid;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/Lockscreen%20Swap;component/Pages/ImagesImporter.xaml", System.UriKind.Relative));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.LBImages = ((System.Windows.Controls.ListBox)(this.FindName("LBImages")));
            this.CutRoot = ((System.Windows.Controls.Grid)(this.FindName("CutRoot")));
            this.CutImage = ((System.Windows.Controls.Image)(this.FindName("CutImage")));
            this.transform = ((System.Windows.Media.CompositeTransform)(this.FindName("transform")));
            this.ImgTop = ((System.Windows.Controls.Image)(this.FindName("ImgTop")));
            this.WaitGrid = ((System.Windows.Controls.Grid)(this.FindName("WaitGrid")));
        }
    }
}
