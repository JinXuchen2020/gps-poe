import { IIncentiveRspModel, IIncentiveLiteRspModel } from "@/models";
import { ConnectProps, Dispatch } from "umi";
import { IModelState, ModelEffect, ModelReducer } from "@/utils";
import {
  createIncentive,
  getIncentive,
  getIncentives,
  updateIncentive,
} from "@/services/incentive";

export const ModelName = "incentiveReleaseModel";

export interface IncentiveReleaseModelState extends IModelState<IIncentiveLiteRspModel, IIncentiveRspModel> {
}

export interface PageProps extends ConnectProps {
  dispatch: Dispatch;
  pageState: IncentiveReleaseModelState;
}

interface IncentiveReleaseModelEffect extends ModelEffect {
}

interface IndexModelType {
  namespace: string;
  state: IncentiveReleaseModelState;
  effects: IncentiveReleaseModelEffect;
  reducers: ModelReducer<IIncentiveRspModel>;
}

const initState = (): IncentiveReleaseModelState => ({
  loading: false,
  pageTitle: undefined,
  modelList: undefined,
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
      const { data } = yield call(getIncentives);
      yield put({
        type: "update",
        payload: {
          modelList: data ? [...data] : undefined,
          loading: false,
        },
      });
    },
    *getModel({ payload }, { call, put }) {
      yield put({ type: "update", payload: { loading: true } });
      const { data } = yield call(getIncentive, payload.id);
      yield put({
        type: "update",
        payload: {
          currentModel: data,
          loading: false,
          isEditing: false,
        },
      });
    },
    *createModel({ payload }, { call, put }) {
      yield put({ type: "update", payload: { loading: true } });
      yield call(createIncentive, payload.requestParams);
      yield put({
        type: "update",
        payload: {
          loading: false,
          isEditing: true,
        },
      });
    },
    *updateModel({ payload }, { call, put }) {
      yield put({ type: "update", payload: { loading: true } });
      yield call(updateIncentive, payload.id, payload.requestParams);
      yield put({
        type: "update",
        payload: {
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
