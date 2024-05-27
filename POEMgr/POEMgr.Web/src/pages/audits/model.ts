import { IIncentiveLiteRspModel, IPoeRequestLiteRspModel, IPoeRequestRspModel } from "@/models";
import { ConnectProps, Dispatch, Effect } from "umi";
import { IModelState, ModelEffect, ModelReducer } from "@/utils";
import { getAuditRequest, getAuditRequests, updateAuditRequest } from "@/services/audit";
import { getIncentives } from "@/services/incentive";

export const ModelName = "AuditModel";

export interface AuditModelState extends IModelState<IPoeRequestLiteRspModel, IPoeRequestRspModel> {
  incentiveModels: IIncentiveLiteRspModel[] | undefined;
}

export interface PageProps extends ConnectProps {
  dispatch: Dispatch;
  pageState: AuditModelState;
}

interface AuditModelEffect extends ModelEffect {
  getIncentives: Effect
}

interface IndexModelType {
  namespace: string;
  state: AuditModelState;
  effects: AuditModelEffect;
  reducers: ModelReducer<IPoeRequestRspModel>;
}

const initState = (): AuditModelState => ({
  loading: false,
  pageTitle: undefined,
  modelList: undefined,
  currentModel: undefined,
  incentiveModels: undefined,
  isEditing: false
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
      const { data } = yield call(getAuditRequests, payload.requestParams);
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
      const { data } = yield call(getAuditRequest, payload.id);
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
      yield call(updateAuditRequest, payload.id, payload.requestParams);
      const { data } = yield call(getAuditRequest, payload.id);
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
