import { IUserTokenModel } from './models';

export default (initialState: IUserTokenModel | undefined) => {
  const { user, token } = (() => {
    if (initialState) {
      return initialState;
    } else {
      return {
        user: undefined,
        token: undefined,
      } as IUserTokenModel;
    }
  })();

  return {
    isAdmin: user?.roleName === 'SuperAdmin' || user?.roleName === 'Admin',
    isUser: user?.roleName === 'SuperAdmin' || user?.roleName === 'Partner',
  };
};
