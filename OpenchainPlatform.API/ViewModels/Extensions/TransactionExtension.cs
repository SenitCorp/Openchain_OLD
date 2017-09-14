using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels.Extensions
{
    public static class TransactionExtension
    {
        public static TransactionViewModel ToViewModel(this Transaction source)
        {
            var destination = new TransactionViewModel();

            if(source == null)
            {
                return null;
            }

            destination.Id = source.Id;
            destination.SequenceId = source.SequenceId;
            destination.CreatedAt = source.CreatedAt;
            
            if(source.Entries != null)
            {
                foreach(var item in source.Entries)
                {
                    destination.Entries.Add(item.ToViewModel());
                }
            }

            return destination;
        }
    }
}
