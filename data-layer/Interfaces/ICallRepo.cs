using System;
using System.Collections.Generic;
using System.Text;

namespace data_layer.Interfaces
{
    public interface ICallRepo
    {
        List<CallDTO> GetAllCalls();
    }
}
