import { RequestStatus, RequestStatusLabel } from "@/constant";
import { ICensusQueryOption, IIncentiveRspModel, IPoeRequestLiteRspModel } from "@/models";
import fileDownload from "js-file-download";
import { ReactNode } from "react";
import { Dispatch, Effect, Reducer } from "umi";
import { notification } from "antd";
import { v4 } from "uuid"
import mime from "mime"
import moment from "moment";
import { sendRequestEmail } from "@/services/request";

export const PageSize = 6;
export const MaxPageSize = 9999;

export interface IModelState<TArray, TSingle> {
  loading: boolean;
  pageTitle?: ReactNode;
  modelList: TArray[] | undefined;
  currentModel: TSingle | undefined;
  isEditing: boolean;
  showNotification?: boolean;
  errMsg?: string;
}

export interface ModelEffect {
  initState: Effect;
  getAll?: Effect;
  getModel?: Effect;
  createModel?: Effect;
  updateModel?: Effect;
  deleteModel?: Effect;
}

export interface ModelReducer<T> {
  update: Reducer<T>;
}

export const dispatchMethod = async <T>(
  dispatch: Dispatch,
  modelName: string,
  methodName: keyof ModelEffect | keyof ModelReducer<T>,
  payload?: any
) => {
  await dispatch({
    type: `${modelName}/${methodName}`,
    payload: payload ? payload : undefined,
  });
};

export const downloadFile = (data: Blob, fileName: string) => {
  fileDownload(data, fileName);
};

export const filterRequest = (
  dataFilter: ICensusQueryOption | undefined,
  modelList: IPoeRequestLiteRspModel[] | undefined
) => {
  if (modelList) {
    let result = modelList.filter((c) => c.status !== RequestStatus.Draft);
    if (dataFilter) {
      Object.keys(dataFilter).forEach((dataIndex) => {
        if (dataFilter[dataIndex as keyof ICensusQueryOption]) {
          if (dataIndex === "fiscalQuarter") {
            if (dataFilter["fiscalYear"]) {
              result = result.filter(
                (c) =>
                  c[dataIndex as keyof IPoeRequestLiteRspModel] ===
                  `${dataFilter["fiscalYear"]}${
                    dataFilter[dataIndex as keyof ICensusQueryOption]
                  }`
              );
            } else {
              result = result.filter((c) =>
                c[dataIndex as keyof IPoeRequestLiteRspModel]
                  ? c[dataIndex as keyof IPoeRequestLiteRspModel]!.endsWith(
                      dataFilter[dataIndex as keyof ICensusQueryOption]
                    )
                  : true
              );
            }
          } else {
            if (dataIndex === "fiscalYear") {
              if (dataFilter["fiscalYear"]) {
                result = result.filter((c) =>
                  c["fiscalQuarter"]
                    ? c["fiscalQuarter"].startsWith(
                        dataFilter[dataIndex as keyof ICensusQueryOption]
                      )
                    : true
                );
              }
            } else {
              result = result.filter(
                (c) =>
                  c[dataIndex as keyof IPoeRequestLiteRspModel]
                  ? c[dataIndex as keyof IPoeRequestLiteRspModel]!.includes(
                      dataFilter[dataIndex as keyof ICensusQueryOption]
                    )
                  : true
              );
            }
          }
        }
      });
    }

    return result;
  } else {
    return modelList;
  }
};

export const getFiscalQuarter = (date: Date | undefined) => {
  if (date) {
    const year = date.getFullYear();
    const month = date.getMonth() + 1;
    let quarter = 0;
    if (month >= 7 && month <= 9) {
      quarter = 1;
    } else if (month >= 10 && month <= 12) {
      quarter = 2;
    } else if (month >= 1 && month <= 3) {
      quarter = 3;
    } else if (month >= 4 && month <= 6) {
      quarter = 4;
    }
    return {
      year: `FY${year - 2000}`,
      quarter: `Q${quarter}`,
    };
  }
};

