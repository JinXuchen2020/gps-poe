import { FunctionComponent, ReactNode, useMemo } from "react";
import { Typography } from "antd";
import {
  CloseOutlined,
  CheckCircleFilled,
  WarningFilled,
  CloseCircleFilled,
} from "@ant-design/icons";

export const NotificationBar: FunctionComponent<{
  hidden: boolean;
  content: ReactNode;
  type?: string;
  close: any;
}> = ({ content, hidden, type, close }) => {
  const notificationIcon = useMemo(() => {
    if (type) {
      switch (type) {
        case "warning":
          return <WarningFilled style={{ color: "#52c41a" }} />;
        case "success":
          return <CheckCircleFilled style={{ color: "#52c41a" }} />;
        case "error":
          return <CloseCircleFilled style={{ color: "red" }} />;
      }
    }
  }, [type]);
  return (
    <div hidden={hidden} className="notification">
      <div style={{ display: "flex", justifyContent: "left" }}>
        <span>{notificationIcon}</span>
        <Typography.Text ellipsis={true} title={content?.toLocaleString()}>
          {content}
        </Typography.Text>
      </div>
      <CloseOutlined className="close" onClick={close} />
    </div>
  );
};
