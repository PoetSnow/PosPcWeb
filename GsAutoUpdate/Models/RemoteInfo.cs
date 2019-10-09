using System;
using System.Collections.Generic;
using System.Text;

namespace GsAutoUpdate
{
    public class RemoteInfo
    {
        public String ApplicationStartName { get; set; }
       // public String AppName { get; set; }
        //public String MinVersion { get; set; }
        public String ReleaseDate { get; set; }
        public String ReleaseUrl { get; set; }
        public String ReleaseVersion { get; set; }
        public String UpdateMode { get; set; }
        public String VersionDesc { get; set; }
        public bool IsAll { get; set; }
        public bool IsMust { get; set; }
    }
}
