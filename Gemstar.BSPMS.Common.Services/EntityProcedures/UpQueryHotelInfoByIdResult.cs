using System;

namespace Gemstar.BSPMS.Common.Services.EntityProcedures
{
    public class UpQueryHotelInfoByIdResult
    {
        public string Grpid { get; set; }
        public string Hid { get; set; }
        public string Name { get; set; }
        public string ServerAddress { get; set; }
        public string DbServer { get; set; }
        public string DbServerInternet { get; set; }
        public string DbName { get; set; }
        public string Logid { get; set; }
        public string LogPwd { get; set; }
        public EntityStatus Status { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string CustomerStatus { get; set; }
        public string VersionId { get; set; }

    }
}
