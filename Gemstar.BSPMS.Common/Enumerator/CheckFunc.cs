namespace Gemstar.BSPMS.Common.Enumerator
{
    /// <summary>
    /// 手机验证码用途
    /// </summary>
    public enum CheckFunc
    {
        /// <summary>
        /// 注册酒店
        /// </summary>
        Register,
        /// <summary>
        /// 重设密码
        /// </summary>
        ResetPassword,
        /// <summary>
        /// 体验试用
        /// </summary>
        TryUsePms,
        /// <summary>
        /// 初始化系统
        /// </summary>
        InitSystem,
        /// <summary>
        /// 业主绑定
        /// </summary>
        OwnerBind,
        /// <summary>
        /// 操作员绑定电脑
        /// </summary>
        UserBindPc,
        /// <summary>
        /// 长租公寓租客绑定
        /// </summary>
        LongGuestBind,
        /// <summary>
        /// 售后工程师绑定微信
        /// </summary>
        ServiceOperatorBind,
        /// <summary>
        /// 运营人员绑定微信
        /// </summary>
        AdminBind,
        /// <summary>
        /// 云工程 维修工绑定
        /// </summary>
        EmWorkerBind,
        /// <summary>
        /// 合约单位绑定微信
        /// </summary>
        CompanyBind
    }
}
