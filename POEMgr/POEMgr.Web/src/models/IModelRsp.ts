export interface IModelRsp<T> {
  code: number;
  msg: string;
  data: T;
}
