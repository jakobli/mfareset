using Microsoft.WindowsAzure.Storage.Table;
using System;
using Microsoft.Azure; 
using Microsoft.WindowsAzure.Storage;
using System.Security.Cryptography;
using System.Text;
using System.Configuration;

namespace mfaReset.Models
{
    public class MfaLogEntity:TableEntity
    {
        public string Requestor { get; set; }
        public string User { get; set; }
        public DateTime RequestTime { get; set; }
        public MfaLogEntity(string requestor, string user, DateTime requestTime)
        {
            Requestor = requestor;
            User = user;
            RequestTime = requestTime;
            this.PartitionKey = requestor;
            this.RowKey = CreateMD5(requestor + user + RequestTime.ToString());
        }
        public MfaLogEntity() { }
        public static string CreateMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
    public class MfaResetRepository
    {
        private readonly CloudTable _cloudTable;
        public MfaResetRepository(string connectionString)
        {
            var cloudAccount = CloudStorageAccount.Parse(connectionString);
            _cloudTable = cloudAccount.CreateCloudTableClient().GetTableReference("mfaresetlogs");
            _cloudTable.CreateIfNotExists();
        }
        public void InsertOrReplace(MfaLogEntity mfaLogEntity)
        {
            var tableOp = TableOperation.InsertOrReplace(mfaLogEntity);
            _cloudTable.Execute(tableOp);
        }
    }

}