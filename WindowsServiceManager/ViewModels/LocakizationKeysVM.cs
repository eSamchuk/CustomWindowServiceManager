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

namespace WindowsServiceManager.ViewModels
{
    [AddINotifyPropertyChangedInterface]
    public class LocalizationKeysVM
    {
        #region props
        public ObservableCollection<EgyptSystem> Systems { get; set; }

        public ObservableCollection<EgyptModule> Modules { get; set; }

        public ObservableCollection<EgyptSubModule> SubModules { get; set; }
        public List<WizardPage> WizardPages { get; private set; }

        public ObservableCollection<string> OutputStrings { get; set; }

        public ObservableCollection<string> OutputStringsKeysOnly { get; set; }

        public ObservableCollection<string> XamlKeys { get; set; }

        public string NamespaceAlias { get; set; } = "lockKeys";

        public string NamespaceActual { get; set; }
        public string NamespaceForXaml { get; set; }

        public string SelectedXamlKey { get; set; }

        public string InputString { get; set; }

        public bool IsSubModulesPresent { get; set; }

        public bool IsPagesPresent { get; set; }

        public bool IsValidationKeys { get; set; }

        public bool IsSubModuleNeeded { get; set; }

        public EgyptSystem SelectedSystem { get; set; }

        public EgyptModule SelectedModule { get; set; }
        public EgyptSubModule SelectedSubModule { get; set; }

        public WizardPage SelectedWizardPage { get; set; }

        public RelayCommand GenerateKeysCommand { get; set; }

        public RelayCommand CopyPropsToBufferCommand { get; set; }

        public RelayCommand CopyKeysToBufferCommand { get; set; }

        public RelayCommand CopyKeysToBufferCommandEG { get; set; }

        public RelayCommand CopyXamlNamespaceCommand { get; set; }

        public RelayCommand CopySelectedXamlKeyComand { get; set; }

        #endregion

        public LocalizationKeysVM()
        {
            this.GenerateKeysCommand = new RelayCommand(this.GenerateKeys);
            this.CopyPropsToBufferCommand = new RelayCommand(this.CopyresultsToBuffer);
            this.CopyKeysToBufferCommand = new RelayCommand(this.CopyKeysToBuffer);
            this.CopyKeysToBufferCommandEG = new RelayCommand(this.CopyKeysToBufferEG);
            this.CopyXamlNamespaceCommand = new RelayCommand(this.CopyXamlNamespace);
            this.CopySelectedXamlKeyComand = new RelayCommand(this.CopySelectedXamlKey);
            this.Systems = new ObservableCollection<EgyptSystem>();

            this.FillData();

            this.SelectedSystem = this.Systems.FirstOrDefault();
        }

        private void CopySelectedXamlKey()
        {
            Clipboard.SetText(this.SelectedXamlKey);
        }

        //Localization Key={x:Static locKeys:LocalizationKeys.TP_Wizard_PersonalInformation}

        private void CopyXamlNamespace()
        {
            Clipboard.SetText(this.NamespaceForXaml);
        }

        private void OnNamespaceAliasChanged()
        {
            this.GenerateNamespace();
        }

        private void OnNamespaceActualChanged()
        {
            this.GenerateNamespace();
        }

        private void GenerateNamespace()
        {
            this.NamespaceForXaml = $"xmlns: {this.NamespaceAlias} = \"clr-namespace:{this.SelectedSystem.NamespaceValue}.StaticData.LocalizationKeys.Wizard;assembly=TravelPassport.StaticData";
        }

        private void CopyKeysToBuffer()
        {
            if (this.OutputStringsKeysOnly?.Count > 0)
            {
                string buff = null;


                foreach (var item in this.OutputStringsKeysOnly)
                {
                    buff += $"{item}\r\n";
                }

                Clipboard.SetText(buff);
            }
        }

        private void CopyKeysToBufferEG()
        {
            if (this.OutputStringsKeysOnly?.Count > 0)
            {
                string buff = null;


                foreach (var item in this.OutputStringsKeysOnly)
                {
                    buff += $"{item} EG\r\n";
                }

                Clipboard.SetText(buff);
            }
        }

        private void CopyresultsToBuffer()
        {
            if (this.OutputStrings?.Count > 0)
            {
                string buff = null;


                foreach (var item in this.OutputStrings)
                {
                    buff += $"{item}{Environment.NewLine}{Environment.NewLine}";
                }

                Clipboard.SetText(buff);
            }
        }

