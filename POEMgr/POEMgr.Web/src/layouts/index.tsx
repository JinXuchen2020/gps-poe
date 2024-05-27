import {
  IRoute,
  matchRoutes,
  Outlet,
  SelectLang,
  useAccess,
  useAccessMarkedRoutes,
  useAppData,
  useLocation,
  useNavigate,
} from "umi";
import "./index.less";
import { Layout, Menu, Breadcrumb } from "antd";
import { ReactNode, useEffect, useMemo, useState } from "react";
import {
  AreaChartOutlined,
  ApartmentOutlined,
  SettingOutlined,
  SendOutlined,
  EyeOutlined,
} from "@ant-design/icons";
import { Exception, Header, LocaleText } from "@/components";
import MediaQuery from "react-responsive";

const { Sider, Content } = Layout;

interface MenuItem {
  key: string;
  label: ReactNode;
  icon: ReactNode;
  hidden: boolean;
  children?: { key: string; label: ReactNode; hidden: boolean }[];
}

export default () => {
  const history = useNavigate();
  const location = useLocation();
  const [selectedMenuItem, setSelectedMenuItem] = useState<MenuItem>();
  const access = useAccess();
  const { clientRoutes } = useAppData();
  const [matchRoute, setMatchRoute] = useState<IRoute>();
  const markedRoutes = useAccessMarkedRoutes(clientRoutes);

  const menuItems: MenuItem[] = [
    {
      key: "/incentiveRelease",
      label: <LocaleText id={"incentiveRelease.titleText"} />,
      icon: <SendOutlined />,
      hidden: !access.isAdmin,
    },
    {
      key: "/incentives",
      label: <LocaleText id={"incentive.titleText"} />,
      icon: <ApartmentOutlined />,
      hidden: !access.isAdmin,
    },
    {
      key: "/census",
      label: <LocaleText id={"census.titleText"} />,
      icon: <AreaChartOutlined />,
      hidden: !access.isAdmin,
    },
    {
      key: "/audits",
      label: <LocaleText id={"audit.titleText"} />,
      icon: <EyeOutlined />,
      hidden: !access.isAdmin,
    },
    {
      key: "/poeRequests",
      label: <LocaleText id={"request.titleText"} />,
      icon: (
        <img width={12} height={12} src={require("@/assets/request.png")} />
      ),
      hidden: !access.isUser,
    },
    {
      key: "/histories",
      label: <LocaleText id={"history.titleText"} />,
      icon: (
        <img width={12} height={12} src={require("@/assets/history.png")} />
      ),
      hidden: !access.isUser,
    },
    {
      key: "/users",
      label: <LocaleText id={"user.titleText"} />,
      icon: <SettingOutlined />,
      hidden: !access.isAdmin,
    },
  ];

  const setRouteActive = (menuInfo: any) => {
    history(`${menuInfo.key}`);
  };

  useEffect(() => {
    const menuItem = menuItems.find((c) => location.pathname.startsWith(c.key));
    setSelectedMenuItem(menuItem);
  }, [location.pathname]);

  useEffect(() => {
    const route = matchRoutes(markedRoutes, location.pathname)?.pop()?.route as
      | IRoute
      | undefined;
    setMatchRoute(route);
  }, [location.pathname]);

  return (
    <Layout>
      <MediaQuery maxWidth={800}>
        <Sider className="side-bar" collapsible={false} collapsed={true}>
          <div className="header">
            <div><img width={16} height={16} src={require("@/assets/favicon.ico")} /></div>
            <SelectLang />
          </div>
          <Menu
            mode="inline"
            activeKey={selectedMenuItem?.key}
            selectedKeys={[selectedMenuItem?.key!]}
            onClick={setRouteActive}
            items={menuItems}
          />
        </Sider>
        <Content className="left-content">
          <Header>
            <Breadcrumb.Item>{selectedMenuItem?.label}</Breadcrumb.Item>
          </Header>
          <Exception route={matchRoute}>
            <Outlet />
          </Exception>
        </Content>
      </MediaQuery>
      <MediaQuery minWidth={800}>
        <Sider className="side-bar" collapsible={false} width={235}>
          <div className="header">
            <div>
              <span><img width={14} height={14} src={require("@/assets/favicon.ico")} /></span>
              <span style={{ marginLeft: 10, marginTop: 2 }}><LocaleText id={"header.titleText"} /></span>
            </div>
            <SelectLang />
          </div>
          <Menu
            mode="inline"
            activeKey={selectedMenuItem?.key}
            selectedKeys={[selectedMenuItem?.key!]}
            onClick={setRouteActive}
            items={menuItems}
          />
        </Sider>
        <Content className="left-content">
          <Header>
            <Breadcrumb.Item>{selectedMenuItem?.label}</Breadcrumb.Item>
          </Header>
          <Exception route={matchRoute}>
            <Outlet />
          </Exception>
        </Content>
      </MediaQuery>
    </Layout>
  );
};
