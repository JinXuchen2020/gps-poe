export enum RequestStatus {
  "Draft" = "Draft",
  "EmailSent" = "EmailSent",
  "Submitted" = "Submitted",
  "Rejected" = "Rejected",
  "Approved" = "Approved",
  "PartialApproved" = "PartialApproved",
  "Expired" = "Expired",
}

export enum RequestStatusLabel {
  "Draft" = 'request.status.draftText',
  "EmailSent" = 'request.status.emailSentText',
  "Submitted" = 'request.status.submittedText',
  "Rejected" = 'request.status.rejectedText',
  "Approved" = 'request.status.approvedText',
  "PartialApproved" = 'request.status.partialApprovedText',
  "Expired" = 'request.status.expiredText',
}

export enum Role {
  "Admin" = 'user.role.adminText',
  "Partner" = 'user.role.partnerText',
  "SuperAdmin" = 'user.role.superAdminText'
}

export const PathAccess = [
  {
    path: "incentiveRelease",
    access: "isAdmin",
  },
  {
    path: "incentives",
    access: "isAdmin",
  },
  {
    path: "census",
    access: "isAdmin",
  },
  {
    path: "audits",
    access: "isAdmin",
  },
  {
    path: "poeRequests",
    access: "isUser",
  },
  {
    path: "histories",
    access: "isUser",
  },
  {
    path: "users",
    access: "isAdmin",
  },
];

