import { RequestStatus } from "@/constant";
import { Select, Button, Space } from "antd";
import {
  IncentiveRequestTable,
  SendEmailModal,
  UploadControl,
} from "./components";
import { FC, ReactNode, useEffect, useMemo, useState } from "react";
import { ModelName, PageProps } from "./model";
import { connect, useIntl } from "umi";
import { dispatchMethod, executeSendEmail } from "@/utils";
import { IPoeRequestLiteRspModel } from "@/models";
import { Loading, LocaleText, NotificationBar } from "@/components";

import "./index.less";
import { sendRequestEmail } from "@/services/request";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, modelList, selectedModels, showNotification, errMsg },
}) => {
  const intl = useIntl();
  const [incentive, setIncentive] = useState<{
    key: number;
    value: string;
    label: string;
  }>();
  const [showSendEmailModal, setShowSendEmailModal] = useState<boolean>(false);
  const [notification, setNotification] = useState<{
    type: string;
    content: ReactNode;
  }>();

  const showSendEmail = async () => {
    if (selectedModels === undefined || selectedModels.length === 0) {
      setNotification({
        type: "warning",
        content: <LocaleText id={"incentive.notification.selectText"} />,
      });
      dispatchMethod(dispatch, ModelName, "update", {
        showNotification: true,
      });
    } else {
      const sentModels = selectedModels.filter(
        (c) => c.status !== RequestStatus.Draft
      );
      if (sentModels.length > 0) {
        setShowSendEmailModal(true);
      } else {
        const requestIds = await handleSendEmail(false);
        executeSendEmail(
          requestIds!,
          "Notify-First",
          intl.formatMessage({ id: "incentive.notification.sendLoadingText" }),
          intl.formatMessage({
            id: "incentive.notification.sendCompletedText",
          }),
          intl.formatMessage({ id: "incentive.notification.sendFailedText" })
        );
      }
    }
  };

  const handleSendEmail = async (duplicate: boolean) => {
    if (selectedModels && modelList) {
      setShowSendEmailModal(false);
      let requests = selectedModels
        .filter((c) => c.status === RequestStatus.Draft)
        .map((c) => c.id);
      if (duplicate) {
        requests = selectedModels.map((c) => c.id);
      }
      await dispatchMethod(dispatch, ModelName, "sendEmail", {
        requests: requests,
      });

      modelList
        .filter(
          (c) => requests.includes(c.id) && c.status === RequestStatus.Draft
        )
        .forEach((c) => {
          c.status = RequestStatus.EmailSent;
        });

      setNotification({
        type: "success",
        content: <LocaleText id={"incentive.notification.sendingText"} />,
      });

      dispatchMethod(dispatch, ModelName, "update", {
        modelList: [...modelList],
        selectedModels: undefined,
        showNotification: true,
      });

      return requests;
    }
  };

  const incentiveRequests = useMemo(() => {
    if (incentive && modelList) {
      return modelList.filter((c) => c.incentiveId === incentive.value);
    } else {
      return undefined;
    }
  }, [incentive, modelList]);

  const selectStatus = useMemo(() => {
    if (incentive) {
      return "";
    } else {
      return "error";
    }
  }, [incentive?.value]);

  const handleSelectRequest = (selectedRows: IPoeRequestLiteRspModel[]) => {
    dispatchMethod(dispatch, ModelName, "update", {
      selectedModels: [...selectedRows],
    });
  };

  const handleImport = async (file: FormData) => {
    await dispatchMethod(dispatch, ModelName, "importExcel", {
      file: file,
    });
  };

  const handleSelectIncentive = (value: string) => {
    if (getOptions) {
      const incentive = getOptions.find((c) => c.value === value);
      setIncentive(incentive);
      dispatchMethod(dispatch, ModelName, "update", {
        selectedModels: [],
      });
    }
  };

  const getOptions = useMemo(() => {
    let options: { key: number; value: string; label: string }[] = [];
    modelList?.forEach((c, index) => {
      if (options.findIndex((t) => t.value === c.incentiveId) < 0) {
        options.push({
          key: index,
          value: c.incentiveId,
          label: c.incentiveName,
        });
      }
    });
    return options;
  }, [modelList]);

  useEffect(() => {
    if (errMsg) {
      setNotification({
        type: "error",
        content: errMsg,
      });
    }
  }, [errMsg]);

  useEffect(() => {
    const currentPath = location.pathname;
    return () => {
      if (!location.pathname.includes(currentPath)) {
        dispatchMethod(dispatch, ModelName, "initState");
      }
    };
  }, []);

  return (
    <>
      <Loading loading={loading} />
      <NotificationBar
        content={notification?.content}
        type={notification?.type}
        hidden={!showNotification}
        close={() => {
          dispatchMethod(dispatch, ModelName, "update", {
            errMsg: undefined,
            showNotification: undefined,
          });
        }}
      />
      <div className="page-title">
        <LocaleText id={"incentive.titleText"} />
      </div>
      <div className="page-content">
        <Space size={"large"}>
          <UploadControl handleUpdate={handleImport} />
          <Select
            options={getOptions}
            style={{ width: 300 }}
            placeholder={<LocaleText id={"incentive.select.placeholderText"} />}
            allowClear={true}
            status={selectStatus}
            onChange={handleSelectIncentive}
          />
          <Button
            disabled={incentive === undefined}
            type="default"
            onClick={showSendEmail}
          >
            <LocaleText id={"incentive.button.sendEmailText"} />
          </Button>
        </Space>
        <div className="table-content">
          <IncentiveRequestTable
            data={incentiveRequests ?? modelList}
            selectedData={selectedModels}
            handleSelect={handleSelectRequest}
            loading={false}
          />
        </div>
      </div>
      <SendEmailModal
        data={selectedModels}
        incentive={incentive}
        show={showSendEmailModal}
        send={async () => {
          const requestIds = await handleSendEmail(true);
          executeSendEmail(
            requestIds!,
            "Notify-First",
            intl.formatMessage({
              id: "incentive.notification.sendLoadingText",
            }),
            intl.formatMessage({
              id: "incentive.notification.sendCompletedText",
            }),
            intl.formatMessage({ id: "incentive.notification.sendFailedText" })
          );
        }}
        cancel={async () => {
          const requestIds = await handleSendEmail(false);
          executeSendEmail(
            requestIds!,
            "Notify-First",
            intl.formatMessage({
              id: "incentive.notification.sendLoadingText",
            }),
            intl.formatMessage({
              id: "incentive.notification.sendCompletedText",
            }),
            intl.formatMessage({ id: "incentive.notification.sendFailedText" })
          );
        }}
      />
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