        private void FillData()
        {
            this.Systems.Add(new EgyptSystem()
            {
                DisplayName = "Travel passport",
                ShortKeyValue = "TP",
                NamespaceValue = "TravelPassport",
                Modules = new ObservableCollection<EgyptModule>()
                {

                    new EgyptModule()
                    {
                        DisplayName = "Controls",
                        ShortKeyValue = "Control",

                        SubModules = new ObservableCollection<EgyptSubModule>()
                        {
                            new EgyptSubModule()
                            {
                                DisplayName = "AppDetailesControl",
                                ShortKeyValue = "AppDetailesControl",

                                Pages = new List<WizardPage>()
                                {
                                    new WizardPage()
                                    {
                                        ShortKeyValue = "AppStatus",
                                         DisplayName = "AppStatus"
                                    },

                                    new WizardPage()
                                    {
                                         DisplayName = "App attributes",
                                         ShortKeyValue = "AppAttribute"
                                    },

                                    new WizardPage()
                                    {
                                         DisplayName = "Tooltips",
                                         ShortKeyValue = "Tooltips"
                                    }
                                }
                            }
                        }
                    },

                    new EgyptModule()
                    {
                        DisplayName = "Wizard",
                        ShortKeyValue = "Wizard",
                        SubModules = new ObservableCollection<EgyptSubModule>()
                        {
                            new EgyptSubModule()
                            {
                                DisplayName = "Wizard pages",
                                ShortKeyValue = "WizardPageName"
                            },

                            new EgyptSubModule()
                            {
                                  DisplayName = "Commands",
                                ShortKeyValue = "Commands"
                            },

                            new EgyptSubModule()
                            {
                                  DisplayName = "Notifications",
                                ShortKeyValue = "Notification"
                            },

                            new EgyptSubModule()
                            {
                                DisplayName = "Primary registation",
                                ShortKeyValue = "Primary",
                                Pages = new List<WizardPage>()
                                {
                                    new WizardPage() { DisplayName = "Issuing reason", ShortKeyValue = "IssuingReason" },
                                    new WizardPage() { DisplayName = "Business activity", ShortKeyValue = "BusinessActivity" },
                                    new WizardPage() { DisplayName = "Address of residence", ShortKeyValue = "AddressOfRresidence" },
                                    new WizardPage() { DisplayName = "Birth detailes", ShortKeyValue = "BirthDetailes" },
                                    new WizardPage() { DisplayName = "Personal data", ShortKeyValue = "PersonalData" },
                                    new WizardPage() { DisplayName = "Education", ShortKeyValue = "Education" },
                                    new WizardPage() { DisplayName = "Military duty", ShortKeyValue = "MilitaryDuty" },
                                    new WizardPage() { DisplayName = "Payments", ShortKeyValue = "Payments" },
                                    new WizardPage() { DisplayName = "Provided documents", ShortKeyValue = "ProvidedDocuments" },
                                    new WizardPage() { DisplayName = "Provided Id card fill", ShortKeyValue = "ProvidedIDCFill" },
                                    new WizardPage() { DisplayName = "Provided Id card search", ShortKeyValue = "ProvidedIDCSearch" },
                                    new WizardPage() { DisplayName = "Provided birth certificate fill", ShortKeyValue = "ProvidedBCFill" },
                                    new WizardPage() { DisplayName = "Provided birth certificate search", ShortKeyValue = "ProvidedBCSearch" },
                                    new WizardPage() { DisplayName = "Representative Id card fill", ShortKeyValue = "RepresentativeIDCFill" },
                                    new WizardPage() { DisplayName = "Representative Id card search", ShortKeyValue = "RepresentativeIDCSearch" },
                                    new WizardPage() { DisplayName = "Representative driver license card fill", ShortKeyValue = "RepresentativeDLFill" },
                                    new WizardPage() { DisplayName = "Representative driver license search", ShortKeyValue = "RepresentativeDLSearch" },
                                    new WizardPage() { DisplayName = "Representative", ShortKeyValue = "RepresentativeInfo" },


                                }
                            },
                        
                        }
                    },

                    new EgyptModule()
                    {
                        DisplayName = "Document manager",
                        ShortKeyValue = "DocManager",

                        SubModules = new ObservableCollection<EgyptSubModule>()
                        {
                            new EgyptSubModule() { DisplayName = "Detailes view", ShortKeyValue = "Detailes" },
                            new EgyptSubModule() { DisplayName = "Editing", ShortKeyValue = "Editing" },
                            new EgyptSubModule() { DisplayName = "ToolTips", ShortKeyValue = "ToolTips" },
                        }

                    },

                    new EgyptModule()
                    {
                        DisplayName = "Payments manager",
                        ShortKeyValue = "Payments",
                    },

                    new EgyptModule()
                    {
                        DisplayName = "Document issuer",
                        ShortKeyValue = "DocIssuer",

                        SubModules = new ObservableCollection<EgyptSubModule>()
                        {
                            new EgyptSubModule() { ShortKeyValue="ScanDocument", DisplayName="Scan document" },
                            new EgyptSubModule() { ShortKeyValue="CheckFingerprints", DisplayName="Check fingerprints" },
                            new EgyptSubModule() { ShortKeyValue="IssueDocument", DisplayName="Issue document" }
                        }


                    },

                    new EgyptModule()
                    {
                        DisplayName = "Templates",
                        ShortKeyValue =  "Templates",
                        SubModules = new ObservableCollection<EgyptSubModule>()
                        {
                            new EgyptSubModule() { DisplayName = "AuxDoc", ShortKeyValue = "AuxDoc" },
                        }
                    }

                }
            });

        }

