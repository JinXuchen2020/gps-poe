import { IPoeRequestLiteRspModel } from "@/models";
import { ColumnsType } from "antd/lib/table";
import { DataIndex } from 'rc-table/lib/interface';
import { FunctionComponent, useState } from "react";
import { Table, Typography } from "antd";
import moment from "moment";
import { LocaleText, RequestStatusControl, ColumnSearchProps } from "@/components";
import { RequestStatus } from "@/constant";

export const PoeRequestTable: FunctionComponent<{
  data: IPoeRequestLiteRspModel[] | undefined;
  select: any;
  loading: boolean;
}> = ({ data, select, loading }) => {
  const [searchedColumn, setSearchedColumn] = useState<DataIndex>();
  const [searchText, setSearchText] = useState("");

  const handleSearch = (
    selectedKeys: string[],
    confirm: (param?: any) => void,
    dataIndex?: DataIndex
  ) => {
    confirm();
    setSearchText(selectedKeys[0]);
    setSearchedColumn(dataIndex);
  };

  const handleReset = (
    confirm: (param?: any) => void,
    clearFilters: () => void
  ) => {
    clearFilters();
    setSearchText("");
    handleSearch([], confirm);
  };

  const columns: ColumnsType<IPoeRequestLiteRspModel> = [
    {
      title: <LocaleText id={"request.table.fiscalQuarterText"} />,
      dataIndex: "fiscalQuarter",
      key: "fiscalQuarter",
      ellipsis: true,
      width: 100,
      sorter: (left, right) =>
        left.fiscalQuarter! > right.fiscalQuarter! ? -1 : 0,
    },
    {
      title: <LocaleText id={"request.table.partnerNameText"} />,
      dataIndex: "partnerName",
      ...ColumnSearchProps("partnerName", searchedColumn, searchText, handleSearch, handleReset),
      key: "partnerName",
      ellipsis: true,
    },
    {
      title: <LocaleText id={"request.table.customerNameText"} />,
      dataIndex: "customerName",
      key: "customerName",
      ...ColumnSearchProps("customerName", searchedColumn, searchText, handleSearch, handleReset),
      ellipsis: true,
    },
    {
      title: <LocaleText id={"request.table.countdownText"} />,
      dataIndex: "deadlineDate",
      key: "endTime",
      width: 120,
      render: (value: string | undefined, record: IPoeRequestLiteRspModel) =>
        getEndTime(record),
    },
    {
      title: <LocaleText id={"request.table.deadlineText"} />,
      dataIndex: "deadlineDate",
      key: "deadlineDate",
      width: 100,
      render: (value: string | undefined) =>
        value ? moment(value).format("YYYY-MM-DD") : undefined,
    },
    {
      title: <LocaleText id={"request.table.statusText"} />,
      dataIndex: "status",
      key: "status",
      width: 120,
      render: (value: string) => <RequestStatusControl data={value} />,
    },
    {
      title: <LocaleText id={"request.table.actionText"} />,
      key: "action",
      width: 100,
      render: (text, record) => {
        return (
          <Typography.Link
            onClick={() => select(record)}
            style={{ marginRight: 8 }}
          >
            {getActionText(record)}
          </Typography.Link>
        );
      },
    },
  ];

  const getEndTime = (record: IPoeRequestLiteRspModel) => {
    if (record.completedDate) {
      return undefined;
    } else if (record.deadlineDate) {
      const hours = moment(record.deadlineDate).diff(moment(), "hours");
      if (hours > 0) {
        const timeDay = Math.floor(hours / 24);
        const timeHour = hours % 24;
        return (
          <LocaleText
            id={"request.table.countdown.valueText"}
            values={{ day: timeDay, hour: timeHour }}
          />
        );
      }
    }
  };

  const getActionText = (record: IPoeRequestLiteRspModel) => {
    let actionTextId = "request.table.action.viewText";
    if (record.status === RequestStatus.EmailSent) {
      actionTextId = "request.table.action.submitText";
    } else if (record.status === RequestStatus.Rejected) {
      actionTextId = "request.table.action.resubmitText";
    }

    return <LocaleText id={actionTextId} />;
  };

  return (
    <Table
      loading={loading}
      size={"small"}
      dataSource={data}
      bordered={true}
      pagination={false}
      rowKey={(record) => record.id}
      columns={columns}
    />
  );
};
