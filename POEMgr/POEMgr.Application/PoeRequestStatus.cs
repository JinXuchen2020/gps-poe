using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POEMgr.Application
{
    public enum PoeRequestStatus
    {
        [Description("未发送")]
        Draft,
        [Description("未提交")]
        EmailSent,
        [Description("已提交")]
        Submitted,
        [Description("待重新提交")]
        Rejected,
        [Description("部分通过")]
        PartialApproved,
        [Description("已通过")]
        Approved,
        [Description("已失效")]
        Expired
    }
}
