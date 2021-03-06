﻿using System.Collections.Generic;

namespace Xpressive.Home.Contracts.Services
{
    public interface IUpnpDeviceResponse
    {
        string Location { get; }
        string IpAddress { get; }
        string Server { get; }
        string Usn { get; }
        IDictionary<string, string> OtherHeaders { get; }

        string FriendlyName { get; }
        string Manufacturer { get; }
        string ModelName { get; }
    }
}
