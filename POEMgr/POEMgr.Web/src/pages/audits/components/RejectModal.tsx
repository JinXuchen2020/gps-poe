import { LocaleText } from "@/components";
import { Modal } from "antd";
import { FC } from "react";
import { EmailEditor } from "./EmailEditor";

export const RejectModal: FC<{
  show: boolean;
  data: string | undefined;
  update: any;
  save: any;
  cancel: any;
}> = ({ show, data, update, save, cancel }) => {
  return (
    <Modal
      title={<LocaleText id={"request.rejectReason.titleText"} />}
      centered
      width={800}
      open={show}
      okText={<LocaleText id={"request.rejectReason.button.submitText"} />}
      cancelText={<LocaleText id={"request.rejectReason.button.cancelText"} />}
      onOk={save}
      onCancel={cancel}
    >
      <div className="modal-content">
        <EmailEditor data={data} update={update} />
      </div>
    </Modal>
  );
};
