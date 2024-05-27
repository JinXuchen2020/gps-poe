import { IPoeRequestFileRspModel } from "../request";
import { ICheckPointRspModel } from "./ICheckPointRspModel";
import { IMailTemplateRspModel } from "./IMailTemplateRspModel";

export interface IIncentiveRspModel {
  id: string;
  name: string;
  welcomeSpeech: string;
  startDate: string;
  endDate: string;
  submitDeadlineDay: number;
  remindEmailDay: number;
  reSubmitDeadlineDay: number;
  rejectCount: number;
  files: IPoeRequestFileRspModel[];
  mailTemplates: IMailTemplateRspModel[];
  checkPoints: ICheckPointRspModel[];
}
