import {
  IIncentiveRspModel,
  IIncentiveLiteRspModel,
  IMailTemplateRspModel,
  ICheckPointRspModel,
  IIncentiveReqModel,
  IPartnerRspModel,
  ICustomerRspModel,
  IPoeRequestRspModel,
  IPoeRequestLiteRspModel,
  IPoeRequestReqModel,
  IPoeRequestFileRspModel,
  IPoeRequestCheckPointRspModel,
  IAuditRequestReqModel,
  ISubscriptionStatusRspModel,
  IPoeRequestLogRspModel,
} from "@/models";
import moment from "moment";
import queryString from "query-string";

let fileTemplates: IMailTemplateRspModel[] = [
  {
    id: "2",
    type: "Notify-First",
    subject: "首次通知",
    content: "中国Azure 用量本地奖励POE.xlsx",
  },
  {
    id: "3",
    type: "Notify-Sec",
    subject: "再次通知",
    content: "中国Azure 用量本地奖励POE.xlsx",
  },
  {
    id: "4",
    type: "Notify-ReSubmit",
    subject: "待重新提交提醒",
    content: "中国Azure 用量本地奖励POE.xlsx",
  },
  {
    id: "5",
    type: "Notify-Expire",
    subject: "失效通知",
    content: "中国Azure 用量本地奖励POE.xlsx",
  },
  {
    id: "6",
    type: "Notify-Complete",
    subject: "完成通知",
    content: "中国Azure 用量本地奖励POE.xlsx",
  },
];

let incentiveFiles: IPoeRequestFileRspModel[] = [
  {
    id: "1",
    name: "中国Azure 用量本地奖励POE.xlsx",
    path: "中国Azure 用量本地奖励POE.xlsx",
  },
];

let requestFiles: IPoeRequestFileRspModel[] = [];

let checkPoints: ICheckPointRspModel[] = [
  {
    id: "1",
    content: "POE内容与Azure服务相关",
  },
  {
    id: "2",
    content: "客户解决方案的需求",
  },
  {
    id: "3",
    content: "提供给客户的解决方案(拓扑图)",
  },
  {
    id: "4",
    content: "客户从解决方案中的收益",
  },
];

let incentives: IIncentiveRspModel[] = [
  {
    id: "1",
    name: "China Azure DPOR PAL Local Rewards",
    startDate: "2022-02-08 09:30:26",
    endDate: "2022-09-08 09:30:26",
    submitDeadlineDay: 2,
    remindEmailDay: 3,
    reSubmitDeadlineDay: 2,
    rejectCount: 3,
    mailTemplates: fileTemplates,
    files: incentiveFiles,
    checkPoints: checkPoints,
  },
  {
    id: "2",
    name: "FY23 Azure SA Disti Local Campaign (21V)",
    startDate: "2022-02-09 09:30:26",
    endDate: "2022-09-08 09:30:26",
    submitDeadlineDay: 2,
    remindEmailDay: 3,
    reSubmitDeadlineDay: 2,
    rejectCount: 3,
    mailTemplates: fileTemplates,
    files: incentiveFiles,
    checkPoints: checkPoints,
  },
  {
    id: "3",
    name: "OSA Sell Local Campaign for D365 and Power Platform",
    startDate: "2022-02-10 09:30:26",
    endDate: "2022-09-08 09:30:26",
    submitDeadlineDay: 2,
    remindEmailDay: 3,
    reSubmitDeadlineDay: 2,
    rejectCount: 3,
    mailTemplates: fileTemplates,
    files: incentiveFiles,
    checkPoints: checkPoints,
  },
];

let partners: IPartnerRspModel[] = [
  {
    id: "1014822",
    name: "Atos Global",
    email: "1122@Microsoft.com",
  },
  {
    id: "6512940",
    name: "Beijing Shouzheng Tongying Software Technology Co., Ltd.",
    email: "aabb@hotmail.com",
  },
  {
    id: "1539877",
    name: "Bespin Global",
    email: "4455@hotmail.com",
  },
];

