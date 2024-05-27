import { AccountInfo } from "@azure/msal-browser";
import { IUserLiteRspModel } from "./IUserLiteRspModel";

export interface IUserTokenModel {
  user?: IUserLiteRspModel;
  token?: AccountInfo;
}