        private void GenerateKeys()
        {
            this.XamlKeys = new ObservableCollection<string>();
            this.OutputStringsKeysOnly = new ObservableCollection<string>();
            this.OutputStrings = new ObservableCollection<string>();
            var input = this.SplitInputIntoWords(this.InputString);
            var rawKeys = this.SplitInputIntoWords(this.InputString, false);

            var t = "{x:Static";
            var t1 = "}";


            foreach (var item in input)
            {
                string tmp = null;

                tmp = $"{this.SelectedSystem.ShortKeyValue}_{this.SelectedModule.ShortKeyValue}";

                if (this.SelectedSubModule != null)
                {
                    tmp += $"_{this.SelectedSubModule.ShortKeyValue}";


                    if (this.SelectedWizardPage != null)
                    {
                        tmp += $"_{this.SelectedWizardPage.ShortKeyValue}";
                    }
                }


                if (this.IsValidationKeys)
                {
                    tmp += $"_Is{item}Valid";
                    this.OutputStringsKeysOnly.Add($"{tmp}\t{rawKeys[input.IndexOf(item)]} is empty");
                    this.XamlKeys.Add($"[LocalizedValidationMessage(LocalizationKeys.{tmp})]");
                    tmp = $"public const string {tmp} = \"{tmp}\";";
                }
                else
                {
                    tmp += $"_{item}";
                    this.XamlKeys.Add($"Localization Key={t} {this.NamespaceAlias}:LocalizationKeys.{tmp}{t1}");
                    this.OutputStringsKeysOnly.Add($"{tmp}\t{rawKeys[input.IndexOf(item)]}");
                    tmp = $"public static string {tmp} => \"{tmp}\";";
                }

                this.OutputStrings.Add(tmp);
            }
        }

        private List<string> SplitInputIntoWords(string inputString, bool transFormRawkeys = true)
        {
            var result = new List<string>();

            if (!string.IsNullOrWhiteSpace(inputString))
            {
                var g = Regex.Matches(inputString, Environment.NewLine).Count;

                if (g == 1)
                {
                    result = new List<string>() { inputString.Substring(0, inputString.Length - 2) };
                }
                else if (g == 0)
                {
                    result = new List<string>() { inputString };
                }
                else
                {
                    result = inputString.Split(new[] { Environment.NewLine }, Int32.MaxValue, StringSplitOptions.RemoveEmptyEntries).ToList();
                }

                if (transFormRawkeys)
                {
                    for (int i = 0; i < result.Count; i++)
                    {
                        if (result[i].Contains(" "))
                        {
                            var split = result[i].Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            result[i] = null;
                            for (int j = 0; j < split.Length; j++)
                            {
                                result[i] += $"{split[j].First().ToString().ToUpper()}{split[j].Substring(1)}";
                            }
                        }
                    }
                }
            }

            return result;
        }
        private void OnSelectedSystemChanged()
        {
            this.Modules = this.SelectedSystem?.Modules;
            this.SelectedModule = this.SelectedSystem?.Modules.FirstOrDefault();
            this.GenerateNamespace();
        }

        private void OnSelectedModuleChanged()
        {
            this.SubModules = this.SelectedModule?.SubModules;
            this.SelectedSubModule = this.SubModules?.FirstOrDefault();
            this.IsSubModulesPresent = this.SelectedSubModule != null;
        }

        private void OnSelectedSubModuleChanged()
        {
            this.WizardPages = this.SelectedSubModule?.Pages;
            this.SelectedWizardPage = this.WizardPages?.FirstOrDefault();
            this.IsPagesPresent = this.SelectedWizardPage != null;
        }

        private void OnIsSubModuleNeededChanged()
        {
            if (this.IsSubModuleNeeded)
            {
                this.IsSubModulesPresent = false;
                this.SelectedSubModule = null;
                this.IsPagesPresent = false;
                this.SelectedWizardPage = null;
            }
            else
            {
                this.IsSubModulesPresent = true;
                this.IsPagesPresent = true;
            }
        }
    }
}
