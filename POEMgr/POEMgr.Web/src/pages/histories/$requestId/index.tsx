import { FC, useEffect } from "react";
import { ModelName, PageProps } from "../model";
import { connect, useParams } from "umi";
import { dispatchMethod } from "@/utils";
import {
  Loading,
  PoeRequestForm,
  RequestContact,
  RequestStatusControl,
} from "@/components";

import "./index.less";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, currentModel },
}) => {
  const { requestId } = useParams();

  useEffect(() => {
    if (currentModel === undefined || currentModel.id !== requestId) {
      dispatchMethod(dispatch, ModelName, "getModel", {
        id: requestId as string,
      });
    }
  }, [requestId]);

  useEffect(() => {
    const currentPath = location.pathname;
    return () => {
      if (
        location.pathname !== currentPath &&
        location.pathname !== "/preview"
      ) {
        dispatchMethod(dispatch, ModelName, "initState");
      }
    };
  }, []);

  return (
    <>
      <Loading loading={loading} />
      <div className="page-title">
        <span style={{ marginRight: 8 }}>{currentModel?.partner.name}</span>
        <RequestStatusControl data={currentModel?.status} />
      </div>
      <div className="page-content">
        <RequestContact data={currentModel} />
        <PoeRequestForm
          data={currentModel}
          save={undefined}
          update={undefined}
          disabled={true}
        />
      </div>
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