let customers: ICustomerRspModel[] = [
  {
    id: "1",
    name: "Fitropy Environmental Science (Shanghai) Company Limited (Ga",
    subscriptions: [
      "FA29EA76-18CE-4484-8536-B9192052AE23",
      "2D4C6CA3-52F7-4CA5-B6A4-41D5804A463D",
    ],
  },
  {
    id: "2",
    name: "Beijing HuaGant Technology Co.,Ltd",
    subscriptions: [
      "FA29EA76-18CE-4484-8536-B9192052AE23",
      "2D4C6CA3-52F7-4CA5-B6A4-41D5804A463D",
    ],
  },
  {
    id: "3",
    name: "Beijing Oushu Technology Co., Ltd. (Gallacake)",
    subscriptions: [
      "FA29EA76-18CE-4484-8536-B9192052AE23",
      "2D4C6CA3-52F7-4CA5-B6A4-41D5804A463D",
    ],
  },
];

let requestStatus: {
  id: string;
  poeRequestId: string;
  checkPointId: string;
  status: string;
  type: string;
}[] = [];

let subscriptionStatus: {
  id: string;
  poeRequestId: string;
  subscriptionId: string;
  status: string;
}[] = [];

let poeRequests: IPoeRequestLiteRspModel[] = [
  {
    id: "1",
    fiscalQuarter: "FY22Q2",
    incentiveId: incentives[0].id,
    incentiveName: incentives[0].name,
    fileName: incentives[0].files[0].name,
    poeTemplatePath: incentives[0].files[0].path,
    partnerId: partners[0].id,
    partnerEmail: partners[0].email,
    partnerName: partners[0].name,
    customerName: customers[0].name,
    status: "Draft",
  },
  {
    id: "2",
    fiscalQuarter: "FY22Q2",
    incentiveId: incentives[0].id,
    incentiveName: incentives[0].name,
    fileName: incentives[0].files[0].name,
    poeTemplatePath: incentives[0].files[0].path,
    partnerId: partners[1].id,
    partnerEmail: partners[1].email,
    partnerName: partners[1].name,
    customerName: customers[1].name,
    status: "Draft",
  },
  {
    id: "3",
    fiscalQuarter: "FY22Q2",
    incentiveId: incentives[0].id,
    incentiveName: incentives[0].name,
    fileName: incentives[0].files[0].name,
    poeTemplatePath: incentives[0].files[0].path,
    partnerId: partners[2].id,
    partnerEmail: partners[2].email,
    partnerName: partners[2].name,
    customerName: customers[2].name,
    status: "Draft",
  },
  {
    id: "4",
    fiscalQuarter: "FY22Q2",
    incentiveId: incentives[1].id,
    incentiveName: incentives[1].name,
    fileName: incentives[1].files[0].name,
    poeTemplatePath: incentives[1].files[0].path,
    partnerId: partners[0].id,
    partnerEmail: partners[0].email,
    partnerName: partners[0].name,
    customerName: customers[0].name,
    status: "Draft",
  },
  {
    id: "5",
    fiscalQuarter: "FY22Q2",
    incentiveId: incentives[1].id,
    incentiveName: incentives[1].name,
    fileName: incentives[1].files[0].name,
    poeTemplatePath: incentives[1].files[0].path,
    partnerId: partners[1].id,
    partnerEmail: partners[1].email,
    partnerName: partners[1].name,
    customerName: customers[1].name,
    status: "Draft",
  },
  {
    id: "6",
    fiscalQuarter: "FY22Q2",
    incentiveId: incentives[1].id,
    incentiveName: incentives[1].name,
    fileName: incentives[1].files[0].name,
    poeTemplatePath: incentives[1].files[0].path,
    partnerId: partners[2].id,
    partnerEmail: partners[2].email,
    partnerName: partners[2].name,
    customerName: customers[2].name,
    status: "Draft",
  },
  {
    id: "7",
    fiscalQuarter: "FY22Q2",
    incentiveId: incentives[2].id,
    incentiveName: incentives[2].name,
    fileName: incentives[2].files[0].name,
    poeTemplatePath: incentives[2].files[0].path,
    partnerId: partners[0].id,
    partnerEmail: partners[0].email,
    partnerName: partners[0].name,
    customerName: customers[0].name,
    status: "Draft",
  },
  {
    id: "8",
    fiscalQuarter: "FY22Q2",
    incentiveId: incentives[2].id,
    incentiveName: incentives[2].name,
    fileName: incentives[2].files[0].name,
    poeTemplatePath: incentives[2].files[0].path,
    partnerId: partners[1].id,
    partnerEmail: partners[1].email,
    partnerName: partners[1].name,
    customerName: customers[1].name,
    status: "Draft",
  },
  {
    id: "9",
    fiscalQuarter: "FY22Q2",
    incentiveId: incentives[2].id,
    incentiveName: incentives[2].name,
    fileName: incentives[2].files[0].name,
    poeTemplatePath: incentives[2].files[0].path,
    partnerId: partners[2].id,
    partnerEmail: partners[2].email,
    partnerName: partners[2].name,
    customerName: customers[2].name,
    status: "Draft",
  },
];

