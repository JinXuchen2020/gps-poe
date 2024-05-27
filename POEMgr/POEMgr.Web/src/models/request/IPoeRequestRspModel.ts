import {
  ICustomerRspModel,
  IIncentiveRspModel,
  IPartnerRspModel,
} from "../incentive";
import { IPoeRequestFileRspModel } from "./IPoeRequestFileRspModel";
import { IPoeRequestCheckPointRspModel } from "./IPoeRequestCheckPointRspModel";
import { IPoeRequestLogRspModel } from "./IPoeRequestLogRspModel";
import { ISubscriptionStatusRspModel } from "./ISubscriptionStatusRspModel";

export interface IPoeRequestRspModel {
  id: string;
  incentive: IIncentiveRspModel;
  partner: IPartnerRspModel;
  customer: ICustomerRspModel;
  status: string;
  deadLineDate: string;
  requestFiles: IPoeRequestFileRspModel[];
  requestCheckPoints: IPoeRequestCheckPointRspModel[];
  auditCheckPoints: IPoeRequestCheckPointRspModel[];
  subscriptions: ISubscriptionStatusRspModel[];
  logs: IPoeRequestLogRspModel[];
}
