using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Core.Api.Models
{
    public class RelApiReponse<TData>
    {
        public RelApiReponse(TData data, ApiReponse reponse)
        {
            Data = data;
            Reponse = reponse;
        }

        public TData Data { get;  }
        public ApiReponse Reponse { get; }
    }
}
