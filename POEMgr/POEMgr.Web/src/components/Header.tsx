import { FunctionComponent } from "react";
import { Breadcrumb } from "antd";
import { useModel } from "umi";
import { logout } from "@/services/user";
import { LocaleText } from "./LocaleText";

export const Header: FunctionComponent<{
  children: any;
}> = ({ children }) => {
  const { initialState } = useModel("@@initialState");
  return (
    <div className="breadcrumb">
      <Breadcrumb>
        <Breadcrumb.Item>
          <LocaleText id={"header.breadcrumbDefaultText"} />
        </Breadcrumb.Item>
        {children}
      </Breadcrumb>
      <div className="user-info">
        <span>{initialState?.user?.email}</span>
        <span onClick={logout}>
          <LocaleText id={"header.logOutText"} />
        </span>
      </div>
    </div>
  );
};
