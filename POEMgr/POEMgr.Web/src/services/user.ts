import { IListRsp, IModelRsp, IUserLiteRspModel, IUserQueryOption, IUserReqModel, IUserRspModel } from "@/models";
import { $fetch, authService, IFetchProps } from "./api";
import queryString from 'query-string';

export const getUsers = async (query: Partial<IUserQueryOption>) => {
  const params = {
    method: "GET",
    url: queryString.stringifyUrl({ url: 'users', query: { ...query } }),
  } as IFetchProps;
  return $fetch<IModelRsp<IListRsp<IUserRspModel>>>(params);
};

export const getLoginUser = async () => {
  const params = {
    method: 'GET',
    url: `users/currentUser`,
  } as IFetchProps;
  return $fetch<IModelRsp<IUserLiteRspModel>>(params);
};

export const updateUser = async (id: string, request: IUserReqModel) => {
  const params = {
    method: "PUT",
    url: `users/${id}`,
    body: request
  } as IFetchProps;
  return $fetch(params);
};

export const logout = async () => {
  await authService.logout()
};
