using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AzureScalabilityTest
{
    public class Emulator
    {
        private string ConnectionString;
        private CancellationToken Token;

        public Emulator(string ConnStr, CancellationToken token)
        {
            ConnectionString = ConnStr;
            Token = token;
        }

        public void Run()
        {
            Client client = new Client(ConnectionString);

            while (!Token.IsCancellationRequested)
            {
                client.RunClientInteraction();

                System.Threading.Thread.Sleep(50);
            }
        }
    }
}
