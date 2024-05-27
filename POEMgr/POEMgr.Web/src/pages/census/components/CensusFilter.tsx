import { FunctionComponent, useMemo } from "react";
import { Select, Form, Input } from "antd";
import { ICensusQueryOption, IIncentiveLiteRspModel } from "@/models";
import { useIntl } from "umi";
import { LocaleText } from "@/components";
import { getFiscalQuarters, getFiscalYears, getStatus } from "@/utils";

export const CensusFilter: FunctionComponent<{
  incentives: IIncentiveLiteRspModel[] | undefined;
  data: ICensusQueryOption | undefined;
  update: any;
}> = ({ incentives, data, update }) => {
  const intl = useIntl();
  const getIncentives = useMemo(() => {
    return incentives?.map((c, index) => {
      return {
        key: index,
        value: c.id,
        label: c.name,
      };
    });
  }, [incentives]);

  const fiscalYears = useMemo(() => {
    return getFiscalYears();
  }, []);

  const fiscalQuarters = useMemo(() => {
    return getFiscalQuarters();
  }, []);

  const status = useMemo(() => {
    return getStatus(intl);
  }, []);

  return (
    <>
      <Form layout={"inline"}>
        <div id={"yearArea"} className={"select-container"}>
          <Form.Item label={<LocaleText id={"census.filter.year.labelText"} />}>
            <Select
              options={fiscalYears}
              value={data?.fiscalYear}
              style={{ width: 100 }}
              placeholder={
                <LocaleText id={"census.filter.year.placeholderText"} />
              }
              allowClear={true}
              getPopupContainer={() => document.getElementById("yearArea")!}
              onChange={(value) => update({ fiscalYear: value })}
            />
          </Form.Item>
        </div>
        <div id={"quarterArea"} className={"select-container"}>
          <Form.Item
            label={<LocaleText id={"census.filter.quarter.labelText"} />}
          >
            <Select
              options={fiscalQuarters}
              value={data?.fiscalQuarter}
              style={{ width: 100 }}
              placeholder={
                <LocaleText id={"census.filter.quarter.placeholderText"} />
              }
              allowClear={true}
              getPopupContainer={() => document.getElementById("quarterArea")!}
              onChange={(value) => update({ fiscalQuarter: value })}
            />
          </Form.Item>
        </div>
        <div id={"incentiveArea"} className={"select-container"}>
          <Form.Item
            label={<LocaleText id={"census.filter.incentive.labelText"} />}
          >
            <Select
              options={getIncentives}
              value={data?.incentiveId}
              style={{ width: 200 }}
              placeholder={
                <LocaleText id={"census.filter.incentive.placeholderText"} />
              }
              showSearch
              optionFilterProp={"label"}
              allowClear={true}
              getPopupContainer={() =>
                document.getElementById("incentiveArea")!
              }
              onChange={(value) =>
                update({ incentiveId: value, partnerName: undefined })
              }
            />
          </Form.Item>
        </div>
        <Form.Item
          label={<LocaleText id={"census.filter.partner.labelText"} />}
        >
          <Input
            value={data?.partnerName}
            style={{ width: 200 }}
            autoComplete={"off"}
            allowClear={true}
            onChange={(val) => update({ partnerName: val.target.value })}
          />
        </Form.Item>
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
      </Form>
    </>
  );
};
