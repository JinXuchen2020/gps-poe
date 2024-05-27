import { IUserRspModel } from "@/models";
import { ColumnsType } from "antd/lib/table";
import { FunctionComponent } from "react";

import { Table, Typography } from "antd";
import { Role } from "@/constant";
import { LocaleText } from "@/components";
import { CollapseText } from "./CollapseText";

export const UserTable: FunctionComponent<{
  data: IUserRspModel[] | undefined;
  assignRole: any;
  disable: any;
  loading: boolean;
}> = ({ data, assignRole, disable, loading }) => {
  const columns: ColumnsType<IUserRspModel> = [
    {
      title: <LocaleText id={"user.table.emailText"} />,
      dataIndex: "partnerEmail",
      key: "partnerEmail",
      ellipsis: true,
    },
    {
      title: <LocaleText id={"user.table.roleText"} />,
      dataIndex: "roleName",
      key: "roleName",
      render: (value: string) =>
        <LocaleText id={Role[value as keyof typeof Role]} /> ?? value,
    },
    {
      title: <LocaleText id={"user.table.idText"} />,
      dataIndex: "partnerId",
      key: "partnerId",
      width: 100,
      ellipsis: true,
      render:(value: any, record: IUserRspModel) => <CollapseText data={record.partnerId.split(";")} prefix={record.id} />,
    },
    {
      title: <LocaleText id={"user.table.partnerNameText"} />,
      dataIndex: "partnerName",
      key: "partnerName",
      ellipsis: true,
      render:(value: any, record: IUserRspModel) => <CollapseText data={record.partnerName.split(";")} prefix={record.id} />,
    },
    {
      title: <LocaleText id={"user.table.statusText"} />,
      dataIndex: "isDisabled",
      key: "isDisabled",
    },
    {
      title: "",
      key: "action",
      render: (text, record) => {
        return (
          <div style={{display: 'flex', flexWrap: 'wrap'}}>
            <Typography.Link
              onClick={() => assignRole(record)}
              style={{ marginRight: 8 }}
              disabled={record.isDisabled === '是'}
            >
              <LocaleText id={"user.table.action.assignRoleText"} />
            </Typography.Link>
            <Typography.Link
              onClick={() => disable(record)}
              style={{ marginRight: 8 }}
            >
              {record.isDisabled === "是" ? (
                <LocaleText id={"user.table.action.enableText"} />
              ) : (
                <LocaleText id={"user.table.action.disableText"} />
              )}
            </Typography.Link>
          </div>
        );
      },
    },
  ];

  return (
    <Table
      loading={loading}
      size={"small"}
      dataSource={data}
      bordered={true}
      rowKey={(record) => record.id ?? record.partnerId}
      columns={columns}
    />
  );
};
