import { IUserRspModel } from "@/models";
import { ConnectProps, Dispatch, Effect } from "umi";
import { IModelState, ModelEffect, ModelReducer } from "@/utils";
import { getUsers, updateUser } from "@/services/user";

export const ModelName = "userModel";

export interface UserModelState extends IModelState<IUserRspModel, IUserRspModel> {
}

export interface PageProps extends ConnectProps {
  dispatch: Dispatch;
  pageState: UserModelState;
}

interface IndexModelType {
  namespace: string;
  state: UserModelState;
  effects: ModelEffect;
  reducers: ModelReducer<IUserRspModel>;
}

const initState = (): UserModelState => ({
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
      const { data } = yield call(getUsers, payload.requestParams);
      yield put({
        type: "update",
        payload: {
          modelList: data ? [...data.list] : undefined,
          loading: false,
          isEditing: false,
        },
      });
    },
    *updateModel({ payload }, { call, put }) {
      yield put({ type: "update", payload: { loading: true } });
      yield call(updateUser, payload.id, payload.requestParams);
      yield put({
        type: "update",
        payload: {
          currentModel: undefined,
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
