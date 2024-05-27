import { FC, useEffect } from "react";
import { ModelName, PageProps } from "../model";
import { connect, useIntl, useLocation, useNavigate } from "umi";
import { dispatchMethod } from "@/utils";
import { IIncentiveLiteRspModel, IIncentiveReqModel, IIncentiveRspModel, IMailTemplateRspModel } from "@/models";
import { Loading, LocaleText } from "@/components";
import { IncentiveForm } from "../components";
import { MailTemplates } from "@/constant";

const Page: FC<PageProps> = ({
  dispatch,
  pageState: { loading, currentModel, modelList },
}) => {
  const navigate = useNavigate();
  const location = useLocation();
  const copyId = (location.state as any)?.copyId;
  const intl = useIntl();

  const handleSave = async () => {
    if (currentModel) {
      const mailTemplates = MailTemplates.map(template => {
        const result = currentModel.mailTemplates?.find(c=>c.type === template.type)
        if(result) {
          return {...result};
        }
        else {
          return {
            type : template.type,
            subject: intl.formatMessage({id: template.subject}),
            content: template.default
          } as IMailTemplateRspModel
        }
      });
      const request: IIncentiveReqModel = {
        ...currentModel,
        checkPoints: currentModel.checkPoints.map((c) => c.content),
        fileIds: currentModel.files.map((c) => c.id),
        mailTemplates: mailTemplates
      };

      await dispatchMethod(dispatch, ModelName, "createModel", {
        requestParams: request,
      });

      navigate("/incentiveRelease", { state: { isCreate: true } });
    }
  };

  const handleUpdate = (props: Partial<IIncentiveRspModel>) => {
    dispatchMethod(dispatch, ModelName, "update", {
      currentModel: { ...currentModel, ...props },
    });
  };

  const handleValidate = (value: any, dataIndex: keyof IIncentiveLiteRspModel) => {
    let result = true;
    if (modelList) {
      const record = modelList.find(c => c[dataIndex] === value)
      if (record) {
        result = false
      }
    }

    return result
  }

  useEffect(() => {
    if (copyId) {
      dispatchMethod(dispatch, ModelName, "getModel", {
        id: copyId as string,
      });
    }
  }, [copyId]);

  return (
    <>
      <Loading loading={loading} />
      <div className="page-title">
        <LocaleText id={"incentiveRelease.titleText"} />
      </div>
      <div className="page-content">
        <IncentiveForm
          data={currentModel}
          validate={handleValidate}
          save={handleSave}
          update={handleUpdate}
        />
      </div>
    </>
  );
};

export default connect((state: any) => ({
  pageState: state[ModelName],
}))(Page);
