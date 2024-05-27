import { IIncentiveLiteRspModel } from "@/models";
import { ColumnsType } from "antd/lib/table";
import { DataIndex } from 'rc-table/lib/interface';
import { FunctionComponent, useState } from "react";
import { Table, Typography } from "antd";
import moment from "moment";
import { ColumnSearchProps, LocaleText } from "@/components";

export const IncentiveTable: FunctionComponent<{
  data: IIncentiveLiteRspModel[] | undefined;
  select: any;
  copy: any;
  loading: boolean;
}> = ({ data, select, copy, loading }) => {
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

  const columns: ColumnsType<IIncentiveLiteRspModel> = [
    {
      title: <LocaleText id={"incentiveRelease.table.incentiveText"} />,
      dataIndex: "name",
      ...ColumnSearchProps("name", searchedColumn, searchText, handleSearch, handleReset),
      key: "name",
      ellipsis: true
    },
    {
      title: <LocaleText id={"incentiveRelease.table.dateText"} />,
      dataIndex: "startDate",
      key: "startDate",
      ellipsis: true,
      render: (value: Date, record: IIncentiveLiteRspModel) => `${moment(record.startDate).format('YYYY-MM-DD')} 至 ${moment(record.endDate).format('YYYY-MM-DD')}`,
      sorter: (a, b) => compareDate(a, b),
      sortDirections: ['ascend', 'descend', 'ascend'],
      defaultSortOrder: 'descend',
    },
    {
      title: <LocaleText id={"incentiveRelease.table.createdNameText"} />,
      dataIndex: "createdName",
      ...ColumnSearchProps("createdName", searchedColumn, searchText, handleSearch, handleReset),
      key: "createdName",
      ellipsis: true
    },
    {
      title: <LocaleText id={"incentiveRelease.table.actionText"} />,
      key: "action",
      width: 120,
      render: (text, record) => {
        return (
          <div style={{display: 'flex', flexWrap: 'wrap'}}>            
            <Typography.Link
              onClick={() => select(record)}
              style={{ marginRight: 8 }}
            >
              <LocaleText id={"incentiveRelease.table.action.editText"} />
            </Typography.Link>
            <Typography.Link
              onClick={() => copy(record)}
              style={{ marginRight: 8 }}
            >
              <LocaleText id={"incentiveRelease.table.action.copyText"} />
            </Typography.Link>
          </div>
        );
      },
    },
  ];

  const compareDate = (a: IIncentiveLiteRspModel, b: IIncentiveLiteRspModel)=> {
    const left = moment(a.startDate).unix();
    const right = moment(b.startDate).unix();
    return left - right;
  }

  return (
    <Table
      loading={loading}
      size={'large'}
      dataSource={data}
      bordered={true}
      rowKey={(record) => record.id}
      columns={columns}
    />
  );
};
