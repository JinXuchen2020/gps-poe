export default {
  npmClient: 'npm',
  plugins: [
    '@umijs/plugins/dist/dva',
    '@umijs/plugins/dist/antd',
    '@umijs/plugins/dist/access',
    '@umijs/plugins/dist/locale',
    '@umijs/plugins/dist/initial-state',
    '@umijs/plugins/dist/model',
  ],
  dva: {},
  access: {},
  initialState: {},
  model: {},
  antd: {},
  hash: true,
  base:'/gps-poe/',
  publicPath: '/gps-poe/',
  locale: {
    default: 'zh-CN',
    baseSeparator: '-',
    baseNavigator: false, // 默认为true。为true时，会使用`navigator.language`覆盖默认
  },
  links:[{rel:'icon', href: `/microsoft.png`}],
  conventionRoutes: {
    exclude: [/\/components\//, /\/model/],
  },
  extraBabelPlugins:
    process.env.NODE_ENV === 'production'
      ? ['babel-plugin-dynamic-import-node']
      : [],
  define: {
    'process.env': {
      API_ROOT:
        process.env.NODE_ENV === 'production'
          ? 'https://gps-poe-api.chinacloudsites.cn/api/'
          : 'http://localhost:3000/api/',
      CLIENT_ID: "4f305035-d3ee-4c0c-9883-17d877417649",
    }
  }
};
