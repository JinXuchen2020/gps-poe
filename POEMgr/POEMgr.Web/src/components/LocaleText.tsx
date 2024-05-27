import { FunctionComponent } from "react";
import { FormattedMessage, useIntl } from "umi";

export const LocaleText: FunctionComponent<{ id: string; values?: {} }> = ({
  id,
  values,
}) => {
  const intl = useIntl();
  const msg = (id: string) =>
    intl.formatMessage(
      {
        id: id,
      },
      {
        ...values,
      }
    );
  return <>{msg(id)}</>;
};
