using System;
using System.Collections.Generic;
using System.Linq;
using AbakHelper.XeroIntegration.Models;
using AbakHelper.XeroIntegration.Services;
using Microsoft.Win32;
using Xero.Api.Core;
using Xero.Api.Core.Model;
using Xero.Api.Example.TokenStores;
using Xero.Api.Infrastructure.Exceptions;
using Xero.Api.Infrastructure.OAuth;
using Xero.Api.Serialization;

namespace AbakHelper.XeroIntegration
{
    public class XeroInvoice
    {
        public Guid Id { get; set; }
    }

    public class XeroIntegrationService : IInvoiceIntegrationService
    {
        private readonly XeroSettings _settings;
        private readonly string _xeroContactName;
        private XeroCoreApi _xeroService;
        private bool _isInitialized;

        public XeroIntegrationService(XeroSettings settings)
        {
            _settings = settings;
            _xeroContactName = settings.ContactName;
        }

        private void InitializeXeroService()
        {
            if (_isInitialized)
                return;
            var user = new ApiUser {Name = Environment.MachineName};

            var publicAuthenticator = new PublicAuthenticator("https://api.xero.com", "https://api.xero.com", "oob", new MemoryTokenStore());

            _xeroService = new XeroCoreApi("https://api.xero.com", publicAuthenticator,
                new Consumer("X1D0FID4QMBMSX6HVLO3PHYYPW9TMY", "ZDQFWJ4F79TOPYSFAMFPDTZEP9SSPH"), user, new DefaultMapper(), new DefaultMapper());
            _isInitialized = true;
        }

        public CreateInvoiceResult CreateInvoice(List<InvoiceLineItem> lineItems, string invoiceNumber, DateTime invoiceDate)
        {
            InitializeXeroService();
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
                return new CreateInvoiceResult { IsScuccess = true, Invoice = new XeroInvoice { Id = result.Id } };
            }
            catch (ValidationException ex)
            {
                return new CreateInvoiceResult { IsScuccess = false, Errors = ex.ValidationErrors.Select(c => c.Message).ToList() };
            }
        }

        public void EmailInvoice(Invoice invoice)
        {
            Microsoft.Office.Interop.Outlook.Application app = new Microsoft.Office.Interop.Outlook.Application();
            Microsoft.Office.Interop.Outlook.MailItem mailItem = app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            mailItem.Subject = $"{_settings.CompanyName} Invoice";
            mailItem.To = invoice.Contact.EmailAddress;

            mailItem.Attachments.Add($"{AppDomain.CurrentDomain.BaseDirectory}{invoice.Number}.pdf");
            mailItem.Display(false);
        }

        public void ApproveInvoice(Invoice invoice)
        {
            invoice.Status = Xero.Api.Core.Model.Status.InvoiceStatus.Authorised;
            UpdateInvoice(invoice);
        }

        public void SaveInvoice(Invoice invoice)
        {
            SaveFileDialog dialog = new SaveFileDialog { AddExtension = true, DefaultExt = ".pdf", FileName = $"{invoice.Number}.pdf" };
            if (dialog.ShowDialog() ?? false)
                System.IO.File.Copy($"{AppDomain.CurrentDomain.BaseDirectory}{invoice.Number}.pdf", dialog.FileName);
        }

        public string GetInvoiceNumber()
        {
            InitializeXeroService();
            var invoice = _xeroService.Invoices.OrderByDescending("Date").Where("Type == \"ACCREC\"").And("Status != \"DELETED\"").Find().FirstOrDefault();
            return invoice?.Number;
        }

        public void DownloadInvoice(object invoice)
        {
            
            //var xeroInvoice = ((Invoice)invoice);
            //var result = _xeroService.PdfFiles.Get(Xero.Api.Core.Model.Types.PdfEndpointType.Invoices, xeroInvoice.Id);
            //result.Save($"{xeroInvoice.Number}.pdf");
        }

        public Invoice UpdateInvoice(Invoice invoice)
        {
            InitializeXeroService();
            return _xeroService.Invoices.Update(invoice);
        }

       
    }
}
