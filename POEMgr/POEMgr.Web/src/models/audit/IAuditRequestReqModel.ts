import { IPoeRequestCheckPointReqModel, IPoeRequestReqModel } from "../request";

export interface IAuditRequestReqModel extends IPoeRequestReqModel {
  auditStatus: IPoeRequestCheckPointReqModel[];
  reason?: string;
}
