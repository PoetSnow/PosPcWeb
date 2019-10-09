namespace Gemstar.BSPMS.Hotel.Services.EntityProcedures
{
    /// <summary>
    /// 查询角色的模块权限结果集
    /// </summary>
    public class UpQueryAuthListForRoleResult
    {
        public string AuthCode { get; set; }
        public string ParentCode { get; set; }
        public string AuthName { get; set; }
        public int? Seqid { get; set; }
        public bool Checked { get; set; }
    }
}
