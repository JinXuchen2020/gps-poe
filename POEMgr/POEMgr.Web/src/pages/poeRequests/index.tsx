import { RequestStatus } from "@/constant";
import { IncentiveRequest } from "./components";
import { FC, useEffect, useMemo, useState } from "react";
import { ModelName, PageProps } from "./model";
import { connect } from "umi";
import { dispatchMethod } from "@/utils";
import { IPoeRequestLiteRspModel, IRequestQueryOption } from "@/models";
import {
  Loading,
  LocaleText,
  NotificationBar,
  RequestFilter,
} from "@/components";

import "./index.less";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, modelList, currentModel, incentiveModels, isEditing },
}) => {
  const [showNotification, setShowNotification] = useState<boolean>(false);
  const [requestFilter, setRequestFilter] = useState<IRequestQueryOption>();
  const [incentiveGroups, setIncentiveGroups] = useState<{[key: string]: IPoeRequestLiteRspModel[]}>({});

  const handleFilter = (props: Partial<IRequestQueryOption>) => {
    if (requestFilter) {
      setRequestFilter({ ...requestFilter, ...props });
    } else {
      setRequestFilter(props);
    }
  };

  const getNotification = useMemo(() => {
    if (
      currentModel &&
      currentModel.status === RequestStatus.Submitted &&
      isEditing
    ) {
      const result = {
        type: "success",
        content: (
          <LocaleText
            id={"request.notification.submitText"}
            values={{ value: currentModel.partner.email }}
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
  }, [currentModel?.status]);

  const incentives = useMemo(() => {
    let options: { key: number; value: string; label: string }[] = [];
    modelList?.forEach((c, index) => {
      if (options.findIndex((t) => t.value === c.incentiveId) < 0) {
        options.push({
          key: index,
          value: c.incentiveId,
          label: c.incentiveName,
        });
      }
    });
    return options;
  }, [modelList]);

  useEffect(() => {
    if (modelList) {
      dispatchMethod(dispatch, ModelName, "update", {
        loading: true,
      });
      new Promise((resolve) => {
        const result: { [key: string]: IPoeRequestLiteRspModel[] } = {};
        let filters = modelList.filter((c) => c.status !== RequestStatus.Draft);
        if (requestFilter?.incentiveId) {
          filters = filters.filter(
            (c) => c.incentiveId === requestFilter.incentiveId
          );
        }
        if (requestFilter?.status) {
          filters = filters.filter(
            (c) => c.status === requestFilter.status
          );
        }
        filters.forEach((c) => {
          const group = c.incentiveName;
          result[group] = result[group] || [];
          result[group].push(c);
        }); 
        resolve(result)
      }).then((result: any) => { 
        setIncentiveGroups(result)
        dispatchMethod(dispatch, ModelName, "update", {
          loading: false,
        });
      })
    }
  }, [modelList, requestFilter]);

  useEffect(() => {
    dispatchMethod(dispatch, ModelName, "getAll");
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
      <NotificationBar
        content={getNotification.content}
        type={getNotification.type}
        hidden={!showNotification}
        close={() => setShowNotification(false)}
      />
      <div className="page-title">
        <LocaleText id={"request.titleText"} />
      </div>
      <div className="page-content">
        <RequestFilter
          incentives={incentives}
          data={requestFilter}
          update={handleFilter}
        />
        {incentiveGroups &&
          Object.keys(incentiveGroups).map((c, index) => (
            <IncentiveRequest key={index} data={incentiveGroups[c]} />
          ))}
        {Object.keys(incentiveGroups).length > 0 && (
          <div className="request-footer">
            <LocaleText
              id={"request.footerText"}
              values={{
                value: (
                  <span style={{ color: "#1890ff" }}>GCRPIT@microsoft.com</span>
                ),
              }}
            />
          </div>
        )}
      </div>
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
