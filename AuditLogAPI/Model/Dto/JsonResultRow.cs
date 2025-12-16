namespace AuditLogAPI.Model.Dto
{
    public class JsonResultRow
    {
        public string? Json { get; set; }  // The JSON string representation of the object 欄位名要對得上 SQL SELECT 的欄位別名
    }
}
