using System;
using System.ComponentModel.DataAnnotations;

namespace Bluefragments.Utilities.Data.SqlServer
{
    public class SqlDataEntity
    {
        public SqlDataEntity()
        {
            Id = Guid.Empty.ToString();
        }

        [Key]
        public string Id { get; set; }
    }
}
