using System;
using System.Collections.Generic;
using System.Text;

namespace AllegroGraphNetCoreClient.Mini
{
   public interface  IAgUrl
    {
        string Url { get; }

        string Username { get; }

        string Password { get; }
    }
}
