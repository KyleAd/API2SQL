using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API2SQL.Data.Dto
{
    [Keyless]
    public class AllRequests
    {
        public class Rootobject
        {
            public Operation Operation { get; set; }
        }

        public class Operation
        {
            public Result Result { get; set; }
            public Detail[] Details { get; set; }
        }

        public class Result
        {
            public string Message { get; set; }
            public string Status { get; set; }
        }

        public class Detail
        {
            [Key]
            [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
            public int WorkOrderId { get; set; }
            public string Requester { get; set; }
            public string AccountName { get; set; }
            public string CreatedBy { get; set; }
            public string Subject { get; set; }
            public string Technician { get; set; }
            public string IsOverDue { get; set; }
            public long DueByTime { get; set; }
            public string Priority { get; set; }
            public long CreatedTime { get; set; }
            public string IgnoreRequest { get; set; }
            public string Status { get; set; }
        }
    }
}
