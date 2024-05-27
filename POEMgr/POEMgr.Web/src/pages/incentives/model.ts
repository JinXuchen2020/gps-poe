import { IModelRsp, IPoeRequestLiteRspModel } from "@/models";
import { ConnectProps, Dispatch, Effect } from "umi";
import { IModelState, ModelEffect, ModelReducer } from "@/utils";
import { sendEmail, importExcel } from "@/services/incentive";

export const ModelName = "incentiveModel";

export interface IncentiveModelState extends IModelState<IPoeRequestLiteRspModel, IPoeRequestLiteRspModel> {
  selectedModels: IPoeRequestLiteRspModel[] | undefined
}

export interface PageProps extends ConnectProps {
  dispatch: Dispatch;
  pageState: IncentiveModelState;
}

interface IncentiveModelEffect extends ModelEffect {
  importExcel: Effect;
  sendEmail: Effect;
}

interface IndexModelType {
  namespace: string;
  state: IncentiveModelState;
  effects: IncentiveModelEffect;
  reducers: ModelReducer<IPoeRequestLiteRspModel>;
}

const initState = (): IncentiveModelState => ({
  loading: false,
  pageTitle: undefined,
  modelList: undefined,
  selectedModels: undefined,
  currentModel: undefined,
  isEditing: false,
  showNotification: undefined,
});

const IndexModel: IndexModelType = {
  namespace: ModelName,
  state: initState(),
  effects: {
    *initState({ payload }, { call, put, select }) {
      yield put({
        type: "update",
        payload: initState(),
      });
    },
    *importExcel({ payload }, { call, put }) {
      yield put({ type: "update", payload: { loading: true } });
      const dataWrapper = (yield call(importExcel, payload.file)) as IModelRsp<IPoeRequestLiteRspModel[]>;
      if (dataWrapper.code === -1) {
        yield put({
          type: "update",
          payload: {
            errMsg: dataWrapper.msg,
            showNotification: true,
            loading: false,
          },
        });        
      }
      else {
        yield put({
          type: "update",
          payload: {
            modelList: dataWrapper.data ? dataWrapper.data : undefined,
            loading: false,
          },
        });
      }
    },
    *sendEmail({ payload }, { call, put, select }) {
      yield put({ type: "update", payload: { loading: true } });
      yield call(sendEmail, payload.requests);
      yield put({
        type: "update",
        payload: {         
          loading: false,
        },
      });
    },
  },
  reducers: {
    update: (state, action) => ({ ...state, ...action.payload }),
  },
};

export default IndexModel;
