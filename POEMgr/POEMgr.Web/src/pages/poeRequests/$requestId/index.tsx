import { FC, useEffect } from "react";
import { ModelName, PageProps } from "../model";
import { Modal } from "antd";
import { connect, useIntl, useNavigate, useParams } from "umi";
import { dispatchMethod } from "@/utils";
import {
  IPoeRequestReqModel,
  IPoeRequestRspModel,
  IPoeRequestCheckPointReqModel,
  ISubscriptionStatusReqModel,
} from "@/models";
import {
  Loading,
  PoeRequestForm,
  RequestContact,
  RequestStatusControl,
} from "@/components";

import "./index.less";
import { RequestStatus } from "@/constant";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, currentModel },
}) => {
  const navigate = useNavigate();
  const intl = useIntl();
  const { requestId } = useParams();

  const confirmSave = async (status: string | undefined = undefined) => {
    if (currentModel) {
      const titleText =
        status === RequestStatus.Submitted
          ? intl.formatMessage(
              { id: "request.confirm.title.submitText" },
              { value: currentModel.requestFiles.length }
            )
          : intl.formatMessage(
              { id: "request.confirm.title.saveText" },
              { value: currentModel.requestFiles.length }
            );
      const okText = intl.formatMessage({
        id: "request.confirm.button.okText",
      });
      const cancelText = intl.formatMessage({
        id: "request.confirm.button.cancelText",
      });
      Modal.confirm({
        title: titleText,
        okText: okText,
        centered: true,
        cancelText: cancelText,
        onOk: async () => {
          handleSave(status);
        },
      });
    }
  };

  const handleSave = async (status: string | undefined = undefined) => {
    if (currentModel) {
      const request: IPoeRequestReqModel = {
        incentiveId: currentModel.incentive.id,
        partnerId: currentModel.partner.id,
        customerId: currentModel.customer.id,
        status: status ?? currentModel.status,
        requestFiles: currentModel.requestFiles.map((file) => file.id),
        requestStatus:
          currentModel.status === RequestStatus.PartialApproved
            ? []
            : currentModel.requestCheckPoints
                .filter((c) =>
                  currentModel.incentive.checkPoints
                    .map((c) => c.id)
                    .includes(c.id)
                )
                .map((status) => {
                  return {
                    ...status,
                  } as IPoeRequestCheckPointReqModel;
                }),
        subscriptionStatus: currentModel.subscriptions.map((c) => {
          return {
            subscriptionId: c.subscriptionId,
            status: c.status,
          } as ISubscriptionStatusReqModel;
        })
      };

      await dispatchMethod(dispatch, ModelName, "updateModel", {
        id: currentModel.id,
        requestParams: request,
      });
      navigate("/poeRequests");
    }
  };

  const handleUpdate = (props: Partial<IPoeRequestRspModel>) => {
    dispatchMethod(dispatch, ModelName, "update", {
      currentModel: { ...currentModel, ...props },
    });
  };

  useEffect(() => {
    if (currentModel === undefined || currentModel.id !== requestId) {
      dispatchMethod(dispatch, ModelName, "getModel", {
        id: requestId as string,
      });
    }
  }, [requestId]);

  useEffect(() => {
    const currentPath = location.pathname;
    return () => {
      if (
        location.pathname !== currentPath &&
        location.pathname !== "/preview"
      ) {
        dispatchMethod(dispatch, ModelName, "initState");
      }
    };
  }, []);

  return (
    <>
      <Loading loading={loading} />
      <div className="page-title">
        <div style={{ marginRight: 8 }}>{currentModel?.partner.name}</div>
        <RequestStatusControl data={currentModel?.status} />
      </div>
      <div className="page-content">
        <RequestContact data={currentModel} />
        <PoeRequestForm
          data={currentModel}
          save={confirmSave}
          update={handleUpdate}
          disabled={false}
        />
      </div>
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
