using Authentication;
using AutoMapper;
using LogService;
using MailService;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using POEMgr.Application.Interfaces;
using POEMgr.Application.TransferModels;
using POEMgr.Domain.Models;
using POEMgr.Repository.CommonQuery;
using POEMgr.Repository.DBContext;
using POEMgr.Repository.DbModels;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace POEMgr.Application.Services
{
    internal class BaseService
    {
        protected readonly IMapper _mapper;
        protected readonly POEContext _POEContext;
        private readonly IClaimsAccessor _claimsAccessor;
        private readonly IPoeLogService _iPoeLogService;
        public List<string> _partnerIds = new List<string>();
        public string _userCode = string.Empty;
        public string _userRole = string.Empty;
        //private readonly IHttpContextAccessor httpContextAccessor;
        //private Guid CurrentUserId => httpContextAccessor.CurrentUserId();

        public BaseService(IMapper mapper, POEContext pOEContext, IClaimsAccessor claimsAccessor, IPoeLogService iPoeLogService)
        {
            _mapper = mapper;
            _POEContext = pOEContext;
            _claimsAccessor = claimsAccessor;

            if (_claimsAccessor.RoleName == "Partner")
            {
                _partnerIds.AddRange(pOEContext.Poe_PartnerIncentive.Where(x => x.Mail.ToLower().Contains(_claimsAccessor.Email.ToLower())).Select(c => c.PartnerId));
                _partnerIds = _partnerIds.Distinct().ToList();
            }

            Sys_User sys_User = pOEContext.Sys_User.Where(x => x.Mail.ToLower() == _claimsAccessor.Email.ToLower()).FirstOrDefault();
            if (sys_User != null)
            {
                _userCode = sys_User.Id;
                _userRole = pOEContext.Sys_UserRole.Where(x => x.UserID == sys_User.Id).FirstOrDefault()?.RoleID;
            }
            else
            {
                _userCode = _claimsAccessor.UserId;
            }

            _iPoeLogService = iPoeLogService;
        }

        public T WriteEntityBase<T>(T p) where T : EntityBase
        {
            if (p != null)
            {
                p.CreatedTime = p.CreatedTime ?? DateTime.Now;
                p.ModifiedTime = DateTime.Now;
                p.CreatedBy = p.CreatedBy ?? _userCode;
                p.ModifiedBy = _userCode;
            }
            return p;
        }

        public List<T> WriteEntityBase<T>(List<T> p) where T : EntityBase
        {
            if (p != null)
            {
                p.ForEach((x) => {
                    x.CreatedTime = x.CreatedTime ?? DateTime.Now;
                    x.ModifiedTime = DateTime.Now;
                    x.CreatedBy = x.CreatedBy ?? _userCode;
                    x.ModifiedBy = _userCode;
                });
            }
            return p;
        }

        public async Task<PaginationList<T>> CommonQueryPaginationListWrapper<T>(CommonQueryParam p) where T : class
        {
            if (p.ColumnQueryParams != null)
            {
                p.ColumnQueryParams = p.ColumnQueryParams.Where(x => !string.IsNullOrWhiteSpace(x.Value) || !string.IsNullOrWhiteSpace(x.ValueMin) || !string.IsNullOrWhiteSpace(x.ValueMax)).ToList();
            }
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<CommonQueryBase> querylist = p.ColumnQueryParams == null ? null : JsonConvert.DeserializeObject<List<CommonQueryBase>>(JsonConvert.SerializeObject(p.ColumnQueryParams));
            CustomQueryCollection queries = new CustomQueryCollection();
            //增加枚举类型
            //queries.TypeList.Add(typeof(StockEnums.FinishFlag_enum));
            if (querylist != null)
            {
                foreach (CommonQueryBase item in querylist)
                {
                    foreach (PropertyInfo item1 in properties)
                    {
                        if (item.Name.ToLower() == item1.Name.ToLower())
                        {
                            item.Name = item1.Name;
                            queries.Add(item);
                            break;
                        }
                    }
                }
            }
            if (p.PageSize <= 0)
            {
                p.PageSize = 9999;
            }
            if (p.PageIndex <= 0)
            {
                p.PageIndex = 1;
            }
            List<T> res = await Task.Run(() => { return _POEContext.Set<T>().Where(queries.Count > 0 ? queries.AsExpression<T>() : x => 1 == 1).ToList(); });
            List<T> res1;
            if (p.PageSize == null || p.PageIndex == null || p.PageSize <= 0 || p.PageIndex <= 0)
            {
                res1 = res;
            }
            else
            {
                res1 = res.Skip(Convert.ToInt16(p.PageSize) * (Convert.ToInt16(p.PageIndex) - 1)).Take(Convert.ToInt16(p.PageSize)).ToList();
            }
            PaginationList<T> PaginationList = new PaginationList<T>();
            PaginationList.List = res1;
            PaginationList.Total = res.Count();
            return PaginationList;
        }

        public async Task<PaginationList<T>> CommonQueryPaginationListWrapper<T>(PaginationQueryRequest p) where T : class
        {
            CommonQueryParam cp = PaginationQueryRequestMapToCommonQueryParam(p);
            if (cp.ColumnQueryParams != null)
            {
                cp.ColumnQueryParams = cp.ColumnQueryParams.Where(x => !string.IsNullOrWhiteSpace(x.Value) || !string.IsNullOrWhiteSpace(x.ValueMin) || !string.IsNullOrWhiteSpace(x.ValueMax)).ToList();
            }
            PropertyInfo[] properties = typeof(T).GetProperties();
            List<CommonQueryBase> querylist = cp.ColumnQueryParams == null ? null : JsonConvert.DeserializeObject<List<CommonQueryBase>>(JsonConvert.SerializeObject(cp.ColumnQueryParams));
            CustomQueryCollection queries = new CustomQueryCollection();
            //增加枚举类型
            //queries.TypeList.Add(typeof(StockEnums.FinishFlag_enum));
            if (querylist != null)
            {
                foreach (CommonQueryBase item in querylist)
                {
                    foreach (PropertyInfo item1 in properties)
                    {
                        if (item.Name.ToLower() == item1.Name.ToLower())
                        {
                            item.Name = item1.Name;
                            queries.Add(item);
                            break;
                        }
                    }
                }
            }
            List<T> res = await Task.Run(() => { return _POEContext.Set<T>().Where(queries.Count > 0 ? queries.AsExpression<T>() : x => 1 == 2).ToList(); });
            List<T> res1;
            if (cp.PageSize == null || cp.PageIndex == null || cp.PageSize <= 0 || cp.PageIndex <= 0)
            {
                res1 = res;
            }
            else
            {
                res1 = res.Skip(Convert.ToInt16(cp.PageSize) * (Convert.ToInt16(cp.PageIndex) - 1)).Take(Convert.ToInt16(cp.PageSize)).ToList();
            }
            PaginationList<T> PaginationList = new PaginationList<T>();
            PaginationList.List = res1;
            PaginationList.Total = res.Count();
            return PaginationList;
        }

        public CommonQueryParam PaginationQueryRequestMapToCommonQueryParam(PaginationQueryRequest p)
        {
            CommonQueryParam query = new CommonQueryParam();
            query.Condition = Condition.AndAlso;
            query.PageSize = p.pageSize;
            query.PageIndex = p.pageIndex;
            query.ColumnQueryParams = new List<ColumnQueryParam>();
            foreach (ColumnQueryParamRequest item in p.columnQueryParams)
            {
                query.ColumnQueryParams.Add(new ColumnQueryParam {
                    Name=item.columnName,
                    Operator= Operators.Contains,
                    Value=item.value,
                    ValueMax=String.Empty,
                    ValueMin=String.Empty
                });
            }
            return query;
        }

        public PaginationList<T> ToPaginationList<T>(List<T> p,int? pageIndex,int? pageSize)
        {
            PaginationList<T> PaginationList = new PaginationList<T>();
            PaginationList.Total = p.Count();
            if (pageIndex!=null && pageIndex>0 && pageSize!=null && pageSize>0)
            {
                int pageSize1 = Convert.ToInt16(pageSize);
                int pageIndex1 = Convert.ToInt16(pageIndex);
                p = p.Skip(pageSize1 * pageIndex1 - 1).Take(pageSize1).ToList();
            }
            PaginationList.List = p;
            return PaginationList;
        }

        public string GetCurrentNumber(string id)
        {
            Poe_CurrentNumber Poe_CurrentNumber = _POEContext.Poe_CurrentNumber.Where(x => x.Id == id).FirstOrDefault();
            if (Poe_CurrentNumber == null)
            {
                throw new Exception("Can not get currentId of "+ id);
            }
            string newNumber = (Convert.ToInt32(Poe_CurrentNumber.CurrentNumber) + 1).ToString().PadLeft(Poe_CurrentNumber.CurrentNumber.Length, '0');
            Poe_CurrentNumber.CurrentNumber = newNumber;
            _POEContext.Update(Poe_CurrentNumber);
            _POEContext.SaveChanges();
            return newNumber;
        }

        public void CancelCurrentNumber(string id, string number)
        {
            Poe_CurrentNumber Poe_CurrentNumber = _POEContext.Poe_CurrentNumber.Where(x => x.Id == id).FirstOrDefault();
            if (Poe_CurrentNumber != null && Poe_CurrentNumber.CurrentNumber == number)
            {
                Poe_CurrentNumber.CurrentNumber = (Convert.ToInt32(Poe_CurrentNumber.CurrentNumber) - 1).ToString().PadLeft(Poe_CurrentNumber.CurrentNumber.Length, '0');
                _POEContext.Update(Poe_CurrentNumber);
                _POEContext.SaveChanges();
            }
        }

        public void WriteDbLog(DbLogType logType, string identity, string content)
        {
            _iPoeLogService.WriteDbLog(logType, identity, _userCode, content);
        }

        public string FiscalQuarter(DateTime startDate)
        {
            string result = "FY";
            int year = Convert.ToDateTime(startDate).Year;
            int month = Convert.ToDateTime(startDate).Month;
            result += month >= 7 ? (year + 1).ToString().Substring(2) : year.ToString().Substring(2);

            if (month >= 7 && month <= 9)
            {
                result += "Q1";
            }
            else if (month >= 10 && month <= 12)
            {
                result += "Q2";
            }
            else if (month >= 1 && month <= 3)
            {
                result += "Q3";
            }
            else if (month >= 4 && month <= 6)
            {
                result += "Q4";
            }

            return result;
        }

        public int WorkDayMinus(DateTime start, DateTime end)
        {
            int count = 0;
            while ((end - start).TotalDays >= 1)
            {
                if (start.DayOfWeek != DayOfWeek.Sunday && start.DayOfWeek != DayOfWeek.Saturday)
                {
                    count++;
                }
                start = start.AddDays(1);
            }
            return count;
        }

        public DateTime? WorkEndDayAdd(DateTime start, int days)
        {
            for (int i = days; i > 0; i--)
            {
                start = start.AddDays(1);
                if (start.DayOfWeek == DayOfWeek.Sunday || start.DayOfWeek == DayOfWeek.Saturday)
                {
                    i++;
                }
            }
            return start;
        }
    }
}
