import { FunctionComponent, useEffect, useRef, useState } from "react";
import { IPoeRequestFileRspModel } from "@/models";
import { Loading } from "./Loading";

export const FileViewer: FunctionComponent<{
  data: IPoeRequestFileRspModel | undefined;
}> = ({ data }) => {
  const iframeRef = useRef(null)
  const [isLoading, setIsLoading] = useState(true)

  useEffect(() => {
    const iframe = iframeRef.current as any
    if (iframe) {      
      iframe.onload = () => {
        setIsLoading(false)
      }
    }
    else {
      setIsLoading(false)
    }
    return () => {
      iframeRef.current = null
    }
  },[])
  return (
    <>
      <Loading loading={isLoading} />
      {data && (
        <iframe
          className="iframe-content"
          loading="lazy"
          ref={iframeRef}
          src={`https://view.officeapps.live.com/op/view.aspx?src=${encodeURIComponent(
            data.path
          )}`}
        ></iframe>
      )}
    </>
  );
};
