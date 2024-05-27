import { LocaleText } from "@/components";
import { Modal, Button } from "antd";
import { FC, ReactNode, useMemo } from "react";

export const ReasonModal: FC<{
  show: boolean;
  title: ReactNode;
  data: string | undefined;
  cancel: any;
}> = ({ show, title, data, cancel }) => {
  const innerHtml = useMemo(() => {
    if (data) {
      return { __html: data };
    } else {
      return { __html: "" };
    }
  }, [data]);

  return (
    <Modal
      title={title}
      centered
      width={800}
      open={show}
      onCancel={cancel}
      footer={
        <Button onClick={cancel}>
          <LocaleText id={"request.rejectReason.button.closeText"} />
        </Button>
      }
    >
      <div className="modal-content">
        <div dangerouslySetInnerHTML={innerHtml} />
      </div>
    </Modal>
  );
};
