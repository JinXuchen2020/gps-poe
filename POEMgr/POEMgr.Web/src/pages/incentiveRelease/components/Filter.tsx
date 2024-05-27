import { FunctionComponent, useMemo } from "react";
import { Select, Form } from "antd";
import { ICensusQueryOption } from "@/models";
import { LocaleText } from "@/components";
import { getFiscalQuarters, getFiscalYears } from "@/utils";

export const Filter: FunctionComponent<{
  data: ICensusQueryOption | undefined;
  update: any;
}> = ({ data, update }) => {
  const fiscalYears = useMemo(() => {
    return getFiscalYears();
  }, []);

  const fiscalQuarters = useMemo(() => {
    return getFiscalQuarters();
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
      </Form>
    </>
  );
};
