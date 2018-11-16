﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Serialization;
using Microsoft.Win32;
using PropertyChanged;
using WindowsServiceManager.Models;

namespace WindowsServiceManager.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class MainViewModel
    {

        const int awaitInterval = 10;

        private XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Service>));

        public Service SelectedService { get; set; }

        public ObservableCollection<Service> Services { get; set; } = new ObservableCollection<Service>();

        public ObservableCollection<string> Messages { get; set; } = new ObservableCollection<string>();




        #region Commands

        public ICommand SaveOnExitCommand { get; set; }

        public ICommand LoadServicesCommand { get; set; }

        public ICommand AddServiceCommand { get; set; }

        public ICommand InstallServiceCommand { get; set; }

        public ICommand StartServiceCommand { get; set; }

        public ICommand StopServiceCommand { get; set; }

        public ICommand RestartServiceCommand { get; set; }

        public ICommand UninstallServiceCommand { get; set; }

        public RelayCommand RemoveServiceCommand { get; set; }
        public object Cursor { get; private set; }


        #endregion

        public MainViewModel()
        {
            this.AddServiceCommand = new RelayCommand(this.AddService);
            this.InstallServiceCommand = new RelayCommand(this.InstallService);
            this.StartServiceCommand = new RelayCommand(this.StartService);
            this.StopServiceCommand = new RelayCommand(this.StopService);
            this.RestartServiceCommand = new RelayCommand(RestartService);
            this.RemoveServiceCommand = new RelayCommand(this.RemoveService);
            this.UninstallServiceCommand = new RelayCommand(this.Uninstall);
            this.LoadServicesCommand = new RelayCommand(this.LoadServices);
            this.SaveOnExitCommand = new RelayCommand(this.SaveOnExit);
        }

        private void SaveOnExit()
        {
            XmlSerializer xs = new XmlSerializer(typeof(ObservableCollection<Service>));

            using (System.IO.FileStream fs = new FileStream("Services.xml", System.IO.FileMode.Create))
            {
                xs.Serialize(fs, Services);
            }
        }

        private void LoadServices()
        {
            using (FileStream fs = new FileStream("Services.xml", System.IO.FileMode.Open))
            {
                Services = xs.Deserialize(fs) as ObservableCollection<Service>;
            }

            SelectedService = Services.FirstOrDefault();

            foreach (var item in Services)
            {
                item.State = this.GetServiceState(item.DisplayName);
            }
        }

        private void AddService()
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

        private void StopService()
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

        private void RestartService()
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

        private void Uninstall()
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

        private void RemoveService()
        {
            Messages.Add($"{SelectedService.DisplayName} successfully removed");
            Services.Remove(SelectedService);
        }

        private void StartService()
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

        private void InstallService()
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