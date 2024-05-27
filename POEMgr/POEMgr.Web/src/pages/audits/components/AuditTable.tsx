import { IPoeRequestLiteRspModel } from "@/models";
import { ColumnsType } from "antd/lib/table";
import { DataIndex } from 'rc-table/lib/interface';
import { FunctionComponent, useState } from "react";

import { Table, Typography } from "antd";
import { ColumnSearchProps, LocaleText, RequestStatusControl } from "@/components";

export const AuditTable: FunctionComponent<{
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
      title: <LocaleText id={"audit.table.incentiveNameText"} />,
      dataIndex: "incentiveName",
      ...ColumnSearchProps("incentiveName", searchedColumn, searchText, handleSearch, handleReset),
      key: "incentiveName",
      ellipsis: true
    },
    {
      title: <LocaleText id={"audit.table.partnerNameText"} />,
      dataIndex: "partnerName",
      ...ColumnSearchProps("partnerName", searchedColumn, searchText, handleSearch, handleReset),
      key: "partnerName",
      ellipsis: true
    },
    {
      title: <LocaleText id={"audit.table.partnerEmailText"} />,
      dataIndex: "partnerEmail",
      key: "partnerEmail",
      ellipsis: true
    },
    {
      title: <LocaleText id={"audit.table.customerNameText"} />,
      dataIndex: "customerName",
      key: "customerName",
      ellipsis: true
    },
    {
      title: <LocaleText id={"audit.table.statusText"} />,
      dataIndex: "status",
      key: "status",
      width: 100,
      render: (value: string) => (
        <RequestStatusControl
          data={value}
          title={<LocaleText id={"audit.table.status.unAuditText"} />}
        />
      ),
    },
    {
      title: <LocaleText id={"audit.table.actionText"} />,
      key: "action",
      width: 80,
      render: (text, record) => {
        return (
          <Typography.Link
            onClick={() => select(record)}
            style={{ marginRight: 8 }}
          >
            <LocaleText id={"audit.table.action.viewText"} />
          </Typography.Link>
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
