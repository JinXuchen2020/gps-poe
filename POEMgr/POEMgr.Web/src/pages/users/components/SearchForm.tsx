import { IUserQueryOption } from "@/models";
import { Button, Form, Input } from "antd";
import { FC } from "react";
import { useSearchParams } from "umi";
import queryString from "query-string";
import { LocaleText } from "@/components";

export const SearchForm: FC = () => {
  const [form] = Form.useForm();
  const [, setSearchParams] = useSearchParams();

  const handleSearch = async () => {
    const formValue = (await form.validateFields()) as IUserQueryOption;
    const filterString = queryString.stringify(formValue);
    setSearchParams(filterString);
    form.resetFields();
  };

  return (
    <Form form={form} layout={"inline"}>
      <Form.Item
        label={<LocaleText id={"user.filter.partnerNameText"} />}
        name="partnerName"
      >
        <Input />
      </Form.Item>
      <Form.Item
        label={<LocaleText id={"user.filter.partnerEmailText"} />}
        name="partnerEmail"
      >
        <Input />
      </Form.Item>
      <Form.Item>
        <Button type="primary" onClick={handleSearch}>
          <LocaleText id={"user.filter.button.searchText"} />
        </Button>
      </Form.Item>
    </Form>
  );
};
