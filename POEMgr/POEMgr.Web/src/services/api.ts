import axios, { AxiosRequestConfig, AxiosRequestHeaders } from 'axios';
import { AuthService } from './authService';

const apiRoot = process.env.API_ROOT;

let errorHandler: (err: any) => void = () => {};

export const setErrorHandler = (handler: (err: any) => void) => {
  errorHandler = handler;
};

export const authService = new AuthService();

export interface IFetchProps {
  method:
    | 'GET'
    | 'POST'
    | 'POSTFILES'
    | 'PUT'
    | 'PATCH'
    | 'DELETE'
    | 'DOWNLOAD';
  url: string;
  body: any | undefined;
}

export const $fetch = async <T>(args: IFetchProps): Promise<T> => {
  const token = process.env.NODE_ENV === 'production' ? await authService.getAccessToken(): '';
  let headers: Partial<AxiosRequestHeaders> = {
    'X-Requested-With': 'XMLHttpRequest',
  };
  if (token) {
    headers['Authorization'] = `Bearer ${token}`;
  }
  let options: AxiosRequestConfig;
  if (args.method === 'POSTFILES') {
    headers['Content-Type'] = 'multipart/form-data';
    options = {
      method: 'POST',
      headers: headers,
      data: args.body,
      cancelToken: axios.CancelToken.source().token,
    };
  } else if (args.method === 'DOWNLOAD') {
    options = {
      method: 'GET',
      headers: headers,
      responseType: 'blob',
      cancelToken: axios.CancelToken.source().token,
    };
  } else {
    headers['Content-Type'] = 'application/json';
    options = {
      method: args.method,
      headers: headers,
      cancelToken: axios.CancelToken.source().token,
      data:
        args.body && args.method !== 'GET' && args.method !== 'DELETE'
          ? JSON.stringify(args.body)
          : undefined,
    };
  }

  let data: T = {} as T;

  try {
    let rsp = await axios(`${apiRoot}${args.url}`, options);
    if (rsp.status === 200) {
      data = await rsp.data;
    } else {
      let errData = await rsp.data;
      errorHandler(errData);
    }
  } catch (err) {
    errorHandler(err);
  }

  return data;
};
