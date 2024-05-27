import { Typography } from "antd";
import { useLocation, useNavigate } from "umi";
import { FileViewer, LocaleText } from "@/components";
import { IPoeRequestFileRspModel } from "@/models";

export default () => {
  const location = useLocation();
  const navigate = useNavigate();
  const file = location.state as IPoeRequestFileRspModel | undefined;
  return (
    <>
      <div className="page-title" onClick={() => navigate(-1)}>
        <Typography.Link style={{ fontWeight: "normal" }}>
          <LocaleText id={"preview.titleText"} />
        </Typography.Link>
      </div>
      <div className="page-content">
        <FileViewer data={file} />
      </div>
    </>
  );
};
