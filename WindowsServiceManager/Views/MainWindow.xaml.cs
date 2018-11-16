using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.ServiceProcess;
using System.Windows;
using System.Linq;
using System.Xml.Serialization;

using PropertyChanged;
using System.IO;
using System.Diagnostics;
using System.Windows.Input;
using System.ComponentModel;
using WindowsServiceManager.Models;
using WindowsServiceManager.ViewModels;

namespace WindowsServiceManager
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {
        public MainViewModel MainVM = new MainViewModel();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = MainVM;
        }
    }
}
