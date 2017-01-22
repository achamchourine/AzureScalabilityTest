if( not exists(select * from sys.schemas where name = 'Test') ) exec('create schema [Test]')
go
if( exists(select * from sys.tables where object_id = object_id('Test.CustomerIds')) ) drop table Test.CustomerIds
if( exists(select * from sys.tables where object_id = object_id('Test.Customers')) ) drop table Test.Customers
if( exists(select * from sys.tables where object_id = object_id('Test.Orders')) ) drop table Test.Orders
if( exists(select * from sys.tables where object_id = object_id('Test.Items')) ) drop table Test.Items
go
create table [Test].[CustomerIds](
[CustomerEmail] varchar(256) not null,
[CustomerPwdHash] varchar(512) not null,
[CustomerId] uniqueidentifier not null
)
go
create index [IXCustomerIds] on [Test].[CustomerIds]([CustomerEmail]) include([CustomerId], [CustomerPwdHash])
go
create table [Test].[Customers](
[CustomerId] uniqueidentifier not null,
[CustomerName] varchar(256) null,
[CreatedOn] datetime not null constraint [DF_Customers_CreatedOn] default getdate(),
[Enabled] bit not null constraint [DF_Customers_Enabled] default 1
)
go
create index [IXCustomers1] on [Test].[Customers]([CustomerId]) include([CustomerName])
go
create index [IXCustomers2] on [Test].[Customers]([CustomerName]) include([CustomerId])
go
create table [Test].[Orders](
[CustomerId] uniqueidentifier not null,
[OrderId] uniqueidentifier not null,
[CreatedOn] datetime not null constraint [DF_Orders_CreatedOn] default getdate(),
[ShipTo] varchar(256) null,
[BillTo] varchar(256) null,
[Status] int not null,
[ShippingFee] money null,
[Taxes] money null
)
go
create index [IXOrders1] on [Test].[Orders]([CustomerId]) include([OrderId])
go
create index [IXOrders2] on [Test].[Orders]([OrderId]) include([CustomerId])
go
create table [Test].[Items](
[CustomerId] uniqueidentifier not null,
[OrderId] uniqueidentifier not null,
[ItemId] uniqueidentifier not null,
[ProductKey] varchar(100) not null,
[ItemDscr] varchar(1000) not null,
[ItemQty] int not null,
[ItemPrice] money not null
)
go
create index [IXItems1] on [Test].[Items]([ItemId]) include([OrderId], [CustomerId])
go
create index [IXItems2] on [Test].[Items]([OrderId]) include([ItemId], [CustomerId])
go
create index [IXItems3] on [Test].[Items]([CustomerId]) include([OrderId], [ItemId])
go
