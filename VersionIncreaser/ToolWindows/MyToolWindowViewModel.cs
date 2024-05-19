using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EnvDTE;
using EnvDTE80;
using Microsoft;
using Microsoft.VisualStudio;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;

namespace VersionIncreaser.ToolWindows
{
    public class MyToolWindowViewModel : ViewModelBase
    {
        const string NODATA = @"no data";
        // Development Tool Environment (DTE) Advanced (2)
        private DTE2 dte;

        // XML docunebt - project config
        XmlDocument document = new();
        XmlNodeList nodesAppDisplayVer;
        XmlNodeList nodesAppVersion;
        XmlNodeList androidPkgFormat;
        int indexRelease, indexDebug;



        bool isChanged = false;
        int ChangeCount = 0;

        private string title;

        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string projectName;

        public string ProjectName
        {
            get => projectName;
            set => SetProperty(ref projectName, value);
        }
        private string fullName;

        public string FullName
        {
            get => fullName;
            set => SetProperty(ref fullName, value);
        }

        private string uniqueName;

        public string UniqueName
        {
            get => uniqueName;
            set => SetProperty(ref uniqueName, value);
        }

        private string appTitle;

        public string AppTitle
        {
            get => appTitle;
            set => SetProperty(ref appTitle, value);
        }

        private string appId;

        public string AppId
        {
            get => appId;
            set => SetProperty(ref appId, value);
        }

        private string appDisplayVer;

        public string AppDisplayVer
        {
            get => appDisplayVer;
            set
            { 
                if (SetProperty(ref appDisplayVer, value))
                {
                }
            }
        }

        private string appVersion;

        public string AppVersion
        {
            get => appVersion;
            set => SetProperty(ref appVersion, value);
        }

        private string androidPkgFormatDbg;

        public string AndroidPkgFormatDbg
        {
            get => androidPkgFormatDbg;
            set => SetProperty(ref androidPkgFormatDbg, value);
        }

        private string androidPkgFormatRls;

        public string AndroidPkgFormatRls
        {
            get => androidPkgFormatRls;
            set => SetProperty(ref androidPkgFormatRls, value);
        }

        public RelayCommand SaveCommand { get; }
        public RelayCommand CancelCommand { get; }
        public RelayCommand MajIncCommand { get; }
        public RelayCommand MinInclCommand { get; }
        public RelayCommand DispVerCancelCommand { get; }
        public RelayCommand VerAddCommand { get; }
        public RelayCommand VerCancelCommand { get; }
        public RelayCommand AabReleaseCommand { get; }
        public RelayCommand ApkReleaseCommand { get; }
        public RelayCommand CancelReleaseCommand { get; }
        public RelayCommand AabDebugCommand { get; }
        public RelayCommand ApkDebugCommand { get; }
        public RelayCommand CancelDebugCommand { get; }

        Dictionary<string, bool> ChangedProp = new();

