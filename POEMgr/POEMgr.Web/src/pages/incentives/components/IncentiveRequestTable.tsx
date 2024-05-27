import { IPoeRequestLiteRspModel } from "@/models";
import { ColumnsType } from "antd/lib/table";
import { FunctionComponent } from "react";

import { Table, ConfigProvider } from "antd";
import { RequestStatus } from "@/constant";
import { LocaleText, RequestStatusControl } from "@/components";

export const IncentiveRequestTable: FunctionComponent<{
  data: IPoeRequestLiteRspModel[] | undefined;
  selectedData: IPoeRequestLiteRspModel[] | undefined;
  handleSelect: any;
  loading: boolean;
}> = ({ data, selectedData, handleSelect, loading }) => {
  const columns: ColumnsType<IPoeRequestLiteRspModel> = [
    {
      title: <LocaleText id={"incentive.table.idText"} />,
      dataIndex: "partnerId",
      key: "partnerId",
      ellipsis: true,
      width: 100,
      sorter: (left, right) => (left.partnerId > right.partnerId ? -1 : 0),
    },
    {
      title: <LocaleText id={"incentive.table.partnerText"} />,
      dataIndex: "partnerName",
      key: "partnerName",
      ellipsis: true,
    },
    {
      title: <LocaleText id={"incentive.table.emailText"} />,
      dataIndex: "partnerEmail",
      key: "partnerEmail",
      ellipsis: true,
    },
    {
      title: <LocaleText id={"incentive.table.customerText"} />,
      dataIndex: "customerName",
      key: "customerName",
      ellipsis: true,
    },
    // {
    //   title: "订阅ID",
    //   dataIndex: "customer",
    //   key: "subscriptionIds",
    //   render: (value: ICustomerRspModel, record: IIncentiveRequestRspModel) => value.subscriptionIds.map(id=> <span>{id}</span>)
    // },
    {
      title: <LocaleText id={"incentive.table.statusText"} />,
      dataIndex: "status",
      key: "status",
      width: 100,
      render: (value: string, record) => (
        <RequestStatusControl
          data={value}
          className={
            value === RequestStatus.Draft
              ? `status-dot ${value.toLowerCase()}`
              : `status-dot submitted`
          }
          title={
            value === RequestStatus.Draft ? (
              <LocaleText id={"request.status.draftText"} />
            ) : (
              <LocaleText id={"request.status.draftSentText"} />
            )
          }
        />
      ),
    },
  ];

  const getSelectedKeys = () => {
    return selectedData?.map((c) => c.id);
  };

  return (
    <ConfigProvider
      renderEmpty={() => (
        <LocaleText id={"incentive.notification.importText"} />
      )}
    >
      <Table
        loading={loading}
        size={"small"}
        dataSource={data}
        rowSelection={{
          selections: [
            {
              key: "selectAll",
              text: (
                <LocaleText id={"incentive.table.selectAllText"} />
              ),
              onSelect: (keys) => {
                handleSelect(data);
              },
            },
            {
              key: "selectNone",
              text: (
                <LocaleText id={"incentive.table.selectNoneText"} />
              ),
              onSelect: (keys) => {
                handleSelect([]);
              },
            },
            {
              key: "notSend",
              text: (
                <LocaleText id={"incentive.table.notSendText"} />
              ),
              onSelect: (keys) => {
                const notSendData = data?.filter(
                  (c) => c.status === RequestStatus.Draft
                );
                handleSelect(notSendData);
              },
            },
          ],
          selectedRowKeys: getSelectedKeys(),
          onChange: (rowKeys, rows) => {
            handleSelect(rows);
          },
        }}
        bordered={true}
        rowKey={(record) => record.id}
        columns={columns}
      />
    </ConfigProvider>
  );
};
