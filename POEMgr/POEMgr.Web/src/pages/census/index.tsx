import { RequestStatus } from "@/constant";
import { Button, Typography } from "antd";
import { CensusFilter, CensusTable, CardItem } from "./components";
import { FC, useEffect, useMemo, useState } from "react";
import { ModelName, PageProps } from "./model";
import { connect, useIntl, useNavigate } from "umi";
import {
  dispatchMethod,
  downloadFile,
  filterRequest,
  getFiscalQuarter,
} from "@/utils";
import {
  ICensusQueryOption,
  IPoeRequestLiteRspModel,
  IRequestQueryOption,
} from "@/models";
import { Loading, LocaleText } from "@/components";
import { DownloadOutlined } from "@ant-design/icons";
import { downloadPoeRequests } from "@/services/audit";

import "./index.less";
import moment from "moment";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, modelList, incentiveModels },
}) => {
  const navigate = useNavigate();
  const intl = useIntl();
  const [filterRequests, setFilterRequests] =
    useState<IPoeRequestLiteRspModel[]>();

  const currentFiscalQuarter = useMemo(() => {
    return getFiscalQuarter(new Date());
  }, []);

  const [dataFilter, setDataFilter] = useState<ICensusQueryOption>({
    fiscalYear: currentFiscalQuarter?.year,
    fiscalQuarter: currentFiscalQuarter?.quarter,
  });

  const filterIncentives = useMemo(() => {
    if (incentiveModels) {
      return incentiveModels.filter((c) => {
        let result = true;
        const fiscalQuarter = getFiscalQuarter(moment(c.startDate).toDate());
        if (fiscalQuarter) {
          if (dataFilter["fiscalYear"]) {
            result = result && dataFilter["fiscalYear"] === fiscalQuarter.year;
          }

          if (dataFilter["fiscalQuarter"]) {
            result =
              result && dataFilter["fiscalQuarter"] === fiscalQuarter.quarter;
          }
          return result;
        }
      });
    }
  }, [incentiveModels, dataFilter]);

  const getCards = useMemo(() => {
    return [
      "census.card.allText",
      "census.card.submitText",
      "census.card.emailSentText",
      "census.card.unAuditText",
      "census.card.rejectedText",
      "census.card.expiredText",
      "census.card.partialApprovedText",
      "census.card.approvedText",
    ].map((c) => {
      let data: number = 0;
      let status: string | undefined = undefined;
      switch (c) {
        case "census.card.allText":
          if (filterRequests) {
            data = filterRequests.filter(
              (c) => c.status !== RequestStatus.Draft
            ).length;
          }
          break;
        case "census.card.submitText":
          if (filterRequests) {
            data = filterRequests.filter(
              (c) => c.status === RequestStatus.Submitted
            ).length;
            status = RequestStatus.Submitted;
          }
          break;
        case "census.card.emailSentText":
          if (filterRequests) {
            data = filterRequests.filter(
              (c) => c.status === RequestStatus.EmailSent
            ).length;
            status = RequestStatus.EmailSent;
          }
          break;
        case "census.card.unAuditText":
          if (filterRequests) {
            data = filterRequests.filter(
              (c) => c.status === RequestStatus.Submitted
            ).length;
            status = RequestStatus.Submitted;
          }
          break;
        case "census.card.rejectedText":
          if (filterRequests) {
            data = filterRequests.filter(
              (c) => c.status === RequestStatus.Rejected
            ).length;
            status = RequestStatus.Rejected;
          }
          break;
        case "census.card.expiredText":
          if (filterRequests) {
            data = filterRequests.filter(
              (c) => c.status === RequestStatus.Expired
            ).length;
            status = RequestStatus.Expired;
          }
          break;
        case "census.card.partialApprovedText":
          if (filterRequests) {
            data = filterRequests.filter(
              (c) => c.status === RequestStatus.PartialApproved
            ).length;
            status = RequestStatus.PartialApproved;
          }
          break;
        case "census.card.approvedText":
          if (filterRequests) {
            data = filterRequests.filter(
              (c) => c.status === RequestStatus.Approved
            ).length;
            status = RequestStatus.Approved;
          }
          break;
      }
      return {
        title: intl.formatMessage({ id: c }),
        data: data,
        action: () =>
          dataFilter
            ? setDataFilter({ ...dataFilter, status: status })
            : setDataFilter({ status: status }),
      };
    });
  }, [filterRequests, dataFilter]);

  const handleFilter = (props: Partial<ICensusQueryOption>) => {
    if (dataFilter) {
      setDataFilter({ ...dataFilter, ...props });
    } else {
      setDataFilter(props);
    }
  };

  const handleSelect = (item: IPoeRequestLiteRspModel) => {
    navigate(`/census/${item.id}`);
  };

  const handleDownload = async () => {
    dispatchMethod(dispatch, ModelName, "update", {
      loading: true,
    });

    downloadPoeRequests({ ...dataFilter }).then((file: Blob) => {
      downloadFile(file, "POE列表.xlsx");
      dispatchMethod(dispatch, ModelName, "update", {
        loading: false,
      });
    });
  };

  const refresh = async () => {
    const query: Partial<IRequestQueryOption> = {};
    await dispatchMethod(dispatch, ModelName, "getAll", {
      requestParams: query,
    });
  };

  useEffect(() => {
    if (modelList) {
      dispatchMethod(dispatch, ModelName, "update", {
        loading: true,
      });
      new Promise((resolve) => {
        const result = filterRequest(dataFilter, modelList);
        resolve(result);
      }).then((result: any) => {
        setFilterRequests(result);
        dispatchMethod(dispatch, ModelName, "update", {
          loading: false,
        });
      });
    }
  }, [modelList, dataFilter]);

  useEffect(() => {
    refresh();
    dispatchMethod(dispatch, ModelName, "getIncentives");
    const currentPath = location.pathname;
    return () => {
      if (!location.pathname.includes(currentPath)) {
        dispatchMethod(dispatch, ModelName, "initState");
      }
    };
  }, []);

  return (
    <>
      <Loading loading={loading} />
      <div className="page-title">
        <LocaleText id={"census.titleText"} />
      </div>
      <div className="page-content">
        <div className="card-container">
          {getCards.map((c, index) => (
            <CardItem
              key={index}
              title={
                <Typography.Text ellipsis={true} title={c.title}>
                  {c.title}
                </Typography.Text>
              }
              data={c.data}
              action={c.action}
            />
          ))}
        </div>
        <div style={{ display: "flex", justifyContent: "space-between" }}>
          <CensusFilter
            incentives={filterIncentives}
            data={dataFilter}
            update={handleFilter}
          />
          <Button
            type="primary"
            icon={<DownloadOutlined />}
            onClick={handleDownload}
          >
            <LocaleText id={"census.button.exportText"} />
          </Button>
        </div>
        <div className="table-content">
          <CensusTable
            data={filterRequests}
            loading={false}
            select={handleSelect}
          />
        </div>
      </div>
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
