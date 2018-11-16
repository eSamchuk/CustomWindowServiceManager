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

namespace WindowsServiceManager
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public partial class MainWindow : Window
    {
        const int awaitInterval = 10;

        private XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Service>));

        public Service SelectedService { get; set; }

        public ObservableCollection<Service> Services { get; set; } = new ObservableCollection<Service>();

        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ////TODO повторне додавання існуючого сервіса

            bool IsWinService = false;
            OpenFileDialog d = new OpenFileDialog();
            d.Multiselect = false;
            d.Filter = "EXE-files (*.exe) | *.exe";

            if (d.ShowDialog() == true)
            {
                Service s = new Service();
                s.ExecutablePath = d.FileName;

                Assembly svcAssembly = Assembly.LoadFrom(d.FileName);
                foreach (Type t in svcAssembly.GetTypes())
                {
                    if (t.BaseType == typeof(ServiceBase))
                    {
                        ConstructorInfo conInf = t.GetConstructor(new Type[] { });
                        if (conInf != null)
                        {
                            ServiceBase svcBase = conInf.Invoke(new object[] { }) as ServiceBase;
                            s.DisplayName = svcBase.ServiceName;
                            IsWinService = true;
                            break;
                        }
                    }
                }

                if (IsWinService)
                {
                    Services.Add(s);
                    Messages.Add($"{new FileInfo(s.ExecutablePath).Name} succesfully added to list");

                    if (IsServiceExist(s.DisplayName))
                    {
                        s.State = GetServiceState(s.DisplayName);
                    }
                    else
                    {
                        s.State = ServiceStates.NotInstalled;
                    }
                }
                else
                {
                    Messages.Add($"{new FileInfo(s.ExecutablePath).Name} is not a Windows service");
                }
            }
        }

        private void Install_Click(object sender, RoutedEventArgs e)
        {
            ////TODO помилка в процесі інсталяції

            this.Cursor = Cursors.Wait;

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319"
            };

            p.Start();

            p.StandardInput.WriteLine($"installutil.exe {SelectedService.ExecutablePath}");

            do
            {
                SelectedService.State = GetServiceState(SelectedService.DisplayName);
            }
            while (SelectedService.State != ServiceStates.Stopped);

            ServiceController sc = new ServiceController(SelectedService.DisplayName);

            SelectedService.State = GetServiceState(SelectedService.DisplayName);

            Messages.Add($"{SelectedService.DisplayName} successfully installed");

            this.Cursor = Cursors.Arrow;

        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceController sc = new ServiceController(SelectedService.DisplayName);

                if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(awaitInterval));
                    Messages.Add($"{SelectedService.DisplayName} started successfully");
                }
            }
            catch (InvalidOperationException)
            {
                Messages.Add($"Service with name '{SelectedService.DisplayName}' was not found");
            }
            catch (Win32Exception ex)
            {
                Messages.Add(ex.Message);
            }
            catch (ArgumentException)
            {
                Messages.Add($"Invalid Service name");
            }

            SelectedService.State = this.GetServiceState(SelectedService.DisplayName);
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceController sc = new ServiceController(this.SelectedService.DisplayName);

                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped);

                    Messages.Add($"{SelectedService.DisplayName} successfully stopped");
                }
            }
            catch (InvalidOperationException)
            {
                Messages.Add($"Service with name '{SelectedService.DisplayName}' was not found");
            }
            catch (Win32Exception ex)
            {
                Messages.Add(ex.Message);
            }
            catch (ArgumentException)
            {
                Messages.Add($"Invalid Service name");
            }

            SelectedService.State = this.GetServiceState(SelectedService.DisplayName);
        }

        private void Restart_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ServiceController sc = new ServiceController(this.SelectedService.DisplayName);

                if (sc.Status == ServiceControllerStatus.Running)
                {
                    sc.Stop();
                    sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(awaitInterval));

                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(awaitInterval));

                    Messages.Add($"{SelectedService.DisplayName} successfully stopped and started again");
                }
            }
            catch (InvalidOperationException)
            {
                Messages.Add($"Service with name '{SelectedService.DisplayName}' was not found");
            }
            catch (Win32Exception ex)
            {
                Messages.Add(ex.Message);
            }
            catch (InvalidEnumArgumentException)
            {
                Messages.Add($"Wrong awaitable service status");
            }
            catch (ArgumentException)
            {
                Messages.Add($"Invalid Service name");
            }
        }

        private void Uninstall_Click(object sender, RoutedEventArgs e)
        {
            //// TODO помилки на етапі видалення - служби нема

            this.Cursor = Cursors.Wait;

            Process p = new Process();
            p.StartInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = "C:\\Windows\\Microsoft.NET\\Framework\\v4.0.30319"
            };

            p.Start();

            p.StandardInput.WriteLine($"installutil.exe /u {SelectedService.ExecutablePath}");

            do
            {
                SelectedService.State = GetServiceState(SelectedService.DisplayName);
            }
            while (SelectedService.State != ServiceStates.NotInstalled);

            ServiceController sc = new ServiceController(SelectedService.DisplayName);

            SelectedService.State = GetServiceState(SelectedService.DisplayName);

            Messages.Add($"{SelectedService.DisplayName} successfully uninstalled");

            this.Cursor = Cursors.Arrow;
        }

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            Messages.Add($"{SelectedService.DisplayName} successfully removed");
            Services.Remove(SelectedService);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using (System.IO.FileStream fs = new FileStream("Services.xml", System.IO.FileMode.Open))
            {
                Services = xs.Deserialize(fs) as ObservableCollection<Service>;
            }

            SelectedService = Services.FirstOrDefault();

            foreach (var item in Services)
            {
                item.State = this.GetServiceState(item.DisplayName);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Service>));

            using (System.IO.FileStream fs = new FileStream("Services.xml", System.IO.FileMode.Create))
            {
                xs.Serialize(fs, Services);
            }
        }

        private ServiceStates GetServiceState(string displayName)
        {
            ServiceStates result = ServiceStates.Undefined;

            if (IsServiceExist(displayName))
            {
                ServiceController sc = new ServiceController(displayName);

                if (sc.Status == ServiceControllerStatus.Running)
                {
                    result = ServiceStates.Running;
                }
                else if (sc.Status == ServiceControllerStatus.Stopped)
                {
                    result = ServiceStates.Stopped;
                }
                else if (sc.Status == ServiceControllerStatus.Paused)
                {
                    result = ServiceStates.Paused;
                }
            }
            else
            {
                result = ServiceStates.NotInstalled;
            }

            return result;
        }

        private bool IsServiceExist(string displayName)
        {
            ServiceController[] services = ServiceController.GetServices();
            var devices = ServiceController.GetDevices();

            bool result = false;

            foreach (var item in services)
            {
                if (item.DisplayName == displayName)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}
