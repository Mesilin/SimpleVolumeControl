using System;
using System.Diagnostics;
using System.Reflection;

namespace VolumeControl
{
    public class Updater
    {
        public Version currentVersion;
        public Version lastVersion;
        public string newVersionPath;
        public string note;
        
        public Updater()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            currentVersion = new Version(fileVersionInfo.FileVersion);
            lastVersion = new Version(0, 0, 0, 0);
        }
    }
}
