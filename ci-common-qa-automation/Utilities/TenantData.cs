using System.Data;

namespace ci_common_qa_automation.Utilities
{
    public static class TenantData
    {
        public static string? GetTenantName(string connectionString, string tenantId)
        {
            string command = "Select TenantName from dbo.Tenant where TenantId ='" + tenantId + "'";
            DataTable dt = DataBaseExecuter.ExecuteCommand("SQL", connectionString, command);
            return dt.Rows.Count == 1 ? dt.Rows[0]["TenantName"].ToString() : "";
        }
    }
}
