import { IIncentiveLiteRspModel, IPoeRequestLiteRspModel, IPoeRequestRspModel } from "@/models";
import { ConnectProps, Dispatch, Effect } from "umi";
import { IModelState, ModelEffect, ModelReducer } from "@/utils";
import { getPoeRequest, getPoeRequests, updatePoeRequest } from "@/services/request";
import { getIncentives } from "@/services/incentive";

export const ModelName = "requestModel";

export interface RequestModelState extends IModelState<IPoeRequestLiteRspModel, IPoeRequestRspModel> {
  incentiveModels: IIncentiveLiteRspModel[] | undefined;
}

export interface PageProps extends ConnectProps {
  dispatch: Dispatch;
  pageState: RequestModelState;
}

interface RequestModelEffect extends ModelEffect {
  getIncentives: Effect
}

interface IndexModelType {
  namespace: string;
  state: RequestModelState;
  effects: RequestModelEffect;
  reducers: ModelReducer<IPoeRequestRspModel>;
}

const initState = (): RequestModelState => ({
  loading: false,
  modelList: undefined,
  incentiveModels: undefined,
  currentModel: undefined,
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
      const { data } = yield call(getPoeRequests);
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
    *updateModel({ payload }, { call, put }) {
      yield put({ type: "update", payload: { loading: true } });
      yield call(updatePoeRequest, payload.id, payload.requestParams);
      const { data } = yield call(getPoeRequest, payload.id);
      yield put({
        type: "update",
        payload: {
          currentModel: data,
          loading: false,
          isEditing: true,
        },
      });
    },
  },
  reducers: {
    update: (state, action) => ({ ...state, ...action.payload }),
  },
};

export default IndexModel;
