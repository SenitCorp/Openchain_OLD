using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels.Extensions
{
    public static partial class AccountExtension
    {
        public static AccountViewModel ToViewModel(this Account source)
        {
            var destination = new AccountViewModel();

            if(destination == null)
            {
                return null;
            }

            destination.Id = source.Id;

            destination.AssetId = source.AssetId;
            destination.Label = source.Label;
            destination.Balance = source.Balance;

            return destination;
        }

        public static List<AccountViewModel> ToViewModel(this List<Account> source)
        {
            var destination = new List<AccountViewModel>();

            if(destination != null)
            {
                foreach(var item in source)
                {
                    destination.Add(item.ToViewModel());
                }
            }

            return destination;
        }
    }
}
