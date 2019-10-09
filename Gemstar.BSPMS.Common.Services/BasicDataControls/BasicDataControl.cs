namespace Gemstar.BSPMS.Common.Services.BasicDataControls
{
    /// <summary>
    /// 基础数据控制
    /// </summary>
    public class BasicDataControl
    {
        /// <summary>
        /// 可以进行所有操作
        /// </summary>
        public static BasicDataControl CanDoAll = new BasicDataControl(true,true, true,  true);
        /// <summary>
        /// 什么操作都不允许操作
        /// </summary>
        public static BasicDataControl CanDoNothing = new BasicDataControl( false, false, false,  false);
        public BasicDataControl(bool canAdd,bool canUpdate,bool canDelete,bool canDisable)
        {
            this.CanAdd = canAdd;
            this.CanUpdate = canUpdate;
            this.CanDelete = canDelete;
            this.CanEnable = canDisable;
            this.CanDisable = canDisable;
        }
        /// <summary>
        /// 是否可以增加
        /// </summary>
        public bool CanAdd { get; private set; }
        /// <summary>
        /// 是否可以修改
        /// </summary>
        public bool CanUpdate { get; private set; }
        /// <summary>
        /// 是否可以删除
        /// </summary>
        public bool CanDelete { get;private set; }
        /// <summary>
        /// 是否可以启用
        /// </summary>
        public bool CanEnable { get;private set; }
        /// <summary>
        /// 是否可以禁用
        /// </summary>
        public bool CanDisable { get;private set; }
        /// <summary>
        /// 不能操作的提示文字
        /// </summary>
        public string MsgStr { get; set; }
    }
}
