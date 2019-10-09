namespace Gemstar.BSPMS.Common.PayManage.AliProviderPay
{
    public class ResultCode
    {
        /// <summary>
        /// 接口调用成功，调用结果请参考具体的API文档所对应的业务返回参数
        /// </summary>
        public const string SUCCESS = "10000";
        /// <summary>
        /// 等待用户授权,后续需要发起轮询流程
        /// </summary>
        public const string WaitingUser = "10003";
        /// <summary>
        /// 服务不可用:	具体失败原因参见接口返回的错误码  ，后续需要调用相应的查询接口以确认结果
        /// </summary>
        public const string ServiceUnavailable = "20000";
        /// <summary>
        /// 授权权限不足:	具体失败原因参见接口返回的错误码  
        /// </summary>
        public const string NoAuth = "20001";
        /// <summary>
        /// 缺少必选参数:	具体失败原因参见接口返回的错误码  
        /// </summary>
        public const string NoRequiredPara = "40001";
        /// <summary>
        /// 非法的参数:	具体失败原因参见接口返回的错误码  
        /// </summary>
        public const string InvalidPara = "40002";
        /// <summary>
        /// 业务处理失败:	具体失败原因参见接口返回的错误码  
        /// </summary>
        public const string FAIL = "40004";
        /// <summary>
        /// 权限不足:	具体失败原因参见接口返回的错误码  
        /// </summary>
        public const string NoEnoughAuth = "40006";
    }
}