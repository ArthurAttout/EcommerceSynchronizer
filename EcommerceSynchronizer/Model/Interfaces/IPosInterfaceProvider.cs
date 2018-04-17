using System.Collections;
using System.Collections.Generic;
using EcommerceSynchronizer.Controllers;
using Microsoft.Extensions.Configuration;

namespace EcommerceSynchronizer.Model.Interfaces
{
    public interface IPOSInterfaceProvider
    {
        IList<IPOSInterface> GetAllInterfaces();
    }
}