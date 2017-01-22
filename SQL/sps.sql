if( exists(select * from sys.procedures where object_id = object_id('Test.GetCustomer')) ) drop procedure Test.GetCustomer
if( exists(select * from sys.procedures where object_id = object_id('Test.GetCustomerOrders')) ) drop procedure Test.GetCustomerOrders
if( exists(select * from sys.procedures where object_id = object_id('Test.GetOrderInfo')) ) drop procedure Test.GetOrderInfo
if( exists(select * from sys.procedures where object_id = object_id('Test.GetOrderItems')) ) drop procedure Test.GetOrderItems
if( exists(select * from sys.procedures where object_id = object_id('Test.NewOrder')) ) drop procedure Test.NewOrder
if( exists(select * from sys.procedures where object_id = object_id('Test.NewItem')) ) drop procedure Test.NewItem
if( exists(select * from sys.procedures where object_id = object_id('Test.UpdateItem')) ) drop procedure Test.UpdateItem
if( exists(select * from sys.procedures where object_id = object_id('Test.DeleteItem')) ) drop procedure Test.DeleteItem
if( exists(select * from sys.procedures where object_id = object_id('Test.CheckoutOrder')) ) drop procedure Test.CheckoutOrder
if( exists(select * from sys.procedures where object_id = object_id('Test.UpdateOrderInfo')) ) drop procedure Test.UpdateOrderInfo
if( exists(select * from sys.procedures where object_id = object_id('Test.NewOrder2')) ) drop procedure Test.NewOrder2
if( exists(select * from sys.procedures where object_id = object_id('Test.GetCustomerId')) ) drop procedure Test.GetCustomerId
go
create procedure [Test].[GetCustomerId]
(
    @CustomerEmail varchar(256),
    @CustomerPwdHash varchar(512),

    @CustomerId uniqueidentifier output
)
as
begin
    /* 
        One call for new and existing customers (for simplicity, to emulate CustomerId retrieving in reality it would be two calls for signing in or signing up)
    */
    set @CustomerId = (select [CustomerId] from [Test].[CustomerIds] where [CustomerEmail] = @CustomerEmail and [CustomerPwdHash] = @CustomerPwdHash)

    if( @CustomerId is null)
    begin
        set @CustomerId = NEWID()

        insert [Test].[CustomerIds]([CustomerEmail], [CustomerPwdHash], [CustomerId]) values(@CustomerEmail, @CustomerPwdHash, @CustomerId)

        insert [Test].[Customers]([CustomerId]) values(@CustomerId)
    end
end
go
create procedure [Test].[GetCustomer]
(
    @CustomerId uniqueidentifier
)
as
begin
    select [CustomerName], [CreatedOn], [Enabled]
      from [Test].[Customers]
     where [CustomerId] = @CustomerId
end
go
create procedure [Test].[GetCustomerOrders]
(
    @CustomerId uniqueidentifier
)
as
begin
    select o.[OrderId], o.[CreatedOn], o.[Status], [OrderTotal] = sum(i.[ItemPrice])
      from [Test].[Orders] o
           join [Test].[Items] i on i.[OrderId] = o.[OrderId]
     where o.[CustomerId] = @CustomerId
    group by o.[OrderId], o.[CreatedOn], o.[Status]
    order by o.[CreatedOn]
end
go
create procedure [Test].[GetOrderInfo]
(
    @CustomerId uniqueidentifier,
    @OrderId uniqueidentifier
)
as
begin
    select o.[CreatedOn], o.[ShipTo], o.[BillTo], o.[Status], o.[ShippingFee], o.[Taxes]
      from [Test].[Orders] o
     where o.[CustomerId] = @CustomerId
       and o.[OrderId] = @OrderId
end
go
create procedure [Test].[GetOrderItems]
(
    @CustomerId uniqueidentifier,
    @OrderId uniqueidentifier
)
as
begin
    select i.[ItemId], i.[ProductKey], i.[ItemDscr], i.[ItemQty], i.[ItemPrice]
      from [Test].[Items] i
     where i.[CustomerId] = @CustomerId
       and i.[OrderId] = @OrderId
    order by i.[ProductKey]
end
go
create procedure [Test].[NewOrder]
(
    @CustomerId uniqueidentifier,
    @OrderId uniqueidentifier output
)
as
begin
    if(@OrderId is null)
        set @Orderid = newid()
    
    if( not exists(select * from [Test].[Customers] where [CustomerId] = @CustomerId) )
    begin
        raiserror('Wrong CustomerId!', 16, 1)
    end
    begin
        insert [Test].[Orders]([CustomerId], [OrderId], [Status]) values(@CustomerId, @OrderId, -1)
    end
