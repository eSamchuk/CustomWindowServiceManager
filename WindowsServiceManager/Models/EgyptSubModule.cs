using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace WindowsServiceManager.Models
{
    [AddINotifyPropertyChangedInterface]
    public class EgyptSubModule
    {
        public string DisplayName { get; set; }

        public string ShortKeyValue { get; set; }


        public List<WizardPage> Pages { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }


    }
}
