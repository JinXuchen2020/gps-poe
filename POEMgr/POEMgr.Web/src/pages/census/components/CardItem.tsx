import { FunctionComponent, ReactNode } from "react";

export const CardItem: FunctionComponent<{
  title: ReactNode;
  data: ReactNode;
  action: any;
}> = ({ title, data, action }) => {
  return (
    <div className="card-item" onClick={action}>
      <div className="card-title">{title}</div>
      <div className="card-content">{data}</div>
    </div>
  );
};