using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace AzureScalabilityTest
{
    public class Client
    {
        private string connStr;
        private SqlConnection connection;
        private string CustomerId;
        private string OrderId;
        private string ItemId;

        #region Private Methods
            
        private void GetCustomerId()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[GetCustomerId]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerEmail", SqlDbType.VarChar, 256);
            par.Value = Guid.NewGuid().ToString();
            comm.Parameters.Add(par);

            par = new SqlParameter("@CustomerPwdHash", SqlDbType.VarChar, 512);
            par.Value = Guid.NewGuid().ToString();
            comm.Parameters.Add(par);

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Direction = ParameterDirection.Output;
            comm.Parameters.Add(par);

            comm.ExecuteNonQuery();

            CustomerId = Convert.ToString(comm.Parameters["@CustomerId"].Value);
        }

        private void GetCustomer()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[GetCustomer]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            SqlDataAdapter da = new SqlDataAdapter(comm);

            DataSet ds = new DataSet();
            da.Fill(ds);
        }

        private void GetCustomerOrders()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[GetCustomerOrders]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            SqlDataAdapter da = new SqlDataAdapter(comm);

            DataSet ds = new DataSet();
            da.Fill(ds);
        }

        private void NewItem()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[NewItem]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            par = new SqlParameter("@OrderId", SqlDbType.VarChar, 100);
            par.Direction = ParameterDirection.Output;
            comm.Parameters.Add(par);

            par = new SqlParameter("@ItemId", SqlDbType.VarChar, 100);
            par.Direction = ParameterDirection.Output;
            comm.Parameters.Add(par);

            par = new SqlParameter("@ProductKey", SqlDbType.VarChar, 100);
            par.Value = Guid.NewGuid().ToString();
            comm.Parameters.Add(par);

            par = new SqlParameter("@ItemDscr", SqlDbType.VarChar, 1000);
            par.Value = Guid.NewGuid().ToString() + ", " + Guid.NewGuid().ToString();
            comm.Parameters.Add(par);

            Random r1 = new Random();
            int qty = r1.Next(1, 101);
            double price = r1.NextDouble();

            par = new SqlParameter("@ItemQty", SqlDbType.Int);
            par.Value = qty;
            comm.Parameters.Add(par);

            par = new SqlParameter("@ItemPrice", SqlDbType.Money);
            par.Value = qty*price;
            comm.Parameters.Add(par);

            comm.ExecuteNonQuery();

            OrderId = Convert.ToString(comm.Parameters["@OrderId"].Value);
            ItemId = Convert.ToString(comm.Parameters["@ItemId"].Value);
        }

        private void DelItem()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[DeleteItem]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            par = new SqlParameter("@ItemId", SqlDbType.VarChar, 100);
            par.Value = ItemId;
            comm.Parameters.Add(par);

            comm.ExecuteNonQuery();
        }

        private void UpdItem()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[UpdateItem]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            par = new SqlParameter("@ItemId", SqlDbType.VarChar, 100);
            par.Value = ItemId;
            comm.Parameters.Add(par);

            Random r1 = new Random();
            int qty = r1.Next(1, 101);
            double price = r1.NextDouble();

            par = new SqlParameter("@ItemQty", SqlDbType.Int);
            par.Value = qty;
            comm.Parameters.Add(par);

            par = new SqlParameter("@ItemPrice", SqlDbType.Money);
            par.Value = qty * price;
            comm.Parameters.Add(par);

            comm.ExecuteNonQuery();
        }

        private void GetOrderItems()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[GetOrderItems]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            par = new SqlParameter("@OrderId", SqlDbType.VarChar, 100);
            par.Value = OrderId;
            comm.Parameters.Add(par);

            SqlDataAdapter da = new SqlDataAdapter(comm);

            DataSet ds = new DataSet();
            da.Fill(ds);
        }

        private void UpdateOrder()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[UpdateOrderInfo]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            par = new SqlParameter("@OrderId", SqlDbType.VarChar, 100);
            par.Value = OrderId;
            comm.Parameters.Add(par);

            Random r1 = new Random();
            double price = r1.NextDouble();
            double fee = r1.Next(1, 50) * price;
            double taxes = r1.Next(1, 20) * price;

            par = new SqlParameter("@ShippingFee", SqlDbType.Money);
            par.Value = fee;
            comm.Parameters.Add(par);

            par = new SqlParameter("@Taxes", SqlDbType.Money);
            par.Value = taxes;
            comm.Parameters.Add(par);

            par = new SqlParameter("@ShipTo", SqlDbType.VarChar, 256);
            par.Value = Guid.NewGuid().ToString() + "," + Guid.NewGuid().ToString();
            comm.Parameters.Add(par);

            par = new SqlParameter("@BillTo", SqlDbType.VarChar, 256);
            par.Value = Guid.NewGuid().ToString() + "," + Guid.NewGuid().ToString();
            comm.Parameters.Add(par);

            comm.ExecuteNonQuery();
           
        }

        private void GetOrderInfo()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[GetOrderInfo]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            par = new SqlParameter("@OrderId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            SqlDataAdapter da = new SqlDataAdapter(comm);

            DataSet ds = new DataSet();
            da.Fill(ds);
            
        }

        private void CheckoutOrder()
        {
            SqlParameter par;

            var comm = new SqlCommand("[Test].[CheckoutOrder]", connection);
            comm.CommandType = CommandType.StoredProcedure;

            par = new SqlParameter("@CustomerId", SqlDbType.VarChar, 100);
            par.Value = CustomerId;
            comm.Parameters.Add(par);

            par = new SqlParameter("@OrderId", SqlDbType.VarChar, 100);
            par.Value = OrderId;
            comm.Parameters.Add(par);

            comm.ExecuteNonQuery();
           
        }

        #endregion

        public void RunClientInteraction()
        {
            try
            {
                connection = new SqlConnection(connStr);

                connection.Open();

                GetCustomerId();
                GetCustomer();
                GetCustomerOrders();

                var r = new Random();

                NewItem();

                if (r.Next(0, 9) == 0)
                {
                    DelItem();
                    NewItem();
                }

                if ((r.Next(0, 9)%2) == 0)
                {
                    UpdItem();
                }

                if (r.Next(0, 9) == 8 || r.Next(0, 9) == 4)
                {
                    NewItem();

                    if ((r.Next(0, 9)%2) == 0)
                    {
                        UpdItem();
                    }
                }

                GetOrderItems();

                UpdateOrder();

                GetOrderInfo();

                CheckoutOrder();

            }
            catch (Exception e)
            {
                
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();

            }
        }

        public Client(string connStr)
        {
            this.connStr = connStr;
        }
    }
}
