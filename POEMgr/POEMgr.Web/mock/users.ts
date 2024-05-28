import { IListRsp, IUserRspModel } from "@/models";
import queryString from "query-string";

let users: IUserRspModel[] = [
  {
    id: "1",
    partnerId: "6512940",
    partnerEmail: "1122@Microsoft.com",
    partnerName: "Beijing Shouzheng Tongying Software Technology Co., Ltd.",
    roleName: "SuperAdmin",
    isDisabled: "否",
  },
  {
    id: "2",
    partnerId: "6512940",
    partnerEmail: "aabb@hotmail.com",
    partnerName: "Beijing Shouzheng Tongying Software Technology Co., Ltd.",
    roleName: "Partner",
    isDisabled: "否",
  },
];

export default {
  "/api/users": async (req: Request, res: Response) => {
    const { partnerName, partnerEmail } = req.url.includes("?")
      ? queryString.parse(req.url.split("?")[1])
      : { partnerName: undefined, partnerEmail: undefined };

    let filters = users;
    if (partnerName) {
      filters = filters.filter((c) =>
        c.partnerName.includes(partnerName as string)
      );
    }

    if (partnerEmail) {
      filters = filters.filter((c) =>
        c.partnerEmail.includes(partnerEmail as string)
      );
    }

    const data : IListRsp<IUserRspModel> = {
      total: filters.length,
      list: filters
    }

    const result = {
      msg: "ok",
      code: 0,
      data: data,
    };
    res.end(JSON.stringify(result));
  },
  "PUT /api/users/1": async (req: Request, res: Response) => {
    const pathname = req.url;
    const id = pathname.split("/")[pathname.split("/").length - 1];
    const { roleName, isDisabled } = req.body as any;

    const index = users.findIndex((c) => c.id === id);
    users[index].roleName = roleName;
    users[index].isDisabled = isDisabled;

    res.end("ok");
  },
  "/api/users/currentUser": async (req: Request, res: Response) => {
    const data = users[0]
    const result ={
      msg: "ok",
      code: 0,
      data: data
    }

    res.end(JSON.stringify(result));
  },
};
