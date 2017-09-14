using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels.Extensions
{
    public static class TransactionEntryExtension
    {
        public static TransactionEntryViewModel ToViewModel(this TransactionEntry source)
        {
            var destination = new TransactionEntryViewModel();

            if(destination == null)
            {
                return null;
            }

            destination.Path = source.LedgerPath;
            destination.Value = source.Value;

            return destination;
        }
    }
}
