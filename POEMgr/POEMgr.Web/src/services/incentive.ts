import { IIncentiveRspModel, IModelRsp, IIncentiveLiteRspModel, IMailTemplateRspModel, IIncentiveReqModel, IPoeRequestLiteRspModel, IPoeRequestFileRspModel } from "@/models";
import { $fetch, IFetchProps } from "./api";

export const getIncentives = async () => {
  const params = {
    method: "GET",
    url: `incentives`,
  } as IFetchProps;
  return $fetch<IModelRsp<IIncentiveLiteRspModel[]>>(params);
};

export const getIncentive = async (id: string) => {
  const params = {
    method: "GET",
    url: `incentives/${id}`,
  } as IFetchProps;
  return $fetch<IModelRsp<IIncentiveRspModel>>(params);
};

export const createIncentive = async (request: IIncentiveReqModel) => {
  const params = {
    method: "POST",
    url: `incentives`,
    body: request
  } as IFetchProps;
  return $fetch(params);
};

export const updateIncentive = async (id: string, request: IIncentiveReqModel) => {
  const params = {
    method: "PUT",
    url: `incentives/${id}`,
    body: request
  } as IFetchProps;
  return $fetch(params);
};

export const importExcel = async (file: FormData) => {
  const params = {
    method: "POSTFILES",
    url: `incentives/excelImport`,
    body: file,
  } as IFetchProps;
  return $fetch<IModelRsp<IPoeRequestLiteRspModel[]>>(params);
};

export const sendEmail = async (requests: string[]) => {
  const params = {
    method: "POST",
    url: `incentives/sendEmail`,
    body: requests,
  } as IFetchProps;
  return $fetch(params);
};

export const uploadFile = async (file: FormData) => {
  const params = {
    method: 'POSTFILES',
    url: `incentives/uploadFile`,
    body: file,
  } as IFetchProps;
  return $fetch<IModelRsp<IPoeRequestFileRspModel>>(params);
};
