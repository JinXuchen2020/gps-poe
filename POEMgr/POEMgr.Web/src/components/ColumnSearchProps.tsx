import { Input, Space, Button } from "antd";
import { ColumnType } from "antd/lib/table";
import { DataIndex } from 'rc-table/lib/interface';
import { SearchOutlined } from "@ant-design/icons";
import Highlighter from "react-highlight-words";
import { LocaleText } from "./LocaleText";

export const ColumnSearchProps = (
  dataIndex: DataIndex,
  searchedColumn: DataIndex | undefined,
  searchText: string,
  search: any,
  reset: any
): ColumnType<any> => ({
  filterDropdown: ({
    setSelectedKeys,
    selectedKeys,
    confirm,
    clearFilters,
    close,
  }) => (
    <div style={{ padding: 8 }} onKeyDown={(e) => e.stopPropagation()}>
      <Input
        value={selectedKeys[0]}
        onChange={(e) =>
          setSelectedKeys(e.target.value ? [e.target.value] : [])
        }
        onPressEnter={() =>
          search(selectedKeys as string[], confirm, dataIndex)
        }
        style={{
          marginBottom: 8,
          display: "block",
          height: 30,
          lineHeight: 30,
        }}
      />
      <Space>
        <Button
          onClick={() =>
            search(selectedKeys as string[], confirm, dataIndex)
          }
          type={"primary"}
          size="small"
          style={{ width: 50 }}
        >
          <LocaleText id={"request.subscription.search.buttonText"} />
        </Button>
        <Button
          onClick={() => clearFilters && reset(confirm, clearFilters)}
          size="small"
          style={{ width: 50 }}
        >
          <LocaleText id={"request.subscription.search.resetText"} />
        </Button>
      </Space>
    </div>
  ),
  filterIcon: (filtered: boolean) => (
    <SearchOutlined style={{ color: filtered ? "#1890ff" : undefined }} />
  ),
  render: (text) =>
    searchedColumn === dataIndex ? (
      <Highlighter
        highlightStyle={{ backgroundColor: "#ffc069", padding: 0 }}
        searchWords={[searchText]}
        autoEscape
        textToHighlight={text ? text.toString() : ""}
      />
    ) : (
      text
    ),
  onFilter: (value, record) =>
    record[dataIndex as any]
      .toString()
      .toLowerCase()
      .includes((value as string).toLowerCase()),
})