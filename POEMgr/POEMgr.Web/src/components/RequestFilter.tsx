import { FunctionComponent, useMemo } from "react";
import { Select, Form, Input } from "antd";
import { IIncentiveLiteRspModel } from "@/models";
import { IRequestQueryOption } from "@/models/request";
import { LocaleText } from "./LocaleText";
import { getStatus } from "@/utils";
import { useIntl } from "umi";

export const RequestFilter: FunctionComponent<{
  incentives: any[] | undefined;
  data: IRequestQueryOption | undefined;
  isAudit?: boolean;
  update: any;
}> = ({ incentives, data, isAudit, update }) => {
  const intl = useIntl();
  const status = useMemo(() => {
    return getStatus(intl);
  }, []);

  return (
    <>
      <Form layout={"inline"}>
        <Form.Item label={<LocaleText id={"filter.incentive.labelText"} />}>
          <Select
            options={incentives}
            value={data?.incentiveId}
            style={{ width: 300 }}
            placeholder={<LocaleText id={"filter.incentive.placeholderText"} />}
            showSearch
            optionFilterProp="label"
            allowClear={true}
            onChange={(value) =>
              update({ incentiveId: value, partnerName: undefined })
            }
          />
        </Form.Item>
        {isAudit ? (
          <Form.Item label={<LocaleText id={"filter.partner.labelText"} />}>
            <Input
              value={data?.partnerName}
              style={{ width: 300 }}
              autoComplete={"off"}
              onChange={(val) => update({ partnerName: val.target.value })}
            />
          </Form.Item>
        ) : (
          <div id={"statusArea"} className={"select-container"}>
            <Form.Item
              label={<LocaleText id={"census.filter.status.labelText"} />}
            >
              <Select
                options={status}
                value={data?.status}
                style={{ width: 100 }}
                placeholder={
                  <LocaleText id={"census.filter.status.placeholderText"} />
                }
                allowClear={true}
                getPopupContainer={() => document.getElementById("statusArea")!}
                onChange={(value) => update({ status: value })}
              />
            </Form.Item>
          </div>
        )}
      </Form>
    </>
  );
};
