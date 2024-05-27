import { Button } from "antd";
import { HistoryFilter } from "./components";
import { FC, useEffect, useMemo, useState } from "react";
import { ModelName, PageProps } from "./model";
import { connect, useNavigate } from "umi";
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

import "./index.less";
import { CensusTable } from "../census/components";
import { downloadPoeRequests } from "@/services/request";
import moment from "moment";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, modelList, incentiveModels },
}) => {
  const navigate = useNavigate();
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

  const handleFilter = (props: Partial<ICensusQueryOption>) => {
    if (dataFilter) {
      setDataFilter({ ...dataFilter, ...props });
    } else {
      setDataFilter(props);
    }
  };

  const handleSelect = (item: IPoeRequestLiteRspModel) => {
    navigate(`/histories/${item.id}`);
  };

  const handleDownload = async () => {
    dispatchMethod(dispatch, ModelName, "update", {
      loading: true,
    });

    downloadPoeRequests({ ...dataFilter }).then((file: Blob) => {
      downloadFile(file, "POE历史.xlsx");
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
        <LocaleText id={"history.titleText"} />
      </div>
      <div className="page-content">
        <div style={{ display: "flex", justifyContent: "space-between" }}>
          <HistoryFilter
            incentives={filterIncentives}
            data={dataFilter}
            update={handleFilter}
          />
          <Button
            type="primary"
            icon={<DownloadOutlined />}
            onClick={handleDownload}
          >
            <LocaleText id={"history.button.exportText"} />
          </Button>
        </div>
        <div className="table-content">
          <CensusTable
            data={filterRequests}
            select={handleSelect}
            loading={false}
          />
        </div>
      </div>
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
