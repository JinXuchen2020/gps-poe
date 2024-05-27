GO
--序号表初始化
if (select count(1) from Poe_CurrentNumber)=0
begin
insert into [dbo].[Poe_CurrentNumber]
select 'Sys_User','10008'
end
GO
--角色表初始化
if (select count(1) from Sys_Role)=0
begin
insert into [dbo].[Sys_Role]
select '001','Partner',getdate(),'initialization',getdate(),'initialization',0
union all
select '002','Auditor',getdate(),'initialization',getdate(),'initialization',0
union all
select '003','Admin',getdate(),'initialization',getdate(),'initialization',0
union all
select '004','SuperAdmin',getdate(),'initialization',getdate(),'initialization',0
end
GO
--用户表初始化
if (select count(1) from Sys_User)=0
begin
INSERT INTO [dbo].[Sys_User]
select '10001','yingjie','yingjie','v-yingjchen@microsoft.com',getdate(),'initialization',getdate(),'initialization',0
union all
select '10002','jinquan','jinquan','v-qujin@microsoft.com',getdate(),'initialization',getdate(),'initialization',0
union all
select '10003','gaochenjun','gaochenjun','v-chenjungao@microsoft.com',getdate(),'initialization',getdate(),'initialization',0
union all
select '10004','jinquan','jinquan','jinquan@beyondsoft.com',getdate(),'initialization',getdate(),'initialization',0
union all
select '10005','chenyingjie04 (BYS)','chenyingjie04 (BYS)','chenyingjie04@us.beyondsoft.com',getdate(),'initialization',getdate(),'initialization',0
union all
select '10006','weigang','weigang','v-weigz@microsoft.com',getdate(),'initialization',getdate(),'initialization',0
union all
select '10007','nickey','nickey','v-nickey@microsoft.com',getdate(),'initialization',getdate(),'initialization',0
union all
select '10008','vinchanmo','vinchanmo','v-vinchanmou@microsoft.com',getdate(),'initialization',getdate(),'initialization',0
end
GO
--用户角色表初始化
if (select count(1) from Sys_UserRole)=0
begin
INSERT INTO [dbo].[Sys_UserRole]
select '00001','10001','004',getdate(),'initialization',getdate(),'initialization',0
union all
select '00002','10002','004',getdate(),'initialization',getdate(),'initialization',0
union all
select '00003','10003','004',getdate(),'initialization',getdate(),'initialization',0
union all
select '00004','10004','004',getdate(),'initialization',getdate(),'initialization',0
union all
select '00005','10005','004',getdate(),'initialization',getdate(),'initialization',0
union all
select '00006','10006','004',getdate(),'initialization',getdate(),'initialization',0
union all
select '00007','10007','004',getdate(),'initialization',getdate(),'initialization',0
union all
select '00008','10008','004',getdate(),'initialization',getdate(),'initialization',0
end
GO
