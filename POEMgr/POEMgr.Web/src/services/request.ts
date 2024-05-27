import { IModelRsp, IRequestQueryOption, IPoeRequestRspModel, IPoeRequestLiteRspModel, IPoeRequestReqModel, IPoeRequestFileRspModel, ICensusQueryOption, IEmailSendReqModel } from "@/models";
import { $fetch, IFetchProps } from "./api";
import queryString from "query-string";

export const getPoeRequests = async (query: Partial<IRequestQueryOption>) => {
  const params = {
    method: "GET",
    url: queryString.stringifyUrl({
      url: "poeRequests",
      query: { ...query },
    }),
  } as IFetchProps;
  return $fetch<IModelRsp<IPoeRequestLiteRspModel[]>>(params);
};

export const getPoeRequest = async (id: string) => {
  const params = {
    method: "GET",
    url: `poeRequests/${id}`,
  } as IFetchProps;
  return $fetch<IModelRsp<IPoeRequestRspModel>>(params);
};

export const updatePoeRequest = async (
  id: string,
  request: IPoeRequestReqModel
) => {
  const params = {
    method: "PUT",
    url: `poeRequests/${id}`,
    body: request,
  } as IFetchProps;
  return $fetch(params);
};

export const downloadPoeRequests = async (query: Partial<ICensusQueryOption>) => {
  const params = {
    method: "DOWNLOAD",
    url: queryString.stringifyUrl({
      url: "poeRequests/exportExcel",
      query: { ...query },
    }),
  } as IFetchProps;
  return $fetch<Blob>(params);
};

export const uploadFile = async (file: FormData) => {
  const params = {
    method: 'POSTFILES',
    url: `poeRequests/uploadFile`,
    body: file,
  } as IFetchProps;
  return $fetch<IModelRsp<IPoeRequestFileRspModel>>(params);
};

export const sendRequestEmail = async (request: IEmailSendReqModel) => {
  const params = {
    method: 'POST',
    url: `emails/send`,
    body: request,
  } as IFetchProps;
  return $fetch<IModelRsp<any>>(params);
};
