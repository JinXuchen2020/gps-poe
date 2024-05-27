export interface IPoeRequestLiteRspModel {
  id: string;
  fiscalQuarter: string;
  incentiveId: string;
  incentiveName: string;
  incentiveWelcomeSpeech: string;
  fileName: string;
  poeTemplatePath: string;
  partnerId: string;
  partnerName: string;
  partnerEmail: string;
  customerName: string;
  startDate?: string;
  deadlineDate?: string;
  completedDate?: string;
  status: string;
}
