using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels.Extensions
{
    public static partial class AccountExtension
    {
        public static AccountAddressViewModel ToViewModel(this AccountAddress source)
        {
            var destination = new AccountAddressViewModel();

            if(destination == null)
            {
                return null;
            }

            destination.Address = source.Address;

            destination.AccountId = source.AccountId;
            destination.Chain = source.Chain;
            destination.Index = source.Index;

            return destination;
        }

        public static List<AccountAddressViewModel> ToViewModel(this List<AccountAddress> source)
        {
            var destination = new List<AccountAddressViewModel>();

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