let poeRequestDetails: IPoeRequestRspModel[] = [];

export default {
  "/api/incentives": async (req: Request, res: Response) => {
    const dataList = incentives.map((c) => {
      const result: IIncentiveLiteRspModel = {
        id: c.id,
        name: c.name,
        startDate: c.startDate,
        endDate: c.endDate,
      };
      return result;
    });
    const result = {
      msg: "ok",
      code: 0,
      data: dataList,
    };
    res.end(JSON.stringify(result));
  },
  "POST /api/incentives": async (req: Request, res: Response) => {
    const requestBoy = req.body as any;
    const request = requestBoy as IIncentiveReqModel;
    const newTemplates = request.mailTemplates.map((c, index) => {
      const result: IMailTemplateRspModel = {
        ...c,
        id: (fileTemplates.length + index).toString(),
      };

      fileTemplates.push(result);
      return result;
    });
    const newFiles = request.fileIds.map((c, index) => {
      const result = incentiveFiles.find((t) => t.id === c)!;
      return result;
    });
    const newPoints = request.checkPoints.map((c, index) => {
      const result: ICheckPointRspModel = {
        content: c,
        id: (checkPoints.length + index).toString(),
      };

      checkPoints.push(result);
      return result;
    });

    const incentive: IIncentiveRspModel = {
      id: (incentives.length + 1).toString(),
      ...request,
      mailTemplates: newTemplates,
      files: newFiles,
      checkPoints: newPoints,
      startDate: moment(request.startDate).format("yyyy-MM-dd HH:mm:ss"),
      endDate: moment(request.startDate).format("yyyy-MM-dd HH:mm:ss")
    };

    incentives.push(incentive);
    const result = {
      msg: "ok",
      code: 0,
      data: null,
    };

    res.end(JSON.stringify(result));
  },
  "PUT /api/incentives/4": async (req: Request, res: Response) => {
    const id = req.url.split("/")[req.url.split("/").length - 1];
    const requestBoy = req.body as any;
    const request = requestBoy as IIncentiveReqModel;
    const newTemplates = request.mailTemplates.map((c, index) => {
      let result = c;
      if (result.id === undefined) {
        result.id = (fileTemplates.length + index).toString();
        fileTemplates.push(result);
      } else {
        const index = fileTemplates.findIndex((c) => c.id === result.id);
        fileTemplates.splice(index, 1);
        fileTemplates.push(result);
      }

      return result;
    });
    const newPoints = request.checkPoints.map((c, index) => {
      let result = checkPoints.find((t) => t.content === c);
      if (result === undefined) {
        result = {
          id: (checkPoints.length + index).toString(),
          content: c,
        };
      }

      return result;
    });

    const newFiles = request.fileIds.map((c, index) => {
      let result = incentiveFiles.find((t) => t.id === c)!;
      return result;
    });

    const index = incentives.findIndex((c) => c.id === id);
    incentives.splice(index, 1);
    const incentive: IIncentiveRspModel = {
      id: id,
      ...request,
      mailTemplates: newTemplates,
      files: newFiles,
      checkPoints: newPoints,
      startDate: moment(request.startDate).format("yyyy-MM-dd HH:mm:ss"),
      endDate: moment(request.startDate).format("yyyy-MM-dd HH:mm:ss")
    };

    incentives.push(incentive);
    const result = {
      msg: "ok",
      code: 0,
      data: null,
    };

    res.end(JSON.stringify(result));
  },
  "/api/incentives/1": async (req: Request, res: Response) => {
    const pathname = req.url;
    const id = pathname.split("/")[pathname.split("/").length - 1];

    const filter = incentives.find((c) => c.id === id);
    const result = {
      msg: "ok",
      code: 0,
      data: filter,
    };
    res.end(JSON.stringify(result));
  },
  "/api/incentives/4": async (req: Request, res: Response) => {
    const pathname = req.url;
    const id = pathname.split("/")[pathname.split("/").length - 1];

    const filter = incentives.find((c) => c.id === id);
    const result = {
      msg: "ok",
      code: 0,
      data: filter,
    };
    res.end(JSON.stringify(result));
  },
  "POST /api/incentives/excelImport": async (req: Request, res: Response) => {
    const result = {
      msg: "ok",
      code: 0,
      data: poeRequests,
    };
    res.end(JSON.stringify(result));
  },
  "POST /api/incentives/sendEmail": async (req: Request, res: Response) => {
    const requestBoy = req.body as any;
    const request = requestBoy as string[];
    for (let item of request) {
      let result = poeRequests.find((c) => c.id === item);
      if (result) {
        const incentive = incentives.find((c) => c.id === result?.incentiveId)!;
        result.status = "EmailSent";
        result.startDate = moment().format("YYYY-MM-DD HH:mm:ss");
        result.deadlineDate = moment()
          .add(incentive.submitDeadlineDay, "days")
          .format("YYYY-MM-DD HH:mm:ss");
      }
    }
    const result = {
      msg: "ok",
      code: 0,
      data: null,
    };

    res.end(JSON.stringify(result));
  },
  "/api/poeRequests": async (req: Request, res: Response) => {
    const { status } = req.url.includes("?")
      ? queryString.parse(req.url.split("?")[1])
      : { status: undefined };

    let filters = poeRequests;
    if (status) {
      filters = filters.filter((c) => c.status === status);
    }
    const result = {
      msg: "ok",
      code: 0,
      data: filters,
    };
    res.end(JSON.stringify(result));
  },
  "/api/poeRequests/1": async (req: Request, res: Response) => {
    const pathname = req.url;
    const id = pathname.split("/")[pathname.split("/").length - 1];

    const filter = poeRequests.find((c) => c.id === id)!;
    let filterDetail = poeRequestDetails.find((c) => c.id === id);
    if (filterDetail === undefined) {
      filterDetail = {
        id: filter.id,
        incentive: incentives.find((c) => c.id === filter.incentiveId)!,
        partner: partners.find((c) => c.id === filter.partnerId)!,
        customer: customers.find((c) => c.name === filter.customerName)!,
        status: filter.status,
        deadLineDate: filter.deadlineDate!,
        requestFiles: [],
        requestCheckPoints: [],
        auditCheckPoints: [],
        subscriptions:[],
        logs:[]
      };

      poeRequestDetails.push(filterDetail);
    }

    const result = {
      msg: "ok",
      code: 0,
      data: filterDetail,
    };
    res.end(JSON.stringify(result));
  },
  "PUT /api/poeRequests/1": async (req: Request, res: Response) => {
    const pathname = req.url;
    const id = pathname.split("/")[pathname.split("/").length - 1];
    const requestBoy = req.body as any;
    const request = requestBoy as IPoeRequestReqModel;

    let filterDetail = poeRequestDetails.find((c) => c.id === id)!;
    let filter = poeRequests.find((c) => c.id === id)!;
    filterDetail.status = request.status;
    filter.status = request.status;

    filterDetail.requestFiles = requestFiles.filter((c) =>
      request.requestFiles.includes(c.id)
    );
    filterDetail.requestCheckPoints = request.requestStatus.map((c) => {
      let result = requestStatus.find(
        (status) =>
          status.checkPointId === c.id &&
          status.poeRequestId === id &&
          status.type === "request"
      );
      if (result) {
        result.status = c.status;
      } else {
        result = {
          id: `${requestStatus.length + 1}`,
          poeRequestId: id,
          checkPointId: c.id,
          type: "request",
          status: c.status,
        };
        requestStatus.push(result);
      }

      return {
        id: result.checkPointId,
        status: result.status,
      } as IPoeRequestCheckPointRspModel;
    });

    const result = {
      msg: "ok",
      code: 0,
      data: filterDetail,
    };
    res.end(JSON.stringify(result));
  },
  "POST /api/incentives/uploadFile": async (req: Request, res: Response) => {
    const newFile: IPoeRequestFileRspModel = {
      id: (incentiveFiles.length + 1).toString(),
      name: "中国Azure 用量本地奖励POE.xlsx",
      path: "中国Azure 用量本地奖励POE.xlsx",
    };

    incentiveFiles.push(newFile);
    const result = {
      msg: "ok",
      code: 0,
      data: newFile,
    };
    res.end(JSON.stringify(result));
  },
  "POST /api/poeRequests/uploadFile": async (req: Request, res: Response) => {
    const data: IPoeRequestFileRspModel = {
      id: `${requestFiles.length + 1}`,
      name: "中国Azure 用量本地奖励POE",
      path: "中国Azure 用量本地奖励POE.xlsx",
    };
    requestFiles.push(data);
    const result = {
      msg: "ok",
      code: 0,
      data: data,
    };
    res.end(JSON.stringify(result));
  },
  "/api/auditRequests": async (req: Request, res: Response) => {
    const { status } = req.url.includes("?")
      ? queryString.parse(req.url.split("?")[1])
      : { status: undefined };

    let filters = poeRequests;
    if (status) {
      filters = filters.filter((c) => c.status === status);
    }
    const result = {
      msg: "ok",
      code: 0,
      data: filters,
    };
    res.end(JSON.stringify(result));
  },
  "/api/auditRequests/1": async (req: Request, res: Response) => {
    const pathname = req.url;
    const id = pathname.split("/")[pathname.split("/").length - 1];
    const filterDetail = poeRequestDetails.find((c) => c.id === id)!;   

    const result = {
      msg: "ok",
      code: 0,
      data: filterDetail,
    };
    res.end(JSON.stringify(result));
  },
  "PUT /api/auditRequests/1": async (req: Request, res: Response) => {
    const pathname = req.url;
    const id = pathname.split("/")[pathname.split("/").length - 1];
    const requestBoy = req.body as any;
    const request = requestBoy as IAuditRequestReqModel;

    let filterDetail = poeRequestDetails.find((c) => c.id === id)!;
    let filter = poeRequests.find((c) => c.id === id)!;
    filterDetail.status = request.status;
    filter.status = request.status;
    if (request.status === "Approved") {
      filter.completedDate = moment().format("YYYY-MM-DD HH:mm:ss");
    }

    filterDetail.auditCheckPoints = request.auditStatus.map((c) => {
      let result = requestStatus.find(
        (status) =>
          status.checkPointId === c.id &&
          status.poeRequestId === id &&
          status.type === "audit"
      );
      if (result) {
        result.status = c.status;
      } else {
        result = {
          id: `${requestStatus.length + 1}`,
          poeRequestId: id,
          checkPointId: c.id,
          type: "audit",
          status: c.status,
        };

        requestStatus.push(result);
      }
      return {
        id: result.checkPointId,
        status: result.status,
      } as IPoeRequestCheckPointRspModel;
    });

    filterDetail.subscriptions = request.subscriptionStatus.map((c) => {
      const result = {
        id: `${subscriptionStatus.length + 1}`,
        poeRequestId: id,
        subscriptionId: c.subscriptionId,
        status: c.status,
      };

      subscriptionStatus.push(result);
      return {
        subscriptionId: result.subscriptionId,
        status: result.status,
      } as ISubscriptionStatusRspModel;
    });

    const log: IPoeRequestLogRspModel = {
      id: (filterDetail.logs.length + 1).toString(),
      content: request.status === "Approved" ? "审核通过" : "审核未通过",
      type: request.status,
      reason: request.reason
    };

    filterDetail.logs.push(log);

    const result = {
      msg: "ok",
      code: 0,
      data: filterDetail,
    };
    res.end(JSON.stringify(result));
  },
};
