using System;
using System.IO;
using System.ServiceProcess;
using PropertyChanged;

namespace WindowsServiceManager.Models

{
    [Serializable]
    [AddINotifyPropertyChangedInterface]
    public class Service
    {
        public Service()
        {
        }

        public string ExecutablePath { get; set; }

        public string DisplayName { get; set; }

        public bool CanInstall
        {
            get
            {
                return this.State == ServiceStates.NotInstalled;
            }
        }

        public bool CanStop
        {
            get
            {
                return this.State == ServiceStates.Running;
            }
        }

        public bool CanStart
        {
            get
            {
                return this.State == ServiceStates.Stopped;
            }
        }

        public bool CanRestart
        {
            get
            {
                return this.State == ServiceStates.Running;
            }
        }

        public bool CanUninstall
        {
            get
            {
                return this.State != ServiceStates.NotInstalled;
            }
        }

        public bool CanRemove
        {
            get
            {
                return true;
            }
        }

        public ServiceStates State { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }

    }
}