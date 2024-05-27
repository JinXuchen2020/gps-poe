import { FunctionComponent, ReactNode, useState } from "react";
import { Button, Space, Typography } from "antd";
import { DownloadOutlined, PaperClipOutlined } from "@ant-design/icons";
import { IPoeRequestRspModel } from "@/models";
import { downloadPoeFiles } from "@/services/audit";
import fileDownload from "js-file-download";
import { Loading } from "./Loading";

export const DownloadButton: FunctionComponent<{
  title: ReactNode;
  disabled: boolean;
  data: IPoeRequestRspModel | undefined;
}> = ({ title, disabled, data }) => {
  const [loading, setLoading] = useState(false)
  const handleDownload = async () => {
    if(data) {
      setLoading(true)
      const file = await downloadPoeFiles(data.id);
      fileDownload(file, "POE文件.zip");
      setLoading(false)
    }
  };

  return (
    <>
      <Loading loading={loading} />
      <Button hidden={disabled} type="default" icon={<DownloadOutlined />} onClick={handleDownload}>
        {title}
      </Button>
    </>
  );
};
