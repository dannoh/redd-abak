using System;
using System.Collections.Generic;
using System.Windows.Input;
using AbakHelper.Integration;
using AbakHelper.XeroIntegration.Models;
using Xero.Api.Core.Model;

namespace AbakHelper.XeroIntegration.Services
{
    public interface IInvoiceIntegrationService
    {
        CreateInvoiceResult CreateInvoice(List<InvoiceLineItem> lineItems, string invoiceNumber, DateTime invoiceDate);
        string GetInvoiceNumber();
        void DownloadInvoice(object invoice);
        void ApproveInvoice(Invoice invoice);
        void SaveInvoice(Invoice invoice);
    }

    public class InvoiceCommand : IInvoiceCommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public string Text { get; set; }
        /// <summary>
        /// MaterialDesign PackIcon Kind
        /// </summary>
        public string IconKind { get; set; }

        public InvoiceCommand(string text, string icon, Action<object> executeAction, Func<object, bool> canExecuteAction = null)
        {
            Text = text;
            IconKind = icon;
            _execute = executeAction;
            if (canExecuteAction == null)
                canExecuteAction = o => true;
            _canExecute = canExecuteAction;
        }

        public bool CanExecute(object invoice)
        {
            return _canExecute(invoice);
        }

        public void Execute(object invoice)
        {
            if (_execute != null && CanExecute(invoice))
                _execute(invoice);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    public interface IInvoiceCommand : ICommand
    {        
        string Text { get; set; }

        /// <summary>
        /// MaterialDesign PackIcon Kind
        /// </summary>
        string IconKind { get; set; }
        void RaiseCanExecuteChanged();
    }
}