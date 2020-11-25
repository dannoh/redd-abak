using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Example.Applications.Private;
using Xero.Api.Infrastructure.Exceptions;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;

namespace AbakHelper.Services
{
    public class XeroIntegrationService
    {
        private readonly string _xeroContactName;
        private readonly XeroCoreApi _xeroService;

        public XeroIntegrationService(string certPath, string certPassword, string consumerKey, string consumerSecret, string xeroContactName)
        {
            _xeroContactName = xeroContactName;
            var user = new ApiUser { Name = Environment.MachineName };
            X509Certificate2 cert = new X509Certificate2(certPath, certPassword, X509KeyStorageFlags.UserKeySet);
            _xeroService = new XeroCoreApi("https://api.xero.com", new PrivateAuthenticator(cert),
                new Consumer(consumerKey, consumerSecret), user, new DefaultMapper(), new DefaultMapper());
        }

        public CreateInvoiceResult CreateInvoice(List<InvoiceLineItem> lineItems, string invoiceNumber, DateTime invoiceDate)
        {
            Invoice invoice = new Invoice
            {
                Contact = new Contact { Name = _xeroContactName },
                Date = invoiceDate,
                DueDate = invoiceDate.AddDays(30),
                Number = invoiceNumber,
                LineAmountTypes = Xero.Api.Core.Model.Types.LineAmountType.Exclusive,
                Status = Xero.Api.Core.Model.Status.InvoiceStatus.Submitted,
                Type = Xero.Api.Core.Model.Types.InvoiceType.AccountsReceivable,
                LineItems = lineItems.Select(c => new LineItem { AccountCode = "200", Description = c.Description, UnitAmount = c.UnitAmount, TaxType = "OUTPUT", Quantity = c.Quantity }).ToList(),
            };
            try
            {
                var result = _xeroService.Invoices.Create(invoice);
                return new CreateInvoiceResult { IsScuccess = true, Invoice = result };
            }
            catch (ValidationException ex)
            {
                return new CreateInvoiceResult { IsScuccess = false, Errors = ex.ValidationErrors.Select(c => c.Message).ToList() };
            }
        }

        public string GetMostRecentInvoiceNumber()
        {
            var invoice = _xeroService.Invoices.OrderByDescending("Date").Where("Type == \"ACCREC\"").And("Status != \"DELETED\"").Find().FirstOrDefault();
            return invoice?.Number;            
        }

        public void DownloadInvoice(Guid invoiceId, string path)
        {
            var result = _xeroService.PdfFiles.Get(Xero.Api.Core.Model.Types.PdfEndpointType.Invoices, invoiceId);
            result.Save(path);
        }

        public Invoice UpdateInvoice(Invoice invoice)
        {
            return _xeroService.Invoices.Update(invoice);
        }
    }

    public class InvoiceLineItem
    {
        public string Description { get; set; }
        public decimal UnitAmount { get; set; }
        public decimal Quantity { get; set; }
    }

    public class CreateInvoiceResult : ExecutionResult
    {
        public Invoice Invoice { get; set; }
    }
    public class ExecutionResult
    {
        public bool IsScuccess { get; set; }
        public List<string> Errors { get; set; }
    }
}