export const getFiscalYears = () => {
  return Array.from(new Array(5).keys()).map((c, index) => {
    const year = new Date().getFullYear() - c - 2000;
    return {
      key: index,
      value: `FY${year}`,
      label: `FY${year}`,
    };
  });
};
export const getFiscalQuarters = () => {
  return ["Q1", "Q2", "Q3", "Q4"].map((c, index) => {
    return {
      key: index,
      value: `${c}`,
      label: c,
    };
  });
};

export const getStatus = (intl: any) => {
  return Object.keys(RequestStatusLabel)
    .filter((c, index) => index > 0)
    .map((c, index) => {
      return {
        key: index,
        value: c,
        label: intl.formatMessage({
          id: RequestStatusLabel[c as keyof typeof RequestStatusLabel],
        }),
      };
    });
};

export const convertBase64ToBlob = (base64: string) => {
  const base64Arr = base64.split(',');
  let imageType = '';
  let base64String = '';
  if(base64Arr.length > 1) {
    base64String = base64Arr[1];
    imageType = base64Arr[0].substring(base64Arr[0].indexOf(':')+1,base64Arr[0].indexOf(';'));
  }
    // 将base64解码
    var bytes = atob(base64String);
    //var bytes = base64;
    var bytesCode = new ArrayBuffer(bytes.length);
     // 转换为类型化数组
    var byteArray = new Uint8Array(bytesCode);
 
    // 将base64转换为ascii码
    for (var i = 0; i < bytes.length; i++) {
        byteArray[i] = bytes.charCodeAt(i);
    }
 
    // 生成Blob对象（文件对象）
    return new File([bytesCode], `image_${v4()}.${mime.getExtension(imageType)}`, {type : imageType});
}

export const convertMailContent = (content: string, incentive: IIncentiveRspModel) => {
  const fiscalQuarter = getFiscalQuarter(new Date())
  const matchMap = { 
    "{A}": "深圳市伊登软件有限公司",
    "{B}": "1016485",
    "{C}": "联英管理（上海）有限公司",
    "{D}": incentive.name,
    "{E}": "<br/>00C7769B-4D82-4AC9-93F1-0D90E9A77302",
    "{F}": moment().format("YYYY-MM-DD"),
    "{G}": moment().format("YYYY-MM-DD"),
    "{H}": `${fiscalQuarter?.year}${fiscalQuarter?.quarter}`,
    "{K}": moment().add(incentive.reSubmitDeadlineDay, 'day').format("YYYY-MM-DD"),
    "{I}": window.location.origin,
    "{J}": "<p><img src='https://filepreview.blob.core.chinacloudapi.cn/poefiles/PoeRequestUploadFiles/image_1becb351-6b87-4c94-b171-d19dda02cf32.png' alt='图像' data-href='' style='width: 100%;'/></p>",
  }

  const contentPattern = Object.keys(matchMap).join('|')
  const contentReg = new RegExp(contentPattern, 'g')
  const convertedString = content.replaceAll(contentReg, (subString) => {
    return matchMap[subString as keyof typeof matchMap]
  })

  const pattern = /<p>邮件标题[:：]?([\s\S]+)<\/p><p>邮件正文[:：]?<\/p>([\s\S]+)/
  const match = pattern.exec(convertedString)
  const subject = match?.[1]
  const mailContent = match?.[2]

  return {
    subject: subject,
    content: mailContent
  }
}

export const executeSendEmail = (requestIds: string[], type: string, loadingMessage: string, successMessage: string, failureMessage: string) => {
  notification.open({
    message: loadingMessage
  })

  sendRequestEmail({
    requestIds: requestIds,
    type
  }).then(rsp => {
    if(rsp.code === 0) {
      notification.success({
        message: successMessage
      })
    }
    else {
      const msgObj = JSON.parse(rsp.msg)
      notification.error({
        message: failureMessage,
        description: msgObj.EmailAddress ? `Email Address: ${msgObj.EmailAddress}` : `${msgObj.ErrorMessage}`
      })
    }
  })
}