end
go
create procedure [Test].[NewItem]
(
    @CustomerId uniqueidentifier,
    @OrderId uniqueidentifier output,
    @ItemId uniqueidentifier output,

    @ProductKey varchar(100),
    @ItemDscr varchar(1000),
    @ItemQty int,
    @ItemPrice money
)
as
begin
/*
    @OrderId If null an order in open status will be used, if there is no one - will be added. 
             If not null will use provided id only if provided id is an open order id!. If there is no such order and there is no open order - one will be added, otherwise - error
    @ItemId  If null will be generated
*/
BEGIN TRY

    if( not exists(select * from [Test].[Customers] where [CustomerId] = @CustomerId) )
    begin
        raiserror('Wrong CustomerId!', 16, 1)
    end

    if(@OrderId is not null and not exists(select * from [Test].[Orders] where [CustomerId] = @CustomerId and [OrderId] = @OrderId and [Status] = -1))
    begin
        raiserror('Wrong OrderId!', 16, 1)
    end

    BEGIN TRAN

    if(@OrderId is null)
    begin
        set @OrderId = (select [OrderId] from [Test].[Orders] where [CustomerId] = @CustomerId and [Status] = -1)
        if(@OrderId is null)
        begin
            set @OrderId = newid()

            EXEC [Test].[NewOrder] @CustomerId = @CustomerId, @OrderId = @OrderId
        end
    end

    if(@ItemId is not null and exists(select * from [Test].[Items] where [CustomerId] = @CustomerId and [ItemId] = @ItemId))
    begin
        raiserror('Wrong ItemId!', 16, 1)
    end

    if(@ItemId is null)
        set @ItemId = newid()

    insert [Test].[Items]([CustomerId], [OrderId], [ItemId], [ProductKey], [ItemDscr], [ItemQty], [ItemPrice])
    values(@CustomerId, @OrderId, @ItemId, @ProductKey, @ItemDscr, @ItemQty, @ItemPrice)

    if(@@TRANCOUNT > 0) COMMIT

END TRY
BEGIN CATCH

    if(@@TRANCOUNT > 0) ROLLBACK;
    THROW;

END CATCH

end
go
create procedure [Test].[UpdateItem]
(
    @CustomerId uniqueidentifier,
    @ItemId uniqueidentifier,

    @ItemQty int,
    @ItemPrice money
)
as
begin
    declare @OrderId uniqueidentifier

    set @orderId = (select [OrderId] from [Test].[Items] where [CustomerId] = @CustomerId and [ItemId] = @ItemId)

    if(not exists(select * from [Test].[Orders] where [CustomerId] = @CustomerId and [OrderId] = @OrderId and [Status] = -1))
    begin
        raiserror('Order closed!', 16, 1)
    end
    begin
        update [Test].[Items] set [ItemQty] = @ItemQty, [ItemPrice] = @ItemPrice where [CustomerId] = @CustomerId and [ItemId] = @ItemId
    end
end
go
create procedure [Test].[DeleteItem]
(
    @CustomerId uniqueidentifier,
    @ItemId uniqueidentifier
)
as
begin
    declare @OrderId uniqueidentifier

    set @orderId = (select [OrderId] from [Test].[Items] where [CustomerId] = @CustomerId and [ItemId] = @ItemId)

    if(not exists(select * from [Test].[Orders] where [CustomerId] = @CustomerId and [OrderId] = @OrderId and [Status] = -1))
    begin
        raiserror('Order closed!', 16, 1)
    end
    begin
        delete from [Test].[Items] where [CustomerId] = @CustomerId and [ItemId] = @ItemId
    end
end
go
create procedure [Test].[CheckoutOrder]
(
    @CustomerId uniqueidentifier,
    @OrderId uniqueidentifier
)
as
begin
    if(not exists(select * from [Test].[Orders] where [CustomerId] = @CustomerId and [OrderId] = @OrderId and [Status] = -1))
    begin
        raiserror('Wrong order!', 16, 1)
    end
    begin
        update [Test].[Orders] set [Status] = 0 where [CustomerId] = @CustomerId and [OrderId] = @OrderId
    end
end
go
create procedure [Test].[UpdateOrderInfo]
(
    @CustomerId uniqueidentifier,
    @OrderId uniqueidentifier,
    @ShipTo varchar(256),
    @BillTo varchar(256),
    @ShippingFee money,
    @Taxes money
)
as
begin
    if(not exists(select * from [Test].[Orders] where [CustomerId] = @CustomerId and [OrderId] = @OrderId and [Status] = -1))
    begin
        raiserror('Wrong order!', 16, 1)
    end
    begin
        update [Test].[Orders] set [ShipTo] = @ShipTo, [BillTo] = @BillTo, [ShippingFee] = @ShippingFee, [Taxes] = @Taxes where [CustomerId] = @CustomerId and [OrderId] = @OrderId
    end
end
go
create procedure [Test].[NewOrder2]
(
    @CustomerId uniqueidentifier,
    @OrderId uniqueidentifier output
)
as
begin
    if(@OrderId is null)
        set @Orderid = newid()
    
    insert [Test].[Orders]([CustomerId], [OrderId], [Status]) values(@CustomerId, @OrderId, -1)

    insert [Test].[Items]([CustomerId], [OrderId], [ItemId], [ProductKey], [ItemDscr], [ItemQty], [ItemPrice])
    values(@CustomerId, @OrderId, newid(), newid(), '', 1, 1)

end
go
