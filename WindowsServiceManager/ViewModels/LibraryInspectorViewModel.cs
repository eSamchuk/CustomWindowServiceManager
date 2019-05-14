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
    public class LibraryInspectorViewModel
    {

        public string SearchPath { get; set; } = @"C:\Work\EgyptAC\AC_Launcher\Client\DesktopClient\bin\Debug\ExternalModules\DriverLicense";

        public RelayCommand OpenFileDialogCommand { get; set; }

        public RelayCommand SearchLibrariesCommand { get; set; }

        public ObservableCollection<string> FoundLibraries { get; set; }

        public ObservableCollection<string> Interfaces { get; set; }


        public string CurrentLibrary { get; set; }


        public LibraryInspectorViewModel()
        {

            this.OpenFileDialogCommand = new RelayCommand(this.OpenFileDialog);
            this.SearchLibrariesCommand = new RelayCommand(this.SearchLibraries);
            this.FoundLibraries = new ObservableCollection<string>();
            this.Interfaces = new ObservableCollection<string>();
        }

        private void SearchLibraries()
        {
            DirectoryInfo di = new DirectoryInfo(this.SearchPath);
            this.FoundLibraries = new ObservableCollection<string>(di.GetFiles("*.dll").Select(x => x.Name));
        }

        private void OnCurrentLibraryChanged()
        {
            try
            {
                Assembly a = Assembly.LoadFrom($"{this.SearchPath}\\{this.CurrentLibrary}");
                this.Interfaces = new ObservableCollection<string>(a.GetTypes().SelectMany(x => x.GetInterfaces().OrderBy(z => z.Name).Select(y => y.Name).Distinct()));
            }
            catch (Exception e)
            {
            }
        }

        private void OpenFileDialog()
        {
            throw new NotImplementedException();
        }
    }
}
