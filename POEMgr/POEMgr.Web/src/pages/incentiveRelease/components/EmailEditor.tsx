import "@wangeditor/editor/dist/css/style.css";
import React, { useState, useEffect, FC } from "react";
import { Editor, Toolbar } from "@wangeditor/editor-for-react";
import { i18nChangeLanguage, IDomEditor, IEditorConfig, IToolbarConfig } from "@wangeditor/editor";
import { IMailTemplateRspModel } from "@/models";
import { uploadFile } from "@/services/request";
import { LocaleText } from "@/components";
import { getLocale } from "umi";

export const EmailEditor: FC<{
  data: IMailTemplateRspModel | undefined;
  update: any;
}> = ({ data, update }) => {
  // editor 实例
  const [editor, setEditor] = useState<IDomEditor | null>(null); // TS 语法

  const handleUpdate = (editor: IDomEditor) => {
    if (editor && data) {
      update({content: editor.getHtml()});
    }
  };

  // 工具栏配置
  const toolbarConfig: Partial<IToolbarConfig> = {}; // TS 语法

  // 编辑器配置
  const editorConfig: Partial<IEditorConfig> = {
    MENU_CONF : {
      'uploadImage' : {
        'customUpload': async (file: File, insertFn: any) => {
          let fileData = new FormData();
          fileData.append("file", file);
          const item = await uploadFile(fileData);
          insertFn(item.data.path, item.data.name, item.data.path)
        }
      }
    }
  };

  // 及时销毁 editor ，重要！
  useEffect(() => {
    const lng = getLocale()
    i18nChangeLanguage(lng)
    return () => {
      if (editor == null) return;
      editor.destroy();
      setEditor(null);
    };
  }, [editor]);

  return (
    <>
      <div style={{ border: "1px solid #ccc", zIndex: 100 }}>
        <Toolbar
          editor={editor}
          defaultConfig={toolbarConfig}
          mode="default"
          style={{ borderBottom: "1px solid #ccc" }}
        />
        <Editor
          defaultConfig={editorConfig}
          defaultHtml={data?.content}
          value={data?.content}
          onCreated={setEditor}
          onChange={(editor) => handleUpdate(editor)}
          mode="default"
          style={{ height: "500px", overflowY: "hidden" }}
        />
      </div>
      <div style={{ marginTop: "15px" }}>
        <span>
          <LocaleText id={"incentiveRelease.emailEditor.description.title"} />
        </span>
        <br />{" "}
        <span>
          <LocaleText id={"incentiveRelease.emailEditor.description.content"} />
        </span>
      </div>
    </>
  );
};
