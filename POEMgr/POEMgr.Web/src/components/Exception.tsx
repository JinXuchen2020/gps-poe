import React from "react";
import { useNavigate, type IRoute } from "umi";
import { Result, Button } from "antd";
import { LocaleText } from "./LocaleText";

export const Exception: React.FC<{
  children: React.ReactNode;
  route?: IRoute;
  notFound?: React.ReactNode;
  noAccessible?: React.ReactNode;
  unAccessible?: React.ReactNode;
  noFound?: React.ReactNode;
}> = (props) => {
  const navigate = useNavigate();
  return (
    // render custom 404
    (!props.route && (props.noFound || props.notFound)) ||
    // render custom 403
    (props.route?.unaccessible && (props.unAccessible || props.noAccessible)) ||
    // render default exception
    ((!props.route || props.route.unaccessible) && (
      <Result
        status={props.route ? "403" : "404"}
        title={props.route ? "403" : "404"}
        subTitle={
          props.route ? (
            <LocaleText id={"exception.noPermissionText"} />
          ) : (
            <LocaleText id={"exception.notFoundText"} />
          )
        }
        extra={
          <Button type="primary" onClick={() => navigate("/")}>
            <LocaleText id={"backText"} />
          </Button>
        }
      />
    )) ||
    // normal render
    props.children
  );
};
