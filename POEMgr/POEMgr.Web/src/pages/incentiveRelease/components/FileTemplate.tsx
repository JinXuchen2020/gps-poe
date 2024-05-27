import { LocaleText } from "@/components";
import { Typography } from "antd";
import { FC, ReactNode } from "react";

export const FileTemplate: FC<{
  title: ReactNode;
  edit: any;
  preview: any;
}> = ({ title, edit, preview }) => {
  return (
    <div>
      <span>{title}</span>
      <span style={{ float: "right" }}>
        <Typography.Link style={{ marginRight: 30 }} onClick={edit}>
          <LocaleText id={"incentiveRelease.mailTemplate.editText"} />
        </Typography.Link>
        <Typography.Link style={{ marginRight: 30 }} onClick={preview}>
          <LocaleText id={"incentiveRelease.mailTemplate.previewText"} />
        </Typography.Link>
      </span>
    </div>
  );
};