export const MailTemplates =[
  {
    title: 'incentiveRelease.mailTemplate.notifyFirst.titleText',
    subject: 'incentiveRelease.mailTemplate.notifyFirst.titleText',
    type: "Notify-First",
    default:  "<p>邮件标题：【{G} 前提交POE】_{H}_{D}_{A}_{C} </p><p>邮件正文：</p><p>【注意事项：1. 请务必在 {I} 提交POE, 其他方式进行的POE提交均被视为无效。2. 请使用附件中的POE模板提交POE材料 (该POE模板是帮助确认POE资料是否齐全。如果直接在POE中填写相关内容，让客户确认，本次会视为POE不合格】</p><p>尊敬的合作伙伴，您好，</p><p> &nbsp; &nbsp;依据 {F} 版的数据，以下客户将贵方确认为数字在案的合作伙伴。由于我们的目标是确保向当前与客户合作以推动 Azure服务消费的合作伙伴提供 {D} 本地奖励，因此需要贵方提供针对以下客户的 POE（执行证明）文档：</p><p>合作伙伴名称：{A} </p><p>MPN Location ID：{B} </p><p>客户名称：{C} </p><p>POE提交截止日期： {G} </p><p>POE （执行证明）文档需要涵盖以下订阅 ID：{E}</p><p>POE是验证合作伙伴为推进客户Azure的消费使用提供相应的技术服务支持。虽然POE的格式在很大程度上是灵活的，但是合作伙伴必须完成附加的Azure POE模板作为POE交付的一部分。这个模板有三个部分需要填写完整: </p><p>第1部分:在合作伙伴和客户信息部分填写完整。确保客户与POE请求中的信息匹配一致。</p><p>第2部分: (使用“x”)标识您帮助客户部署了基于Azure的解决方案/工作负载。</p><p>第3部分: 请提供以下详情：</p><ol><li>描述客户对上述解决方案/工作负载的需求。</li><li>描述或提供解决方案架构的副本。</li><li>描述客户从解决方案中获得的收益。</li><li>描述或提供解决方案的执行计划的副本。</li><li>提供一份由客户盖章的文件或邮件确认，证实合作伙伴有提供相关服务，例如可以是署名的PO, SOW或POC文档。客户确认邮件必须是来自客户公司的域名，而不是来自私人或第三方邮箱。如果客户没有企业域名邮箱，需要用其他邮箱，例如 @163.com， @QQ.com 等进行服务确认，则需要提供带该客户联系邮箱的最终用户盖章合同，或者客户盖章的邮箱确认函作为补充材料证明非企业域名邮箱的合规性。</li></ol><p>要赢取针对上列客户的{D}本地奖励，请向我们提供一份包含以上名单中的客户以及推动{D}用量消费所提供的服务。</p><p>要赢取此奖励，合作伙伴的服务应该包括基于云基础设施和管理、应用程序创新、安全性和遵从性或数据平台和分析的解决方案的部署。概念验证、客户培训、帐户/订阅设置，或者在没有工作负载部署或数据存储的情况下进行的 前期规划均不足以赢取此项奖励。</p><p>该项目奖励提升Azure使用量的合作伙伴。因此，以下列出的内容将视为无效的POE。</p><ul><li>报价或授权比较</li><li>文件内容显示，合作伙伴所提供的服务仅针对以使用者为前提，且未消费Azure的非计量Azure 服务。</li><li>文件缺少客户确认</li><li>文件缺少实际工作负载的详细资讯或移转/部署至Azure的资料</li></ul><p>请注意：Microsoft 可随时要求合作伙伴提供奖励执行证明文件 (POE)，以验证合作伙伴就指定的订阅为客户提供了具备 Azure 奖励资格的技术服务。Microsoft在致力于公平地对待合作伙伴的是同时，也将考虑诸多因素来决定何时提供POE，以达到对风险的最佳管理。Microsoft将与POE多次执行失败的合作伙伴一起配合，以帮助合作伙伴理解POE的要求，然后Microsoft将验证在以后的POE请求中是否满足这些要求。</p><p>敬祝商祺！</p><p>Microsoft</p><p>Greater China Region Partner Incentive Team </p>"
  },
  {
    title: 'incentiveRelease.mailTemplate.notifySec.titleText',
    subject: 'incentiveRelease.mailTemplate.notifySec.titleText',
    type: "Notify-Sec",
    default: "<p>邮件标题：【提醒！{G} 前提交POE】_{H}_{D}_{A}_{C} </p><p>邮件正文：</p><p>【注意事项：1. 请务必在 {I} 提交POE, 其他方式进行的POE提交均被视为无效。2. 请使用附件中的POE模板提交POE材料 (该POE模板是帮助确认POE资料是否齐全。如果直接在POE中填写相关内容，让客户确认，本次会视为POE不合格】</p><p>尊敬的合作伙伴，您好，</p><p> &nbsp; &nbsp;依据 {F} 版的数据，以下客户将贵方确认为数字在案的合作伙伴。由于我们的目标是确保向当前与客户合作以推动 Azure服务消费的合作伙伴提供 {D} 本地奖励，因此需要贵方提供针对以下客户的 POE（执行证明）文档：</p><p>合作伙伴名称：{A} </p><p>MPN Location ID：{B} </p><p>客户名称：{C} </p><p>POE提交截止日期： {G} </p><p>POE （执行证明）文档需要涵盖以下订阅 ID：{E}</p><p>POE是验证合作伙伴为推进客户Azure的消费使用提供相应的技术服务支持。虽然POE的格式在很大程度上是灵活的，但是合作伙伴必须完成附加的Azure POE模板作为POE交付的一部分。这个模板有三个部分需要填写完整: </p><p>第1部分:在合作伙伴和客户信息部分填写完整。确保客户与POE请求中的信息匹配一致。</p><p>第2部分: (使用“x”)标识您帮助客户部署了基于Azure的解决方案/工作负载。</p><p>第3部分: 请提供以下详情：</p><ol><li>描述客户对上述解决方案/工作负载的需求。</li><li>描述或提供解决方案架构的副本。</li><li>描述客户从解决方案中获得的收益。</li><li>描述或提供解决方案的执行计划的副本。</li><li>提供一份由客户盖章的文件或邮件确认，证实合作伙伴有提供相关服务，例如可以是署名的PO, SOW或POC文档。客户确认邮件必须是来自客户公司的域名，而不是来自私人或第三方邮箱。如果客户没有企业域名邮箱，需要用其他邮箱，例如 @163.com， @QQ.com 等进行服务确认，则需要提供带该客户联系邮箱的最终用户盖章合同，或者客户盖章的邮箱确认函作为补充材料证明非企业域名邮箱的合规性。</li></ol><p>要赢取针对上列客户的{D}本地奖励，请向我们提供一份包含以上名单中的客户以及推动{D}用量消费所提供的服务。</p><p>要赢取此奖励，合作伙伴的服务应该包括基于云基础设施和管理、应用程序创新、安全性和遵从性或数据平台和分析的解决方案的部署。概念验证、客户培训、帐户/订阅设置，或者在没有工作负载部署或数据存储的情况下进行的 前期规划均不足以赢取此项奖励。</p><p>该项目奖励提升Azure使用量的合作伙伴。因此，以下列出的内容将视为无效的POE。</p><ul><li>报价或授权比较</li><li>文件内容显示，合作伙伴所提供的服务仅针对以使用者为前提，且未消费Azure的非计量Azure 服务。</li><li>文件缺少客户确认</li><li>文件缺少实际工作负载的详细资讯或移转/部署至Azure的资料</li></ul><p>请注意：Microsoft 可随时要求合作伙伴提供奖励执行证明文件 (POE)，以验证合作伙伴就指定的订阅为客户提供了具备 Azure 奖励资格的技术服务。Microsoft在致力于公平地对待合作伙伴的是同时，也将考虑诸多因素来决定何时提供POE，以达到对风险的最佳管理。Microsoft将与POE多次执行失败的合作伙伴一起配合，以帮助合作伙伴理解POE的要求，然后Microsoft将验证在以后的POE请求中是否满足这些要求。</p><p>敬祝商祺！</p><p>Microsoft</p><p>Greater China Region Partner Incentive Team </p>"
  },
  {
    title: 'incentiveRelease.mailTemplate.notifyResubmit.titleText',
    subject: 'incentiveRelease.mailTemplate.notifyResubmit.titleText',
    type: "Notify-ReSubmit",
    default: "<p>邮件标题：【提醒！{K}前重新提交POE】_{H}_{D}_{A}_{C}</p><p>邮件正文：</p><p>尊敬的合作伙伴，您好，</p><p>请在重新提交计时时间变量个工作日内（{K}前）补充如下POE内容，且如果再次补充POE内容不符合要求，本次POE审核将不予通过：</p><p>{J} </p><p>谢谢。</p><p><img src=\"https://poestorage.blob.core.chinacloudapi.cn/poefiles/contact.jpg\" alt=\"contact.jpg\" data-href=\"https://poestorage.blob.core.chinacloudapi.cn/poefiles/contact.jpg\" style=\"width: 30%;\"/></p>"
  },
  {
    title: 'incentiveRelease.mailTemplate.notifyExpire.titleText',
    subject: 'incentiveRelease.mailTemplate.notifyExpire.titleText',
    type: "Notify-Expire",
    default:"<p>邮件标题：【POE失效通知】_{H}_{D}_{A}_{C}</p><p>邮件正文：</p><p>尊敬的合作伙伴，您好，</p><p>您的POE 未能通过本次审核。您本次POE已失效。</p><p>有任何疑问请随时于我们联系。谢谢。</p><p><img src=\"https://poestorage.blob.core.chinacloudapi.cn/poefiles/contact.jpg\" alt=\"contact.jpg\" data-href=\"https://poestorage.blob.core.chinacloudapi.cn/poefiles/contact.jpg\" style=\"width: 30%;\"/></p>"
  },
  {
    title: 'incentiveRelease.mailTemplate.notifyComplete.titleText',
    subject: 'incentiveRelease.mailTemplate.notifyComplete.titleText',
    type: "Notify-Complete",
    default:"<p>邮件标题：【POE提交完成】_{H}_{D}_{A}_{C}</p><p>邮件正文：</p><p>尊敬的合作伙伴，您好，</p><p>您提交的POE已经通过审核。</p><p>通过审核的订阅号为：{E}</p><p>本邮件仅通知贵司的POE已经通过审核，赢取此奖励以GCR Partner Incentive Team的最终计算结果为准，请知悉。</p><p>谢谢。</p><p><img src=\"https://poestorage.blob.core.chinacloudapi.cn/poefiles/contact.jpg\" alt=\"contact.jpg\" data-href=\"https://poestorage.blob.core.chinacloudapi.cn/poefiles/contact.jpg\" style=\"width: 30%;\"/></p>"
  }
]