using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsServiceManager.Models;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Reflection;
using System.IO;

namespace WindowsServiceManager.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class ComparerViewModel
    {
        //C:\Work\EgyptAC\AC_Launcher\ExternalModules
        //C:\Work\EgyptAC\AC_Launcher\Client\DesktopClient\bin\Debug\ExternalModules\

        public string DirectoryOne { get; set; } = @"C:\Work\EgyptAC\AC_Launcher\Client\DesktopClient\bin\Debug\ExternalModules\DriverLicense";
        public string DirectoryTwo { get; set; } = @"C:\Work\EgyptAC\AC_Launcher\Client\DesktopClient\bin\Debug\ExternalModules\TravelPassport";

        public RelayCommand CompareDirectoriesCommand { get; set; }

        public ObservableCollection<WrappedFile> DirOneFiles { get; set; } 

        public ObservableCollection<WrappedFile> DirTwoFiles { get; set; }

        public ComparerViewModel()
        {
            this.DirOneFiles = new ObservableCollection<WrappedFile>();
            this.DirTwoFiles = new ObservableCollection<WrappedFile>();
            this.CompareDirectoriesCommand = new RelayCommand(this.CompareDirectories);
        }

        private void CompareDirectories()
        {
            this.DirOneFiles.Clear();
            this.DirTwoFiles.Clear();

            List<string> f1 = Directory.GetFiles(this.DirectoryOne, "*.dll")?.Select(x => x.Substring(x.LastIndexOf(@"\") + 1, x.Length - (x.LastIndexOf(@"\") + 1))).ToList();
            List<string> f2 = Directory.GetFiles(this.DirectoryTwo, "*.dll")?.Select(x => x.Substring(x.LastIndexOf(@"\") + 1, x.Length - (x.LastIndexOf(@"\") + 1))).ToList();

            foreach (var item in f1)
            {

                //if (!f2.Contains(item))
                {
                    this.DirOneFiles.Add(new WrappedFile(item) { IsIncluded = f2.Contains(item) });
                }
            }

            foreach (var item in f2)
            {
                //if (!f1.Contains(item))
                {
                    this.DirTwoFiles.Add(new WrappedFile(item) { IsIncluded = f1.Contains(item) });
                }
            }


        }
    }
}
