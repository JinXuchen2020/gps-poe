import { LocaleText } from "@/components";
import { Role } from "@/constant";
import { IUserRspModel } from "@/models";
import { Form, Modal, Select } from "antd";
import { FC, useMemo } from "react";

export const AssignRoleModal: FC<{
  data: IUserRspModel | undefined;
  show: boolean;
  update: any;
  save: any;
  cancel: any;
}> = ({ data, show, update, save, cancel }) => {
  const getOptions = useMemo(() => {
    return Object.keys(Role).map((c, index) => {
      return {
        key: index,
        value: c,
        label: <LocaleText id={Role[c as keyof typeof Role]} />,
      };
    });
  }, []);

  return (
    <Modal
      title={<LocaleText id={"user.assignModal.titleText"} />}
      centered
      open={show}
      okText={<LocaleText id={"user.assignModal.okText"} />}
      cancelText={<LocaleText id={"user.assignModal.cancelText"} />}
      onOk={save}
      onCancel={cancel}
    >
      <div className="modal-description">{data?.partnerEmail}</div>
      <div className="modal-content">
        <Form layout={"inline"}>
          <Form.Item
            label={<LocaleText id={"user.assignModal.role.labelText"} />}
          >
            <Select
              options={getOptions}
              style={{ width: 150 }}
              value={data?.roleName}
              onChange={(value) => update({ roleName: value })}
            />
          </Form.Item>
        </Form>
      </div>
    </Modal>
  );
};
