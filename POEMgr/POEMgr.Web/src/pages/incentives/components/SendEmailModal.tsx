import { LocaleText } from "@/components";
import { IPoeRequestLiteRspModel } from "@/models";
import { Modal, Space } from "antd";
import { FC } from "react";

export const SendEmailModal: FC<{
  data: IPoeRequestLiteRspModel[] | undefined;
  incentive: { key: number; value: string; label: string } | undefined;
  show: boolean;
  send: any;
  cancel: any;
}> = ({ data, incentive, show, send, cancel }) => {
  return (
    <Modal
      title={<LocaleText id={"incentive.sendEmail.titleText"} />}
      centered
      open={show}
      okText={<LocaleText id={"incentive.sendEmail.okText"} />}
      cancelText={<LocaleText id={"incentive.sendEmail.cancelText"} />}
      onOk={send}
      onCancel={cancel}
    >
      <div className="modal-description">
        <LocaleText
          id={"incentive.sendEmail.descriptionText"}
          values={{ value: incentive?.label }}
        />
      </div>
      <div className="modal-content">
        <Space direction="vertical">
          {data?.filter((c) => c.status !== "Draft").map((c) => c.partnerName)}
        </Space>
      </div>
    </Modal>
  );
};
