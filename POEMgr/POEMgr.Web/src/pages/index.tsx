import { PathAccess } from "@/constant";
import { useEffect } from "react";
import { useAccessMarkedRoutes, useAppData, useNavigate } from "umi";

export default () => {
  const navigate = useNavigate()
  const { clientRoutes } = useAppData();
  const markedRoutes = useAccessMarkedRoutes(clientRoutes);

  useEffect(() => {
    const accessRoutes = markedRoutes[0].routes.filter(c=>!c['unaccessible'] && PathAccess.map(c=>c.path).includes(c["path"]))
    if(accessRoutes.length > 0) {
      navigate(accessRoutes[0].path)
    }
  },[markedRoutes])
  return (
    <>
    </>
  );
}
