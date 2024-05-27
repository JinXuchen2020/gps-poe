import { IPoeRequestLiteRspModel } from "@/models";
import { FunctionComponent, useMemo } from "react";
import { Typography } from "antd";
import { PoeRequestTable } from "./PoeRequestTable";
import { useNavigate } from "umi";
import { LocaleText } from "@/components";

export const IncentiveRequest: FunctionComponent<{
  data: IPoeRequestLiteRspModel[] | undefined;
}> = ({ data }) => {
  const navigate = useNavigate();
  const incentive = useMemo(() => {
    if (data) {
      return {
        id: data[0].incentiveId,
        name: data[0].incentiveName,
        welcomeSpeech: data[0].incentiveWelcomeSpeech,
        poeTemplatePath: data[0].poeTemplatePath,
        poeTemplateName: data[0].fileName,
      };
    } else {
      return undefined;
    }
  }, [data]);

  const handleSelect = (item: IPoeRequestLiteRspModel) => {
    navigate(`/poeRequests/${item.id}`);
  };

  return (
    <>
      <div className="request-title">{incentive?.name}</div>
      <div style={{whiteSpace: 'pre-wrap'}}>
        {incentive?.welcomeSpeech ?? (
          <LocaleText
            id={"request.descriptionText"}
            values={{ line: <br />, incentive: incentive?.name }}
          />
        )}
        <Typography.Link href={incentive?.poeTemplatePath}>
          <LocaleText
            id={"request.description.downloadText"}
            values={{ value: incentive?.poeTemplateName }}
          />
        </Typography.Link>
      </div>
      <div className="table-content">
        <PoeRequestTable data={data} select={handleSelect} loading={false} />
      </div>
    </>
  );
};
