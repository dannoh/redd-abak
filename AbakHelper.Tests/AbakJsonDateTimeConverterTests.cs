using System;
using System.Text.RegularExpressions;
using AbakHelper.Integration.Models;
using Newtonsoft.Json;
using Xunit;

namespace AbakHelper.Tests
{
    public class AbakJsonDateTimeConverterTests
    {
        [Fact]
        public void ReadJson_Valid_ReturnsCorrectDateTime()
        {
            string data = "{\"Descriptions\" : null,\"TransactType\" : \"T\",\"CodeDep\" : \"\",\"DisplayDesc\" : \"Consulting by Associate (CONS)\",\"DisplayCode\" : \"CONS\",\"Quantity\" : 8.0000,\"HasNote\" : false,\"Id\" : \"201610311939598d\",\"Date\" : new Date(2016, 09, 31, 00, 00, 00),\"Description\" : \"FPP Design and Development\",\"PayCode\" : \"REG\",\"PayCodeDescription\" : \"Regular (REG)\",\"IsBillable\" : true,\"IsRefundable\" : false,\"ClientName\" : \"Versent Corporation ULC (dba. Laser Quest) (VERSENTLASER)\",\"ProjectName\" : \"Versent Laser Quest - Frequent Player Project FPP (CONS0001001)\",\"PhaseName\" : \"\",\"TaskDesc\" : \"Consulting by Associate (CONS)\",\"PhaseId\" : \"\",\"TimeStart\" : null,\"TimeEnd\" : null,\"IsAffectingTimebank\" : true,\"IsTransactApproved\" : true,\"IsTransactProjectApproved\" : false,\"IsInvoiced\" : true,\"InvoicingStatus\" : \"F\",\"IsTransferredToPayroll\" : false,\"IsAccepted\" : false,\"IsRefused\" : false,\"IsWaiting\" : false,\"Reference\" : \"\",\"TimesheetQuantity\" : 0.0,\"APTransferStatus\" : \"\",\"IsHourly\" : true,\"PhaseParentId\" : \"\",\"PhaseParentName\" : \"\",\"PhaseRootId\" : \"\",\"PhaseRootName\" : \"\",\"HasDocuments\" : false,\"IsProjectApproved\" : false,\"ProjectApprover\" : \"\",\"IsTransactLocked\" : true,\"IsDeletable\" : false}";
            data = Regex.Replace(data, "(new\\ Date\\(\\d{4},\\ \\d{2},\\ \\d{2},\\ 00,\\ 00,\\ 00\\))", "\"$1\"");
            var result = JsonConvert.DeserializeObject<TimeTrackerItem>(data);
            Assert.Equal(new DateTime(2016, 10, 31), result.Date);
        }
    }
}
