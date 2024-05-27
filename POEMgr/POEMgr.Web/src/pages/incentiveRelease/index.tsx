import { Button, Space } from "antd";
import { Filter, IncentiveTable } from "./components";
import { FC, useEffect, useMemo, useState } from "react";
import { ModelName, PageProps } from "./model";
import { connect, useLocation, useNavigate } from "umi";
import { dispatchMethod, getFiscalQuarter } from "@/utils";
import { ICensusQueryOption, IIncentiveLiteRspModel } from "@/models";
import { Loading, LocaleText, NotificationBar } from "@/components";
import moment from "moment";

import "./index.less";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, currentModel, modelList, isEditing },
}) => {
  const navigate = useNavigate();
  const location = useLocation();
  const isCreate = (location.state as any)?.isCreate;
  const [showNotification, setShowNotification] = useState<boolean>(false);
  const currentFiscalQuarter = useMemo(() => {
    return getFiscalQuarter(new Date());
  }, []);

  const [dataFilter, setDataFilter] = useState<ICensusQueryOption>({
    fiscalYear: currentFiscalQuarter?.year,
    fiscalQuarter: currentFiscalQuarter?.quarter,
  });

  const filterIncentives = useMemo(() => {
    if (modelList) {
      return modelList.filter((c) => {
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
  }, [modelList, dataFilter]);

  const handleFilter = (props: Partial<ICensusQueryOption>) => {
    if (dataFilter) {
      setDataFilter({ ...dataFilter, ...props });
    } else {
      setDataFilter(props);
    }
  };

  const handleCreate = () => {
    dispatchMethod(dispatch, ModelName, "update", {
      currentModel: undefined,
    });
    navigate("/incentiveRelease/create");
  };

  const handleSelect = (item: IIncentiveLiteRspModel) => {
    navigate(`/incentiveRelease/${item.id}`);
  };

  const handleCopy = (item: IIncentiveLiteRspModel) => {
    navigate(`/incentiveRelease/create`, { state: { copyId: item.id } });
  };

  const getNotification = useMemo(() => {
    if (currentModel && isEditing) {
      const result = {
        type: "success",
        content: isCreate ? (
          <LocaleText
            id={"incentiveRelease.notification.createText"}
            values={{ value: currentModel.name }}
          />
        ) : (
          <LocaleText
            id={"incentiveRelease.notification.updateText"}
            values={{ value: currentModel.name }}
          />
        ),
      };
      setShowNotification(true);
      return result;
    } else {
      setShowNotification(false);
      return {
        type: undefined,
        content: undefined,
      };
    }
  }, [currentModel?.id]);

  useEffect(() => {
    dispatchMethod(dispatch, ModelName, "getAll");
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
      <NotificationBar
        content={getNotification.content}
        type={getNotification.type}
        hidden={!showNotification}
        close={() => setShowNotification(false)}
      />
      <div className="page-title">
        <LocaleText id={"incentiveRelease.titleText"} />
      </div>
      <div className="page-content">
        <Space size={"large"}>
          <Button type="primary" onClick={handleCreate}>
            <LocaleText id={"incentiveRelease.button.createText"} />
          </Button>
          <Filter data={dataFilter} update={handleFilter} />
        </Space>
        <div className="table-content">
          <IncentiveTable
            data={filterIncentives}
            select={handleSelect}
            copy={handleCopy}
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
