import { IModelRsp, IRequestQueryOption, IPoeRequestLiteRspModel, IAuditRequestReqModel, ICensusQueryOption, IPoeRequestRspModel } from "@/models";
import { $fetch, IFetchProps } from "./api";
import queryString from 'query-string';

export const getAuditRequests = async (query: Partial<IRequestQueryOption>) => {
  const params = {
    method: "GET",
    url: queryString.stringifyUrl({ url: 'auditRequests', query: { ...query } }),
  } as IFetchProps;
  return $fetch<IModelRsp<IPoeRequestLiteRspModel[]>>(params);
};

export const getAuditRequest = async (id: string) => {
  const params = {
    method: "GET",
    url: `auditRequests/${id}`,
  } as IFetchProps;
  return $fetch<IModelRsp<IPoeRequestRspModel>>(params);
};

export const updateAuditRequest = async (id: string, request: IAuditRequestReqModel) => {
  const params = {
    method: "PUT",
    url: `auditRequests/${id}`,
    body: request
  } as IFetchProps;
  return $fetch(params);
};

export const downloadPoeRequests = async (query: Partial<ICensusQueryOption>) => {
  const params = {
    method: "DOWNLOAD",
    url: queryString.stringifyUrl({
      url: "auditRequests/exportExcel",
      query: { ...query },
    }),
  } as IFetchProps;
  return $fetch<Blob>(params);
}

export const downloadPoeFiles = async (id: string) => {
  const params = {
    method: "DOWNLOAD",
    url: `auditRequests/${id}/downloadfiles`,
  } as IFetchProps;
  return $fetch<Blob>(params);
};