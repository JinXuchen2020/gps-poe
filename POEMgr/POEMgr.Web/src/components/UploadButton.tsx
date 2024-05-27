import { FunctionComponent, ReactNode, useState } from "react";
import { Upload, Button, Typography, message } from "antd";
import { UploadRequestOption as RcCustomRequestOptions } from "rc-upload/lib/interface";
import { UploadOutlined } from "@ant-design/icons";
import { IPoeRequestFileRspModel } from "@/models";
import { uploadFile } from "@/services/request";
import { PaperClipOutlined } from "@ant-design/icons";
import { useIntl, useNavigate } from "umi";
import { LocaleText } from "./LocaleText";
import { Loading } from "./Loading";
import mime from "mime";

export const UploadButton: FunctionComponent<{
  title: ReactNode;
  disabled: boolean;
  update: any;
  deleteItem: any;
  multiple?: boolean;
  fileList: IPoeRequestFileRspModel[] | undefined;
}> = ({ title, disabled, multiple, update, deleteItem, fileList }) => {
  const navigate = useNavigate();
  const intl = useIntl();
  const [fileCount, setFileCount] = useState(0);
  const [poeFiles, setPoeFiles] = useState<IPoeRequestFileRspModel[]>([]);
  const [loading, setLoading] = useState(false);
  const handleUpload = async (options: RcCustomRequestOptions) => {
    const { file } = options;
    let fileData = new FormData();
    fileData.append("file", file);
    const item = await uploadFile(fileData);
    poeFiles.push(item.data);
    if (poeFiles.length === fileCount) {
      update(poeFiles, false);
      setLoading(false);
      setPoeFiles([]);
    }
  };

  const handleBeforeUpload = (file: any, FileList: any[]) => {
    const isZip =
      file.type === "application/x-zip-compressed" ||
      file.type === "application/x-rar-compressed";
    const isLarge20M = file.size / 1024 / 1024 > 20;
    if (!isZip && !isLarge20M) {
      setFileCount(FileList.length);
      setLoading(true);
      update([], true);
    } else {
      message.error(
        intl.formatMessage({ id: "request.materialUpload.errorText" })
      );
      return false;
    }
  };

  return (
    <>
      <Loading loading={loading} />
      {fileList && fileList.length > 0 && (
        <div className={"materials"}>
          {fileList.map((file, index) => (
            <div className={"material-item"} key={file.id}>
              <span
                style={{
                  display: "flex",
                  justifyContent: "left",
                  width: "80%",
                }}
              >
                <span>
                  <PaperClipOutlined
                    style={{ paddingRight: 4, color: "blue", opacity: "50%" }}
                  />
                </span>
                <Typography.Link
                  title={file.name}
                  ellipsis={true}
                  href={file.path}
                  target={"_blank"}
                >
                  {file.name}
                </Typography.Link>
              </span>
              <span>
                <Typography.Link
                  style={{ marginRight: 30 }}
                  onClick={() => {
                    const type = mime.getType(file.path)
                    if (type === "application/vnd.ms-outlook" || type === "message/rfc822") {
                      message.warning(intl.formatMessage({id: 'request.materialUpload.preview.warningText'}))
                    }
                    else {                      
                      navigate(`/preview`, { state: file });
                    }
                  }}
                >
                  <LocaleText
                    id={"request.materialUpload.preview.buttonText"}
                  />
                </Typography.Link>
                <Typography.Link
                  style={{ marginRight: 20 }}
                  onClick={() => deleteItem(index)}
                  hidden={disabled}
                >
                  <LocaleText id={"request.materialUpload.delete.buttonText"} />
                </Typography.Link>
              </span>
            </div>
          ))}
        </div>
      )}
      <div hidden={disabled}>
        <Upload
          disabled={multiple ? false : fileList ? fileList.length > 0 : false}
          accept="*"
          showUploadList={false}
          beforeUpload={handleBeforeUpload}
          customRequest={handleUpload}
          multiple={multiple}
        >
          <Button type="default" icon={<UploadOutlined />}>
            {title}
          </Button>
        </Upload>
        {!multiple && (fileList === undefined || fileList.length === 0) && (
          <div style={{ fontSize: 12, marginTop: 2, color: "red" }}>
            <LocaleText id={"request.materialUpload.remindText"} />
          </div>
        )}
      </div>
    </>
  );
};
