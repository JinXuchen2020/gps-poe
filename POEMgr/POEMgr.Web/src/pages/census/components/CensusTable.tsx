import { IPoeRequestLiteRspModel } from "@/models";
import { ColumnsType } from "antd/lib/table";
import { FunctionComponent } from "react";

import { Table, Typography } from "antd";
import moment from "moment";
import { LocaleText, RequestStatusControl } from "@/components";

export const CensusTable: FunctionComponent<{
  data: IPoeRequestLiteRspModel[] | undefined;
  select?: any;
  loading: boolean;
}> = ({ data, select, loading }) => {
  const columns: ColumnsType<IPoeRequestLiteRspModel> = [
    {
      title: <LocaleText id={"census.table.fiscalQuarterText"} />,
      dataIndex: "fiscalQuarter",
      key: "fiscalQuarter",
      ellipsis: true,
      width: 100,
      sorter: (left, right) =>
        left.fiscalQuarter! > right.fiscalQuarter! ? -1 : 0,
    },
    {
      title: <LocaleText id={"census.table.incentiveText"} />,
      dataIndex: "incentiveName",
      key: "incentiveName",
      ellipsis: true,
    },
    {
      title: <LocaleText id={"census.table.quarterText"} />,
      dataIndex: "partnerName",
      key: "partnerName",
      ellipsis: true,
    },
    {
      title: <LocaleText id={"census.table.customerText"} />,
      dataIndex: "customerName",
      key: "customerName",
      ellipsis: true,
    },
    {
      title: <LocaleText id={"census.table.deadlineText"} />,
      dataIndex: "deadlineDate",
      key: "deadlineDate",
      width: 100,
      render: (value: string | undefined) =>
        value ? moment(value).format("YYYY-MM-DD") : undefined,
    },
    {
      title: <LocaleText id={"census.table.completeDateText"} />,
      dataIndex: "completedDate",
      key: "completedDate",
      ellipsis: true,
      width: 100,
      render: (value: string | undefined) =>
        value ? moment(value).format("YYYY-MM-DD") : undefined,
    },
    {
      title: <LocaleText id={"census.table.statusText"} />,
      dataIndex: "status",
      key: "status",
      width: 120,
      render: (value: string) => <RequestStatusControl data={value} />,
    },
    {
      title:
        select === undefined ? (
          ""
        ) : (
          <LocaleText id={"census.table.actionText"} />
        ),
      key: "action",
      width: select === undefined ? "0" : 80,
      render: (text, record) => {
        return (
          select && (
            <Typography.Link
              onClick={() => select(record)}
              style={{ marginRight: 8 }}
            >
              <LocaleText id={"census.table.action.viewText"} />
            </Typography.Link>
          )
        );
      },
    },
  ];

  return (
    <Table
      loading={loading}
      size={"middle"}
      dataSource={data}
      bordered={true}
      rowKey={(record) => record.id}
      columns={columns}
    />
  );
};
