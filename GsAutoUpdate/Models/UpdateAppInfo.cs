namespace GsAutoUpdate.Models
{
    public class UpdateAppInfo
    {
        /// <summary>
        /// 待更新程序名称
        /// </summary>
        public string AppName { get; set; }


        /// <summary>
        /// 酒店
        /// </summary>
        public string Hid { get; set; }

        /// <summary>
        /// 更新目录
        /// </summary>
        public string WatUpdateDirectory { get; set; }

        /// <summary>
        /// 是否弹窗更新
        /// </summary>
        public int IsPopup { get; set; }

        ///// <summary>
        ///// 更新模式(覆盖更新/增量更新 )
        ///// </summary>
        //public string UpateModel { get; set; }


    }
}
