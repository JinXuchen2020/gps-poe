import { Modal } from "antd";
import { AssignRoleModal, SearchForm, UserTable } from "./components";
import { FC, ReactNode, useEffect, useState } from "react";
import { ModelName, PageProps } from "./model";
import { connect, useIntl, useSearchParams } from "umi";
import { dispatchMethod } from "@/utils";
import { IUserQueryOption, IUserReqModel, IUserRspModel } from "@/models";
import { Loading, LocaleText, NotificationBar } from "@/components";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, modelList, currentModel },
}) => {
  const intl = useIntl();
  const [searchParams] = useSearchParams();
  const [showNotification, setShowNotification] = useState<boolean>(false);
  const [notificationContent, setNotificationContent] = useState<ReactNode>();
  const [notificationType, setNotificationType] = useState<
    "warning" | "success"
  >();

  const [showAssignRoleModal, setShowAssignRoleModal] =
    useState<boolean>(false);

  const displayNotification = (
    content: ReactNode,
    type: "warning" | "success"
  ) => {
    setShowNotification(true);
    setNotificationContent(content);
    setNotificationType(type);
  };

  const refresh = async () => {
    const query: Partial<IUserQueryOption> = Object.fromEntries(searchParams);
    await dispatchMethod(dispatch, ModelName, "getAll", {
      requestParams: query,
    });
  };

  const handleUpdate = (props: Partial<IUserRspModel>) => {
    dispatchMethod(dispatch, ModelName, "update", {
      currentModel: { ...currentModel, ...props },
    });
  };

  const showAssignRole = (data: IUserRspModel) => {
    dispatchMethod(dispatch, ModelName, "update", {
      currentModel: data,
    });

    setShowAssignRoleModal(true);
  };

  const showUserDisable = (data: IUserRspModel) => {
    const title =
      data.isDisabled === "否"
        ? intl.formatMessage(
            { id: "user.disableModal.title.disableText" },
            { value: data.partnerEmail }
          )
        : intl.formatMessage(
            { id: "user.disableModal.title.enableText" },
            { value: data.partnerEmail }
          );

    const okText =
      data.isDisabled === "否"
        ? intl.formatMessage({ id: "user.disableModal.okButton.disableText" })
        : intl.formatMessage({ id: "user.disableModal.okButton.enableText" });

    const cancelText = intl.formatMessage({
      id: "user.disableModal.cancelText",
    });
    const notificationText =
      data.isDisabled === "否"
        ? intl.formatMessage(
            { id: "user.notification.disableUserText" },
            { value: data.partnerEmail }
          )
        : intl.formatMessage(
            { id: "user.notification.enableUserText" },
            { value: data.partnerEmail }
          );
    Modal.confirm({
      title: title,
      okText: okText,
      centered: true,
      cancelText: cancelText,
      okButtonProps:
        data.isDisabled === "否"
          ? { style: { backgroundColor: "#ff4d4f" } }
          : undefined,
      onOk: async () => {
        const request: IUserReqModel = {
          ...data,
          isDisabled: `${data.isDisabled === "否" ? "是" : "否"}`,
        };

        await dispatchMethod(dispatch, ModelName, "updateModel", {
          id: data.id,
          requestParams: request,
        });

        displayNotification(notificationText, "success");
        await refresh();
      },
    });
  };

  const handleSave = async () => {
    if (currentModel) {
      const request: IUserReqModel = {
        ...currentModel,
        isDisabled: undefined,
      };

      setShowAssignRoleModal(false);
      await dispatchMethod(dispatch, ModelName, "updateModel", {
        id: currentModel.id,
        requestParams: request,
      });

      displayNotification(
        <LocaleText id={"user.notification.saveRoleText"} />,
        "success"
      );
      await refresh();
    }
  };

  const handleCancel = () => {
    dispatchMethod(dispatch, ModelName, "update", {
      currentModel: undefined,
    });

    setShowAssignRoleModal(false);
  };

  useEffect(() => {
    refresh();
  }, [searchParams]);

  useEffect(() => {
    refresh();
  }, [searchParams]);

  return (
    <>
      <Loading loading={loading} />
      <NotificationBar
        content={notificationContent}
        type={notificationType}
        hidden={!showNotification}
        close={() => setShowNotification(false)}
      />
      <div className="page-title">
        <LocaleText id={"user.titleText"} />
      </div>
      <div className="page-content">
        <SearchForm />
        <div className="table-content">
          <UserTable
            data={modelList}
            loading={false}
            assignRole={showAssignRole}
            disable={showUserDisable}
          />
        </div>
      </div>
      <AssignRoleModal
        data={currentModel}
        show={showAssignRoleModal}
        update={handleUpdate}
        save={handleSave}
        cancel={handleCancel}
      />
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
