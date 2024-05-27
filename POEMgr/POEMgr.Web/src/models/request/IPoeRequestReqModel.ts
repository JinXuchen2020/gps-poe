import { IPoeRequestCheckPointReqModel } from "./IPoeRequestCheckPointReqModel";
import { ISubscriptionStatusReqModel } from "./ISubscriptionStatusReqModel";

export interface IPoeRequestReqModel {
  incentiveId: string;
  partnerId: string;
  customerId: string;
  status: string;
  deadLineDate?: string;
  requestFiles: string[];
  requestStatus: IPoeRequestCheckPointReqModel[];
  subscriptionStatus: ISubscriptionStatusReqModel[];
}
