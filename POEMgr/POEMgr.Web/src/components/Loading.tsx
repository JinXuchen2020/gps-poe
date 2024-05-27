import { Spin } from "antd";
import { FunctionComponent } from "react";
import { LocaleText } from "./LocaleText";

export const Loading: FunctionComponent<{ loading: boolean }> = ({
  loading,
}) => {
  return (
    <div hidden={!loading}>
      <div className="ant-modal-mask">
        <div className="ant-modal-wrap ant-modal-centered">
          <div className="ant-modal" style={{ width: 150 }}>
            <div className="ant-modal-content">
              <div
                className="ant-modal-body"
                style={{ height: 100, verticalAlign: "middle" }}
              >
                <Spin
                  size="large"
                  style={{ marginTop: 25 }}
                  tip={<LocaleText id={"loadingText"} />}
                >
                  {" "}
                </Spin>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
