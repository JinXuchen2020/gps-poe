import { LocaleText, ReasonModal, UploadButton } from "@/components";
import {
  ICheckPointRspModel,
  IMailTemplateRspModel,
  IIncentiveRspModel,
  IPoeRequestFileRspModel,
} from "@/models";
import {
  Button,
  Form,
  Input,
  DatePicker,
  Space,
  Modal,
  InputNumber,
} from "antd";
import moment from "moment";
import { FC, useEffect, useMemo, useState } from "react";
import { PlusOutlined, CloseOutlined } from "@ant-design/icons";
import { FileTemplate } from "./FileTemplate";
import { EmailEditor } from "./EmailEditor";
import { MailTemplates } from "@/constant";
import { useIntl } from "umi";
import { convertMailContent } from "@/utils";

export const IncentiveForm: FC<{
  data: IIncentiveRspModel | undefined;
  save: any;
  update: any;
  validate: any;
  cancel?: any;
}> = ({ data, save, update, validate, cancel }) => {
  const [form] = Form.useForm();
  const intl = useIntl();
  const [isAddIndex, setIsAddIndex] = useState<number>();
  const [showEditorModal, setShowEditorModal] = useState(false);
  const [showPreviewModal, setShowPreviewModal] = useState(false);
  const [currentTemplate, setCurrentTemplate] =
    useState<IMailTemplateRspModel>();

  const handleSave = async () => {
    await form.validateFields(["name", "dateRange", "files", "checkPoints"]);
    save();
  };

  const handleCancel = async () => {
    cancel();
  };

  const handleAddFile = async (
    item: IPoeRequestFileRspModel[],
    isUploading: boolean
  ) => {
    if (!item.includes(null)) {
      if (data?.files) {
        update({ files: [...data.files, ...item] });
      } else {
        update({ files: [...item] });
      }

      !isUploading && (await form.validateFields(["files"]));
    }
  };

  const handleDeleteFile = async (index: number) => {
    if (data) {
      data.files.splice(index, 1);
      update({
        files: [...data.files],
      });
    }
  };

  const handleUpdateCheckPoint = async () => {
    const { checkPoints }: { checkPoints: ICheckPointRspModel[] } =
      await form.validateFields(["checkPoints"]);
    if (!checkPoints.includes(undefined)) {
      update({ checkPoints: [...checkPoints] });
      setIsAddIndex(undefined);
    }
  };

  const handleDeleteCheckPoint = (itemIndex: number) => {
    if (data?.checkPoints) {
      data.checkPoints.splice(itemIndex, 1);
      update({ checkPoints: [...data.checkPoints] });
    }

    setIsAddIndex(undefined);
  };

  const handleShowEmail = (emailType: string) => {
    const defaultTemplate = MailTemplates.find((c) => c.type === emailType)!;
    let fileTemplate = data?.mailTemplates.find((c) => c.type === emailType);
    if (fileTemplate === undefined) {
      fileTemplate = {
        type: emailType,
        subject: intl.formatMessage({ id: defaultTemplate.subject }),
        content: defaultTemplate.default,
      };
    }

    setCurrentTemplate(fileTemplate);
  };

  const previewTemplate = useMemo(() => {
    if (currentTemplate && data) {
      return convertMailContent(currentTemplate.content, data);
    }
  }, [currentTemplate]);

  const welcomeSpeech = useMemo(() => {
    if (data) {
      const result = intl.formatMessage(
        { id: "incentiveRelease.subscription.defaultText" },
        {
          line: "\r\n",
          incentive: data?.name,
        }
      );      

      return result;
    }
  }, [data]);

  const handleUpdateTemplate = (props: Partial<IMailTemplateRspModel>) => {
    setCurrentTemplate({ ...currentTemplate!, ...props });
  };

  const handleSaveTemplate = () => {
    if (currentTemplate) {
      if (data?.mailTemplates) {
        let fileTemplate = data.mailTemplates.find(
          (c) => c.type === currentTemplate.type
        );
        if (fileTemplate) {
          fileTemplate.content = currentTemplate.content;
          update({ mailTemplates: [...data.mailTemplates] });
        } else {
          update({ mailTemplates: [...data.mailTemplates, currentTemplate] });
        }
      } else {
        update({ mailTemplates: [currentTemplate] });
      }

      setShowEditorModal(false);
      setCurrentTemplate(undefined);
    }
  };

  const nameValidator = (rule: any, value: any, callback: any) => {
    if (data) {
      const result = validate(value, "name");
      if (result) {
        return Promise.resolve();
      } else {
        return Promise.reject(
          <LocaleText id={"incentiveRelease.name.repeatText"} />
        );
      }
    } else {
      return Promise.reject(
        <LocaleText id={"incentiveRelease.name.repeatText"} />
      );
    }
  };

  const fileValidator = (rule: any, value: any, callback: any) => {
    if (data) {
      if (data.files && data.files.length > 0) {
        return Promise.resolve();
      } else {
        return Promise.reject(
          <LocaleText id={"incentiveRelease.template.requiredText"} />
        );
      }
    } else {
      return Promise.reject(
        <LocaleText id={"incentiveRelease.template.requiredText"} />
      );
    }
  };

  const checkPointValidator = (rule: any, value: any, callback: any) => {
    if (data) {
      if (data.checkPoints && data.checkPoints.length > 0) {
        return Promise.resolve();
      } else {
        if (isAddIndex === undefined) {
          return Promise.reject(
            <LocaleText id={"incentiveRelease.checkpoint.requiredText"} />
          );
        } else {
          return Promise.resolve();
        }
      }
    } else {
      if (isAddIndex === undefined) {
        return Promise.reject(
          <LocaleText id={"incentiveRelease.checkpoint.requiredText"} />
        );
      } else {
        return Promise.resolve();
      }
    }
  };

  useEffect(() => {
    if (data) {
      if (data.welcomeSpeech) {
        form.setFieldsValue(data);
        const dateRange =
          data.startDate && data.endDate
            ? [moment(data.startDate), moment(data.endDate)]
            : undefined;
        form.setFieldValue("dateRange", dateRange);
      } else {
        update({ welcomeSpeech: welcomeSpeech });
      }
    }
  }, [data]);

  return (
    <>
      <Form form={form} layout={"vertical"} autoComplete={"off"}>
        <Form.Item
          label={<LocaleText id={"incentiveRelease.name.labelText"} />}
          name="name"
          requiredMark={"optional"}
          rules={[
            {
              required: true,
              whitespace: true,
              message: <LocaleText id={"incentiveRelease.name.requiredText"} />,
            },
            { validator: nameValidator },
          ]}
        >
          <Input
            value={data?.name}
            className={"form-input"}
            autoFocus={true}
            onChange={(val) => update({ name: val.target.value })}
          />
        </Form.Item>
        <div id={"dateRangeArea"} className={"select-container"}>
          <Form.Item
            label={<LocaleText id={"incentiveRelease.dateRange.labelText"} />}
            requiredMark={"optional"}
            name={"dateRange"}
            rules={[
              {
                required: true,
                whitespace: true,
                type: "array",
                message: (
                  <LocaleText id={"incentiveRelease.dateRange.requiredText"} />
                ),
              },
            ]}
          >
            <DatePicker.RangePicker
              className={"form-input"}
              onChange={(range) =>
                update({
                  startDate: `${range?.[0]?.format("YYYY-MM-DD")}`,
                  endDate: `${range?.[1]?.format("YYYY-MM-DD")}`,
                })
              }
              getPopupContainer={() =>
                document.getElementById("dateRangeArea")!
              }
              format="YYYY-MM-DD"
            />
          </Form.Item>
        </div>
        <Form.Item
          label={<LocaleText id={"incentiveRelease.subscription.labelText"} />}
          name="welcomeSpeech"
        >
          <Input.TextArea
            value={data?.welcomeSpeech}
            rows={6}
            className={"form-input"}
            onChange={(val) => update({ welcomeSpeech: val.target.value })}
          />
        </Form.Item>
        <Form.Item
          label={<LocaleText id={"incentiveRelease.limit.labelText"} />}
        >
          <Space direction="vertical">
            <div className="materials-limit">
              <LocaleText
                id={"incentiveRelease.limit.submitDeadlineText"}
                values={{
                  value: (
                    <InputNumber
                      controls={false}
                      value={data?.submitDeadlineDay}
                      onChange={(val) => update({ submitDeadlineDay: val })}
                    />
                  ),
                }}
              />
            </div>
            <div className="materials-limit">
              <LocaleText
                id={"incentiveRelease.limit.remindText"}
                values={{
                  value: (
                    <InputNumber
                      controls={false}
                      value={data?.remindEmailDay}
                      onChange={(val) => update({ remindEmailDay: val })}
                    />
                  ),
                }}
              />
            </div>
            <div className="materials-limit">
              <LocaleText
                id={"incentiveRelease.limit.resubmitDeadlineText"}
                values={{
                  value: (
                    <InputNumber
                      controls={false}
                      value={data?.reSubmitDeadlineDay}
                      onChange={(val) => update({ reSubmitDeadlineDay: val })}
                    />
                  ),
                }}
              />
            </div>
            <div className="materials-limit">
              <LocaleText
                id={"incentiveRelease.limit.rejectText"}
                values={{
                  value: (
                    <InputNumber
                      controls={false}
                      value={data?.rejectCount}
                      onChange={(val) => update({ rejectCount: val })}
                    />
                  ),
                }}
              />
            </div>
          </Space>
        </Form.Item>
        <Form.Item
          label={<LocaleText id={"incentiveRelease.template.labelText"} />}
          name={"files"}
          wrapperCol={{ span: 8 }}
          rules={[
            {
              validator: fileValidator,
            },
          ]}
        >
          <UploadButton
            title={<LocaleText id={"incentiveRelease.template.buttonText"} />}
            disabled={false}
            update={handleAddFile}
            deleteItem={handleDeleteFile}
            fileList={data?.files}
          />
        </Form.Item>
        <Form.Item
          label={<LocaleText id={"incentiveRelease.checkpoint.labelText"} />}
          name={"checkPoints"}
          rules={[
            {
              validator: checkPointValidator,
            },
          ]}
        >
          <Form.List name={"checkPoints"}>
            {(fields, { add, remove }) => (
              <>
                {fields.map((field, index) => (
                  <div
                    key={index}
                    style={{
                      marginBottom: 8,
                      display: "flex",
                      justifyContent: "left",
                      alignItems: "center",
                    }}
                  >
                    <Form.Item
                      name={[field.name, "content"]}
                      noStyle
                      key={field.key}
                      validateTrigger={["onBlur"]}
                      rules={[
                        {
                          required: true,
                          whitespace: true,
                          message: (
                            <LocaleText
                              id={
                                "incentiveRelease.checkpoint.content.requiredText"
                              }
                            />
                          ),
                        },
                      ]}
                    >
                      <Input
                        className={"form-input"}
                        readOnly={index !== isAddIndex}
                        autoFocus={index === isAddIndex}
                        onClick={() => setIsAddIndex(index)}
                        onBlur={() => handleUpdateCheckPoint()}
                      />
                    </Form.Item>
                    <span style={{ marginLeft: 8 }}>
                      <CloseOutlined
                        onClick={() => handleDeleteCheckPoint(index)}
                      />
                    </span>
                  </div>
                ))}
                <Form.Item>
                  <Button
                    type="default"
                    icon={<PlusOutlined />}
                    onClick={() => {
                      if (isAddIndex === undefined) {
                        add();
                        setIsAddIndex(
                          data?.checkPoints
                            ? data.checkPoints.length + 1 - 1
                            : 0
                        );
                      }
                    }}
                  >
                    <LocaleText id={"incentiveRelease.checkpoint.buttonText"} />
                  </Button>
                </Form.Item>
              </>
            )}
          </Form.List>
        </Form.Item>
        <Form.Item
          label={<LocaleText id={"incentiveRelease.mailTemplate.labelText"} />}
        >
          <div className="template-container">
            {MailTemplates.map((c, index) => (
              <FileTemplate
                key={index}
                title={<LocaleText id={c.title} />}
                edit={() => {
                  handleShowEmail(c.type);
                  setShowEditorModal(true);
                }}
                preview={() => {
                  handleShowEmail(c.type);
                  setShowPreviewModal(true);
                }}
              />
            ))}
          </div>
        </Form.Item>
        <Form.Item>
          {cancel ? (
            <Space>
              <Button type="default" onClick={handleCancel}>
                <LocaleText id={"incentiveRelease.button.cancelText"} />
              </Button>
              <Button type="primary" onClick={handleSave}>
                <LocaleText id={"incentiveRelease.button.updateText"} />
              </Button>
            </Space>
          ) : (
            <Button type="primary" onClick={handleSave}>
              <LocaleText id={"incentiveRelease.button.createText"} />
            </Button>
          )}
        </Form.Item>
      </Form>
      <Modal
        title={<LocaleText id={"incentiveRelease.emailEditor.titleText"} />}
        width={800}
        centered
        open={showEditorModal}
        footer={
          <div style={{ display: "flex", justifyContent: "space-between" }}>
            <Button
              onClick={() => {
                const mailTemplate = MailTemplates.find(
                  (c) => c.type === currentTemplate!.type
                )!;
                setCurrentTemplate({
                  ...currentTemplate!,
                  content: mailTemplate.default,
                });
              }}
            >
              <LocaleText
                id={"incentiveRelease.emailEditor.button.defaultText"}
              />
            </Button>
            <Space>
              <Button onClick={() => setShowEditorModal(false)}>
                <LocaleText
                  id={"incentiveRelease.emailEditor.button.cancelText"}
                />
              </Button>
              <Button type={"primary"} onClick={handleSaveTemplate}>
                <LocaleText
                  id={"incentiveRelease.emailEditor.button.saveText"}
                />
              </Button>
            </Space>
          </div>
        }
        destroyOnClose={true}
        onOk={handleSaveTemplate}
        onCancel={() => setShowEditorModal(false)}
      >
        <div className="modal-content">
          <EmailEditor data={currentTemplate} update={handleUpdateTemplate} />
        </div>
      </Modal>
      <ReasonModal
        show={showPreviewModal}
        title={previewTemplate?.subject}
        data={previewTemplate?.content}
        cancel={() => setShowPreviewModal(false)}
      />
    </>
  );
};
