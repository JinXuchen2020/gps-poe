import { RequestStatus } from "@/constant";
import { AuditTable } from "./components";
import { FC, ReactNode, useEffect, useMemo, useState } from "react";
import { ModelName, PageProps } from "./model";
import { connect, useNavigate } from "umi";
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
  const navigate = useNavigate();
  const [requestFilter, setRequestFilter] = useState<IRequestQueryOption>();

  const [showNotification, setShowNotification] = useState<boolean>(false);

  const filterRequests = useMemo(() => {
    if (modelList) {
      let result = modelList;
      if (requestFilter) {
        if (requestFilter.incentiveId) {
          result = result.filter(
            (c) => c.incentiveId === requestFilter.incentiveId
          );
        }

        if (requestFilter.partnerName) {
          result = result.filter((c) =>
            c.partnerName.includes(requestFilter.partnerName)
          );
        }
      }

      return result;
    } else {
      return modelList;
    }
  }, [modelList, requestFilter]);

  const getNotification = useMemo(() => {
    if (
      currentModel &&
      isEditing &&
      (currentModel.status === RequestStatus.Rejected ||
        currentModel.status === RequestStatus.Approved)
    ) {
      const result: { type: string; content: ReactNode } = {
        type: "success",
        content: "",
      };
      if (currentModel.status === RequestStatus.Rejected) {
        result.content = (
          <LocaleText
            id={"audit.notification.rejectedText"}
            values={{
              value: currentModel.partner.email,
            }}
          />
        );
      } else if (currentModel.status === RequestStatus.Approved) {
        result.content = (
          <LocaleText
            id={"audit.notification.approvedText"}
            values={{
              value: currentModel.partner.email,
            }}
          />
        );
      } else if (currentModel.status === RequestStatus.PartialApproved) {
        result.content = (
          <LocaleText
            id={"audit.notification.partialApprovedText"}
            values={{
              value: currentModel.partner.email,
            }}
          />
        );
      }
      setShowNotification(true);
      return result;
    } else {
      setShowNotification(false);
      return {
        type: undefined,
        content: undefined,
      };
    }
  }, [currentModel?.id, currentModel?.status]);

  const handleFilter = (props: Partial<IRequestQueryOption>) => {
    if (requestFilter) {
      setRequestFilter({ ...requestFilter, ...props });
    } else {
      setRequestFilter(props);
    }
  };

  const handleSelect = async (request: IPoeRequestLiteRspModel) => {
    navigate(`/audits/${request.id}`);
  };

  const refresh = async (status: string) => {
    const query: Partial<IRequestQueryOption> = {
      status: status !== "All" ? status : undefined,
    };
    await dispatchMethod(dispatch, ModelName, "getAll", {
      requestParams: query,
    });
  };

  useEffect(() => {
    refresh(RequestStatus.Submitted);
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
        <LocaleText id={"audit.titleText"} />
      </div>
      <div className="page-content">
        {/* <RequestFilter
          incentives={incentiveModels}
          data={requestFilter}
          update={handleFilter}
          isAudit={true}
        /> */}
        <div className="table-content">
          <AuditTable
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
