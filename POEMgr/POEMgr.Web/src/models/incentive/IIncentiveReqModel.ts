import { IMailTemplateRspModel } from "./IMailTemplateRspModel";

export interface IIncentiveReqModel {
  name: string;
  welcomeSpeech: string;
  startDate: string;
  endDate: string;
  submitDeadlineDay: number;
  remindEmailDay: number;
  reSubmitDeadlineDay: number;
  rejectCount: number;
  fileIds: string[];
  mailTemplates: IMailTemplateRspModel[];
  checkPoints: string[];
}