        public MyToolWindowViewModel()
        {
            ChangedProp.Add(nameof(AppDisplayVer), false);
            ChangedProp.Add(nameof(AppVersion), false);
            ChangedProp.Add(nameof(AndroidPkgFormatDbg), false);
            ChangedProp.Add(nameof(AndroidPkgFormatRls), false);

            // Save changed data to project file and close window.
            SaveCommand = new RelayCommand(
                execute: () =>
                {
                    var result = VS.MessageBox.Show("Save Command", "Are you sure you want to save the changed data?");
                    if (result == VSConstants.MessageBoxResult.IDCANCEL) return;
                    if (ChangedProp[nameof(AppDisplayVer)])
                    {
                        nodesAppDisplayVer[0].InnerText = AppDisplayVer;
                    }
                    if (ChangedProp[nameof(AppVersion)])
                    {
                        nodesAppVersion[0].InnerText = AppVersion;
                    }
                    if (ChangedProp[nameof(AndroidPkgFormatDbg)])
                    {
                        androidPkgFormat[indexDebug - 1].InnerText = AndroidPkgFormatDbg;
                    }
                    if (ChangedProp[nameof(AndroidPkgFormatRls)])
                    {
                        androidPkgFormat[indexRelease - 1].InnerText = AndroidPkgFormatRls;
                    }

                    document.Save(FullName);
                    Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                    dte.ActiveWindow.Close();
                },
                canExecute: () =>
                {
                    return isChanged;
                }
            );
            // Cancel all changes and close window.
            CancelCommand = new RelayCommand(
                execute: () =>
                {
                    var result = VS.MessageBox.Show("Cancel Command", "Are you sure you want to cancel the changes to the data?");
                    if (result == VSConstants.MessageBoxResult.IDCANCEL) return;

                    Microsoft.VisualStudio.Shell.ThreadHelper.ThrowIfNotOnUIThread();
                    dte.ActiveWindow.Close();
                },
                canExecute: () => true
            );
            // Increvent Major Display Version.
            MajIncCommand = new RelayCommand(
                execute: () =>
                {
                    //VS.MessageBox.Show("Major Version Increment Command", "Maj++ Button is pressed!");
                    var ver = AppDisplayVer;
                    string[] parts = ver.Split('.');
                    if (int.TryParse(parts[0],out int intMaj))
                    {
                        intMaj++;
                        ver = $"{intMaj}.{parts[1]}";
                        AppDisplayVer = ver ;
                        if (!ChangedProp[nameof(AppDisplayVer)])
                        {
                            ChangeCount++;
                        }
                        ChangedProp[nameof(AppDisplayVer)] = true;
                        isChanged = true;
                        DispVerCancelCommand.NotifyCanExecuteChanged();
                        SaveCommand.NotifyCanExecuteChanged();
                    }
                },
                canExecute: () => AppDisplayVer != NODATA

            );
            // Increment Minor Display Version.
            MinInclCommand = new RelayCommand(
                execute: () =>
                {
                    //VS.MessageBox.Show("Minor Version Increment Command", "Min++ Button is pressed!");
                    var ver = AppDisplayVer;
                    string[] parts = ver.Split('.');
                    if (int.TryParse(parts[1], out int intMin))
                    {
                        intMin++;
                        ver = $"{parts[0]}.{intMin}";
                        AppDisplayVer = ver;
                        if (!ChangedProp[nameof(AppDisplayVer)])
                        {
                            ChangeCount++;
                        }
                        ChangedProp[nameof(AppDisplayVer)] = true;
                        isChanged = true;
                        DispVerCancelCommand.NotifyCanExecuteChanged();
                        SaveCommand.NotifyCanExecuteChanged();
                    }
                },
                canExecute: () => AppDisplayVer != NODATA

            );
            // Camcel all changes to the Application Display Version.
            DispVerCancelCommand = new RelayCommand(
                execute: () =>
                {
                    //VS.MessageBox.Show("Display Version Cancel Command", "Cancel Button is pressed!");
                    AppDisplayVer = nodesAppDisplayVer.Count > 0 ? nodesAppDisplayVer[0].InnerText : NODATA;
                    ChangeCount--;
                    if (ChangeCount  <= 0)
                    {
                        isChanged = false;
                        SaveCommand.NotifyCanExecuteChanged();
                        ChangeCount = 0;
                    }
                    ChangedProp[nameof(AppDisplayVer)] = false;
                    DispVerCancelCommand.NotifyCanExecuteChanged();
                },
                canExecute: () => ChangedProp[nameof(AppDisplayVer)] 
                    && AppDisplayVer != NODATA
            );
            // Increment the Application Version.
            VerAddCommand = new RelayCommand(
                execute: () =>
                {
                    //VS.MessageBox.Show("Version Add Command", "Add 1 Button is pressed!");
                    var ver = AppVersion;
                    if (int.TryParse(ver, out int intVer))
                    {
                        intVer++;
                        ver = $"{intVer}";
                        AppVersion = ver;
                        if (!ChangedProp[nameof(AppVersion)])
                        {
                            ChangeCount++;
                        }
                        ChangedProp[nameof(AppVersion)] = true;
                        isChanged = true;
                        VerCancelCommand.NotifyCanExecuteChanged();
                        SaveCommand.NotifyCanExecuteChanged();
                    }
                },
                canExecute: () => AppVersion != NODATA
            );
            // Cancel the changes to the Application Version.
            VerCancelCommand = new RelayCommand(
                execute: () =>
                {
                    VS.MessageBox.Show("Version Cancel Command", "Cancel Button is pressed!");
                    AppVersion = nodesAppVersion.Count > 0 ? nodesAppVersion[0].InnerText : NODATA;
                    ChangeCount--;
                    if (ChangeCount <= 0)
                    {
                        isChanged = false;
                        SaveCommand.NotifyCanExecuteChanged();
                        ChangeCount = 0;
                    }
                    ChangedProp[nameof(AppVersion)] = false;
                    VerCancelCommand.NotifyCanExecuteChanged();
                },
                canExecute: () => ChangedProp[nameof(AppVersion)]
                    && AppVersion != NODATA
            );
            // Set Android Package Format of Release to aab.
            AabReleaseCommand = new RelayCommand(
                execute: () =>
                {
                    //VS.MessageBox.Show("aab Release Command", "Set aab for Release!");
                    AndroidPkgFormatRls = "aab";
                    if (!ChangedProp[nameof(AndroidPkgFormatRls)])
                    {
                        ChangeCount++;
                    }
                    ChangedProp[nameof(AndroidPkgFormatRls)] = true;
                    isChanged = true;
                    AabReleaseCommand.NotifyCanExecuteChanged();
                    ApkReleaseCommand.NotifyCanExecuteChanged();
                    CancelReleaseCommand.NotifyCanExecuteChanged();
                    SaveCommand.NotifyCanExecuteChanged();
                },
                canExecute: () => AndroidPkgFormatRls != "aab"
                    && AndroidPkgFormatRls != NODATA
            ); ;
            // Set Android Package Format of Release to apk.
            ApkReleaseCommand = new RelayCommand(
                execute: () =>
                {
                    //VS.MessageBox.Show("apk Release Command", "Set apk for Release!");
                    AndroidPkgFormatRls = "apk";
                    if (!ChangedProp[nameof(AndroidPkgFormatRls)])
                    {
                        ChangeCount++;
                    }
                    ChangedProp[nameof(AndroidPkgFormatRls)] = true;
                    isChanged = true;
                    AabReleaseCommand.NotifyCanExecuteChanged();
                    ApkReleaseCommand.NotifyCanExecuteChanged();
                    CancelReleaseCommand.NotifyCanExecuteChanged();
                    SaveCommand.NotifyCanExecuteChanged();
                },
                canExecute: () => AndroidPkgFormatRls != "apk"
                    && AndroidPkgFormatRls != NODATA
            );
            // Cancel Android Package Format of release.
            CancelReleaseCommand = new RelayCommand(
                execute: () =>
                {
                    VS.MessageBox.Show("Cancel Release Command", "Cancel Release Format!");
                    AndroidPkgFormatRls = indexRelease > 0 ? 
                        androidPkgFormat[indexRelease - 1].InnerText : NODATA;
                    ChangeCount--;
                    if (ChangeCount <= 0)
                    {
                        isChanged = false;
                        SaveCommand.NotifyCanExecuteChanged();
                        ChangeCount = 0;
                    }
                    ChangedProp[nameof(AndroidPkgFormatRls)] = false;
                    AabReleaseCommand.NotifyCanExecuteChanged();
                    ApkReleaseCommand.NotifyCanExecuteChanged();
                    CancelReleaseCommand.NotifyCanExecuteChanged();
                },
                canExecute: () => ChangedProp[nameof(AndroidPkgFormatRls)]
                    && AndroidPkgFormatRls != NODATA
            );
            // Set Android Package Format of Debug to aab.
            AabDebugCommand = new RelayCommand(
                execute: () =>
                {
                    //VS.MessageBox.Show("aab Debug Command", "Set aab for Debug!");
                    AndroidPkgFormatDbg = "aab";
                    if (!ChangedProp[nameof(AndroidPkgFormatDbg)])
                    {
                        ChangeCount++;
                    }
                    ChangedProp[nameof(AndroidPkgFormatDbg)] = true;
                    isChanged = true;
                    AabDebugCommand.NotifyCanExecuteChanged();
                    ApkDebugCommand.NotifyCanExecuteChanged();
                    CancelDebugCommand.NotifyCanExecuteChanged();
                    SaveCommand.NotifyCanExecuteChanged();
                },
                canExecute: () => AndroidPkgFormatDbg != "aab"
                     && AndroidPkgFormatDbg != NODATA
           ); ;
            // Set Android Package Format of Debug to apk.
            ApkDebugCommand = new RelayCommand(
                execute: () =>
                {
                    VS.MessageBox.Show("apk Debug Command", "Set apk for Debug!");
                    AndroidPkgFormatDbg = "apk";
                    if (!ChangedProp[nameof(AndroidPkgFormatDbg)])
                    {
                        ChangeCount++;
                    }
                    ChangedProp[nameof(AndroidPkgFormatDbg)] = true;
                    isChanged = true;
                    AabDebugCommand.NotifyCanExecuteChanged();
                    ApkDebugCommand.NotifyCanExecuteChanged();
                    CancelDebugCommand.NotifyCanExecuteChanged();
                    SaveCommand.NotifyCanExecuteChanged();
                },
                canExecute: () => AndroidPkgFormatDbg != "apk"
                     && AndroidPkgFormatDbg != NODATA
            );
            // Cancel Android Package Format of Debug.
            CancelDebugCommand = new RelayCommand(
                execute: () =>
                {
                    //VS.MessageBox.Show("Cancel Debug Command", "Cancel Debug Format!");
                    AndroidPkgFormatDbg = indexDebug > 0 ? 
                        androidPkgFormat[indexDebug - 1].InnerText : NODATA;
                    ChangeCount--;
                    if (ChangeCount <= 0)
                    {
                        isChanged = false;
                        SaveCommand.NotifyCanExecuteChanged();
                        ChangeCount = 0;
                    }
                    ChangedProp[nameof(AndroidPkgFormatDbg)] = false;
                    AabDebugCommand.NotifyCanExecuteChanged();
                    ApkDebugCommand.NotifyCanExecuteChanged();
                    CancelDebugCommand.NotifyCanExecuteChanged();
                },
                canExecute: () => ChangedProp[nameof(AndroidPkgFormatDbg)]
                     && AndroidPkgFormatDbg != NODATA
            );

            Title = "Information about Project";
            ThreadHelper.ThrowIfNotOnUIThread();

            //RefreshData(); //(Calls from Show Method)
        }

