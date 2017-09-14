using OpenchainPlatform.Core.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenchainPlatform.API.ViewModels.Extensions
{
    public static class AssetExtension
    {
        public static AssetViewModel ToViewModel(this Asset source)
        {
            var destination = new AssetViewModel();

            if(destination == null)
            {
                return null;
            }

            destination.Id = source.Id;

            destination.AccountAddress = source.AccountAddress;

            destination.IssuedUnits = source.IssuedUnits;

            return destination;
        }
    }
}
