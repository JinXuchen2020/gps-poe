import '@wangeditor/editor/dist/css/style.css'
import React, { useState, useEffect, FC } from "react";
import { Editor, Toolbar } from "@wangeditor/editor-for-react";
import { IDomEditor, IEditorConfig, IToolbarConfig, i18nChangeLanguage, SlateTransforms, DomEditor } from "@wangeditor/editor";
import { uploadFile } from '@/services/request';
import { getLocale } from 'umi';
import { convertBase64ToBlob } from '@/utils';

export const EmailEditor: FC<{data: string | undefined, update: any}> = ({data, update}) => {
  // editor 实例
  const [editor, setEditor] = useState<IDomEditor | null>(null); // TS 语法

  const handleUpdate = async (editor: IDomEditor) => {
    if(editor && update) {
      const images = editor.getElemsByType("image");

      await Promise.all(images.map(async (image) => {
        const src = image['src'] as string;
        if(!src.startsWith("https")) {
          const fileBlob = convertBase64ToBlob(src)
          let fileData = new FormData();
          fileData.append("file", fileBlob);
          const res = await uploadFile(fileData)
          image['src'] = res.data.path
          image['style'] = {
            width: '100%'
          }
          const domNode = document.getElementById(image.id)!;
          const slateNode = DomEditor.toSlateNode(editor, domNode);
          const path = DomEditor.findPath(editor, slateNode);
          SlateTransforms.setNodes(editor, image, { at: path})
        }
      }))

      update(editor.getHtml())
    }
  }

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
          value={data}
          onCreated={setEditor}
          onChange={(editor) => handleUpdate(editor)}
          mode="default"
          style={{ height: "500px", overflowY: "hidden" }}
        />
      </div>
    </>
  );
}
