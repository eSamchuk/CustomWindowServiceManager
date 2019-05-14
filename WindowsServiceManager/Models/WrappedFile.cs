using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyChanged;

namespace WindowsServiceManager.Models
{
    [AddINotifyPropertyChangedInterface]
    public class WrappedFile
    {
        public string FileName { get; set; }

        public bool IsIncluded { get; set; }

        public WrappedFile()
        {

        }

        public WrappedFile(string name)
        {
            this.FileName = name;
        }

    }
}
