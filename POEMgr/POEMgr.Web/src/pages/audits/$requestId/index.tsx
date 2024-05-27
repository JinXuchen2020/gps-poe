import { Modal } from "antd";
import { FC, useEffect, useState } from "react";
import { ModelName, PageProps } from "../model";
import { connect, useIntl, useNavigate, useParams } from "umi";
import { dispatchMethod, executeSendEmail } from "@/utils";
import {
  IAuditRequestReqModel,
  ISubscriptionStatusReqModel,
  IPoeRequestRspModel,
  IPoeRequestCheckPointReqModel,
} from "@/models";
import {
  Loading,
  PoeRequestForm,
  RequestContact,
  RequestStatusControl,
} from "@/components";
import { RejectModal } from "../components";

import "./index.less";
import { RequestStatus } from "@/constant";
import { sendRequestEmail } from "@/services/request";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, currentModel },
}) => {
  const navigate = useNavigate();
  const intl = useIntl();
  const { requestId } = useParams();
  const [showRejectModal, setShowRejectModal] = useState<boolean>(false);
  const [rejectReason, setRejectReason] = useState<string>();

  const handleSave = async (status: string | undefined = undefined) => {
    if (status === RequestStatus.Rejected) {
      showReject();
    } else if (status === RequestStatus.Approved) {
      handleApproval();
    } else {
      completeSave(status);
    }
  };

  const completeSave = async (
    status: string | undefined = undefined,
    reason: string | undefined = undefined
  ) => {
    if (currentModel) {
      const request: IAuditRequestReqModel = {
        incentiveId: currentModel.incentive.id,
        partnerId: currentModel.partner.id,
        customerId: currentModel.customer.id,
        status: status ?? currentModel.status,
        deadLineDate: currentModel.deadLineDate,
        requestFiles: [],
        requestStatus: [],
        auditStatus: currentModel.auditCheckPoints
          .filter((c) =>
            currentModel.incentive.checkPoints.map((c) => c.id).includes(c.id)
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
        }),
        reason: reason,
      };

      await dispatchMethod(dispatch, ModelName, "updateModel", {
        id: currentModel.id,
        requestParams: request,
      });
      navigate("/audits");
    }
  };

  const showReject = () => {
    setShowRejectModal(true);
    const rejectLogs = currentModel?.logs.filter((c) =>
      c.reason ? true : false
    );
    if (rejectLogs && rejectLogs.length > 0) {
      setRejectReason(rejectLogs[rejectLogs.length - 1].reason);
    }
  };

  const cancelReject = () => {
    setShowRejectModal(false);
  };

  const handleReject = async () => {
    setShowRejectModal(false);
    await completeSave(RequestStatus.Rejected, rejectReason);
    executeSendEmail(
      [currentModel!.id],
      "Notify-ReSubmit",
      intl.formatMessage({ id: "incentive.notification.sendLoadingText" }),
      intl.formatMessage({
        id: "incentive.notification.sendCompletedText",
      }),
      intl.formatMessage({ id: "incentive.notification.sendFailedText" })
    );
  };

  const handleApproval = async () => {
    if (currentModel && currentModel.customer) {
      if (
        currentModel.subscriptions.filter((c) => c.status !== "").length ===
        currentModel.customer.subscriptions.length
      ) {
        await completeSave(RequestStatus.Approved)
        executeSendEmail(
          [currentModel!.id],
          "Notify-Complete",
          intl.formatMessage({ id: "incentive.notification.sendLoadingText" }),
          intl.formatMessage({
            id: "incentive.notification.sendCompletedText",
          }),
          intl.formatMessage({ id: "incentive.notification.sendFailedText" })
        );
      } else {
        const titleText = intl.formatMessage({
          id: "audit.subscription.warning.titleText",
        });
        const contentText = intl.formatMessage({
          id: "audit.subscription.warning.contentText",
        });
        const okText = intl.formatMessage({
          id: "audit.subscription.warning.confirmText",
        });
        const cancelText = intl.formatMessage({
          id: "audit.subscription.warning.cancelText",
        });
        Modal.confirm({
          title: titleText,
          content: contentText,
          okText: okText,
          centered: true,
          cancelText: cancelText,
          onOk: async () => {
            await completeSave(RequestStatus.PartialApproved);
            executeSendEmail(
              [currentModel!.id],
              "Notify-Complete",
              intl.formatMessage({ id: "incentive.notification.sendLoadingText" }),
              intl.formatMessage({
                id: "incentive.notification.sendCompletedText",
              }),
              intl.formatMessage({ id: "incentive.notification.sendFailedText" })
            );
          },
        });
      }
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
        <span style={{ marginRight: 8 }}>{currentModel?.partner?.name}</span>
        <RequestStatusControl data={currentModel?.status} />
      </div>
      <div className="page-content">
        <RequestContact data={currentModel} />
        <PoeRequestForm
          data={currentModel}
          save={handleSave}
          update={handleUpdate}
          isAudit={true}
          disabled={false}
        />
      </div>
      <RejectModal
        show={showRejectModal}
        data={rejectReason}
        update={setRejectReason}
        save={handleReject}
        cancel={cancelReject}
      />
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
