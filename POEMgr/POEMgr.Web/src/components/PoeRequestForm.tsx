import { Space, Button, Form, Checkbox, Row, Col, Popover } from "antd";
import {
  FC,
  useEffect,
  useLayoutEffect,
  useMemo,
  useRef,
  useState,
} from "react";
import { IPoeRequestFileRspModel, IPoeRequestRspModel } from "@/models";
import { DownloadButton } from "./DownloadButton";
import { UploadButton } from "./UploadButton";
import { SubscriptionTable } from "./SubscriptionTable";
import { RequestStatus } from "@/constant";
import { ReasonModal } from "./ReasonModal";
import { LocaleText } from "./LocaleText";

export const PoeRequestForm: FC<{
  data: IPoeRequestRspModel | undefined;
  isAudit?: boolean;
  disabled: boolean;
  save: any;
  update: any;
  cancel?: any;
}> = ({ data, save, update, isAudit, disabled, cancel }) => {
  const [form] = Form.useForm();
  const logRef = useRef(null);
  const [isUploading, setIsUploading] = useState(false);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [height, setHeight] = useState(0);
  const handleSave = async () => {
    save();
  };

  const handleSubmit = async () => {
    await form.validateFields();
    save(RequestStatus.Submitted);
  };

  const handleReject = async () => {
    save(RequestStatus.Rejected);
  };

  const handleApproval = async () => {
    await form.validateFields();
    save(RequestStatus.Approved);
  };

  const handleRequestCheckPoint = (isChecked: boolean, itemId: string) => {
    if (data) {
      let result = data.requestCheckPoints.find((c) => c.id === itemId);
      if (result) {
        result.status = isChecked ? "checked" : "";
        update({ requestCheckPoints: [...data.requestCheckPoints] });
      } else {
        result = {
          id: itemId,
          status: isChecked ? "checked" : "",
        };
        if (data.requestCheckPoints) {
          update({
            requestCheckPoints: [...data.requestCheckPoints, result],
          });
        } else {
          update({ requestCheckPoints: [result] });
        }
      }
    }
  };

  const handleAuditCheckPoint = (isChecked: boolean, itemId: string) => {
    if (data) {
      let result = data.auditCheckPoints.find((c) => c.id === itemId);
      if (result) {
        result.status = isChecked ? "checked" : "";
        update({ auditCheckPoints: [...data.auditCheckPoints] });
      } else {
        result = {
          id: itemId,
          status: isChecked ? "checked" : "",
        };

        if (data.auditCheckPoints) {
          update({
            auditCheckPoints: [...data.auditCheckPoints, result],
          });
        } else {
          update({ auditCheckPoints: [result] });
        }
      }
    }
  };

  const handleCheckSubscriptions = (selectedItems: string[]) => {
    if (data) {
      const selectedStatus = selectedItems.map((item) => {
        let result = data.subscriptions.find((c) => c.subscriptionId === item);
        if (result) {
          result.status = "checked";
        } else {
          result = {
            subscriptionId: item,
            status: "checked",
          };
        }

        return result;
      });

      const unSelectedStatus = data.subscriptions
        .filter((c) => !selectedItems.includes(c.subscriptionId))
        .map((item) => {
          item.status = "";
          return item;
        });

      update({ subscriptions: [...selectedStatus, ...unSelectedStatus] });
    }
  };

  const handleAddFile = async (
    item: IPoeRequestFileRspModel[],
    isUploading: boolean
  ) => {
    if (item.length > 0) {
      if (data?.requestFiles) {
        update({ requestFiles: [...data.requestFiles, ...item] });
      } else {
        update({ requestFiles: [...item] });
      }

      await form.validateFields(["material"]);
    }

    setIsUploading(isUploading);
  };

  const handleDeleteFile = async (index: number) => {
    if (data) {
      data.requestFiles.splice(index, 1);
      update({
        requestFiles: [...data.requestFiles],
      });
    }
  };

  const getMaterialCheckOptions = useMemo(() => {
    return data?.incentive.checkPoints.map((c, index) => {
      return {
        key: index,
        label: c.content,
        value: c.id!,
      };
    });
  }, [data]);

  const getUploadStatus = useMemo(() => {
    if (data) {
      return data.requestCheckPoints
        .filter((c) => c.status === "checked")
        .map((c) => c.id);
    }
  }, [data]);

  const getAuditStatus = useMemo(() => {
    if (data) {
      return data.auditCheckPoints
        .filter((c) => c.status === "checked")
        .map((c) => c.id);
    }
  }, [data]);

  const rejectReason = useMemo(() => {
    const rejectLogs = data?.logs.filter((c) => (c.reason ? true : false));
    if (rejectLogs && rejectLogs.length > 0) {
      return rejectLogs[rejectLogs.length - 1].reason;
    } else {
      return "";
    }
  }, [data]);

  const reasonHtml = useMemo(() => {
    if (rejectReason) {
      return { __html: rejectReason };
    } else {
      return { __html: "" };
    }
  }, [rejectReason]);

  const fileValidator = (rule: any, value: any, callback: any) => {
    if (data && !isUploading) {
      if (data.requestFiles && data.requestFiles.length > 0) {
        return Promise.resolve();
      } else {
        return Promise.reject(
          <LocaleText id={"request.materialRequired.errorText"} />
        );
      }
    } else {
      return Promise.resolve();
    }
  };

  const materialCheckValidator = (rule: any, value: any, callback: any) => {
    if (data) {
      let checkedLength;
      const allLength = data.incentive.checkPoints.length;
      if (isAudit) {
        checkedLength = data.auditCheckPoints.filter(
          (c) => c.status !== ""
        ).length;
      } else {
        checkedLength = data.requestCheckPoints.filter(
          (c) => c.status !== ""
        ).length;
      }
      if (checkedLength >= allLength) {
        return Promise.resolve();
      } else {
        return Promise.reject(
          <LocaleText id={"request.materialCheckRequired.errorText"} />
        );
      }
    } else {
      return Promise.reject(
        <LocaleText id={"request.materialCheckRequired.errorText"} />
      );
    }
  };

  useEffect(() => {
    if (data) {
      form.setFieldsValue(data);
    }
  }, [data]);

  useLayoutEffect(() => {
    if (data?.logs && data?.logs.length > 0) {
      const result = 40 * data?.logs.length + 2 * data?.logs.length;
      if (result > 300) {
        setHeight(300);
      } else {
        setHeight(result);
      }
    }
  }, [data?.logs]);

  return (
    <>
      <Form form={form} layout={"vertical"}>
        {rejectReason &&
          !isAudit &&
          (data?.status === RequestStatus.Rejected ||
            data?.status === RequestStatus.Expired) && (
            <Form.Item
              name="rejectReason"
              label={
                <span className="form-label material-error">
                  <LocaleText id={"request.rejectReason.buttonText"} />
                </span>
              }
            >
              <div className="reject-reason">
                <div dangerouslySetInnerHTML={reasonHtml} />
              </div>
            </Form.Item>
          )}
        <Form.Item
          name="material"
          label={
            <span className="form-label">
              <LocaleText id={"request.materialUpload.labelText"} />
            </span>
          }
          rules={[
            {
              validator: fileValidator,
            },
          ]}
        >
          <div
            style={{ marginBottom: 8 }}
            hidden={
              disabled ||
              (data?.status !== RequestStatus.EmailSent &&
                data?.status !== RequestStatus.PartialApproved &&
                data?.status !== RequestStatus.Rejected)
            }
          >
            <LocaleText id={"request.materialUpload.descriptionText"} />
          </div>
          <Space direction="vertical">
            <UploadButton
              title={<LocaleText id={"request.materialUpload.buttonText"} />}
              disabled={
                (data?.status !== RequestStatus.EmailSent &&
                  data?.status !== RequestStatus.PartialApproved &&
                  data?.status !== RequestStatus.Rejected) ||
                disabled
              }
              update={handleAddFile}
              multiple={true}
              deleteItem={handleDeleteFile}
              fileList={data?.requestFiles}
            />
            <DownloadButton
              disabled={
                !disabled &&
                (data?.status === RequestStatus.EmailSent ||
                  data?.status === RequestStatus.Rejected ||
                  data?.status === RequestStatus.PartialApproved)
              }
              title={<LocaleText id={"request.materialDownload.buttonText"} />}
              data={data}
            />
          </Space>
        </Form.Item>
        <Form.Item
          name="material-check"
          label={
            <span className="form-label">
              <LocaleText id={"request.materialCheck.labelText"} />
            </span>
          }
          rules={[
            {
              validator: materialCheckValidator,
            },
          ]}
          wrapperCol={{ span: 16 }}
        >
          <Row>
            <Col span={12}>
              <div className="check-title">
                <LocaleText id={"request.materialCheck.submitCheckText"} />
              </div>
              <Checkbox.Group
                value={getUploadStatus}
                disabled={
                  (data?.status !== RequestStatus.EmailSent &&
                    data?.status !== RequestStatus.Rejected) ||
                  disabled
                }
              >
                <Space direction="vertical">
                  {getMaterialCheckOptions?.map((c) => (
                    <Checkbox
                      key={c.key}
                      value={c.value}
                      onChange={(e) =>
                        handleRequestCheckPoint(e.target.checked, c.value)
                      }
                    >
                      {c.label}
                    </Checkbox>
                  ))}
                </Space>
              </Checkbox.Group>
            </Col>
            <Col span={12}>
              <div className="check-title">
                <LocaleText id={"request.materialCheck.auditCheckText"} />
              </div>
              <Checkbox.Group
                value={getAuditStatus}
                disabled={!isAudit || disabled}
              >
                <Space direction="vertical">
                  {getMaterialCheckOptions?.map((c) => (
                    <Checkbox
                      key={c.key}
                      value={c.value}
                      onChange={(e) =>
                        handleAuditCheckPoint(e.target.checked, c.value)
                      }
                    >
                      {c.label}
                    </Checkbox>
                  ))}
                </Space>
              </Checkbox.Group>
            </Col>
          </Row>
        </Form.Item>
        <Form.Item
          label={
            <span className="form-label">
              <LocaleText id={"request.subscription.labelText"} />
            </span>
          }
          wrapperCol={{ span: 12 }}
        >
          {data && (
            <SubscriptionTable
              data={data?.customer.subscriptions}
              selectedData={data?.subscriptions
                ?.filter((c) => c.status)
                .map((c) => c.subscriptionId)}
              handleSelect={handleCheckSubscriptions}
              disabled={
                data?.status === RequestStatus.Expired ||
                data?.status === RequestStatus.Approved
              }
              loading={false}
            />
          )}
        </Form.Item>
        <Form.Item
          label={
            <span className="form-label">
              <LocaleText id={"request.log.labelText"} />
            </span>
          }
          wrapperCol={{ span: 12 }}
        >
          <div className="log-container" style={{ height }}>
            <div className="content">
              {data?.logs.map((c, index) => (
                <>
                  {c.content ? (
                    <Popover
                      content={
                        <div
                          style={{ width: 800 }}
                          dangerouslySetInnerHTML={{ __html: c.content }}
                        />
                      }
                      trigger="hover"
                      showArrow={false}
                      placement={"right"}
                    >
                      <div key={c.id}>
                        <div>{c.title}</div>
                      </div>
                    </Popover>
                  ) : (
                    <div key={c.id}>
                      <div>{c.title}</div>
                    </div>
                  )}
                </>
              ))}
            </div>
          </div>
        </Form.Item>
        <Form.Item>
          {isAudit ? (
            <Space
              hidden={data?.status === RequestStatus.Rejected || disabled}
              className="footer-button"
              direction="horizontal"
              size={"large"}
            >
              <Button type="default" onClick={handleSave}>
                <LocaleText id={"request.button.saveText"} />
              </Button>
              <Button type="default" onClick={handleReject}>
                <LocaleText id={"request.button.rejectText"} />
              </Button>
              <Button type="primary" onClick={handleApproval}>
                <LocaleText id={"request.button.approveText"} />
              </Button>
            </Space>
          ) : (
            <Space
              hidden={
                (data?.status !== RequestStatus.EmailSent &&
                  data?.status !== RequestStatus.PartialApproved &&
                  data?.status !== RequestStatus.Rejected) ||
                disabled
              }
              className="footer-button"
              direction="horizontal"
              size={"large"}
            >
              <Button type="default" onClick={handleSave}>
                <LocaleText id={"request.button.saveText"} />
              </Button>
              <Button type="primary" onClick={handleSubmit}>
                <LocaleText id={"request.button.submitText"} />
              </Button>
            </Space>
          )}
        </Form.Item>
      </Form>
      <ReasonModal
        show={showRejectModal}
        title={<LocaleText id={"request.rejectReason.titleText"} />}
        data={rejectReason}
        cancel={() => setShowRejectModal(false)}
      />
    </>
  );
};
