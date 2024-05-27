import { ColumnsType } from "antd/lib/table";
import { DataIndex } from 'rc-table/lib/interface';
import { FunctionComponent, useEffect, useMemo, useState } from "react";
import { Table } from "antd";
import { LocaleText } from "./LocaleText";
import { ColumnSearchProps } from "./ColumnSearchProps";

interface ISubscription {
  subscriptionId: string;
}

export const SubscriptionTable: FunctionComponent<{
  data: string[] | undefined;
  selectedData: string[] | undefined;
  handleSelect: any;
  loading: boolean;
  disabled: boolean;
}> = ({ data, selectedData, handleSelect, loading, disabled }) => {
  const [selectedRowKeys, setSelectedRowKeys] = useState<React.Key[]>();
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

  const columns: ColumnsType<ISubscription> = [
    {
      title: <LocaleText id={"request.subscription.table.idText"} />,
      dataIndex: "subscriptionId",
      ...ColumnSearchProps("subscriptionId", searchedColumn, searchText, handleSearch, handleReset),
      key: "subscriptionId",
    },
  ];

  const subscriptions = useMemo(() => {
    return data?.map((c) => ({ subscriptionId: c } as ISubscription));
  }, [data]);

  useEffect(() => {
    if (selectedData) {
      setSelectedRowKeys(selectedData);
    }
  }, [selectedData]);

  return (
    <Table
      loading={loading}
      size={"small"}
      dataSource={subscriptions}
      pagination={false}
      scroll={{ y: 240 }}
      locale={{
        selectionAll: (
          <LocaleText id={"request.subscription.table.selectionAllText"} />
        ),
        selectNone: (
          <LocaleText id={"request.subscription.table.selectNoneText"} />
        ),
      }}
      rowSelection={{
        selections: [
          Table.SELECTION_ALL,
          Table.SELECTION_NONE,
          {
            key: "checked",
            text: <LocaleText id={"request.subscription.table.checkedText"} />,
            onSelect: (keys) => {
              setSelectedRowKeys(selectedData);
            },
          },
          {
            key: "unchecked",
            text: (
              <LocaleText id={"request.subscription.table.uncheckedText"} />
            ),
            onSelect: (keys) => {
              const unSelectedData = data?.filter(
                (c) => !selectedData?.includes(c)
              );
              setSelectedRowKeys(unSelectedData);
            },
          },
        ],
        getCheckboxProps: () => {
          return {
            disabled: disabled,
          };
        },
        selectedRowKeys: selectedRowKeys,
        onChange: (rowKeys, rows) => {
          handleSelect(rowKeys);
        },
      }}
      bordered
      rowKey={(record) => record.subscriptionId}
      columns={columns}
    />
  );
};
