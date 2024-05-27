import { FunctionComponent } from "react";
import { Upload, Button } from "antd";
import { UploadRequestOption as RcCustomRequestOptions } from "rc-upload/lib/interface";
import { LocaleText } from "@/components";

export const UploadControl: FunctionComponent<{
  handleUpdate: any;
}> = ({ handleUpdate }) => {
  const handleUpload = async (options: RcCustomRequestOptions) => {
    const { file } = options;
    let fileData = new FormData();
    fileData.append("file", file);
    handleUpdate(fileData);
  };

  return (
    <>
      <Upload
        accept=".xlsx,.xls"
        showUploadList={false}
        customRequest={handleUpload}
      >
        <Button type="primary">
          <LocaleText id={"incentive.button.importText"} />
        </Button>
      </Upload>
    </>
  );
};
