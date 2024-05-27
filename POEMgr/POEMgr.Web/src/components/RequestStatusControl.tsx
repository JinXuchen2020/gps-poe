import { RequestStatusLabel } from "@/constant";
import { FunctionComponent, ReactNode } from "react";
import { LocaleText } from "./LocaleText";

export const RequestStatusControl: FunctionComponent<{
  data: string | undefined;
  className?: string;
  title?: ReactNode;
}> = ({ data, className, title }) => {
  return (
    <>
      {data && (
        <>
          <span className={className ?? `status-dot ${data.toLowerCase()}`}>
            ●
          </span>
          <span>
            {title ? (
              title
            ) : (
              <LocaleText
                id={RequestStatusLabel[data as keyof typeof RequestStatusLabel]}
              />
            )}
          </span>
        </>
      )}
    </>
  );
};
