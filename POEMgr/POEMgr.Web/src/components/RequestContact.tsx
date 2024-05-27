import { FunctionComponent } from "react";
import { Typography, DatePicker } from "antd";
import moment from "moment";
import { IPoeRequestRspModel } from "@/models";
import { LocaleText } from "./LocaleText";
import { RequestStatus } from "@/constant";

export const RequestContact: FunctionComponent<{
  data?: IPoeRequestRspModel;
  update?: any;
}> = ({ data, update }) => {
  return (
    <>
      <div className="contact">
        <div>
          <Typography.Text title={`${data?.partner.email}`} ellipsis={true}>
            <LocaleText
              id={"request.contact.emailText"}
              values={{ value: data?.partner.email }}
            />
          </Typography.Text>
        </div>
        <div>
          <Typography.Text title={`${data?.partner.id}`} ellipsis={true}>
            <LocaleText
              id={"request.contact.partnerOneIdText"}
              values={{ value: data?.partner.id }}
            />
          </Typography.Text>
        </div>
        <div>
          <Typography.Text title={`${data?.customer.name}`} ellipsis={true}>
            <LocaleText
              id={"request.contact.customerText"}
              values={{ value: data?.customer.name }}
            />
          </Typography.Text>
        </div>
      </div>
      <div className="contact-incentive">
        <div>
          <Typography.Text title={`${data?.incentive.name}`} ellipsis={true}>
            <LocaleText
              id={"request.contact.incentiveText"}
              values={{ value: data?.incentive.name }}
            />
          </Typography.Text>
        </div>
        <div>
          {update && data?.status !== RequestStatus.Approved && data?.status !== RequestStatus.Expired ? (
            <LocaleText
              id={"request.contact.deadlineText"}
              values={{
                value: (
                  <span id={"deadlineArea"} className={"select-container"}>
                    <DatePicker
                      style={{width: 200}}
                      value={moment(data?.deadLineDate)}
                      onChange={(value) =>
                        update(`${value?.format("YYYY-MM-DD")}T00:00:00`)
                      }
                      getPopupContainer={() =>
                        document.getElementById("deadlineArea")!
                      }
                      format="YYYY-MM-DD"
                    />
                  </span>
                ),
              }}
            />
          ) : (
            <LocaleText
              id={"request.contact.deadlineText"}
              values={{
                value: moment(data?.deadLineDate).format("YYYY-MM-DD"),
              }}
            />
          )}
        </div>
      </div>
    </>
  );
};
