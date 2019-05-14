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
    public class EgyptSystem
    {
        public string DisplayName { get; set; }

        public string ShortKeyValue { get; set; }

        public string NamespaceValue { get; set; }


        public ObservableCollection<EgyptModule> Modules { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }
}
