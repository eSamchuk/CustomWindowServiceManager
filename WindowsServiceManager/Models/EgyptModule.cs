using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;
namespace WindowsServiceManager.Models
{
    [AddINotifyPropertyChangedInterface]
    public class EgyptModule
    {
        public string DisplayName { get; set; }

        public string ShortKeyValue { get; set; }

        public ObservableCollection<EgyptSubModule> SubModules { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }

    }
}
