using PropertyChanged;

namespace WindowsServiceManager.Models
{
    [AddINotifyPropertyChangedInterface]
    public class WizardPage
    {
        public string DisplayName { get; set; }

        public string ShortKeyValue { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }

    }
}