import { IIncentiveLiteRspModel, IPoeRequestLiteRspModel } from "@/models";
import { ConnectProps, Dispatch, Effect } from "umi";
import { IModelState, ModelEffect, ModelReducer } from "@/utils";
import { getIncentives } from "@/services/incentive";
import { getPoeRequest, getPoeRequests } from "@/services/request";

export const ModelName = "HistoryModel";

export interface HistoryModelState
  extends IModelState<IPoeRequestLiteRspModel, any> {
  incentiveModels: IIncentiveLiteRspModel[] | undefined;
}

export interface PageProps extends ConnectProps {
  dispatch: Dispatch;
  pageState: HistoryModelState;
}

interface HistoryModelEffect extends ModelEffect {
  getIncentives: Effect;
}

interface IndexModelType {
  namespace: string;
  state: HistoryModelState;
  effects: HistoryModelEffect;
  reducers: ModelReducer<any>;
}

const initState = (): HistoryModelState => ({
  loading: false,
  pageTitle: undefined,
  modelList: undefined,
  currentModel: undefined,
  incentiveModels: undefined,
  isEditing: false,
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
    *getAll({ payload }, { call, put }) {
      yield put({ type: "update", payload: { loading: true } });
      const { data } = yield call(getPoeRequests, payload.requestParams);
      yield put({
        type: "update",
        payload: {
          modelList: data ? [...data] : undefined,
          loading: false,
        },
      });
    },
    *getIncentives({ payload }, { call, put }) {
      yield put({ type: "update", payload: { loading: true } });
      const { data } = yield call(getIncentives);
      yield put({
        type: "update",
        payload: {
          incentiveModels: data ? [...data] : undefined,
          loading: false,
        },
      });
    },
    *getModel({ payload }, { call, put }) {
      yield put({ type: "update", payload: { loading: true } });
      const { data } = yield call(getPoeRequest, payload.id);
      yield put({
        type: "update",
        payload: {
          currentModel: data,
          loading: false,
          isEditing: false,
        },
      });
    },
  },
  reducers: {
    update: (state, action) => ({ ...state, ...action.payload }),
  },
};

export default IndexModel;
