import { FC, useEffect } from "react";
import { ModelName, PageProps } from "../model";
import { connect, useNavigate, useParams } from "umi";
import { dispatchMethod } from "@/utils";
import { IIncentiveLiteRspModel, IIncentiveReqModel, IIncentiveRspModel } from "@/models";
import { Loading, LocaleText } from "@/components";
import { IncentiveForm } from "../components";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, currentModel, modelList },
}) => {
  const navigate = useNavigate();
  const { incentiveId } = useParams();

  const handleSave = async () => {
    if (currentModel) {
      const request: IIncentiveReqModel = {
        ...currentModel,
        checkPoints: currentModel.checkPoints.map((c) => c.content),
        fileIds: currentModel.files.map((c) => c.id),
      };

      await dispatchMethod(dispatch, ModelName, "updateModel", {
        id: currentModel.id,
        requestParams: request,
      });

      navigate("/incentiveRelease");
    }
  };

  const handelCancel = () => {
    navigate("/incentiveRelease");
  };

  const handleUpdate = (props: Partial<IIncentiveRspModel>) => {
    dispatchMethod(dispatch, ModelName, "update", {
      currentModel: { ...currentModel, ...props },
    });
  };

  const handleValidate = (value: any, dataIndex: keyof IIncentiveLiteRspModel) => {
    let result = true;
    if (modelList && currentModel) {
      const record = modelList.find(c => c[dataIndex] === value)
      if (record && record.id !== currentModel.id) {
        result = false
      }
    }

    return result
  }

  useEffect(() => {
    if (currentModel === undefined || currentModel.id !== incentiveId) {
      dispatchMethod(dispatch, ModelName, "getModel", {
        id: incentiveId as string,
      });
    }
  }, [incentiveId]);

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
        <LocaleText id={"incentiveRelease.titleText"} />
      </div>
      <div className="page-content">
        <IncentiveForm
          data={currentModel}
          save={handleSave}
          validate={handleValidate}
          update={handleUpdate}
          cancel={handelCancel}
        />
      </div>
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