        public void RefreshData()
        {
            isChanged = false;
            ChangeCount = 0;
            indexRelease = indexDebug = 0;
            ChangedProp.Clear();
            ChangedProp.Add(nameof(AppDisplayVer), false);
            ChangedProp.Add(nameof(AppVersion), false);
            ChangedProp.Add(nameof(AndroidPkgFormatDbg), false);
            ChangedProp.Add(nameof(AndroidPkgFormatRls), false);

            dte = VersionIncreaserPackage.GetGlobalService(typeof(EnvDTE.DTE)) as DTE2;
            Assumes.Present(dte);
            var selectedItem = dte.SelectedItems.Item(1);
            var selectedProject = selectedItem.Project;
            ProjectName = selectedItem.Name;
            FullName = selectedProject.FullName;
            UniqueName = selectedProject.UniqueName;

            document.Load(FullName);

            XmlNodeList nodesAppTitle = document.GetElementsByTagName("ApplicationTitle");
            AppTitle = nodesAppTitle.Count > 0 ? nodesAppTitle[0].InnerText : NODATA;
            XmlNodeList nodesAppId = document.GetElementsByTagName("ApplicationId");
            AppId = nodesAppId.Count > 0 ? nodesAppId[0].InnerText : NODATA;
            nodesAppDisplayVer = document.GetElementsByTagName("ApplicationDisplayVersion");
            AppDisplayVer = nodesAppDisplayVer.Count > 0 ? nodesAppDisplayVer[0].InnerText : NODATA;
            nodesAppVersion = document.GetElementsByTagName("ApplicationVersion");
            AppVersion = nodesAppVersion.Count > 0 ? nodesAppVersion[0].InnerText : NODATA;

            androidPkgFormat = document.GetElementsByTagName("AndroidPackageFormat");
            if (androidPkgFormat.Count > 0)
            {
                XmlNode parentNode;
                XmlNode condition;
                string conditionText;
                Dictionary<string, string> paramsCond;
                //var PropGrpCondition = androidPkgFormat[0].ParentNode.;
                for (int i = 0; i < androidPkgFormat.Count; i++)
                {
                    parentNode = androidPkgFormat[i].ParentNode;
                    condition = parentNode.Attributes.GetNamedItem("Condition");
                    conditionText = condition.InnerText;
                    paramsCond = ParseCondition(conditionText);

                    switch (paramsCond["Configuration"])
                    {
                        case "Release":
                            indexRelease = i + 1;
                            break;
                        case "Debug":
                            indexDebug = i + 1;
                            break;
                    }
                }

            }
            AndroidPkgFormatRls = indexRelease > 0 ? 
                androidPkgFormat[indexRelease - 1].InnerText : NODATA;
            AndroidPkgFormatDbg = indexDebug > 0 ? 
                androidPkgFormat[indexDebug - 1].InnerText : NODATA;

            MajIncCommand.NotifyCanExecuteChanged();
            MinInclCommand.NotifyCanExecuteChanged();
            DispVerCancelCommand.NotifyCanExecuteChanged();
            VerAddCommand.NotifyCanExecuteChanged();
            VerCancelCommand.NotifyCanExecuteChanged();
            AabDebugCommand.NotifyCanExecuteChanged();
            ApkDebugCommand.NotifyCanExecuteChanged();
            CancelDebugCommand.NotifyCanExecuteChanged();
            AabReleaseCommand.NotifyCanExecuteChanged();
            ApkReleaseCommand.NotifyCanExecuteChanged();
            CancelReleaseCommand.NotifyCanExecuteChanged();
            SaveCommand.NotifyCanExecuteChanged();
            CancelCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Parsing 'Condition' attribute value to pairs.
        /// <example>
        /// 	<PropertyGroup Condition="'$(Configuration)|$(TargetFramework)|$(Platform)'=='Release|net7.0-android|AnyCPU'">
        /// 	  <AndroidPackageFormat>apk</AndroidPackageFormat>
        /// 	  <AndroidUseAapt2>True</AndroidUseAapt2>
        /// 	  <AndroidCreatePackagePerAbi>False</AndroidCreatePackagePerAbi>
        /// 	</PropertyGroup>
        /// </example>
        /// </summary>
        /// <param name="input">'Condition' attribute value</param>
        /// <returns>Pairs key : value</returns>
        Dictionary<string, string> ParseCondition(string input)
        {
            Dictionary<string, string> pairs = new();

            string[] parts = input.Split(new string[] { "==" }, StringSplitOptions.RemoveEmptyEntries);

            string[] keys = parts[0].Split('|');
            string[] values = parts[1].Split('|');

            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = keys[i].Trim('\'', '$', '(', ')');
                values[i] = values[i].Trim('\'', '$', '(', ')');
                pairs.Add(keys[i], values[i]);
            }
            return pairs;
        }
    }
}
